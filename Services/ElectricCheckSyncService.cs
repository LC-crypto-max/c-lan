using c_lan.Configuration;
using c_lan.Models;
using Microsoft.Data.Sqlite;
using System.Globalization;
using System.Net.Http.Json;
using System.Text.Json;

namespace c_lan.Services;

public sealed class ElectricCheckSyncService : IDisposable
{
    private readonly HttpClient _httpClient;
    private readonly ElectricCheckSyncStateStore _stateStore;
    private readonly SemaphoreSlim _gate = new(1, 1);
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public ElectricCheckSyncService(ElectricCheckSyncStateStore stateStore, HttpClient? httpClient = null)
    {
        _stateStore = stateStore;
        _httpClient = httpClient ?? new HttpClient { Timeout = TimeSpan.FromSeconds(15) };
    }

    public async Task ResetSyncStateAsync(ElectricCheckSyncSettings settings, CancellationToken token)
    {
        Validate(settings);
        await _gate.WaitAsync(token);
        try { await _stateStore.ResetAsync(StateKey(settings), token); }
        finally { _gate.Release(); }
    }

    public async Task<ElectricCheckSyncResult> SyncOnceAsync(ElectricCheckSyncSettings settings, CancellationToken token)
    {
        Validate(settings);
        await _gate.WaitAsync(token);
        try
        {
            long cursor = await _stateStore.LoadAsync(StateKey(settings), token);
            int total = 0;
            long last = cursor;
            while (true)
            {
                List<ElectricCheckRecordPayload> records = await ReadBatchAsync(settings, last, token);
                if (records.Count == 0) return new ElectricCheckSyncResult(total, last, total == 0 ? "没有新数据" : $"已同步 {total} 条");
                await SendBatchAsync(settings, records, token);
                last = records[^1].SourceRowId;
                await _stateStore.SaveAsync(StateKey(settings), last, token);
                total += records.Count;
            }
        }
        finally { _gate.Release(); }
    }

    /// <summary>出站前刷新本件全部明细，包括游标已经越过但后来被更新的项目；不改变全库同步游标。</summary>
    public async Task SyncRangeAsync(ElectricCheckSyncSettings settings, long before, long last, CancellationToken token)
    {
        Validate(settings);
        if (before < 0 || last <= before) throw new ArgumentException("检测行号范围无效。");
        await _gate.WaitAsync(token);
        try
        {
            long cursor = before;
            while (true)
            {
                List<ElectricCheckRecordPayload> records = await ReadBatchAsync(settings, cursor, token, last);
                if (records.Count == 0) return;
                await SendBatchAsync(settings, records, token);
                cursor = records[^1].SourceRowId;
            }
        }
        finally { _gate.Release(); }
    }

    private async Task SendBatchAsync(ElectricCheckSyncSettings settings, List<ElectricCheckRecordPayload> records, CancellationToken token)
    {
        ElectricCheckBatchPayload payload = new()
        {
            RequestId = Guid.NewGuid().ToString("N"), DeviceNo = settings.DeviceNo, Records = records
        };
        HttpResponseMessage response;
        try
        {
            response = await _httpClient.PostAsJsonAsync(
                new Uri(new Uri(settings.ServerBaseUrl.TrimEnd('/') + "/"), "openapi/electric-check/records:batch"), payload, JsonOptions, token);
        }
        catch (HttpRequestException ex)
        {
            throw new InvalidOperationException($"无法连接服务端 {settings.ServerBaseUrl}: {ex.Message}", ex);
        }
        using (response)
        {
            string body = await response.Content.ReadAsStringAsync(token);
            if (!response.IsSuccessStatusCode)
                throw new InvalidOperationException($"服务端返回 {(int)response.StatusCode} {response.ReasonPhrase}:\r\n{body}");
            using JsonDocument json = JsonDocument.Parse(body);
            if (!json.RootElement.TryGetProperty("code", out JsonElement code) || code.GetInt32() != 200)
                throw new InvalidOperationException("服务端未确认电检明细入库，同步游标保持不变。");
        }
    }

    public async Task RunAsync(ElectricCheckSyncSettings settings, Action<ElectricCheckSyncResult> report, CancellationToken token)
    {
        using PeriodicTimer timer = new(TimeSpan.FromSeconds(Math.Max(1, settings.PollIntervalSeconds)));
        do
        {
            try { report(await SyncOnceAsync(settings, token)); }
            catch (OperationCanceledException) when (token.IsCancellationRequested) { break; }
            catch (Exception ex) { report(new ElectricCheckSyncResult(0, 0, ex.Message)); }
        } while (await timer.WaitForNextTickAsync(token));
    }

    private static async Task<List<ElectricCheckRecordPayload>> ReadBatchAsync(ElectricCheckSyncSettings settings, long cursor, CancellationToken token, long? last = null)
    {
        string table = QuoteIdentifier(settings.TableName);
        string sql = $"SELECT rowid, TestDateTime, LightName, TestItemName, HighLimit, LowerLimit, TestValue, TestResult, LightType, TestID, TemplateItemID, TemplateItemName, Remark, SNCode FROM {table} WHERE rowid > $cursor AND rowid <= $last ORDER BY rowid LIMIT $limit";
        using SqliteConnection connection = new(new SqliteConnectionStringBuilder { DataSource = settings.SqliteFilePath, Mode = SqliteOpenMode.ReadOnly }.ToString());
        await connection.OpenAsync(token);
        using SqliteCommand command = new(sql, connection);
        command.Parameters.AddWithValue("$cursor", cursor);
        command.Parameters.AddWithValue("$last", last ?? long.MaxValue);
        command.Parameters.AddWithValue("$limit", Math.Clamp(settings.BatchSize, 1, 200));
        using SqliteDataReader reader = await command.ExecuteReaderAsync(token);
        List<ElectricCheckRecordPayload> result = [];
        while (await reader.ReadAsync(token)) result.Add(Map(reader));
        return result;
    }

    private static ElectricCheckRecordPayload Map(SqliteDataReader reader) => new()
    {
        SourceRowId = reader.GetInt64(0),
        TestDateTime = FormatTestDateTimeForApi(reader.GetString(1)),
        LightName = Required(reader.GetValue(2)),
        TestItemName = Required(reader.GetValue(3)),
        HighLimit = Optional(reader.GetValue(4)),
        LowerLimit = Optional(reader.GetValue(5)),
        TestValue = Optional(reader.GetValue(6)),
        TestResult = Required(reader.GetValue(7)),
        LightType = Required(reader.GetValue(8)),
        TestId = Required(reader.GetValue(9)),
        TemplateItemId = int.TryParse(Optional(reader.GetValue(10)), out int id) ? id : 0,
        TemplateItemName = Optional(reader.GetValue(11)),
        Remark = Optional(reader.GetValue(12)),
        // 采集端仅上传原始明细；SN由服务端同事负责绑定，不采用仪器原始SN。
        SnCode = null
    };

    private static string Required(object value) => Optional(value) ?? throw new InvalidDataException("电检记录存在必填字段为空");
    private static string? Optional(object value) => value is DBNull ? null : value?.ToString()?.Trim() is { Length: > 0 } text ? text : null;
    public static string FormatTestDateTimeForApi(string value) => DateTime.Parse(value, CultureInfo.InvariantCulture).ToString("yyyy-MM-dd HH:mm:ss.fffffff", CultureInfo.InvariantCulture);
    private static string QuoteIdentifier(string value) => string.IsNullOrWhiteSpace(value) ? throw new ArgumentException("表名不能为空") : "\"" + value.Replace("\"", "\"\"") + "\"";
    internal static string StateKey(ElectricCheckSyncSettings s) => $"{s.ServerBaseUrl.TrimEnd('/')}|{s.DeviceNo}|{Path.GetFullPath(s.SqliteFilePath)}|{s.TableName}";
    internal static void Validate(ElectricCheckSyncSettings s)
    {
        if (string.IsNullOrWhiteSpace(s.DeviceNo) || string.IsNullOrWhiteSpace(s.SqliteFilePath) || string.IsNullOrWhiteSpace(s.ServerBaseUrl)) throw new ArgumentException("同步配置不完整");
        if (!string.Equals(s.TableName, "ElectricCheck", StringComparison.OrdinalIgnoreCase)) throw new ArgumentException("同步只支持 ElectricCheck 表");
        if (!File.Exists(s.SqliteFilePath)) throw new FileNotFoundException("SQLite 文件不存在", s.SqliteFilePath);
    }

    public void Dispose() { _httpClient.Dispose(); _gate.Dispose(); }
}
