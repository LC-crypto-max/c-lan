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
        Thread thread = new(() =>
        {
            try { CheckControls(); }
            catch (Exception ex) { failure = ex; }
        });
        thread.SetApartmentState(ApartmentState.STA);
        thread.Start();
        thread.Join();
        if (failure is not null) ExceptionDispatchInfo.Capture(failure).Throw();
        Console.WriteLine("Failed entry SN editing UI checks passed.");
    }

    private static void CheckControls()
    {
        // 不显示窗体，避免触发启动配置、注册表写入和后台同步。
        using ElectricCheckSyncService sync = new(new ElectricCheckSyncStateStore());
        using Form1 form = new(null!, null!, null!, sync);
        using ElectricCheckScanService scans = Field<ElectricCheckScanService>(form, "_scanService");
        using CancellationTokenSource cancellation = Field<CancellationTokenSource>(form, "_scanCancellation");
        TextBox input = Field<TextBox>(form, "_scanInput");
        Button submit = Field<Button>(form, "_scanSubmit");
        TextBox server = Field<TextBox>(form, "_syncServerTextBox");
        ElectricCheckScanState state = new() { ScanId = "scan-A", SnCode = "SN-ORIGINAL", Status = "PENDING_ENTRY" };
        typeof(ElectricCheckScanService).GetProperty(nameof(ElectricCheckScanService.Current))!.SetValue(scans, state);
        Refresh(form);
        Check(input.Enabled && submit.Enabled && submit.Text == "重新入站" && input.Text == "SN-ORIGINAL",
            "failed entry must refill SN, unlock editing and expose manual re-entry");
        Check(!server.Enabled, "editing SN must keep the server configuration locked");

        input.Text = "SN-EDITED";
        Refresh(form);
        Check(input.Text == "SN-EDITED", "status refresh must not overwrite an operator's SN edit");
        typeof(Form1).GetField("_scanOperation", BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(form, true);
        Refresh(form);
        Check(!input.Enabled && !submit.Enabled && input.Text == "SN-EDITED",
            "in-flight entry verification must lock controls without losing edited input");

        typeof(Form1).GetField("_scanOperation", BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(form, false);
        state.Status = "TESTING";
        Refresh(form);
        Check(!input.Enabled && !submit.Enabled && input.Text == "", "a confirmed entry must clear the draft and lock SN");
        state.Status = "COMPLETED";
        Refresh(form);
        Check(input.Enabled && submit.Enabled && server.Enabled && submit.Text == "扫码入站",
            "completed scans must allow the next product and restore the normal submit button");
    }

    private static T Field<T>(Form1 form, string name) =>
        (T)typeof(Form1).GetField(name, BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(form)!;

    private static void Refresh(Form1 form) =>
        typeof(Form1).GetMethod("RefreshScanUi", BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(form, [null]);

    private static void Check(bool condition, string message)
    {
        if (!condition) throw new InvalidOperationException(message);
    }
}
