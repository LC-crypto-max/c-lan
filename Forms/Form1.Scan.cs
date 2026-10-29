// Archived scanner UI wiring. Collector mode must not initialize, enqueue or poll SN business.
#if false
using c_lan.Configuration;
using c_lan.Models;
using c_lan.Services;

namespace c_lan;

public partial class Form1
{
    private ElectricCheckScanService _scanService = null!;
    private System.Windows.Forms.Timer _scanTimer = null!;
    private readonly CancellationTokenSource _scanCancellation = new();
    private Task _scanWork = Task.CompletedTask;
    private bool _scanOperation;
    private TextBox _scanInput = null!;
    private Button _scanSubmit = null!;
    private Label _scanDeviceInfo = null!;
    private Label _scanStateInfo = null!;
    private string? _editableScanId;

    private void InitializeScanUi()
    {
        _scanService = new ElectricCheckScanService(_syncService, new ElectricCheckScanStateStore());
        TabPage page = new("产品扫码") { Padding = new Padding(20), BackColor = Color.White };
        TableLayoutPanel layout = new() { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 4 };
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        _scanDeviceInfo = new Label { AutoSize = true, Margin = new Padding(0, 0, 0, 12) };
        FlowLayoutPanel input = new() { AutoSize = true, Dock = DockStyle.Fill, WrapContents = true };
        input.Controls.Add(new Label { Text = "产品 SN", AutoSize = true, Margin = new Padding(0, 8, 10, 0) });
        _scanInput = new TextBox { Width = 300, MaxLength = 128, AccessibleName = "产品SN" };
        _scanSubmit = new Button { Text = "扫码入站", AutoSize = true, Height = 32 };
        Button retry = new() { Text = "检查 / 重试", AutoSize = true, Height = 32 };
        input.Controls.Add(_scanInput); input.Controls.Add(_scanSubmit); input.Controls.Add(retry);
        _scanStateInfo = new Label { Dock = DockStyle.Fill, AutoEllipsis = true, Padding = new Padding(0, 16, 0, 0) };
        Label hint = new()
        {
            AutoSize = true, MaximumSize = new Size(750, 0),
            Text = "扫码枪请设置为回车结束。入站失败后可修改SN并点击“重新入站”；入站成功前请勿检测。已有结果且稳定满一分钟，上传成功后自动出站。"
        };
        layout.Controls.Add(_scanDeviceInfo, 0, 0); layout.Controls.Add(input, 0, 1);
        layout.Controls.Add(_scanStateInfo, 0, 2); layout.Controls.Add(hint, 0, 3);
        page.Controls.Add(layout);
        ResultTabControl.TabPages.Insert(0, page);
        ResultTabControl.SelectedTab = page;
        page.Enter += (_, _) => { if (_scanInput.Enabled) _scanInput.Focus(); };
        _scanSubmit.Click += async (_, _) => await BeginScanOperationAsync(_scanInput.Text);
        _scanInput.KeyDown += async (_, e) =>
        {
            if (e.KeyCode != Keys.Enter) return;
            e.SuppressKeyPress = true;
            await BeginScanOperationAsync(_scanInput.Text);
        };
        retry.Click += async (_, _) => await BeginScanOperationAsync(
            _scanService.Current?.Status == "PENDING_ENTRY" ? _scanInput.Text : null);
        _scanTimer = new System.Windows.Forms.Timer(components!) { Interval = (int)ElectricCheckScanService.CheckInterval.TotalMilliseconds };
        _scanTimer.Tick += async (_, _) => await BeginScanOperationAsync();
        RefreshScanUi();
    }

    private Task BeginScanOperationAsync(string? sn = null)
    {
        if (_scanOperation || _scanCancellation.IsCancellationRequested) return Task.CompletedTask;
        _scanWork = PerformScanOperationAsync(sn);
        return _scanWork;
    }

    private async Task PerformScanOperationAsync(string? sn)
    {
        string? error = null;
        _scanOperation = true;
        RefreshScanUi();
        try
        {
            ElectricCheckSyncSettings settings = BuildSyncSettings();
            // 尚未配置的程序仍可使用数据库浏览，扫码功能等待配置完成。
            if (sn is null && (string.IsNullOrWhiteSpace(settings.DeviceNo)
                    || string.IsNullOrWhiteSpace(settings.ServerBaseUrl) || !File.Exists(settings.SqliteFilePath))) return;
            if (sn is null) await _scanService.PollAsync(settings, _scanCancellation.Token);
            else
            {
                SaveSyncSettings();
                await _scanService.StartAsync(settings, sn, _scanCancellation.Token);
                _scanInput.Clear();
            }
        }
        catch (OperationCanceledException) when (_scanCancellation.IsCancellationRequested) { }
        catch (Exception ex) { error = "操作暂未成功，保留当前记录：" + ex.Message; }
        finally
        {
            _scanOperation = false;
            if (!IsDisposed && !_scanCancellation.IsCancellationRequested)
            {
                RefreshScanUi(error);
                if (_scanInput.Enabled) _scanInput.Focus();
            }
        }
    }

    private void RefreshScanUi(string? error = null)
    {
        ElectricCheckScanState? state = _scanService.Current;
        bool pendingEntry = state?.Status == "PENDING_ENTRY";
        bool locked = _scanOperation || _scanService.HasActiveScan;
        _scanInput.Enabled = !_scanOperation && (!_scanService.HasActiveScan || pendingEntry);
        _scanSubmit.Enabled = _scanInput.Enabled;
        _scanSubmit.Text = pendingEntry ? "重新入站" : "扫码入站";
        if (pendingEntry && _editableScanId != state!.ScanId)
        {
            _scanInput.Text = state.SnCode;
            _scanInput.SelectAll();
            _editableScanId = state.ScanId;
        }
        else if (!pendingEntry && _editableScanId is not null)
        {
            _scanInput.Clear();
            _editableScanId = null;
        }
        _syncDeviceTextBox.Enabled = !locked; _syncServerTextBox.Enabled = !locked;
        _databaseTypeComboBox.Enabled = !locked; _sqliteFileComboBox.Enabled = !locked;
        _browseSqliteButton.Enabled = !locked; HostText.Enabled = !locked;
        _scanDeviceInfo.Text = "当前设备：" + _syncDeviceTextBox.Text;
        string status = state?.Status switch
        {
            "PENDING_ENTRY" => _scanOperation ? "正在核对 / 提交入站" : "入站未确认（可修改SN）",
            "TESTING" => "检测中", "PENDING_COMPLETE" => "自动出站重试中",
            "COMPLETED" => "已出站", "CANCELLED" => "已取消", _ => "等待扫码"
        };
        _scanStateInfo.Text = $"当前 SN：{state?.SnCode ?? "—"}\r\n状态：{status}    检测项目：{state?.ObservedRecordCount ?? 0}\r\n\r\n"
            + (error ?? state?.Message ?? "请扫描产品条码入站。");
    }
}

#endif
