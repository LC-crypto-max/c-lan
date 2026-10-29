using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Windows.Forms;
using c_lan;
using c_lan.Configuration;
using c_lan.Models;
using c_lan.Services;

internal static class ElectricCheckScanUiChecks
{
    public static void Run()
    {
        Exception? failure = null;
        Thread thread = new(() => { try { CheckControls(); CheckStartupUpload(); } catch (Exception ex) { failure = ex; } });
        thread.SetApartmentState(ApartmentState.STA);
        thread.Start(); thread.Join();
        if (failure is not null) ExceptionDispatchInfo.Capture(failure).Throw();
        Console.WriteLine("Collector dashboard, maintenance gates and disposal checks passed.");
    }

    private static void CheckControls()
    {
        using ElectricCheckSyncService sync = new(new ElectricCheckSyncStateStore());
        using Form1 form = new(null!, null!, null!, sync);
        Check(!Descendants(form).Any(c => c.AccessibleName == "产品SN" || c.Text == "扫码入站"),
            "collector must have no SN input or entry action");
        Check(typeof(Form1).GetField("_scanService", BindingFlags.Instance | BindingFlags.NonPublic)?.GetValue(form) is null,
            "collector must never initialize the archived SN service");
        Check(typeof(Form1).GetField("_scanTimer", BindingFlags.Instance | BindingFlags.NonPublic)?.GetValue(form) is null,
            "collector must have no scan polling timer");
        TextBox server = Field<TextBox>(form, "_syncServerTextBox");
        TextBox device = Field<TextBox>(form, "_syncDeviceTextBox");
        server.Text = "http://collector.invalid"; device.Text = "DRAFT-DEVICE";
        Invoke(form, "RefreshCollectorUi");
        Check(!server.Enabled && !Field<Panel>(form, "ConnectionPanel").Enabled,
            "server, device, source and SQL configuration must start locked");
        Check(!Field<bool>(form, "_maintenanceUnlocked") && !Field<ToolStripMenuItem>(form, "SyncNowMenuItem").Available,
            "tray retransmission must start locked");
        Check(!(bool)Invoke(form, "TryUnlockMaintenance", "wrong")!, "wrong maintenance password must fail");
        Check((bool)Invoke(form, "TryUnlockMaintenance", "lyc3191")! && server.Enabled,
            "correct password must enable maintenance");
        Invoke(form, "ShowSyncResult", new ElectricCheckSyncResult(3, 5, "已同步 3 条"));
        Check(Field<Label>(form, "_collectorLastUpload").Text.Contains("3"), "dashboard must show last successful upload");
        Invoke(form, "ShowSyncResult", new ElectricCheckSyncResult(0, 0, "服务端暂不可用"));
        Check(Field<Label>(form, "_collectorError").Text.Contains("服务端暂不可用"), "dashboard must expose upload failures");
        Invoke(form, "LeaveMaintenance");
        Check(!server.Enabled && !Field<ToolStripMenuItem>(form, "SyncNowMenuItem").Available,
            "leaving maintenance must lock settings and tray actions");
        // Locked handlers must return before touching null services or changing the saved draft.
        foreach (string method in new[] { "ConnectButton_Click", "TestButton_Click", "SaveConnectionButton_Click", "DeleteConnectionButton_Click", "ExecuteQueryButton_Click" })
            Invoke(form, method, null, EventArgs.Empty);
        ((Task)Invoke(form, "SyncNowAsync", true)!).GetAwaiter().GetResult();
        Check(server.Text == "http://collector.invalid" && device.Text == "DRAFT-DEVICE",
            "background status and blocked actions must preserve configuration drafts");
        Invoke(form, "TryUnlockMaintenance", "lyc3191");
        Invoke(form, "Form1_FormClosing", null, new FormClosingEventArgs(CloseReason.UserClosing, false));
        Check(!Field<bool>(form, "_maintenanceUnlocked"), "closing to tray must require a fresh password");
        form.Hide();
        using Form1 restarted = new(null!, null!, null!, sync);
        Check(!Field<bool>(restarted, "_maintenanceUnlocked"), "new windows must start locked");
    }

    private static void CheckStartupUpload()
    {
        using CollectorUploadChecks.Fixture fixture = new();
        string root = @"Software\c-lan-collector-checks-" + Guid.NewGuid().ToString("N");
        string legacyDraft = Path.Combine(fixture.Directory, "electric-scan-legacy.json");
        File.WriteAllText(legacyDraft, "untouched historical draft");
        ElectricCheckSyncSettingsStore settings = new(fixture.Directory);
        settings.Save(fixture.Settings);
        fixture.Server.FailNext = true;
        try
        {
            using Form1 form = new(null!, null!, null!, fixture.Sync);
            SetField(form, "_syncSettingsStore", settings);
            SetField(form, "_startupService", new WindowsStartupService(root + @"\Run", root + @"\Settings"));
            form.Show();
            PumpUntil(() => fixture.Server.Rows.Count == 3, "startup background collector must retry and upload SQLite without SN work");
            DateTime until = DateTime.UtcNow.AddMilliseconds(1200);
            while (DateTime.UtcNow < until) { Application.DoEvents(); Thread.Sleep(5); }
            Check(fixture.Server.Paths.Count >= 3 && fixture.Server.Paths.All(p => p == "/openapi/electric-check/records:batch"),
                "startup and subsequent polling must only send raw batch uploads");
            Check(File.ReadAllText(legacyDraft) == "untouched historical draft", "startup must not migrate or consume historical SN drafts");
            Check(!Field<bool>(form, "_maintenanceUnlocked"), "background collection must keep maintenance locked");
            Field<CheckBox>(form, "_autoSyncCheckBox").Checked = false;
            Check(Field<CheckBox>(form, "_autoSyncCheckBox").Checked && settings.Load()!.Enabled,
                "locked auto-sync toggle must not stop collection or modify settings");
            Field<CheckBox>(form, "_startupCheckBox").Checked = false;
            Check(Field<CheckBox>(form, "_startupCheckBox").Checked,
                "locked startup toggle must not change the saved startup preference");
            Invoke(form, "TryUnlockMaintenance", "lyc3191");
            Invoke(form, "ShowFromTray");
            Check(!Field<bool>(form, "_maintenanceUnlocked"), "restoring the tray window must lock maintenance again");
            SetField(form, "_allowClose", true);
            form.Close();
            PumpUntil(() => form.IsDisposed, "collector shutdown must finish without any scan timer/service");
        }
        finally { Microsoft.Win32.Registry.CurrentUser.DeleteSubKeyTree(root, false); }
    }

    private static void PumpUntil(Func<bool> condition, string message)
    {
        DateTime deadline = DateTime.UtcNow.AddSeconds(8);
        while (!condition() && DateTime.UtcNow < deadline) { Application.DoEvents(); Thread.Sleep(5); }
        Check(condition(), message);
    }
    private static void SetField(Form1 form, string name, object value) => typeof(Form1).GetField(name, BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(form, value);

    private static IEnumerable<Control> Descendants(Control parent) => parent.Controls.Cast<Control>().SelectMany(c => new[] { c }.Concat(Descendants(c)));
    private static T Field<T>(Form1 form, string name) => (T)typeof(Form1).GetField(name, BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(form)!;
    private static object? Invoke(Form1 form, string name, params object?[] args) => typeof(Form1).GetMethod(name, BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(form, args);
    private static void Check(bool condition, string message) { if (!condition) throw new InvalidOperationException(message); }
}
