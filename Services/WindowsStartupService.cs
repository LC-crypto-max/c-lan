using Microsoft.Win32;

namespace c_lan.Services;

public sealed class WindowsStartupService
{
    private readonly string _runKeyPath;
    private readonly string _settingsKeyPath;

    public WindowsStartupService(string runKeyPath = @"Software\Microsoft\Windows\CurrentVersion\Run",
        string settingsKeyPath = @"Software\c-lan")
    {
        _runKeyPath = runKeyPath;
        _settingsKeyPath = settingsKeyPath;
    }

    public bool IsEnabled()
    {
        using RegistryKey? settings = Registry.CurrentUser.OpenSubKey(_settingsKeyPath);
        return settings?.GetValue("StartWithWindows") is not int enabled || enabled != 0;
    }

    public void SetEnabled(bool enabled, string executablePath)
    {
        if (enabled) ArgumentException.ThrowIfNullOrWhiteSpace(executablePath);
        using RegistryKey settings = Registry.CurrentUser.CreateSubKey(_settingsKeyPath);
        using RegistryKey run = Registry.CurrentUser.CreateSubKey(_runKeyPath);
        if (enabled) run.SetValue("c-lan", $"\"{executablePath}\"", RegistryValueKind.String);
        else run.DeleteValue("c-lan", throwOnMissingValue: false);
        settings.SetValue("StartWithWindows", enabled ? 1 : 0, RegistryValueKind.DWord);
    }
}
