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
    private Task _scanEntryWork = Task.CompletedTask;
    private bool _scanOperation;
    private int _scanEnqueueCount;
    private Panel _scanPanel = null!;
    private TextBox _scanInput = null!;
    private Button _scanSubmit = null!;
    private Label _scanDeviceInfo = null!;
    private Label _scanStateInfo = null!;
    private DataGridView _scanGrid = null!;

    private void InitializeScanUi()
    {
        _scanService = new ElectricCheckScanService(_syncService, new ElectricCheckScanStateStore());
        _scanPanel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(24), BackColor = Color.White };
        TableLayoutPanel layout = new() { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 4 };
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        TableLayoutPanel heading = new() { Dock = DockStyle.Fill, AutoSize = true, ColumnCount = 2 };
        heading.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        heading.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        _scanDeviceInfo = new Label { AutoSize = true, Margin = new Padding(0, 8, 0, 16) };
        Button settings = new() { Text = "设置", AutoSize = true, AccessibleName = "打开维护设置" };
        settings.Click += (_, _) => ShowMaintenancePassword();
        heading.Controls.Add(_scanDeviceInfo, 0, 0);
        heading.Controls.Add(settings, 1, 0);
        FlowLayoutPanel input = new() { AutoSize = true, Dock = DockStyle.Fill, WrapContents = true };
        input.Controls.Add(new Label { Text = "产品 SN", AutoSize = true, Margin = new Padding(0, 10, 10, 0) });
        _scanInput = new TextBox { Width = 420, MaxLength = 128, AccessibleName = "产品SN", Font = new Font(Font.FontFamily, 14) };
        _scanSubmit = new Button { Text = "扫码入站", AutoSize = true, Height = 36 };
        input.Controls.Add(_scanInput);
        input.Controls.Add(_scanSubmit);
        _scanStateInfo = new Label
        {
            AutoSize = true, Dock = DockStyle.Fill, AutoEllipsis = true,
            Margin = new Padding(0, 10, 0, 16),
            Text = "扫码枪以回车结束；可连续扫码。记录按扫码顺序关联检测结果。"
        };
        _scanGrid = new DataGridView
        {
            Dock = DockStyle.Fill, ReadOnly = true, AllowUserToAddRows = false,
            AllowUserToDeleteRows = false, AllowUserToResizeRows = false,
            RowHeadersVisible = false, AutoGenerateColumns = false,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            BackgroundColor = Color.White, BorderStyle = BorderStyle.None,
            AccessibleName = "最近扫码记录"
        };
        foreach ((string name, string title, int weight) in new[]
        {
            ("SnCode", "产品 SN", 23), ("ScanTime", "扫码时间", 20),
            ("Status", "状态", 13), ("FinalResult", "结果", 9), ("Message", "说明", 35)
        })
            _scanGrid.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = name, HeaderText = title, FillWeight = weight,
                SortMode = DataGridViewColumnSortMode.NotSortable
            });
        _scanGrid.RowTemplate.Height = 32;
        _scanGrid.DefaultCellStyle.WrapMode = DataGridViewTriState.False;
        layout.Controls.Add(heading, 0, 0);
        layout.Controls.Add(input, 0, 1);
        layout.Controls.Add(_scanStateInfo, 0, 2);
        layout.Controls.Add(_scanGrid, 0, 3);
        _scanPanel.Controls.Add(layout);
        Controls.Add(_scanPanel);
        _scanPanel.BringToFront();
        InitializeMaintenanceUi();
        _scanSubmit.Click += async (_, _) => await BeginEnqueueAsync(_scanInput.Text);
        _scanInput.KeyDown += async (_, e) =>
        {
            if (e.KeyCode != Keys.Enter) return;
            e.SuppressKeyPress = true;
            await BeginEnqueueAsync(_scanInput.Text);
        };
        _scanTimer = new System.Windows.Forms.Timer(components!) { Interval = 2000 };
        _scanTimer.Tick += async (_, _) => await BeginScanOperationAsync();
        RefreshScanUi();
    }

    private Task BeginEnqueueAsync(string sn)
    {
        if (_scanCancellation.IsCancellationRequested) return Task.CompletedTask;
        // 本地保存期间下一支扫码可直接替换选中的文字，不等待后台网络请求。
        _scanInput.SelectAll();
        Task work = EnqueueScanAsync(sn);
        _scanEntryWork = _scanEntryWork.IsCompleted ? work : Task.WhenAll(_scanEntryWork, work);
        return work;
    }

    private async Task EnqueueScanAsync(string sn)
    {
        string? error = null;
        _scanEnqueueCount++;
        RefreshScanUi();
        try
        {
            ElectricCheckSyncSettings settings = BuildSyncSettings();
            SaveSyncSettings();
            await _scanService.EnqueueAsync(settings, sn, _scanCancellation.Token);
            if (_scanInput.Text == sn) _scanInput.Clear();
        }
        catch (OperationCanceledException) when (_scanCancellation.IsCancellationRequested) { }
        catch (Exception ex) { error = "扫码未保存：" + ex.Message; }
        finally
        {
            _scanEnqueueCount--;
            if (!IsDisposed && !_scanCancellation.IsCancellationRequested)
            {
                RefreshScanUi(error);
                if (!_maintenanceUnlocked) _scanInput.Focus();
            }
        }
    }

    private Task BeginScanOperationAsync()
    {
        if (_scanOperation || _scanCancellation.IsCancellationRequested) return Task.CompletedTask;
        _scanWork = PollScansAsync();
        return _scanWork;
    }

    private async Task PollScansAsync()
    {
        string? error = null;
        _scanOperation = true;
        RefreshScanUi();
        try
        {
            ElectricCheckSyncSettings settings = BuildSyncSettings();
            if (string.IsNullOrWhiteSpace(settings.DeviceNo) || string.IsNullOrWhiteSpace(settings.ServerBaseUrl)
                || !File.Exists(settings.SqliteFilePath)) return;
            await _scanService.PollAsync(settings, _scanCancellation.Token);
        }
        catch (OperationCanceledException) when (_scanCancellation.IsCancellationRequested) { }
        catch (Exception ex) { error = "后台处理暂未成功，记录已保留：" + ex.Message; }
        finally
        {
            _scanOperation = false;
            if (!IsDisposed && !_scanCancellation.IsCancellationRequested) RefreshScanUi(error);
        }
    }

    private void RefreshScanUi(string? error = null)
    {
        _scanInput.Enabled = !_scanCancellation.IsCancellationRequested;
        _scanSubmit.Enabled = _scanInput.Enabled;
        bool locked = !_maintenanceUnlocked || _scanOperation || _scanEnqueueCount > 0 || _scanService.HasActiveScan;
        ConnectionPanel.Enabled = !locked;
        _syncPanel.Enabled = !locked;
        _scanDeviceInfo.Text = "产品扫码    设备：" + (_syncDeviceTextBox.Text.Length == 0 ? "尚未配置" : _syncDeviceTextBox.Text);
        _scanStateInfo.Text = error ?? "扫码枪以回车结束；可连续扫码。记录按扫码顺序关联检测结果。";

        _scanGrid.Rows.Clear();
        foreach (ElectricCheckScanState state in _scanService.Entries.OrderBy(s => s.Sequence).TakeLast(200))
        {
            string status = state.Status switch
            {
                "PENDING_ENTRY" => "等待入站", "TESTING" => state.ObservedRecordCount > 0 ? "检测中" : "已入站，待检测", "PENDING_COMPLETE" => "出站重试中",
                "COMPLETED" => "已出站", "CANCELLED" => "已取消", "REJECTED" => "入站拒绝",
                "ENTRY_FAILED" => "入站失败", _ => state.Status
            };
            string message = state.Message.Replace('\r', ' ').Replace('\n', ' ');
            int row = _scanGrid.Rows.Add(state.SnCode, state.ScanTime, status, state.FinalResult ?? "—", message);
            _scanGrid.Rows[row].Cells[4].ToolTipText = state.Message;
        }
    }
}

#endif
