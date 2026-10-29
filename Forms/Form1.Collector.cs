namespace c_lan;

public partial class Form1
{
    private Panel _collectorPanel = null!;
    private Label _collectorDevice = null!;
    private Label _collectorStatus = null!;
    private Label _collectorLastUpload = null!;
    private Label _collectorError = null!;
    private bool _collectorDisposed;
    private readonly CancellationTokenSource _collectorCancellation = new();
    private Task _collectorWork = Task.CompletedTask;

    private void InitializeCollectorUi()
    {
        _collectorPanel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(24), BackColor = Color.White };
        FlowLayoutPanel layout = new() { Dock = DockStyle.Fill, FlowDirection = FlowDirection.TopDown, WrapContents = false, AutoScroll = true };
        layout.Controls.Add(new Label { Text = "电检数据采集", AutoSize = true, Font = new Font(Font.FontFamily, 18), Margin = new Padding(0, 0, 0, 24) });
        foreach (Label label in new[]
        {
            _collectorDevice = new Label { AccessibleName = "采集设备" },
            _collectorStatus = new Label { AccessibleName = "采集状态" },
            _collectorLastUpload = new Label { Text = "最近上传：本次启动尚未上传", AccessibleName = "最近上传" },
            _collectorError = new Label { Text = "最近错误：无", AccessibleName = "最近错误" }
        })
        {
            label.AutoSize = true;
            label.MaximumSize = new Size(850, 0);
            label.Margin = new Padding(0, 0, 0, 18);
            layout.Controls.Add(label);
        }
        Button settings = new() { Text = "设置", AutoSize = true, AccessibleName = "打开维护设置" };
        settings.Click += (_, _) => ShowMaintenancePassword();
        layout.Controls.Add(settings);
        _collectorPanel.Controls.Add(layout);
        Controls.Add(_collectorPanel);
        _collectorPanel.BringToFront();
        InitializeMaintenanceUi();
    }

    private void RefreshCollectorUi()
    {
        ConnectionPanel.Enabled = _syncPanel.Enabled = MainSplitContainer.Enabled = _maintenanceUnlocked;
        _collectorDevice.Text = "设备：" + (string.IsNullOrWhiteSpace(_syncDeviceTextBox.Text) ? "尚未配置" : _syncDeviceTextBox.Text);
        _collectorStatus.Text = "采集状态：" + _syncStatusLabel.Text;
    }

    private void DisposeCollectorResources()
    {
        if (_collectorDisposed) return;
        _collectorDisposed = true;
        _syncSettingsTimer?.Stop();
        _cancellationTokenSource?.Cancel();
        _syncLoopCancellation?.Cancel();
        _collectorCancellation.Cancel();
        _notifyIcon.Visible = false;
        // Direct Dispose can occur without FormClosing. Wait for the uploader before releasing its gate/client.
        Task work = _collectorWork;
        _ = work.ContinueWith(_ =>
        {
            _syncLoopCancellation?.Dispose();
            _collectorCancellation.Dispose();
            _syncService.Dispose();
        }, CancellationToken.None, TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);
    }
}
