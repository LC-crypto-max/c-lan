using c_lan.Services;
using Microsoft.Win32;

internal static class WindowsStartupChecks
{
    internal static void Run()
    {
        string root = @"Software\c-lan-checks-" + Guid.NewGuid().ToString("N");
        string runPath = root + @"\Run";
        string settingsPath = root + @"\Settings";
        const string executable = @"C:\Program Files\电检工具\c#lan.exe";
        try
        {
            WindowsStartupService service = new(runPath, settingsPath);
            Assert(service.IsEnabled(), "startup must default to enabled");
            using (RegistryKey run = Registry.CurrentUser.CreateSubKey(runPath))
                run.SetValue("unrelated-program", "preserve-me");
            service.SetEnabled(true, executable);
            using (RegistryKey run = Registry.CurrentUser.OpenSubKey(runPath)!)
                Assert((string?)run.GetValue("c-lan") == $"\"{executable}\"", "startup must quote executable paths");
            service.SetEnabled(false, executable);
            Assert(!new WindowsStartupService(runPath, settingsPath).IsEnabled(), "disabled preference must survive restart");
            using (RegistryKey run = Registry.CurrentUser.OpenSubKey(runPath)!)
            {
                Assert(run.GetValue("c-lan") is null, "disabling must remove the application's startup entry");
                Assert((string?)run.GetValue("unrelated-program") == "preserve-me", "other startup entries must remain unchanged");
            }
            service.SetEnabled(service.IsEnabled(), executable);
            using (RegistryKey run = Registry.CurrentUser.OpenSubKey(runPath)!)
                Assert(run.GetValue("c-lan") is null, "restarting must not re-enable disabled startup");
            service.SetEnabled(true, executable);
            Assert(new WindowsStartupService(runPath, settingsPath).IsEnabled(), "startup can be enabled again");
            Console.WriteLine("Optional Windows startup and preference persistence checks passed.");
        }
        finally
        {
            Registry.CurrentUser.DeleteSubKeyTree(root, throwOnMissingSubKey: false);
        }
    }

    private static void Assert(bool condition, string message)
    {
        if (!condition) throw new InvalidOperationException(message);
    }
}
