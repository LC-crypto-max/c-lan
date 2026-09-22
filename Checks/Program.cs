using c_lan.Utilities;
using c_lan.Configuration;
using c_lan.Models;
using c_lan.Services;
using System.Text.Json;
using Microsoft.Data.Sqlite;

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

static void Assert(bool condition, string message)
{
    if (!condition) throw new InvalidOperationException(message);
}
