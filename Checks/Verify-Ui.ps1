# 离屏检查窗体布局；不读取用户连接、不执行数据库或同步操作。
$ErrorActionPreference = 'Stop'
$previewRoot = Join-Path $env:TEMP ('c-lan-ui-check-' + [Guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Path $previewRoot | Out-Null
$projectPath = [System.Security.SecurityElement]::Escape((Join-Path (Split-Path $PSScriptRoot -Parent) 'c#lan.csproj'))
@"
<Project Sdk="Microsoft.NET.Sdk">
<PropertyGroup><OutputType>Exe</OutputType><TargetFramework>net10.0-windows</TargetFramework><UseWindowsForms>true</UseWindowsForms><ImplicitUsings>enable</ImplicitUsings><Nullable>enable</Nullable></PropertyGroup>
<ItemGroup><ProjectReference Include="$projectPath" /></ItemGroup>
</Project>
"@ | Set-Content -LiteralPath (Join-Path $previewRoot 'Preview.csproj') -Encoding utf8
@'
using System.Reflection;
using c_lan;
internal static class Preview
{
    [STAThread]
    static void Main()
    {
        Application.SetHighDpiMode(HighDpiMode.DpiUnaware);
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        foreach (var size in new[] {new Size(1384, 900), new Size(1120, 800)})
        {
            using var form = new Form1(null!, null!, null!, null!);
            Get<TextBox>(form, "_syncDeviceTextBox").Text = "DEVICE-01";
            form.WindowState = FormWindowState.Normal;
            form.Size = size;
            var shown = typeof(Form1).GetMethod("Form1_Shown", BindingFlags.NonPublic | BindingFlags.Instance)!;
            form.Shown -= (EventHandler)Delegate.CreateDelegate(typeof(EventHandler), form, shown);
            form.ShowInTaskbar = false;
            form.StartPosition = FormStartPosition.Manual;
            form.Location = new Point(-20000, -20000);
            form.Show();
            form.Size = size;
            foreach (int mode in new[] {0, 1, 0})
            {
                Get<ComboBox>(form, "_databaseTypeComboBox").SelectedIndex = mode;
                Layout(form);
                using var bitmap = new Bitmap(form.Width, form.Height);
                form.DrawToBitmap(bitmap, new Rectangle(Point.Empty, bitmap.Size));
                foreach (var field in new[]{"ExecuteQueryButton", "StopQueryButton", "ClearSqlButton", "ReadOnlyCheckBox", "_syncNowButton", "_fullSyncButton", "_databaseTypeComboBox", "ShowPasswordCheckBox", "PasswordText"})
                {
                    Control control = Get<Control>(form, field);
                    if (!control.Visible) continue;
                    if (!control.Parent!.ClientRectangle.Contains(control.Bounds))
                        throw new Exception($"Clipped {field}: {control.Bounds} within {control.Parent.ClientRectangle}");
                    foreach (Control sibling in control.Parent.Controls)
                        if (sibling != control && sibling.Visible && control.Bounds.IntersectsWith(sibling.Bounds))
                            throw new Exception($"Overlapping {field} and {sibling.Name}");
                }
                if (form.Icon is null || Get<NotifyIcon>(form,"_notifyIcon").Icon is null) throw new Exception("Missing application icon");
                var file = Path.Combine(AppContext.BaseDirectory, $"preview-{size.Width}-{mode}.png");
                bitmap.Save(file);
                Console.WriteLine(file);
            }
        }
        Console.WriteLine("PASS: toolbar bounds, button overlap, and window/tray icons at both sizes and database modes.");
    }
    static T Get<T>(Form form, string name) => (T)typeof(Form1).GetField(name, BindingFlags.NonPublic | BindingFlags.Instance)!.GetValue(form)!;
    static void Layout(Control control) { control.PerformLayout(); foreach(Control child in control.Controls) Layout(child); }
}



'@ | Set-Content -LiteralPath (Join-Path $previewRoot 'Program.cs') -Encoding utf8
dotnet run --project (Join-Path $previewRoot 'Preview.csproj')
if ($LASTEXITCODE -ne 0) { throw 'UI layout verification failed.' }

