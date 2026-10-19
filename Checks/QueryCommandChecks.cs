using c_lan.Data;
using c_lan.Models;
using c_lan.Services;
using c_lan.Utilities;
using Microsoft.Data.Sqlite;

internal static class QueryCommandChecks
{
    internal static async Task RunAsync()
    {
        string databasePath = Path.Combine(Path.GetTempPath(), $"c-lan-commands-{Guid.NewGuid():N}.db");
        try
        {
            using (SqliteConnection setup = new($"Data Source={databasePath};Pooling=False"))
                await setup.OpenAsync();
            QueryService service = new(new DatabaseProviderFactory(), new ReadOnlySqlValidator());
            ConnectionProfile profile = new() { DatabaseType = DatabaseType.SQLite, DatabaseFilePath = databasePath };
            Task<QueryResult> Execute(string sql, bool readOnly = false) => service.ExecuteAsync(profile,
                new QueryRequest { SqlText = sql, IsReadOnly = readOnly }, CancellationToken.None);

            async Task<QueryResult> Succeeds(string sql)
            {
                QueryResult result = await Execute(sql);
                Assert(result.IsSuccess, $"{sql}: {result.ErrorMessage}");
                return result;
            }

            await Succeeds("-- writable mode must allow schema changes\nCREATE TABLE commands (id INTEGER PRIMARY KEY, sn TEXT);");
            await Succeeds("CREATE INDEX sn_index ON commands(sn);");
            QueryResult inserted = await Succeeds("INSERT INTO commands VALUES (1, 'SN-A'), (2, 'SN-B'), (3, 'SN-C');");
            Assert(inserted.AffectedRows == 3, "INSERT must report its affected row count");
            QueryResult createdAfterInsert = await Succeeds("CREATE TABLE count_probe (id INTEGER);");
            Assert(createdAfterInsert.AffectedRows is null, "CREATE must not report a previous INSERT count");
            QueryResult droppedAfterInsert = await Succeeds("DROP TABLE count_probe;");
            Assert(droppedAfterInsert.AffectedRows is null, "DROP must not report a previous INSERT count");
            QueryResult pragmaAfterInsert = await Succeeds("PRAGMA user_version=1;");
            Assert(pragmaAfterInsert.AffectedRows is null, "PRAGMA must not report a previous INSERT count");
            foreach (string write in new[]
            {
                "INSERT INTO commands VALUES (4, 'SN-D');", "UPDATE commands SET sn='OTHER';",
                "DELETE FROM commands;", "REPLACE INTO commands VALUES (1, 'OTHER');",
                "CREATE TABLE forbidden (id INTEGER);", "ALTER TABLE commands ADD COLUMN extra TEXT;",
                "DROP TABLE commands;", "WITH candidate AS (SELECT 4, 'SN-D') INSERT INTO commands SELECT * FROM candidate;"
            })
                Assert(!(await Execute(write, true)).IsSuccess, $"read-only mode allowed: {write}");
            QueryResult original = await Execute("SELECT * FROM commands ORDER BY id;", true);
            Assert(original.IsSuccess && original.RowCount == 3 && original.Rows!.Rows[0][1].ToString() == "SN-A",
                "read-only rejections must leave stored data unchanged");

            QueryResult updated = await Succeeds("UPDATE commands SET sn='SN-EDITED' WHERE id=1;");
            Assert(updated.AffectedRows == 1, "UPDATE must report its affected row count");
            await Succeeds("REPLACE INTO commands VALUES (2, 'SN-REPLACED');");
            await Succeeds("WITH candidate AS (SELECT 4, 'SN-D') INSERT INTO commands SELECT * FROM candidate;");
            QueryResult deleted = await Succeeds("DELETE FROM commands WHERE id=3;");
            Assert(deleted.AffectedRows == 1, "DELETE must report its affected row count");
            await Succeeds("-- comments must not change command dispatch\nALTER TABLE commands RENAME COLUMN sn TO sn_code;");
            QueryResult edited = await Execute("SELECT sn_code FROM commands ORDER BY id;", true);
            Assert(edited.IsSuccess && edited.RowCount == 3 && edited.Rows!.Rows[0][0].ToString() == "SN-EDITED"
                && edited.Rows.Rows[1][0].ToString() == "SN-REPLACED", "writes must persist and queries must still return rows");
            QueryResult returning = await Succeeds("INSERT INTO commands VALUES (5, 'SN-RETURNING') RETURNING id, sn_code;");
            Assert(returning.RowCount == 1 && returning.Rows!.Rows[0][1].ToString() == "SN-RETURNING", "RETURNING must display its result");
            QueryResult pragma = await Succeeds("PRAGMA table_info(commands);");
            Assert(pragma.RowCount == 2, "commands that return metadata must display their result");
            Assert(!(await Execute("DELETE FROM commands; DROP TABLE commands;")).IsSuccess, "one command per execution must remain enforced");
            Assert(!(await Execute("-- empty SQL")).IsSuccess, "empty SQL must remain rejected");
            await Succeeds("DROP INDEX sn_index;");
            await Succeeds("DROP TABLE commands;");
            QueryResult dropped = await Execute("SELECT name FROM sqlite_master WHERE name='commands';", true);
            Assert(dropped.IsSuccess && dropped.RowCount == 0, "DROP must persist");
            Console.WriteLine("Writable SQL commands and read-only protection checks passed.");
        }
        finally
        {
            SqliteConnection.ClearAllPools();
            File.Delete(databasePath);
        }
    }

    private static void Assert(bool condition, string message)
    {
        if (!condition) throw new InvalidOperationException(message);
    }
}
