namespace c_lan;

public partial class Form1
{
    private bool _maintenanceUnlocked;

    private void InitializeMaintenanceUi()
    {
        Button back = new()
        {
            Text = "返回采集状态", AutoSize = true, Anchor = AnchorStyles.Top | AnchorStyles.Right,
            Location = new Point(HeaderPanel.Width - 116, 15), AccessibleName = "返回采集状态并锁定设置"
        };
        back.Click += (_, _) => LeaveMaintenance();
        HeaderPanel.Controls.Add(back);
        back.BringToFront();
        Text = "电检数据采集";
        LeaveMaintenance();
    }

    private void ShowMaintenancePassword()
    {
        using Form dialog = new()
        {
            Text = "维护设置", FormBorderStyle = FormBorderStyle.FixedDialog,
            StartPosition = FormStartPosition.CenterParent, ClientSize = new Size(320, 142),
            MinimizeBox = false, MaximizeBox = false, ShowInTaskbar = false
        };
        Label label = new() { Text = "请输入维护密码", AutoSize = true, Location = new Point(20, 18) };
        TextBox password = new() { UseSystemPasswordChar = true, Location = new Point(20, 48), Width = 280, AccessibleName = "维护密码" };
        Button confirm = new() { Text = "打开", DialogResult = DialogResult.OK, Location = new Point(132, 96) };
        Button cancel = new() { Text = "取消", DialogResult = DialogResult.Cancel, Location = new Point(224, 96) };
        dialog.Controls.AddRange([label, password, confirm, cancel]);
        dialog.AcceptButton = confirm;
        dialog.CancelButton = cancel;
        if (dialog.ShowDialog(this) == DialogResult.OK && !TryUnlockMaintenance(password.Text))
            MessageBox.Show(this, "密码错误。", "维护设置", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        password.Clear();
        
    }

    private bool TryUnlockMaintenance(string password)
    {
        if (!string.Equals(password, "lyc3191", StringComparison.Ordinal)) return false;
        _maintenanceUnlocked = true;
        _collectorPanel.Visible = false;
        HeaderPanel.Visible = MainSplitContainer.Visible = MainStatusStrip.Visible = true;
        SyncNowMenuItem.Available = true;
        RefreshCollectorUi();
        return true;
    }

    private void LeaveMaintenance()
    {
        _syncSettingsTimer.Stop();
        SaveSyncSettings();
        _maintenanceUnlocked = false;
        HeaderPanel.Visible = MainSplitContainer.Visible = MainStatusStrip.Visible = false;
        SyncNowMenuItem.Available = false;
        _collectorPanel.Visible = true;
        _collectorPanel.BringToFront();
        RefreshCollectorUi();
    }

    private void SelectMaintenanceTab(TabPage page)
    {
        if (_maintenanceUnlocked) ResultTabControl.SelectedTab = page;
    }
}
