using c_lan.Configuration;
using c_lan.Models;
using Microsoft.Data.Sqlite;
using System.Globalization;
using System.Net.Http.Json;
using System.Text.Json;

namespace c_lan.Services;

public sealed class ElectricCheckSyncService : IDisposable
{
    private readonly HttpClient _httpClient = new() { Timeout = TimeSpan.FromSeconds(15) };
    private readonly ElectricCheckSyncStateStore _stateStore;
    private readonly SemaphoreSlim _gate = new(1, 1);
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public ElectricCheckSyncService(ElectricCheckSyncStateStore stateStore) => _stateStore = stateStore;

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
                ElectricCheckBatchPayload payload = new()
                {
                    RequestId = $"{settings.DeviceNo}-{records[0].SourceRowId}-{records[^1].SourceRowId}",
                    DeviceNo = settings.DeviceNo,
                    Records = records
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
                    if (!response.IsSuccessStatusCode)
                    {
                        string body = await response.Content.ReadAsStringAsync(token);
                        throw new InvalidOperationException($"服务端返回 {(int)response.StatusCode} {response.ReasonPhrase}:\r\n{body}");
                    }
                }
                last = records[^1].SourceRowId;
                await _stateStore.SaveAsync(StateKey(settings), last, token);
                total += records.Count;
            }
        }
        finally { _gate.Release(); }
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

    private static async Task<List<ElectricCheckRecordPayload>> ReadBatchAsync(ElectricCheckSyncSettings settings, long cursor, CancellationToken token)
    {
        string table = QuoteIdentifier(settings.TableName);
        string sql = $"SELECT rowid, TestDateTime, LightName, TestItemName, HighLimit, LowerLimit, TestValue, TestResult, LightType, TestID, TemplateItemID, TemplateItemName, Remark, SNCode FROM {table} WHERE rowid > $cursor ORDER BY rowid LIMIT $limit";
        using SqliteConnection connection = new(new SqliteConnectionStringBuilder { DataSource = settings.SqliteFilePath, Mode = SqliteOpenMode.ReadOnly }.ToString());
        await connection.OpenAsync(token);
        using SqliteCommand command = new(sql, connection);
        command.Parameters.AddWithValue("$cursor", cursor);
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
        SnCode = Optional(reader.GetValue(13))
    };

    private static string Required(object value) => Optional(value) ?? throw new InvalidDataException("电检记录存在必填字段为空");
    private static string? Optional(object value) => value is DBNull ? null : value?.ToString()?.Trim() is { Length: > 0 } text ? text : null;
    public static string FormatTestDateTimeForApi(string value) =>
        DateTime.Parse(value, CultureInfo.InvariantCulture)
            .ToString("yyyy-MM-dd HH:mm:ss.fffffff", CultureInfo.InvariantCulture);
    private static string QuoteIdentifier(string value) => string.IsNullOrWhiteSpace(value) ? throw new ArgumentException("表名不能为空") : "\"" + value.Replace("\"", "\"\"") + "\"";
    private static string StateKey(ElectricCheckSyncSettings s) => $"{s.DeviceNo}|{s.SqliteFilePath}|{s.TableName}";
    private static void Validate(ElectricCheckSyncSettings s)
    {
        if (string.IsNullOrWhiteSpace(s.DeviceNo) || string.IsNullOrWhiteSpace(s.SqliteFilePath) || string.IsNullOrWhiteSpace(s.ServerBaseUrl)) throw new ArgumentException("同步配置不完整");
        if (!string.Equals(s.TableName, "ElectricCheck", StringComparison.OrdinalIgnoreCase)) throw new ArgumentException("同步只支持 ElectricCheck 表");
        if (!File.Exists(s.SqliteFilePath)) throw new FileNotFoundException("SQLite 文件不存在", s.SqliteFilePath);
    }

    public void Dispose() { _httpClient.Dispose(); _gate.Dispose(); }
}
