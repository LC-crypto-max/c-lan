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
        await CheckSnNormalizationAsync();
        await CheckContinuousQueueAsync();
        await CheckEnqueueDuringNetworkWaitAsync();
        await CheckEntryRetriesAsync();
        await CheckDuplicateServerEntryAsync();
        await CheckFifoSegmentsAsync();
        await CheckQuietMinuteAsync();
        await CheckChangedValuesAsync();
        await CheckUploadFailuresAsync();
        await CheckRestartObservationAsync();
        await CheckCompletionRetryAsync();
        await CheckLostCompletionResponseAsync();
        await CheckRepeatedTestIdAsync();
        await CheckLateResultsAsync();
        Console.WriteLine("FIFO queue, SQLite recovery, idempotent retries and result boundary checks passed.");
    }

    private static async Task CheckSnNormalizationAsync()
    {
        using Fixture f = new();
        ElectricCheckScanState trimmed = await f.EnqueueAsync("\0\t SN-X \r\0");
        Check(trimmed.SnCode == "SN-X", "SN normalization must match Java trim for all edge characters <= U+0020");
        ElectricCheckScanState unicode = await f.EnqueueAsync("\u00a0SN-X\u00a0");
        Check(unicode.SnCode == "\u00a0SN-X\u00a0", "non-ASCII spaces must remain consistent with Java registry comparison");
    }

    private static async Task CheckContinuousQueueAsync()
    {
        using Fixture f = new();
        ElectricCheckScanState first = await f.EnqueueAsync(" SN-A ");
        ElectricCheckScanState second = await f.EnqueueAsync("sn-a");
        Check(first.SnCode == "SN-A" && first.Status == "PENDING_ENTRY", "enqueue must trim SN and save a pending entry");
        Check(f.Server.Requests == 0, "local enqueue must not send or wait for any HTTP request");
        Check(f.Scans.Entries.Count == 2 && first.Sequence < second.Sequence, "continuous scanning must preserve FIFO order and case-sensitive SNs");
        await RejectAsync(() => f.EnqueueAsync("SN-A"));
        Check(f.Scans.Entries.Count == 2, "duplicate local SN must not create another entry");
        string firstId = first.ScanId;
        f.Restart();
        await f.PollAsync();
        Check(f.Scans.Entries.Count == 2 && f.Scans.Entries[0].ScanId == firstId, "all queued entries and their order must survive restart");
        Check(f.Server.Scans.Count == 2 && f.Server.EntryBodies.Count == 2, "one device must accept multiple pending products");
        Check(f.Server.StatusCalls > 0, "queue polling must use the batch status endpoint");
        Check(f.Server.Scans.Values.All(scan => scan.Identity.GetProperty("sourceRowIdBeforeScan").GetInt64() == 1), "first queue baseline must exclude instrument history");
        f.Clock.Advance(600);
        await f.PollAsync();
        Check(f.Server.Completions.Count == 0, "no result rows must never cause an automatic exit");

        using Fixture compatibility = new();
        await compatibility.Scans.StartAsync(compatibility.Settings, "SN-FIRST", CancellationToken.None);
        await compatibility.Scans.StartAsync(compatibility.Settings, "SN-SECOND", CancellationToken.None);
        Check(compatibility.Scans.Entries.Count == 2 && compatibility.Server.Scans.Count == 2,
            "legacy StartAsync must accept a second product while the first is testing");
    }

    private static async Task CheckEntryRetriesAsync()
    {
        using Fixture f = new();
        ElectricCheckScanState first = await f.EnqueueAsync("SN-A");
        await f.EnqueueAsync("SN-B");
        f.Server.FailEntry = true;
        await f.PollAllowFailureAsync();
        Check(f.Server.EntryBodies.Count == 1 && f.Server.Scans.Count == 0, "unknown first entry must block later POSTs without losing queued scans");
        string original = f.Server.EntryBodies[0];
        Check(f.Scans.Entries.Count == 2 && f.Scans.Entries[0].Status == "PENDING_ENTRY", "network errors must retain the pending queue");
        f.Restart();
        f.Server.FailEntry = false;
        await f.PollAsync();
        Check(f.Server.EntryBodies.Count == 3 && f.Server.EntryBodies[1] == original,
            "entry retry after restart must keep the exact payload before submitting the next SN");
        Check(f.Server.Scans.Keys.First() == first.ScanId && f.Server.Scans.Count == 2, "entry retry must not reorder or duplicate scans");

        using Fixture lost = new();
        ElectricCheckScanState queued = await lost.EnqueueAsync("SN-ORIGINAL");
        lost.Server.LoseEntryResponse = true;
        await lost.PollAllowFailureAsync();
        lost.Restart();
        await lost.PollAsync();
        Check(lost.Server.EntryBodies.Count == 1 && lost.Scans.Entries.Single().ScanId == queued.ScanId
            && lost.Scans.Entries.Single().Status == "TESTING", "lost entry response must recover by scanId without another POST");

        using Fixture offline = new();
        await offline.EnqueueAsync("SN-A");
        offline.Server.FailLookup = true;
        await offline.PollAllowFailureAsync();
        Check(offline.Server.EntryBodies.Count == 0 && offline.Scans.Entries.Single().Status == "PENDING_ENTRY",
            "failed verification must preserve the pending SN and must not blindly POST");
    }

    private static async Task CheckEnqueueDuringNetworkWaitAsync()
    {
        using Fixture f = new();
        await f.EnqueueAsync("SN-A");
        f.Server.EntryStarted = new(TaskCreationOptions.RunContinuationsAsynchronously);
        f.Server.ReleaseEntry = new(TaskCreationOptions.RunContinuationsAsynchronously);
        Task pending = f.PollAsync();
        try
        {
            await f.Server.EntryStarted.Task.WaitAsync(TimeSpan.FromSeconds(5));
            int requestCount = f.Server.Requests;
            ElectricCheckScanState second = await f.EnqueueAsync("SN-B").WaitAsync(TimeSpan.FromSeconds(2));
            Check(second.Status == "PENDING_ENTRY" && f.Scans.Entries.Count == 2 && f.Server.Requests == requestCount,
                "scanning during a pending HTTP response must save locally without waiting for the network");
        }
        finally
        {
            f.Server.ReleaseEntry.SetResult(true);
            await pending;
        }
        f.Server.EntryStarted = null; f.Server.ReleaseEntry = null;
        await f.PollAsync();
        Check(f.Server.Scans.Count == 2 && f.Server.EntryBodies.Count == 2, "overlapping poll and enqueue must retain both scans exactly once");
    }

    private static async Task CheckDuplicateServerEntryAsync()
    {
        using Fixture f = new();
        f.Server.RegisteredSns.Add("SN-TAKEN");
        await f.EnqueueAsync("SN-TAKEN");
        await f.EnqueueAsync("SN-NEXT");
        await f.PollAsync();
        Check(f.Scans.Entries[0].Status == "REJECTED" && f.Scans.Entries[1].Status == "TESTING",
            "DUPLICATE_SN must reject only that entry and allow the next queued SN to enter");
        Check(f.Server.Scans.Count == 1 && f.Server.Scans.Values.Single().Identity.GetProperty("snCode").GetString() == "SN-NEXT",
            "duplicate SN must not create a remote scan");
        f.Restart();
        await RejectAsync(() => f.EnqueueAsync("SN-TAKEN"));
        Check(f.Server.EntryBodies.Count == 2, "rejected SN must not be silently resubmitted after restart");
    }

    private static async Task CheckFifoSegmentsAsync()
    {
        using Fixture f = new();
        ElectricCheckScanState first = await f.EnqueueAsync("SN-A");
        await f.PollAsync();
        f.AddRow(2, "a1", "A"); f.AddRow(3, "a2", "A");
        ElectricCheckScanState second = await f.EnqueueAsync("SN-B");
        ElectricCheckScanState third = await f.EnqueueAsync("SN-C");
        f.AddRow(4, "b1", "B"); f.AddRow(5, "a3", "A");
        await f.PumpAsync();
        Check(f.Server.Completions.Count == 2, "known TestID boundaries may close the first two batches immediately");
        CheckRange(f.Server.Completions[0], first.ScanId, 2, 3, 2);
        CheckRange(f.Server.Completions[1], second.ScanId, 4, 4, 1);
        Check(!f.Server.UploadedRows.Contains(1) && !f.Server.UploadedRows.Contains(5), "closed batches must exclude history and the final open group");
        f.Restart(); await f.PollAsync();
        f.Clock.Advance(59); await f.PollAsync();
        Check(f.Server.Completions.Count == 2, "final segment requires a full observed quiet minute after restart");
        f.Clock.Advance(1); await f.PollAsync();
        Check(f.Server.Completions.Count == 3, "last FIFO segment must finish after sixty unchanged seconds");
        CheckRange(f.Server.Completions[2], third.ScanId, 5, 5, 1);
        Check(f.Server.UploadedRows.SetEquals([2L, 3L, 4L, 5L]), "FIFO ranges must neither skip nor consume another segment");
        Check(f.Scans.Entries.All(entry => entry.Status == "COMPLETED" && entry.FinalResult == "OK"), "completed entries must expose final results");
        f.Restart(); await RejectAsync(() => f.EnqueueAsync(" SN-A "));
        Check(f.Scans.Entries.Count == 3, "completed SNs must remain reserved after reboot");
    }

    private static async Task CheckQuietMinuteAsync()
    {
        using Fixture f = new();
        await f.EnqueueAsync("SN-A"); await f.PollAsync();
        f.AddRow(2, "10"); f.Clock.Advance(60); await f.PollAsync();
        Check(f.Server.Completions.Count == 0, "first observation starts a full quiet minute");
        f.Clock.Advance(59); await f.PollAsync();
        Check(f.Server.Completions.Count == 0, "59 seconds of silence must not complete a batch");
        f.AddRow(3, "20"); f.Clock.Advance(1); await f.PollAsync();
        Check(f.Server.Completions.Count == 0, "extra rows must restart the quiet minute");
        f.Clock.Advance(60); await f.PollAsync();
        Check(f.Server.Completions.Count == 1 && f.Server.Completions[0].Count == 2, "one unchanged minute must complete the batch");
        f.Clock.Advance(60); await f.PollAsync();
        Check(f.Server.CompleteBodies.Count == 1, "completed scans must not repeatedly exit");
    }

    private static async Task CheckChangedValuesAsync()
    {
        using Fixture f = new();
        await f.EnqueueAsync("SN-A"); await f.PollAsync();
        f.AddRow(2, "10"); await f.PollAsync(); await f.SyncAsync();
        f.Clock.Advance(60); f.UpdateRow(2, "99"); await f.PollAsync();
        Check(f.Server.Completions.Count == 0, "changed values must restart silence even when rowid stays unchanged");
        f.Clock.Advance(60); await f.PollAsync();
        Check(f.Server.Completions.Count == 1 && f.Server.UploadedValues[2] == "99", "completion must refresh values after the incremental cursor passed the row");
    }

    private static async Task CheckUploadFailuresAsync()
    {
        using Fixture f = new();
        await f.EnqueueAsync("SN-A"); await f.PollAsync();
        f.AddRow(2, "10"); await f.PollAsync(); f.Clock.Advance(60);
        f.Server.FailUpload = true; await f.PollAllowFailureAsync();
        Check(f.Server.CompleteBodies.Count == 0, "HTTP upload failure must prevent completion");
        f.Server.FailUpload = false; f.Server.FailUploadBusiness = true; await f.PollAllowFailureAsync();
        Check(f.Server.CompleteBodies.Count == 0, "HTTP200 business upload failure must prevent completion");
        f.Server.FailUploadBusiness = false; await f.PollAsync();
        Check(f.Server.Completions.Count == 1, "successful range upload retry may complete the batch");

        using Fixture cursor = new();
        cursor.Server.FailUploadBusiness = true; await RejectAsync(() => cursor.SyncAsync());
        cursor.Server.FailUploadBusiness = false;
        Check((await cursor.SyncAsync()).Sent == 1, "business failure must leave the incremental cursor unchanged");
    }

    private static async Task CheckLateResultsAsync()
    {
        using Fixture f = new();
        await f.EnqueueAsync("SN-A"); await f.PollAsync();
        f.AddRow(2, "10"); await f.PollAsync(); f.Clock.Advance(60);
        f.Server.OnUpload = () => f.AddRow(3, "late"); await f.PollAsync();
        Check(f.Server.CompleteBodies.Count == 0, "results arriving during upload must cancel completion and restart silence");
        f.Clock.Advance(60); await f.PollAsync();
        Check(f.Server.Completions.Single().Count == 2, "retry must upload and complete the late result too");
    }

    private static async Task CheckRestartObservationAsync()
    {
        using Fixture f = new();
        await f.EnqueueAsync("SN-A"); await f.PollAsync();
        f.AddRow(2, "10"); await f.PollAsync();
        f.Clock.Advance(600); f.Restart(); await f.PollAsync();
        Check(f.Server.Completions.Count == 0, "unobserved downtime must not count as a quiet minute");
        f.Clock.Advance(60); await f.PollAsync();
        Check(f.Server.Completions.Count == 1, "recovered batches may complete after a full observed minute");
    }

    private static async Task CheckCompletionRetryAsync()
    {
        using Fixture f = new();
        ElectricCheckScanState first = await f.EnqueueAsync("SN-A");
        await f.EnqueueAsync("SN-B"); await f.PollAsync();
        f.AddRow(2, "10"); await f.PollAsync(); f.Clock.Advance(60);
        f.Server.FailComplete = true; await f.PollAllowFailureAsync();
        string original = f.Server.CompleteBodies.Single();
        f.AddRow(3, "next", "next"); f.Server.FailComplete = false; f.Restart(); await f.PollAsync();
        Check(f.Server.CompleteBodies.Count == 2 && f.Server.CompleteBodies[1] == original, "ambiguous completion must retry the exact persisted payload despite later rows and restart");
        CheckRange(f.Server.Completions.Single(), first.ScanId, 2, 2, 1);
        await f.PumpAsync(); f.Clock.Advance(60); await f.PollAsync();
        Check(f.Server.Completions.Count == 2 && f.Server.Completions[1].First == 3, "pending completion retry must leave subsequent rows for the next SN");
    }

    private static async Task CheckLostCompletionResponseAsync()
    {
        using Fixture f = new();
        await f.EnqueueAsync("SN-A"); await f.PollAsync();
        f.AddRow(2, "10"); await f.PollAsync(); f.Clock.Advance(60);
        f.Server.LoseCompleteResponse = true; await f.PollAllowFailureAsync(); f.Restart(); await f.PollAsync();
        Check(f.Scans.Entries.Single().Status == "COMPLETED" && f.Scans.Entries.Single().FinalResult == "OK" && f.Server.CompleteBodies.Count == 1,
            "lost committed completion response must recover status and final result without repeating exit");
        await f.EnqueueAsync("SN-B"); f.AddRow(3, "next"); await f.PumpAsync(); f.Clock.Advance(60); await f.PollAsync();
        Check(f.Server.Completions.Count == 2 && f.Server.Completions[1].First == 3, "status recovery must advance the shared watermark exactly once");
    }

    private static async Task CheckRepeatedTestIdAsync()
    {
        using Fixture f = new();
        await f.EnqueueAsync("SN-A"); await f.EnqueueAsync("SN-B"); await f.PollAsync();
        f.AddRow(2, "first", "SAME"); await f.PollAsync(); f.Clock.Advance(60); await f.PollAsync();
        f.AddRow(3, "second", "SAME"); await f.PumpAsync();
        Check(f.Server.Completions.Count == 1, "same TestID after a completed quiet minute belongs to the next batch");
        f.Clock.Advance(60); await f.PollAsync();
        Check(f.Server.Completions.Count == 2 && f.Server.Completions[0].Last == 2 && f.Server.Completions[1].First == 3,
            "separate quiet-minute batches must not merge merely because TestID repeats");
    }

    private static void CheckRange(CompletedRange range, string scanId, long first, long last, int count) =>
        Check(range.ScanId == scanId && range.First == first && range.Last == last && range.Count == count,
            $"FIFO scan {scanId} must bind rows {first}..{last} ({count} records), got {range}");
    private static void Check(bool value, string message) { if (!value) throw new InvalidOperationException(message); }
    private static async Task RejectAsync(Func<Task> action)
    {
        try { await action(); }
        catch (Exception ex) when (ex is HttpRequestException or InvalidOperationException or ArgumentException) { return; }
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
            AddRow(1, "history", "history");
            _sync = new(new ElectricCheckSyncStateStore(_directory), new HttpClient(Server, disposeHandler: false));
            Scans = CreateService();
        }
        private ElectricCheckScanService CreateService() => new(_sync, new ElectricCheckScanStateStore(_directory), new HttpClient(Server, disposeHandler: false), Clock);
        public void Restart() { Scans.Dispose(); Scans = CreateService(); }
        public Task<ElectricCheckScanState> EnqueueAsync(string sn) => Scans.EnqueueAsync(Settings, sn, CancellationToken.None);
        public Task<ElectricCheckScanState?> PollAsync() => Scans.PollAsync(Settings, CancellationToken.None);
        public async Task PollAllowFailureAsync()
        {
            try { await PollAsync(); }
            catch (Exception ex) when (ex is HttpRequestException or InvalidOperationException) { }
        }
        public async Task PumpAsync() { for (int i = 0; i < 4; i++) await PollAsync(); }
        public Task<ElectricCheckSyncResult> SyncAsync() => _sync.SyncOnceAsync(Settings, CancellationToken.None);
        public void AddRow(long row, string value, string testId = "test")
        {
            using SqliteConnection connection = new($"Data Source={Settings.SqliteFilePath};Pooling=False");
            connection.Open(); using SqliteCommand command = connection.CreateCommand();
            command.CommandText = "INSERT INTO ElectricCheck(rowid,TestDateTime,LightName,TestItemName,TestValue,TestResult,LightType,TestID,SNCode) VALUES($row,'2026-10-19 09:00:10.123','灯','电流',$value,'OK','近光',$testId,'INSTRUMENT-WRONG')";
            command.Parameters.AddWithValue("$row", row); command.Parameters.AddWithValue("$value", value); command.Parameters.AddWithValue("$testId", testId);
            command.ExecuteNonQuery();
        }
        public void UpdateRow(long row, string value)
        {
            using SqliteConnection connection = new($"Data Source={Settings.SqliteFilePath};Pooling=False");
            connection.Open(); using SqliteCommand command = connection.CreateCommand();
            command.CommandText = "UPDATE ElectricCheck SET TestValue=$value WHERE rowid=$row";
            command.Parameters.AddWithValue("$value", value); command.Parameters.AddWithValue("$row", row); command.ExecuteNonQuery();
        }
        private void Execute(string sql)
        {
            using SqliteConnection connection = new($"Data Source={Settings.SqliteFilePath};Pooling=False");
            connection.Open(); using SqliteCommand command = new(sql, connection); command.ExecuteNonQuery();
        }
        public void Dispose()
        {
            Scans.Dispose(); _sync.Dispose(); Server.Dispose(); SqliteConnection.ClearAllPools();
            string resolved = Path.GetFullPath(_directory);
            string temporaryRoot = Path.GetFullPath(Path.GetTempPath()).TrimEnd(Path.DirectorySeparatorChar) + Path.DirectorySeparatorChar;
            if (!resolved.StartsWith(temporaryRoot, StringComparison.OrdinalIgnoreCase) || !Path.GetFileName(resolved).StartsWith("c-lan-scan-", StringComparison.Ordinal))
                throw new InvalidOperationException("Unexpected test cleanup path");
            Directory.Delete(resolved, recursive: true);
        }
    }

    private sealed record CompletedRange(string ScanId, long First, long Last, int Count);
    private sealed class ServerScan(JsonElement identity)
    {
        public JsonElement Identity { get; } = identity.Clone();
        public string Status { get; set; } = "TESTING";
        public CompletedRange? Completion { get; set; }
        public object Response() => new
        {
            scanId = Identity.GetProperty("scanId").GetString(), deviceNo = Identity.GetProperty("deviceNo").GetString(),
            snCode = Identity.GetProperty("snCode").GetString(), scanTime = Identity.GetProperty("scanTime").GetString(),
            sourceRowIdBeforeScan = Identity.GetProperty("sourceRowIdBeforeScan").GetInt64(), status = Status,
            firstSourceRowId = Completion?.First, lastSourceRowId = Completion?.Last,
            expectedRecordCount = Completion?.Count, finalResult = Status == "COMPLETED" ? "OK" : null
        };
    }

    private sealed class FakeServer : HttpMessageHandler
    {
        public int Requests;
        public int StatusCalls;
        public bool FailUpload;
        public bool FailUploadBusiness;
        public bool FailComplete;
        public bool LoseCompleteResponse;
        public bool FailEntry;
        public bool FailLookup;
        public bool LoseEntryResponse;
        public TaskCompletionSource<bool>? EntryStarted;
        public TaskCompletionSource<bool>? ReleaseEntry;
        public Action? OnUpload;
        public readonly Dictionary<string, ServerScan> Scans = new(StringComparer.Ordinal);
        public readonly HashSet<string> RegisteredSns = new(StringComparer.Ordinal);
        public readonly HashSet<long> UploadedRows = [];
        public readonly Dictionary<long, string?> UploadedValues = [];
        public readonly List<string> CompleteBodies = [];
        public readonly List<CompletedRange> Completions = [];
        public readonly List<string> EntryBodies = [];
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            Requests++;
            Uri uri = request.RequestUri!;
            string path = uri.AbsolutePath;
            string body = request.Content is null ? "{}" : await request.Content.ReadAsStringAsync(cancellationToken);
            using JsonDocument json = JsonDocument.Parse(body);
            JsonElement data = json.RootElement;
            if (path.EndsWith("records:batch", StringComparison.Ordinal))
            {
                if (FailUpload) return Reply(new { code = 500, msg = "upload rejected" }, HttpStatusCode.InternalServerError);
                if (FailUploadBusiness) return Reply(new { code = 500, msg = "business rejected" });
                foreach (JsonElement item in data.GetProperty("records").EnumerateArray())
                {
                    Check(!item.TryGetProperty("snCode", out JsonElement sourceSn) || sourceSn.ValueKind == JsonValueKind.Null, "FIFO uploads must omit instrument SN so server binding follows scan order");
                    long row = item.GetProperty("sourceRowId").GetInt64();
                    UploadedRows.Add(row); UploadedValues[row] = item.GetProperty("testValue").GetString();
                }
                Action? action = OnUpload; OnUpload = null; action?.Invoke();
                return Reply(new { code = 200 });
            }
            if (path.EndsWith("/complete", StringComparison.Ordinal))
            {
                CompleteBodies.Add(body);
                string id = Uri.UnescapeDataString(path.Split('/')[^2]);
                Check(Scans.ContainsKey(id), "only an acknowledged entry may complete");
                CompletedRange range = new(id, data.GetProperty("firstSourceRowId").GetInt64(), data.GetProperty("lastSourceRowId").GetInt64(), data.GetProperty("expectedRecordCount").GetInt32());
                Check(range.First <= range.Last && UploadedRows.Count(row => row >= range.First && row <= range.Last) == range.Count, "completion must carry an explicit range of successfully uploaded rows");
                if (FailComplete) return Reply(new { code = 500 }, HttpStatusCode.InternalServerError);
                ServerScan scan = Scans[id];
                if (scan.Completion is null)
                {
                    Check(Completions.All(previous => previous.Last < range.First || previous.First > range.Last), "completed ranges must never overlap");
                    scan.Completion = range; Completions.Add(range); scan.Status = "COMPLETED";
                }
                else Check(scan.Completion == range, "completion retry must preserve the original range");
                if (LoseCompleteResponse) { LoseCompleteResponse = false; throw new HttpRequestException("completion committed but response was lost"); }
                return Reply(new { code = 200, data = scan.Response() });
            }
            if (request.Method == HttpMethod.Post && path.EndsWith("/scans", StringComparison.Ordinal))
            {
                EntryBodies.Add(body);
                EntryStarted?.TrySetResult(true);
                if (ReleaseEntry is not null) await ReleaseEntry.Task.WaitAsync(cancellationToken);
                if (FailEntry) return Reply(new { code = 500 }, HttpStatusCode.InternalServerError);
                string id = data.GetProperty("scanId").GetString()!;
                string sn = data.GetProperty("snCode").GetString()!;
                if (!Scans.TryGetValue(id, out ServerScan? scan))
                {
                    if (!RegisteredSns.Add(sn)) return Reply(new { code = 409, businessCode = "DUPLICATE_SN", msg = "SN already entered" }, HttpStatusCode.Conflict);
                    scan = new(data); Scans.Add(id, scan);
                }
                else Check(scan.Identity.GetRawText() == data.GetRawText(), "entry retries must preserve all five original fields");
                if (LoseEntryResponse) { LoseEntryResponse = false; throw new HttpRequestException("entry committed but response was lost"); }
                return Reply(new { code = 200, data = scan.Response() });
            }
            if (request.Method == HttpMethod.Get)
            {
                if (FailLookup) return Reply(new { code = 500 }, HttpStatusCode.InternalServerError);
                Dictionary<string, string> query = uri.Query.TrimStart('?').Split('&', StringSplitOptions.RemoveEmptyEntries)
                    .Select(part => part.Split('=', 2)).ToDictionary(part => Uri.UnescapeDataString(part[0]), part => Uri.UnescapeDataString(part[1]));
                Check(query.GetValueOrDefault("deviceNo") == "EC-1", "status lookups must identify the configured device");
                if (path.EndsWith("/status", StringComparison.Ordinal))
                {
                    StatusCalls++;
                    string[] ids = query["scanIds"].Split(',');
                    Check(ids.Length is > 0 and <= 100 && ids.Distinct().Count() == ids.Length, "batch status requests need 1..100 distinct scan IDs");
                    return Reply(new { code = 200, data = ids.Where(Scans.ContainsKey).Select(id => Scans[id].Response()).ToArray() });
                }
                if (path.EndsWith("/current", StringComparison.Ordinal)) return Reply(new { code = 200, data = Scans.Values.FirstOrDefault(scan => scan.Status == "TESTING")?.Response() });
                string id = Uri.UnescapeDataString(path.Split('/')[^1]);
                return Scans.TryGetValue(id, out ServerScan? scan) ? Reply(new { code = 200, data = scan.Response() }) : Reply(new { code = 404 }, HttpStatusCode.NotFound);
            }
            throw new InvalidOperationException("Unexpected request: " + uri);
        }
        private static HttpResponseMessage Reply(object value, HttpStatusCode status = HttpStatusCode.OK) => new(status)
        { Content = new StringContent(JsonSerializer.Serialize(value), Encoding.UTF8, "application/json") };
    }
}
