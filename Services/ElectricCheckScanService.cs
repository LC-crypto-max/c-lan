using System.Globalization;
using System.Net;
using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text.Json;
using c_lan.Configuration;
using c_lan.Models;
using Microsoft.Data.Sqlite;

namespace c_lan.Services;

/// <summary>连续扫码持久化排队；按源库连续检测批次先进先出绑定。</summary>
public sealed class ElectricCheckScanService : IDisposable
{
    public static readonly TimeSpan CheckInterval = TimeSpan.FromMinutes(1);
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);
    private readonly ElectricCheckSyncService _sync;
    private readonly ElectricCheckScanStateStore _legacy;
    private readonly ElectricCheckScanQueueStore _store;
    private readonly HttpClient _http;
    private readonly TimeProvider _clock;
    private readonly SemaphoreSlim _localGate = new(1, 1);
    private readonly SemaphoreSlim _pollGate = new(1, 1);
    private readonly List<ElectricCheckScanState> _entries = [];
    private readonly Dictionary<string, (string Fingerprint, long Changed)> _observations = [];
    private string? _loadedKey;

    public IReadOnlyList<ElectricCheckScanState> Entries => _entries.AsReadOnly();
    public ElectricCheckScanState? Current => _entries.FirstOrDefault(IsActive) ?? _entries.LastOrDefault();
    public bool HasActiveScan => _entries.Any(IsActive);
    private static bool IsActive(ElectricCheckScanState state) => state.Status is "PENDING_ENTRY" or "TESTING" or "PENDING_COMPLETE";

    public ElectricCheckScanService(ElectricCheckSyncService sync, ElectricCheckScanStateStore store,
        HttpClient? httpClient = null, TimeProvider? clock = null, ElectricCheckScanQueueStore? queueStore = null)
    {
        _sync = sync;
        _legacy = store;
        _store = queueStore ?? new ElectricCheckScanQueueStore(store.DirectoryPath);
        _http = httpClient ?? new HttpClient { Timeout = TimeSpan.FromSeconds(15) };
        _clock = clock ?? TimeProvider.System;
    }

    /// <summary>仅本地事务入队，不等待网络；后台处理不会占用这个短锁。</summary>
    public async Task<ElectricCheckScanState> EnqueueAsync(ElectricCheckSyncSettings settings, string sn, CancellationToken token)
    {
        ElectricCheckSyncService.Validate(settings);
        // 与Java String.trim及历史登记SQL保持同一规则。
        int start = 0, end = sn.Length;
        while (start < end && sn[start] <= '\u0020') start++;
        while (end > start && sn[end - 1] <= '\u0020') end--;
        sn = sn[start..end];
        if (sn.Length is 0 or > 128) throw new ArgumentException("SN不能为空且不能超过128个字符。");
        await _localGate.WaitAsync(token);
        try
        {
            await LoadAsync(settings, token);
            ElectricCheckScanState? previous = _store.Find(ServerKey(settings), sn);
            if (previous is not null)
            {
                if (previous.Status != "ENTRY_FAILED" || !_entries.Any(e => e.ScanId == previous.ScanId))
                    throw new InvalidOperationException($"该SN已扫码：{sn}（{StatusText(previous.Status)}），请查看列表。");
                ElectricCheckScanState retry = _entries.Single(e => e.ScanId == previous.ScanId);
                retry.Status = "PENDING_ENTRY";
                retry.Message = "等待重新核对入站。";
                Save(settings, retry);
                return retry;
            }
            long baseline = await ReadMaxRowAsync(settings, token);
            ElectricCheckScanState state = new()
            {
                ScanId = Guid.NewGuid().ToString("N"), DeviceNo = settings.DeviceNo, SnCode = sn,
                ScanTime = LocalTime(), SourceRowIdBeforeScan = baseline,
                Status = "PENDING_ENTRY", Message = "已保存，等待服务端确认入站；可继续扫描下一件。"
            };
            _store.Add(ServerKey(settings), Key(settings), state, baseline);
            _entries.Add(state);
            return state;
        }
        finally { _localGate.Release(); }
    }

    public async Task<ElectricCheckScanState> StartAsync(ElectricCheckSyncSettings settings, string sn, CancellationToken token)
    {
        ElectricCheckScanState state = await EnqueueAsync(settings, sn, token);
        await PollAsync(settings, token);
        return state;
    }

    public async Task<ElectricCheckScanState?> PollAsync(ElectricCheckSyncSettings settings, CancellationToken token)
    {
        ElectricCheckSyncService.Validate(settings);
        if (!await _pollGate.WaitAsync(0, token)) return Current;
        try
        {
            await _localGate.WaitAsync(token);
            try { await LoadAsync(settings, token); }
            finally { _localGate.Release(); }
            // 不越过结果未知的旧入站请求，确保服务端接收顺序等于扫码顺序。
            foreach (ElectricCheckScanState pending in _entries.Where(e => e.Status == "PENDING_ENTRY").ToArray())
            {
                try { await EnterAsync(settings, pending, token); }
                catch (OperationCanceledException) when (token.IsCancellationRequested) { throw; }
                catch (Exception ex)
                {
                    pending.Message = "入站待确认，将按原记录重试：" + ex.Message;
                    Save(settings, pending);
                    throw;
                }
            }
            await RefreshStatusesAsync(settings, token);
            ElectricCheckScanState? head = _entries.Where(e => e.Status is "TESTING" or "PENDING_COMPLETE")
                .OrderBy(e => e.Id > 0 ? e.Id : long.MaxValue).ThenBy(e => e.Sequence).FirstOrDefault();
            if (head is not null)
            {
                try { await AdvanceAsync(settings, head, token); }
                catch (OperationCanceledException) when (token.IsCancellationRequested) { throw; }
                catch (Exception ex)
                {
                    head.Message = "处理暂未成功，保留记录重试：" + ex.Message;
                    Save(settings, head);
                    throw;
                }
            }
            return Current;
        }
        finally { _pollGate.Release(); }
    }

    private async Task LoadAsync(ElectricCheckSyncSettings settings, CancellationToken token)
    {
        string key = Key(settings);
        if (_loadedKey == key) return;
        if (HasActiveScan) throw new InvalidOperationException("队列尚有未完成记录，不能切换设备、服务器或源库。");
        List<ElectricCheckScanState> loaded = _store.Load(key);
        if (loaded.Count == 0)
        {
            ElectricCheckScanState? legacy = await _legacy.LoadAsync(key, token);
            if (legacy is not null)
            {
                if (string.IsNullOrWhiteSpace(legacy.ScanId) || string.IsNullOrWhiteSpace(legacy.SnCode)
                    || legacy.DeviceNo != settings.DeviceNo || legacy.SourceRowIdBeforeScan < 0)
                    throw new InvalidDataException("旧扫码缓存不完整，请核实后恢复。");
                legacy.FirstSourceRowId ??= legacy.Completion?.FirstSourceRowId;
                long migratedWatermark = legacy.Status == "COMPLETED"
                    ? legacy.LastSourceRowId ?? legacy.Completion?.LastSourceRowId ?? legacy.ObservedLastRowId
                    : legacy.SourceRowIdBeforeScan;
                // 导入旧记录和已完成水位也须一次提交，避免迁移中断后重复分配旧结果。
                _store.Add(ServerKey(settings), key, legacy, migratedWatermark);
                loaded.Add(legacy);
            }
        }
        _entries.Clear();
        _entries.AddRange(loaded);
        _observations.Clear();
        _loadedKey = key;
    }

    private async Task EnterAsync(ElectricCheckSyncSettings settings, ElectricCheckScanState state, CancellationToken token)
    {
        try
        {
            ElectricCheckScanState? remote = await GetAsync(settings,
                "scans/" + Uri.EscapeDataString(state.ScanId) + "?deviceNo=" + Uri.EscapeDataString(state.DeviceNo), token, true);
            remote ??= await PostAsync(settings, "scans", new
            { state.ScanId, state.DeviceNo, state.SnCode, state.ScanTime, state.SourceRowIdBeforeScan }, token);
            Adopt(settings, state, remote);
        }
        catch (ScanApiException ex) when (ex.Status == HttpStatusCode.Conflict && ex.BusinessCode == "DUPLICATE_SN")
        {
            state.Status = "REJECTED";
            state.BusinessCode = ex.BusinessCode;
            state.Message = "该SN已在服务端扫码，不再重复入站。";
            Save(settings, state);
        }
        catch (ScanApiException ex) when (ex.Status == HttpStatusCode.BadRequest)
        {
            state.Status = "ENTRY_FAILED";
            state.Message = "入站失败：" + ex.Message;
            Save(settings, state);
        }
    }

    private async Task RefreshStatusesAsync(ElectricCheckSyncSettings settings, CancellationToken token)
    {
        ElectricCheckScanState[] confirmed = _entries.Where(e => e.Status is "TESTING" or "PENDING_COMPLETE").ToArray();
        foreach (ElectricCheckScanState[] chunk in confirmed.Chunk(100))
        {
            string ids = string.Join(",", chunk.Select(e => e.ScanId));
            using HttpResponseMessage response = await _http.GetAsync(Endpoint(settings,
                "scans/status?deviceNo=" + Uri.EscapeDataString(settings.DeviceNo) + "&scanIds=" + Uri.EscapeDataString(ids)), token);
            JsonElement data = await ReadDataAsync(response, token);
            if (data.ValueKind != JsonValueKind.Array) throw new InvalidDataException("批量状态接口未返回记录列表，请确认后端版本。");
            List<ElectricCheckScanState> remote = data.Deserialize<List<ElectricCheckScanState>>(JsonOptions)!;
            foreach (ElectricCheckScanState state in chunk)
            {
                ElectricCheckScanState stored = remote.SingleOrDefault(e => e.ScanId == state.ScanId)
                    ?? throw new InvalidDataException("服务端未返回已入站记录，请核实后恢复队列。");
                Adopt(settings, state, stored);
            }
        }
    }

    private void Adopt(ElectricCheckSyncSettings settings, ElectricCheckScanState state, ElectricCheckScanState remote)
    {
        if (remote.ScanId != state.ScanId || remote.DeviceNo != state.DeviceNo || remote.SnCode != state.SnCode
            || remote.SourceRowIdBeforeScan != state.SourceRowIdBeforeScan)
            throw new InvalidOperationException("服务端扫码内容与本地记录不一致，已暂停自动绑定。");
        if (remote.Status is not ("TESTING" or "COMPLETED" or "CANCELLED"))
            throw new InvalidOperationException("服务端返回未知的扫码状态。");
        state.Id = remote.Id;
        state.FinalResult = remote.FinalResult;
        if (remote.Status == "COMPLETED")
        {
            long last = remote.LastSourceRowId ?? state.Completion?.LastSourceRowId ?? 0;
            if (last < 1) throw new InvalidDataException("已出站记录缺少结果末行，不能推进队列。");
            state.Status = "COMPLETED";
            state.FirstSourceRowId = remote.FirstSourceRowId ?? state.Completion?.FirstSourceRowId;
            state.LastSourceRowId = last;
            state.Message = "已出站，结果按扫码顺序完成绑定。";
            Save(settings, state, last);
            _observations.Remove(state.ScanId);
        }
        else
        {
            // 服务端仍检测中时保留已经冻结的完成报文，超时后可原样重传。
            if (state.Status != "PENDING_COMPLETE" || remote.Status == "CANCELLED") state.Status = remote.Status;
            if (remote.Status == "CANCELLED") state.Message = "已异常中止。";
            else if (state.Completion is null) state.Message = "已入站，等待按顺序绑定检测结果。";
            Save(settings, state);
        }
    }

    private async Task AdvanceAsync(ElectricCheckSyncSettings settings, ElectricCheckScanState state, CancellationToken token)
    {
        long watermark = _store.Watermark(Key(settings)) ?? throw new InvalidDataException("扫码来源水位缺失。");
        if (state.Completion is not null && state.Status == "PENDING_COMPLETE")
        {
            long first = state.Completion.FirstSourceRowId ?? state.SourceRowIdBeforeScan + 1;
            Snapshot frozen = await ReadSnapshotAsync(settings, first - 1, token, state.Completion.LastSourceRowId);
            if (frozen.Count != state.Completion.ExpectedRecordCount || frozen.Last != state.Completion.LastSourceRowId
                || (state.ObservedFingerprint is not null && frozen.Fingerprint != state.ObservedFingerprint))
                throw new InvalidDataException("已冻结的检测结果发生变化，请核实；不会移动到下一SN。");
            await CompleteAsync(settings, state, token);
            return;
        }
        Snapshot snapshot = await ReadSnapshotAsync(settings, watermark, token);
        if (snapshot.Count == 0)
        {
            state.ObservedRecordCount = 0;
            state.Message = "已入站，等待首批检测结果。";
            Save(settings, state);
            return;
        }
        bool changed = !_observations.TryGetValue(state.ScanId, out var observation) || observation.Fingerprint != snapshot.Fingerprint;
        if (changed)
        {
            observation = (snapshot.Fingerprint, _clock.GetTimestamp());
            _observations[state.ScanId] = observation;
            state.FirstSourceRowId = snapshot.First;
            state.ObservedFingerprint = snapshot.Fingerprint;
            state.ObservedLastRowId = snapshot.Last;
            state.ObservedRecordCount = snapshot.Count;
            state.LastChangedAt = _clock.GetUtcNow();
            state.Message = "检测中；本批结果稳定一分钟或出现下一批后自动出站。";
            Save(settings, state);
        }
        if (!snapshot.Closed && _clock.GetElapsedTime(observation.Changed, _clock.GetTimestamp()) < CheckInterval) return;
        await _sync.SyncRangeAsync(settings, snapshot.First - 1, snapshot.Last, token);
        Snapshot afterUpload = await ReadSnapshotAsync(settings, watermark, token);
        if (snapshot.Fingerprint != afterUpload.Fingerprint)
        {
            _observations[state.ScanId] = (afterUpload.Fingerprint, _clock.GetTimestamp());
            state.FirstSourceRowId = afterUpload.First;
            state.ObservedFingerprint = afterUpload.Fingerprint;
            state.ObservedLastRowId = afterUpload.Last;
            state.ObservedRecordCount = afterUpload.Count;
            state.LastChangedAt = _clock.GetUtcNow();
            state.Message = "上传期间结果发生变化，将重新观察本批结果。";
            Save(settings, state);
            return;
        }
        state.Completion = new()
        {
            DeviceNo = state.DeviceNo, CompletedAt = LocalTime(), FirstSourceRowId = snapshot.First,
            LastSourceRowId = snapshot.Last, ExpectedRecordCount = snapshot.Count
        };
        state.Status = "PENDING_COMPLETE";
        state.Message = "检测结果已上传，等待确认出站。";
        Save(settings, state);
        await CompleteAsync(settings, state, token);
    }

    private async Task CompleteAsync(ElectricCheckSyncSettings settings, ElectricCheckScanState state, CancellationToken token)
    {
        ElectricCheckScanState remote = await PostAsync(settings,
            "scans/" + Uri.EscapeDataString(state.ScanId) + "/complete", state.Completion!, token);
        if (remote.Status != "COMPLETED") throw new InvalidOperationException("服务端尚未确认出站，将保留原完成报文重试。");
        Adopt(settings, state, remote);
    }

    private void Save(ElectricCheckSyncSettings settings, ElectricCheckScanState state, long? last = null) => _store.Save(Key(settings), state, last);
    private static string Key(ElectricCheckSyncSettings settings) => ElectricCheckSyncService.StateKey(settings);
    private static string ServerKey(ElectricCheckSyncSettings settings) => new Uri(settings.ServerBaseUrl.TrimEnd('/') + "/").AbsoluteUri;
    private string LocalTime() => _clock.GetLocalNow().ToString("yyyy-MM-dd'T'HH:mm:ss.fffzzz", CultureInfo.InvariantCulture);
    private static string StatusText(string status) => status switch
    {
        "PENDING_ENTRY" => "入站待确认", "TESTING" => "已入站/检测中", "PENDING_COMPLETE" => "待出站",
        "COMPLETED" => "已出站", "CANCELLED" => "已中止", "REJECTED" => "重复SN", _ => "入站失败"
    };
    private static Uri Endpoint(ElectricCheckSyncSettings settings, string suffix) => new(
        new Uri(settings.ServerBaseUrl.TrimEnd('/') + "/"), "openapi/electric-check/" + suffix);

    private async Task<ElectricCheckScanState?> GetAsync(ElectricCheckSyncSettings settings, string suffix,
        CancellationToken token, bool allowNotFound = false)
    {
        using HttpResponseMessage response = await _http.GetAsync(Endpoint(settings, suffix), token);
        if (allowNotFound && response.StatusCode == HttpStatusCode.NotFound) return null;
        JsonElement data = await ReadDataAsync(response, token);
        return data.ValueKind == JsonValueKind.Null ? null : data.Deserialize<ElectricCheckScanState>(JsonOptions);
    }
    private async Task<ElectricCheckScanState> PostAsync(ElectricCheckSyncSettings settings, string suffix, object payload, CancellationToken token)
    {
        using HttpResponseMessage response = await _http.PostAsJsonAsync(Endpoint(settings, suffix), payload, JsonOptions, token);
        JsonElement data = await ReadDataAsync(response, token);
        return data.ValueKind == JsonValueKind.Null ? throw new InvalidDataException("服务端未返回扫码记录。")
            : data.Deserialize<ElectricCheckScanState>(JsonOptions) ?? throw new InvalidDataException("扫码记录无效。");
    }
    private static async Task<JsonElement> ReadDataAsync(HttpResponseMessage response, CancellationToken token)
    {
        string body = await response.Content.ReadAsStringAsync(token);
        JsonDocument json;
        try { json = JsonDocument.Parse(body); }
        catch (JsonException) { throw new InvalidDataException($"服务端返回 {(int)response.StatusCode}，响应格式无效。"); }
        using (json)
        {
            JsonElement root = json.RootElement;
            if (!response.IsSuccessStatusCode || !root.TryGetProperty("code", out JsonElement code) || code.GetInt32() != 200)
            {
                string message = root.TryGetProperty("msg", out JsonElement msg) ? msg.GetString() ?? "扫码操作未成功" : "扫码操作未成功";
                string? businessCode = root.TryGetProperty("businessCode", out JsonElement business) ? business.GetString() : null;
                throw new ScanApiException(response.StatusCode, message, businessCode);
            }
            return root.TryGetProperty("data", out JsonElement data) ? data.Clone() : JsonSerializer.SerializeToElement<object?>(null);
        }
    }
    private static SqliteConnection Source(ElectricCheckSyncSettings settings) => new(
        new SqliteConnectionStringBuilder { DataSource = settings.SqliteFilePath, Mode = SqliteOpenMode.ReadOnly, Pooling = false }.ToString());
    private static async Task<long> ReadMaxRowAsync(ElectricCheckSyncSettings settings, CancellationToken token)
    {
        using SqliteConnection connection = Source(settings);
        await connection.OpenAsync(token);
        using SqliteCommand command = new("SELECT COALESCE(MAX(rowid),0) FROM ElectricCheck", connection);
        return Convert.ToInt64(await command.ExecuteScalarAsync(token), CultureInfo.InvariantCulture);
    }
    private static async Task<Snapshot> ReadSnapshotAsync(ElectricCheckSyncSettings settings, long before,
        CancellationToken token, long? through = null)
    {
        using SqliteConnection connection = Source(settings);
        await connection.OpenAsync(token);
        using SqliteCommand command = new("SELECT rowid,* FROM ElectricCheck WHERE rowid>$before AND rowid<=$last ORDER BY rowid", connection);
        command.Parameters.AddWithValue("$before", before);
        command.Parameters.AddWithValue("$last", through ?? long.MaxValue);
        using SqliteDataReader reader = await command.ExecuteReaderAsync(token);
        int testOrdinal = reader.GetOrdinal("TestID");
        using IncrementalHash hash = IncrementalHash.CreateHash(HashAlgorithmName.SHA256);
        long first = 0, last = before;
        int count = 0;
        string? testId = null;
        bool closed = false;
        while (await reader.ReadAsync(token))
        {
            string currentTest = reader.IsDBNull(testOrdinal) ? "" : reader.GetValue(testOrdinal).ToString()!.Trim();
            if (through is null && count > 0 && currentTest != testId) { closed = true; break; }
            if (++count > 10000) throw new InvalidDataException("单批检测结果超过10000条，已暂停绑定，请核实检测批次。");
            testId = currentTest;
            last = reader.GetInt64(0);
            if (first == 0) first = last;
            object?[] values = new object?[reader.FieldCount];
            for (int i = 0; i < values.Length; i++) values[i] = reader.IsDBNull(i) ? null : reader.GetValue(i);
            hash.AppendData(JsonSerializer.SerializeToUtf8Bytes(values));
        }
        if (count == 0 && before > await ReadMaxRowAsync(settings, token))
            throw new InvalidDataException("源库行号已回退，请核实源库是否被替换；队列水位保持不变。");
        return new(first, last, count, Convert.ToHexString(hash.GetHashAndReset()), closed);
    }
    private sealed record Snapshot(long First, long Last, int Count, string Fingerprint, bool Closed);
    private sealed class ScanApiException(HttpStatusCode status, string message, string? businessCode)
        : InvalidOperationException(message)
    {
        public HttpStatusCode Status { get; } = status;
        public string? BusinessCode { get; } = businessCode;
    }
    public void Dispose() { _http.Dispose(); _localGate.Dispose(); _pollGate.Dispose(); }
}
