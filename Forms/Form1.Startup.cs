using c_lan.Services;

namespace c_lan;

public partial class Form1
{
    private readonly WindowsStartupService _startupService = new();
    private CheckBox _startupCheckBox = null!;
    private bool _startupSettingsLoaded;

    private void InitializeStartupUi()
    {
        _startupCheckBox = new CheckBox
        {
            Text = "开机自启", AccessibleName = "开机自启", Name = "_startupCheckBox",
            AutoSize = true, Checked = true, Margin = new Padding(12, 7, 0, 0)
        };
        _syncPanel.Controls.Add(_startupCheckBox);
        _startupCheckBox.CheckedChanged += (_, _) =>
        {
            if (!_startupSettingsLoaded) return;
            try { _startupService.SetEnabled(_startupCheckBox.Checked, Application.ExecutablePath); }
            catch (Exception ex) when (ex is UnauthorizedAccessException or System.Security.SecurityException or IOException)
            {
                _startupSettingsLoaded = false;
                _startupCheckBox.Checked = !_startupCheckBox.Checked;
                _startupSettingsLoaded = true;
                MessageBox.Show("保存开机自启设置失败：" + ex.Message, "开机自启", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        };
    }

    private void LoadStartupSetting()
    {
        try
        {
            _startupCheckBox.Checked = _startupService.IsEnabled();
            _startupService.SetEnabled(_startupCheckBox.Checked, Application.ExecutablePath);
        }
        catch (Exception ex) when (ex is UnauthorizedAccessException or System.Security.SecurityException or IOException)
        {
            MessageBox.Show("读取或应用开机自启设置失败：" + ex.Message, "开机自启", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        _startupSettingsLoaded = true;
    }
}
