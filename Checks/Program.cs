using c_lan.Utilities;
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

static void Assert(bool condition, string message)
{
    if (!condition) throw new InvalidOperationException(message);
}
