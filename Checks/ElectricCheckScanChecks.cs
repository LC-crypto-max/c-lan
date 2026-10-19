using System.Globalization;
using System.Net;
using System.Text;
using System.Text.Json;
using c_lan.Configuration;
using c_lan.Models;
using c_lan.Services;
using Microsoft.Data.Sqlite;

internal static class ElectricCheckScanChecks
{
    public static async Task RunAsync()
    {
        using Fixture f = new();
        ElectricCheckScanState entered = await f.Scans.StartAsync(f.Settings, " SN-A ", CancellationToken.None);
        Check(entered.Status == "TESTING" && entered.SnCode == "SN-A", "scan must enter with configured device and trimmed SN");
        Check(f.Server.LastBaseline == 1 && f.Server.DeviceNo == "EC-1", "scan must snapshot source MAX(rowid), not the old sync cursor");
        bool refused = false;
        try { await f.Scans.StartAsync(f.Settings, "SN-B", CancellationToken.None); }
        catch (InvalidOperationException) { refused = true; }
        Check(refused, "another product cannot enter while the first is active");

        f.Clock.Advance(60);
        await f.Scans.PollAsync(f.Settings, CancellationToken.None);
        Check(f.Server.CompleteCalls == 0, "no results must never mean completed");
        f.AddRow(2, "10");
        f.Clock.Advance(60);
        await f.Scans.PollAsync(f.Settings, CancellationToken.None);
        Check(f.Server.CompleteCalls == 0, "first observed results must start a full quiet minute");
        f.Clock.Advance(59);
        await f.Scans.PollAsync(f.Settings, CancellationToken.None);
        Check(f.Server.CompleteCalls == 0, "59 seconds of silence is insufficient");
        f.AddRow(3, "20");
        f.Clock.Advance(1);
        await f.Scans.PollAsync(f.Settings, CancellationToken.None);
        Check(f.Server.CompleteCalls == 0, "new rows must restart the quiet minute");
        f.Clock.Advance(60);
        ElectricCheckScanState? done = await f.Scans.PollAsync(f.Settings, CancellationToken.None);
        Check(done?.Status == "COMPLETED", "one unchanged minute must complete this scan");
        Check(f.Server.UploadedRows.SetEquals([2L, 3L]), "all current product rows must upload before completion");
        Check(f.Server.CompletedCount == 2 && f.Server.CompletedLastRow == 3, "completion must exclude historical rows and carry this product's count");
        int calls = f.Server.CompleteCalls;
        f.Clock.Advance(60);
        await f.Scans.PollAsync(f.Settings, CancellationToken.None);
        Check(f.Server.CompleteCalls == calls, "completed scans must not repeatedly exit");

        using Fixture updated = new();
        await updated.Scans.StartAsync(updated.Settings, "SN-A", CancellationToken.None);
        updated.AddRow(2, "10");
        await updated.Scans.PollAsync(updated.Settings, CancellationToken.None);
        await updated.SyncAsync();
        updated.Clock.Advance(60);
        updated.UpdateRow(2, "99");
        await updated.Scans.PollAsync(updated.Settings, CancellationToken.None);
        Check(updated.Server.CompleteCalls == 0, "updated result values must reset silence even when rowid does not change");
        updated.Clock.Advance(60);
        await updated.Scans.PollAsync(updated.Settings, CancellationToken.None);
        Check(updated.Server.CompleteCalls == 1, "updated results may complete after a fresh quiet minute");
        Check(updated.Server.UploadedValues[2] == "99", "completion must refresh values even after the incremental cursor passed the row");

        using Fixture failedUpload = new();
        await failedUpload.Scans.StartAsync(failedUpload.Settings, "SN-A", CancellationToken.None);
        failedUpload.AddRow(2, "10");
        await failedUpload.Scans.PollAsync(failedUpload.Settings, CancellationToken.None);
        failedUpload.Clock.Advance(60);
        failedUpload.Server.FailUpload = true;
        await RejectAsync(() => failedUpload.Scans.PollAsync(failedUpload.Settings, CancellationToken.None));
        Check(failedUpload.Server.CompleteCalls == 0, "failed uploads must prevent exit");
        failedUpload.Server.FailUpload = false;
        await failedUpload.Scans.PollAsync(failedUpload.Settings, CancellationToken.None);
        Check(failedUpload.Server.CompleteCalls == 1, "a successful retry may finish once all rows are uploaded");

        using Fixture late = new();
        await late.Scans.StartAsync(late.Settings, "SN-A", CancellationToken.None);
        late.AddRow(2, "10");
        await late.Scans.PollAsync(late.Settings, CancellationToken.None);
        late.Clock.Advance(60);
        late.Server.OnUpload = () => late.AddRow(3, "late");
        await late.Scans.PollAsync(late.Settings, CancellationToken.None);
        Check(late.Server.CompleteCalls == 0, "results arriving during upload must cancel the pending exit and restart silence");

        using Fixture restart = new();
        await restart.Scans.StartAsync(restart.Settings, "SN-A", CancellationToken.None);
        restart.AddRow(2, "10");
        await restart.Scans.PollAsync(restart.Settings, CancellationToken.None);
        restart.Clock.Advance(600);
        restart.Restart();
        await restart.Scans.PollAsync(restart.Settings, CancellationToken.None);
        Check(restart.Server.CompleteCalls == 0, "unobserved downtime must not count as a quiet minute");
        restart.Clock.Advance(60);
        await restart.Scans.PollAsync(restart.Settings, CancellationToken.None);
        Check(restart.Server.CompleteCalls == 1, "recovered scans may finish after observing a full minute");

        using Fixture retry = new();
        await retry.Scans.StartAsync(retry.Settings, "SN-A", CancellationToken.None);
        retry.AddRow(2, "10");
        await retry.Scans.PollAsync(retry.Settings, CancellationToken.None);
        retry.Clock.Advance(60);
        retry.Server.FailComplete = true;
        await RejectAsync(() => retry.Scans.PollAsync(retry.Settings, CancellationToken.None));
        string original = retry.Server.CompleteBodies.Single();
        retry.Server.FailComplete = false;
        retry.Restart();
        retry.Clock.Advance(60);
        await retry.Scans.PollAsync(retry.Settings, CancellationToken.None);
        Check(retry.Server.CompleteBodies.Count == 2 && retry.Server.CompleteBodies[1] == original,
            "ambiguous completion must retry the exact persisted payload across restart");

        using Fixture acknowledged = new();
        await acknowledged.Scans.StartAsync(acknowledged.Settings, "SN-A", CancellationToken.None);
        acknowledged.AddRow(2, "10");
        await acknowledged.Scans.PollAsync(acknowledged.Settings, CancellationToken.None);
        acknowledged.Clock.Advance(60); acknowledged.Server.LoseCompleteResponse = true;
        await RejectAsync(() => acknowledged.Scans.PollAsync(acknowledged.Settings, CancellationToken.None));
        acknowledged.Restart();
        ElectricCheckScanState? recovered = await acknowledged.Scans.PollAsync(acknowledged.Settings, CancellationToken.None);
        Check(recovered?.Status == "COMPLETED" && acknowledged.Server.CompleteCalls == 1,
            "a committed completion with a lost response must recover from server status");

        using Fixture businessError = new();
        businessError.Server.FailUploadBusiness = true;
        bool businessRejected = false;
        try { await businessError.SyncAsync(); }
        catch (InvalidOperationException) { businessRejected = true; }
        Check(businessRejected, "HTTP200 with business failure must not acknowledge a batch");
        businessError.Server.FailUploadBusiness = false;
        Check((await businessError.SyncAsync()).Sent == 1, "business failure must leave the incremental cursor unchanged");
        Console.WriteLine("Electric check scan and one-minute completion checks passed.");
    }

    private static void Check(bool value, string message)
    {
        if (!value) throw new InvalidOperationException(message);
    }

    private static async Task RejectAsync(Func<Task<ElectricCheckScanState?>> action)
    {
        try { await action(); }
        catch (Exception ex) when (ex is HttpRequestException or InvalidOperationException) { return; }
        throw new InvalidOperationException("operation should have failed");
    }

    private sealed class ManualClock : TimeProvider
    {
        private DateTimeOffset _now = DateTimeOffset.Parse("2026-10-19T09:00:00+08:00", CultureInfo.InvariantCulture);
        private long _elapsedTicks;
        public override DateTimeOffset GetUtcNow() => _now.ToUniversalTime();
        public override long TimestampFrequency => TimeSpan.TicksPerSecond;
        public override long GetTimestamp() => _elapsedTicks;
        public void Advance(int seconds) { _now = _now.AddSeconds(seconds); _elapsedTicks += TimeSpan.FromSeconds(seconds).Ticks; }
    }

    private sealed class Fixture : IDisposable
    {
        private readonly string _directory = Path.Combine(Path.GetTempPath(), $"c-lan-scan-{Guid.NewGuid():N}");
        public readonly ElectricCheckSyncSettings Settings;
        public readonly ManualClock Clock = new();
        public readonly FakeServer Server = new();
        private readonly ElectricCheckSyncService _sync;
        public ElectricCheckScanService Scans { get; private set; }
        public Fixture()
        {
            Directory.CreateDirectory(_directory);
            Settings = new() { DeviceNo = "EC-1", SqliteFilePath = Path.Combine(_directory, "results.db"), ServerBaseUrl = "http://electric-check.test" };
            Execute("CREATE TABLE ElectricCheck (TestDateTime TEXT,LightName TEXT,TestItemName TEXT,HighLimit TEXT,LowerLimit TEXT,TestValue TEXT,TestResult TEXT,LightType TEXT,TestID TEXT,TemplateItemID INTEGER,TemplateItemName TEXT,Remark TEXT,SNCode TEXT)");
            AddRow(1, "history");
            _sync = new(new ElectricCheckSyncStateStore(_directory), new HttpClient(Server, disposeHandler: false));
            Scans = CreateService();
        }
        private ElectricCheckScanService CreateService() => new(_sync, new ElectricCheckScanStateStore(_directory), new HttpClient(Server, disposeHandler: false), Clock);
        public void Restart() { Scans.Dispose(); Scans = CreateService(); }
        public Task<ElectricCheckSyncResult> SyncAsync() => _sync.SyncOnceAsync(Settings, CancellationToken.None);
        public void AddRow(long row, string value)
        {
            using SqliteConnection connection = new($"Data Source={Settings.SqliteFilePath};Pooling=False");
            connection.Open();
            using SqliteCommand command = connection.CreateCommand();
            command.CommandText = "INSERT INTO ElectricCheck(rowid,TestDateTime,LightName,TestItemName,TestValue,TestResult,LightType,TestID) VALUES($row,'2026-10-19 09:00:10.123','灯','电流',$value,'OK','近光','test')";
            command.Parameters.AddWithValue("$row", row); command.Parameters.AddWithValue("$value", value);
            command.ExecuteNonQuery();
        }
        public void UpdateRow(long row, string value)
        {
            using SqliteConnection connection = new($"Data Source={Settings.SqliteFilePath};Pooling=False");
            connection.Open(); using SqliteCommand command = connection.CreateCommand();
            command.CommandText = "UPDATE ElectricCheck SET TestValue=$value WHERE rowid=$row";
            command.Parameters.AddWithValue("$value", value); command.Parameters.AddWithValue("$row", row);
            command.ExecuteNonQuery();
        }
        private void Execute(string sql)
        {
            using SqliteConnection connection = new($"Data Source={Settings.SqliteFilePath};Pooling=False");
            connection.Open(); using SqliteCommand command = new(sql, connection); command.ExecuteNonQuery();
        }
        public void Dispose()
        {
            Scans.Dispose(); _sync.Dispose(); Server.Dispose();
            SqliteConnection.ClearAllPools();
            string resolved = Path.GetFullPath(_directory);
            string temporaryRoot = Path.GetFullPath(Path.GetTempPath()).TrimEnd(Path.DirectorySeparatorChar) + Path.DirectorySeparatorChar;
            if (!resolved.StartsWith(temporaryRoot, StringComparison.OrdinalIgnoreCase)
                || !Path.GetFileName(resolved).StartsWith("c-lan-scan-", StringComparison.Ordinal))
                throw new InvalidOperationException("Unexpected test cleanup path");
            Directory.Delete(_directory, recursive: true);
        }
    }

    private sealed class FakeServer : HttpMessageHandler
    {
        private JsonElement? _scan;
        private string _status = "TESTING";
        public long LastBaseline;
        public string? DeviceNo;
        public int CompleteCalls;
        public int CompletedCount;
        public long CompletedLastRow;
        public bool FailUpload;
        public bool FailUploadBusiness;
        public bool FailComplete;
        public bool LoseCompleteResponse;
        public Action? OnUpload;
        public readonly HashSet<long> UploadedRows = [];
        public readonly Dictionary<long, string?> UploadedValues = [];
        public readonly List<string> CompleteBodies = [];
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            string path = request.RequestUri!.AbsolutePath;
            string body = request.Content is null ? "{}" : await request.Content.ReadAsStringAsync(cancellationToken);
            using JsonDocument json = JsonDocument.Parse(body);
            JsonElement data = json.RootElement;
            if (path.EndsWith("records:batch"))
            {
                if (FailUpload) return Reply(new { code = 500, msg = "upload rejected" }, HttpStatusCode.InternalServerError);
                if (FailUploadBusiness) return Reply(new { code = 500, msg = "business rejected" });
                foreach (JsonElement item in data.GetProperty("records").EnumerateArray())
                {
                    long row = item.GetProperty("sourceRowId").GetInt64();
                    UploadedRows.Add(row); UploadedValues[row] = item.GetProperty("testValue").GetString();
                }
                Action? action = OnUpload; OnUpload = null; action?.Invoke();
                return Reply(new { code = 200 });
            }
            if (path.EndsWith("/complete"))
            {
                CompleteCalls++; CompleteBodies.Add(body);
                CompletedCount = data.GetProperty("expectedRecordCount").GetInt32();
                CompletedLastRow = data.GetProperty("lastSourceRowId").GetInt64();
                Check(UploadedRows.Contains(CompletedLastRow), "completion must follow successful result upload");
                if (FailComplete) return Reply(new { code = 500 }, HttpStatusCode.InternalServerError);
                _status = "COMPLETED";
                if (LoseCompleteResponse)
                {
                    LoseCompleteResponse = false;
                    return Reply(new { code = 500 }, HttpStatusCode.InternalServerError);
                }
                return ScanReply();
            }
            if (request.Method == HttpMethod.Post && path.EndsWith("/scans"))
            {
                _scan = data.Clone(); _status = "TESTING";
                LastBaseline = data.GetProperty("sourceRowIdBeforeScan").GetInt64();
                DeviceNo = data.GetProperty("deviceNo").GetString();
                return ScanReply();
            }
            if (request.Method == HttpMethod.Get)
                return _scan is null || (path.EndsWith("/current") && _status != "TESTING") ? Reply(new { code = 200 }) : ScanReply();
            throw new InvalidOperationException("Unexpected request: " + request.RequestUri);
        }
        private HttpResponseMessage ScanReply()
        {
            JsonElement scan = _scan!.Value;
            return Reply(new { code = 200, data = new {
                scanId = scan.GetProperty("scanId").GetString(), deviceNo = DeviceNo,
                snCode = scan.GetProperty("snCode").GetString(), scanTime = scan.GetProperty("scanTime").GetString(),
                sourceRowIdBeforeScan = LastBaseline, status = _status
            }});
        }
        private static HttpResponseMessage Reply(object value, HttpStatusCode status = HttpStatusCode.OK) => new(status)
        { Content = new StringContent(JsonSerializer.Serialize(value), Encoding.UTF8, "application/json") };
    }
}
