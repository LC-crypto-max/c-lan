using c_lan.Utilities;
using c_lan.Configuration;
using c_lan.Models;
using c_lan.Services;
using System.Text.Json;
using Microsoft.Data.Sqlite;
using c_lan.Data;

int evaluatedRows = 0;

using SqliteConnection connection = new("Data Source=:memory:");
await connection.OpenAsync();
connection.CreateFunction("count_row", (long value) =>
{
    evaluatedRows++;
    return value;
});

using SqliteCommand command = connection.CreateCommand();
command.CommandText = """
    WITH RECURSIVE numbers(value) AS
    (
        SELECT 1
        UNION ALL
        SELECT value + 1 FROM numbers WHERE value < 10000
    )
    SELECT count_row(value) AS value FROM numbers
    """;

using SqliteDataReader reader = await command.ExecuteReaderAsync();
(System.Data.DataTable table, bool isTruncated) =
    await BoundedDataTableReader.LoadAsync(reader, 2, CancellationToken.None);

Assert(table.Rows.Count == 2, $"expected 2 rows, got {table.Rows.Count}");
Assert(isTruncated, "expected the result to be marked as truncated");
Assert(evaluatedRows == 3, $"expected 3 rows to be read, got {evaluatedRows}");

Console.WriteLine("Bounded query read check passed.");

ElectricCheckBatchPayload payload = new() { RequestId = "r", DeviceNo = "d", Records = [new ElectricCheckRecordPayload { SourceRowId = 1, LightName = "灯", TestItemName = "项目", TestResult = "OK", LightType = "类型", TestId = "t" }] };
string payloadJson = JsonSerializer.Serialize(payload, new JsonSerializerOptions(JsonSerializerDefaults.Web));
Assert(payloadJson.Contains("\"sourceRowId\":1"), "payload must use camelCase sourceRowId");
Assert(payloadJson.Contains("\"testDateTime\":\""), "payload must contain formatted testDateTime");
Console.WriteLine("Electric check payload check passed.");

string apiDate = ElectricCheckSyncService.FormatTestDateTimeForApi("2026-09-21 16:29:03.0931445");
Assert(apiDate == "2026-09-21 16:29:03.0931445", $"unexpected API date: {apiDate}");
Console.WriteLine("Electric check date format check passed.");

ElectricCheckSyncStateStore stateStore = new();
string stateKey = $"check-{Guid.NewGuid():N}";
await stateStore.SaveAsync(stateKey, 123, CancellationToken.None);
await stateStore.ResetAsync(stateKey, CancellationToken.None);
Assert(await stateStore.LoadAsync(stateKey, CancellationToken.None) == 0, "sync state reset must clear the cursor");
Console.WriteLine("Electric check state reset check passed.");

string renameDatabase = Path.Combine(Path.GetTempPath(), $"c-lan-rename-{Guid.NewGuid():N}.db");
try
{
    using (SqliteConnection setup = new($"Data Source={renameDatabase};Pooling=False"))
    {
        await setup.OpenAsync();
        using SqliteCommand setupCommand = setup.CreateCommand();
        setupCommand.CommandText = "CREATE TABLE electriccheck (cn_code varchar(128) DEFAULT NULL); INSERT INTO electriccheck VALUES ('SN-001'); CREATE INDEX code_index ON electriccheck(cn_code);";
        await setupCommand.ExecuteNonQueryAsync();
    }
    QueryService queryService = new(new DatabaseProviderFactory(), new ReadOnlySqlValidator());
    ConnectionProfile profile = new() { DatabaseType = DatabaseType.SQLite, DatabaseFilePath = renameDatabase };
    const string renameSql = "-- 在此输入 SQLite 查询语句\nALTER TABLE electriccheck RENAME COLUMN cn_code TO sn_code;";
    QueryResult denied = await queryService.ExecuteAsync(profile, new QueryRequest { SqlText = renameSql, IsReadOnly = true }, CancellationToken.None);
    Assert(!denied.IsSuccess, "read-only mode must reject column renaming");
    QueryResult renamed = await queryService.ExecuteAsync(profile, new QueryRequest { SqlText = renameSql, IsReadOnly = false }, CancellationToken.None);
    Assert(renamed.IsSuccess, $"column rename failed: {renamed.ErrorMessage}");
    QueryResult data = await queryService.ExecuteAsync(profile, new QueryRequest { SqlText = "SELECT sn_code FROM electriccheck INDEXED BY code_index", IsReadOnly = true }, CancellationToken.None);
    Assert(data.IsSuccess && data.Rows?.Rows.Count == 1 && data.Rows.Rows[0][0].ToString() == "SN-001", "rename must preserve data and the index");
    Console.WriteLine("SQLite column rename and read-only checks passed.");
}
finally
{
    SqliteConnection.ClearAllPools();
    File.Delete(renameDatabase);
}

static void Assert(bool condition, string message)
{
    if (!condition) throw new InvalidOperationException(message);
}
