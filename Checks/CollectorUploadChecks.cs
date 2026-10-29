using System.Net;
using System.Text;
using System.Text.Json;
using c_lan.Configuration;
using c_lan.Models;
using c_lan.Services;
using Microsoft.Data.Sqlite;

internal static class CollectorUploadChecks
{
    internal static async Task RunAsync()
    {
        using Fixture fixture = new();
        fixture.Server.FailNext = true;
        bool failed = false;
        try { await fixture.Sync.SyncOnceAsync(fixture.Settings, CancellationToken.None); }
        catch (InvalidOperationException) { failed = true; }
        Check(failed, "network failure must be reported");
        ElectricCheckSyncResult sent = await fixture.Sync.SyncOnceAsync(fixture.Settings, CancellationToken.None);
        Check(sent.Sent == 3 && sent.LastRowId == 3, "failed batches must retry without advancing the cursor");
        ElectricCheckSyncResult empty = await fixture.Sync.SyncOnceAsync(fixture.Settings, CancellationToken.None);
        Check(empty.Sent == 0 && fixture.Server.Rows.SequenceEqual(new long[] { 1, 2, 3 }), "successful batches must not resend uploaded rows");
        using ElectricCheckSyncService restarted = new(new ElectricCheckSyncStateStore(fixture.Directory), new HttpClient(fixture.Server, false));
        Check((await restarted.SyncOnceAsync(fixture.Settings, CancellationToken.None)).Sent == 0, "cursor must survive restart");
        Check(fixture.Server.Paths.All(p => p == "/openapi/electric-check/records:batch"), "collector must never call scan or completion endpoints");
        Console.WriteLine("Collector SQLite batch upload, retry, cursor and endpoint checks passed.");
    }

    internal sealed class Fixture : IDisposable
    {
        internal readonly string Directory = Path.Combine(Path.GetTempPath(), "c-lan-collector-" + Guid.NewGuid().ToString("N"));
        internal readonly Server Server = new();
        internal readonly ElectricCheckSyncService Sync;
        internal readonly ElectricCheckSyncSettings Settings;
        internal Fixture()
        {
            System.IO.Directory.CreateDirectory(Directory);
            string file = Path.Combine(Directory, "source.db");
            using (SqliteConnection connection = new($"Data Source={file};Pooling=False"))
            {
                connection.Open();
                using SqliteCommand command = connection.CreateCommand();
                command.CommandText = """
                    CREATE TABLE ElectricCheck (TestDateTime TEXT,LightName TEXT,TestItemName TEXT,HighLimit TEXT,LowerLimit TEXT,TestValue TEXT,TestResult TEXT,LightType TEXT,TestID TEXT,TemplateItemID INTEGER,TemplateItemName TEXT,Remark TEXT,SNCode TEXT);
                    INSERT INTO ElectricCheck VALUES ('2026-10-28 08:00:00','L','Item',NULL,NULL,'1','OK','Type','T',1,NULL,NULL,'INSTRUMENT-SN');
                    INSERT INTO ElectricCheck SELECT * FROM ElectricCheck;
                    INSERT INTO ElectricCheck SELECT * FROM ElectricCheck LIMIT 1;
                    """;
                command.ExecuteNonQuery();
            }
            Settings = new() { Enabled = true, DeviceNo = "COLLECTOR-1", SqliteFilePath = file, ServerBaseUrl = "http://collector.invalid", BatchSize = 2, PollIntervalSeconds = 1 };
            Sync = new(new ElectricCheckSyncStateStore(Directory), new HttpClient(Server, false));
        }
        public void Dispose()
        {
            Sync.Dispose();
            SqliteConnection.ClearAllPools();
            string full = Path.GetFullPath(Directory);
            if (!full.StartsWith(Path.GetFullPath(Path.GetTempPath()), StringComparison.OrdinalIgnoreCase) || !Path.GetFileName(full).StartsWith("c-lan-collector-")) throw new InvalidOperationException("Invalid fixture cleanup path");
            System.IO.Directory.Delete(full, true);
        }
    }
    internal sealed class Server : HttpMessageHandler
    {
        internal bool FailNext;
        internal readonly List<string> Paths = [];
        internal readonly List<long> Rows = [];
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken token)
        {
            Paths.Add(request.RequestUri!.AbsolutePath);
            Check(request.Method == HttpMethod.Post && Paths[^1] == "/openapi/electric-check/records:batch", "unexpected SN business HTTP request");
            using JsonDocument json = JsonDocument.Parse(await request.Content!.ReadAsStringAsync(token));
            Check(json.RootElement.GetProperty("deviceNo").GetString() == "COLLECTOR-1", "upload must retain configured device");
            if (FailNext) { FailNext = false; return new(HttpStatusCode.ServiceUnavailable) { Content = new StringContent("retry") }; }
            foreach (JsonElement row in json.RootElement.GetProperty("records").EnumerateArray())
            {
                Check(!row.TryGetProperty("snCode", out JsonElement sn) || sn.ValueKind == JsonValueKind.Null, "collector must leave SN binding to the server");
                Rows.Add(row.GetProperty("sourceRowId").GetInt64());
            }
            return new(HttpStatusCode.OK) { Content = new StringContent("{\"code\":200}", Encoding.UTF8, "application/json") };
        }
    }
    private static void Check(bool condition, string message) { if (!condition) throw new InvalidOperationException(message); }
}
