using System.Globalization;
using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text.Json;
using c_lan.Configuration;
using c_lan.Models;
using Microsoft.Data.Sqlite;

namespace c_lan.Services;

/// <summary>一机一件：每分钟观察源库，至少已有结果且稳定满一分钟后请求出站。</summary>
public sealed class ElectricCheckScanService : IDisposable
{
    public static readonly TimeSpan CheckInterval = TimeSpan.FromMinutes(1);
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);
    private readonly ElectricCheckSyncService _sync;
    private readonly ElectricCheckScanStateStore _store;
    private readonly HttpClient _http;
    private readonly TimeProvider _clock;
    private readonly SemaphoreSlim _gate = new(1, 1);
    private string? _loadedKey;
    private string? _observedScanId;
    private long _lastChangeTimestamp;

    public ElectricCheckScanState? Current { get; private set; }
    public bool HasActiveScan => Current?.Status is "PENDING_ENTRY" or "TESTING" or "PENDING_COMPLETE";

    public ElectricCheckScanService(ElectricCheckSyncService sync, ElectricCheckScanStateStore store,
        HttpClient? httpClient = null, TimeProvider? clock = null)
    {
        _sync = sync;
        _store = store;
        _http = httpClient ?? new HttpClient { Timeout = TimeSpan.FromSeconds(15) };
        _clock = clock ?? TimeProvider.System;
    }

    public async Task<ElectricCheckScanState> StartAsync(ElectricCheckSyncSettings settings, string sn, CancellationToken token)
    {
        ElectricCheckSyncService.Validate(settings);
        sn = sn.Trim();
        if (sn.Length is 0 or > 128) throw new ArgumentException("SN不能为空且不能超过128个字符。");
        await _gate.WaitAsync(token);
        try
        {
            await LoadAsync(settings, token);
            if (HasActiveScan) throw new InvalidOperationException("上一件尚未出站，请等待检测完成或核实当前记录。");
            ElectricCheckScanState? remote = await GetAsync(settings, "scans/current?deviceNo=" + Uri.EscapeDataString(settings.DeviceNo), token);
            if (remote is not null)
            {
                Current = remote;
                Current.Message = "已恢复服务器上的待检测产品，请先完成上一件。";
                await SaveAsync(settings, token);
                throw new InvalidOperationException(Current.Message);
            }
            long baseline = await ReadMaxRowAsync(settings, token);
            Current = new()
            {
                ScanId = Guid.NewGuid().ToString("N"), DeviceNo = settings.DeviceNo, SnCode = sn,
                ScanTime = LocalTime(), SourceRowIdBeforeScan = baseline,
                Status = "PENDING_ENTRY", Message = "正在提交入站，成功前请勿开始检测。"
            };
            await SaveAsync(settings, token);
            await EnterAsync(settings, token);
            return Current;
        }
        finally { _gate.Release(); }
    }

    public async Task<ElectricCheckScanState?> PollAsync(ElectricCheckSyncSettings settings, CancellationToken token)
    {
        ElectricCheckSyncService.Validate(settings);
        await _gate.WaitAsync(token);
        try
        {
            await LoadAsync(settings, token);
            if (Current is null)
            {
                Current = await GetAsync(settings, "scans/current?deviceNo=" + Uri.EscapeDataString(settings.DeviceNo), token);
                if (Current is null) return null;
                await SaveAsync(settings, token);
            }
            if (!HasActiveScan) return Current;
            if (Current.Status == "PENDING_ENTRY") await EnterAsync(settings, token);

            ElectricCheckScanState remote = await GetAsync(settings, "scans/" + Uri.EscapeDataString(Current.ScanId)
                + "?deviceNo=" + Uri.EscapeDataString(settings.DeviceNo), token)
                ?? throw new InvalidOperationException("服务器未找到当前入站记录，请核实后恢复。");
            VerifyIdentity(remote);
            if (remote.Status is "COMPLETED" or "CANCELLED")
            {
                Current.Status = remote.Status;
                Current.Message = remote.Status == "COMPLETED" ? "已出站，检测结果已关联SN。" : "本次检测已取消。";
                await SaveAsync(settings, token);
                return Current;
            }
            if (remote.Status != "TESTING") throw new InvalidOperationException("服务器返回了未知的检测状态。");

            Snapshot snapshot = await ReadSnapshotAsync(settings, Current.SourceRowIdBeforeScan, token);
            bool pending = Current.Status == "PENDING_COMPLETE" && Current.Completion is not null;
            if ((!pending && _observedScanId != Current.ScanId) || Current.ObservedFingerprint != snapshot.Fingerprint)
            {
                Observe(snapshot);
                await SaveAsync(settings, token);
                return Current;
            }
            if (snapshot.Count == 0)
            {
                Current.Message = "等待首条检测结果；没有结果不会自动出站。";
                return Current;
            }
            if (!pending && _clock.GetElapsedTime(_lastChangeTimestamp, _clock.GetTimestamp()) < CheckInterval)
                return Current;

            // 范围重传也刷新已修改的旧行；不能只凭增量游标认定全部结果已同步。
            await _sync.SyncRangeAsync(settings, Current.SourceRowIdBeforeScan, snapshot.LastRowId, token);
            Snapshot afterUpload = await ReadSnapshotAsync(settings, Current.SourceRowIdBeforeScan, token);
            if (afterUpload.Fingerprint != snapshot.Fingerprint)
            {
                Observe(afterUpload);
                await SaveAsync(settings, token);
                return Current;
            }
            Current.Completion ??= new()
            {
                DeviceNo = Current.DeviceNo, CompletedAt = LocalTime(),
                LastSourceRowId = snapshot.LastRowId, ExpectedRecordCount = snapshot.Count
            };
            Current.Status = "PENDING_COMPLETE";
            Current.Message = "检测结果已稳定一分钟，正在自动出站。";
            await SaveAsync(settings, token);
            ElectricCheckScanState completed = await PostAsync(settings,
                "scans/" + Uri.EscapeDataString(Current.ScanId) + "/complete", Current.Completion, token);
            VerifyIdentity(completed);
            if (completed.Status != "COMPLETED") throw new InvalidOperationException("出站接口尚未成功，程序会保留当前记录并自动重试。");
            Current.Status = "COMPLETED";
            Current.Message = "已出站，检测结果已关联SN，可扫描下一件。";
            await SaveAsync(settings, token);
            return Current;
        }
        finally { _gate.Release(); }
    }

    private async Task LoadAsync(ElectricCheckSyncSettings settings, CancellationToken token)
    {
        string key = ElectricCheckSyncService.StateKey(settings);
        if (_loadedKey == key) return;
        if (HasActiveScan) throw new InvalidOperationException("检测中不能切换设备、服务器或源库配置。");
        Current = await _store.LoadAsync(key, token);
        if (Current is not null && (string.IsNullOrWhiteSpace(Current.ScanId) || string.IsNullOrWhiteSpace(Current.SnCode)
                || Current.DeviceNo != settings.DeviceNo || string.IsNullOrWhiteSpace(Current.ScanTime)
                || Current.SourceRowIdBeforeScan < 0
                || Current.Status is not ("PENDING_ENTRY" or "TESTING" or "PENDING_COMPLETE" or "COMPLETED" or "CANCELLED")
                || (Current.Status == "PENDING_COMPLETE" && Current.Completion is null)))
            throw new InvalidDataException("本地扫码记录不完整，请核实后恢复，不能自动出站。");
        _loadedKey = key;
        _observedScanId = null;
    }

    private async Task EnterAsync(ElectricCheckSyncSettings settings, CancellationToken token)
    {
        ElectricCheckScanState state = Current!;
        ElectricCheckScanState remote = await PostAsync(settings, "scans", new
        {
            state.ScanId, state.DeviceNo, state.SnCode, state.ScanTime, state.SourceRowIdBeforeScan
        }, token);
        VerifyIdentity(remote);
        state.Status = remote.Status;
        state.Message = remote.Status == "TESTING" ? "已入站，等待检测结果；每分钟检查一次。" : "服务器已确认该记录的最终状态。";
        if (state.Status is not ("TESTING" or "COMPLETED" or "CANCELLED"))
            throw new InvalidOperationException("服务器未确认有效入站状态。");
        await SaveAsync(settings, token);
    }

    private void Observe(Snapshot snapshot)
    {
        ElectricCheckScanState state = Current!;
        state.Status = "TESTING"; state.Completion = null;
        state.ObservedFingerprint = snapshot.Fingerprint;
        state.ObservedRecordCount = snapshot.Count; state.ObservedLastRowId = snapshot.LastRowId;
        state.LastChangedAt = _clock.GetUtcNow();
        _observedScanId = state.ScanId; _lastChangeTimestamp = _clock.GetTimestamp();
        state.Message = snapshot.Count == 0 ? "等待首条检测结果；没有结果不会自动出站。"
            : $"已观察到 {snapshot.Count} 条结果；稳定满一分钟后自动出站。";
    }

    private Task SaveAsync(ElectricCheckSyncSettings settings, CancellationToken token) =>
        _store.SaveAsync(ElectricCheckSyncService.StateKey(settings), Current!, token);

    private void VerifyIdentity(ElectricCheckScanState remote)
    {
        if (remote.ScanId != Current!.ScanId || remote.DeviceNo != Current.DeviceNo
            || remote.SnCode != Current.SnCode || remote.SourceRowIdBeforeScan != Current.SourceRowIdBeforeScan)
            throw new InvalidOperationException("服务器记录与本次扫码不一致，已停止自动出站。");
    }

    private string LocalTime() => _clock.GetLocalNow().ToString("yyyy-MM-dd'T'HH:mm:ss.fffzzz", CultureInfo.InvariantCulture);

    private Uri Endpoint(ElectricCheckSyncSettings settings, string suffix) => new(
        new Uri(settings.ServerBaseUrl.TrimEnd('/') + "/"), "openapi/electric-check/" + suffix);

    private async Task<ElectricCheckScanState?> GetAsync(ElectricCheckSyncSettings settings, string suffix, CancellationToken token)
    {
        using HttpResponseMessage response = await _http.GetAsync(Endpoint(settings, suffix), token);
        return await ReadResponseAsync(response, token);
    }

    private async Task<ElectricCheckScanState> PostAsync(ElectricCheckSyncSettings settings, string suffix, object payload, CancellationToken token)
    {
        using HttpResponseMessage response = await _http.PostAsJsonAsync(Endpoint(settings, suffix), payload, JsonOptions, token);
        return await ReadResponseAsync(response, token) ?? throw new InvalidDataException("接口未返回扫码记录。");
    }

    private static async Task<ElectricCheckScanState?> ReadResponseAsync(HttpResponseMessage response, CancellationToken token)
    {
        string body = await response.Content.ReadAsStringAsync(token);
        if (!response.IsSuccessStatusCode) throw new InvalidOperationException($"服务端返回 {(int)response.StatusCode}：{body}");
        using JsonDocument json = JsonDocument.Parse(body);
        if (!json.RootElement.TryGetProperty("code", out JsonElement code) || code.GetInt32() != 200)
            throw new InvalidOperationException("服务端未确认扫码操作：" + body);
        return json.RootElement.TryGetProperty("data", out JsonElement data) && data.ValueKind != JsonValueKind.Null
            ? data.Deserialize<ElectricCheckScanState>(JsonOptions) : null;
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

    private static async Task<Snapshot> ReadSnapshotAsync(ElectricCheckSyncSettings settings, long baseline, CancellationToken token)
    {
        using SqliteConnection connection = Source(settings);
        await connection.OpenAsync(token);
        using SqliteCommand command = new("SELECT rowid,* FROM ElectricCheck WHERE rowid > $baseline ORDER BY rowid", connection);
        command.Parameters.AddWithValue("$baseline", baseline);
        using SqliteDataReader reader = await command.ExecuteReaderAsync(token);
        using IncrementalHash hash = IncrementalHash.CreateHash(HashAlgorithmName.SHA256);
        long last = baseline;
        int count = 0;
        while (await reader.ReadAsync(token))
        {
            if (++count > 10000) throw new InvalidDataException("本次结果超过10000条，请核实扫码边界，已停止自动出站。");
            last = reader.GetInt64(0);
            object?[] values = new object?[reader.FieldCount];
            for (int i = 0; i < values.Length; i++) values[i] = reader.IsDBNull(i) ? null : reader.GetValue(i);
            hash.AppendData(JsonSerializer.SerializeToUtf8Bytes(values));
        }
        if (count == 0 && baseline > await ReadMaxRowAsync(settings, token))
            throw new InvalidDataException("源库行号已小于扫码前行号，请核实数据库是否被替换。");
        return new(last, count, Convert.ToHexString(hash.GetHashAndReset()));
    }

    private sealed record Snapshot(long LastRowId, int Count, string Fingerprint);

    public void Dispose() { _http.Dispose(); _gate.Dispose(); }
}
