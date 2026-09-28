using c_lan.Models;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace c_lan.Configuration;

public class ConnectionProfileStore
{
    private readonly string _configurationFilePath;
    private readonly string? _legacyPath;
    private static readonly JsonSerializerOptions Options = new() { WriteIndented = true, PropertyNameCaseInsensitive = true };
    private const string Prefix = "dpapi:";

    public ConnectionProfileStore(string? configurationFilePath = null)
    {
        if (configurationFilePath is null) _legacyPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "c-lan", "connections.json");
        _configurationFilePath = configurationFilePath ?? Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "c-lan-browser", "connections.json");
    }

    public async Task<List<ConnectionProfile>> LoadAsync(CancellationToken token)
    {
        string path = File.Exists(_configurationFilePath) ? _configurationFilePath : _legacyPath ?? _configurationFilePath;
        if (!File.Exists(path)) return new();
        try
        {
            var profiles = JsonSerializer.Deserialize<List<ConnectionProfile>>(await File.ReadAllTextAsync(path, token), Options) ?? new();
            foreach (var profile in profiles)
            {
                if (!profile.Password.StartsWith(Prefix, StringComparison.Ordinal)) continue;
                try
                {
                    profile.Password = Encoding.UTF8.GetString(ProtectedData.Unprotect(
                        Convert.FromBase64String(profile.Password[Prefix.Length..]), null, DataProtectionScope.CurrentUser));
                }
                catch (Exception ex) when (ex is CryptographicException or FormatException)
                {
                    // 换 Windows 用户后要求重新输入密码，仍保留其余连接参数。
                    profile.Password = "";
                }
            }
            return profiles;
        }
        catch (JsonException ex) { throw new InvalidOperationException("连接配置 JSON 损坏，请先备份并检查配置文件。", ex); }
    }

    public async Task SaveAsync(List<ConnectionProfile> profiles, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(profiles);
        // 序列化副本，避免把界面当前使用的密码替换成密文。
        var copies = JsonSerializer.Deserialize<List<ConnectionProfile>>(JsonSerializer.Serialize(profiles, Options), Options)!;
        foreach (var profile in copies)
            profile.Password = !profile.SavePassword || string.IsNullOrEmpty(profile.Password) ? "" : Prefix + Convert.ToBase64String(
                ProtectedData.Protect(Encoding.UTF8.GetBytes(profile.Password), null, DataProtectionScope.CurrentUser));
        string path = Path.GetFullPath(_configurationFilePath);
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        string temporary = path + "." + Guid.NewGuid().ToString("N") + ".tmp";
        try
        {
            await File.WriteAllTextAsync(temporary, JsonSerializer.Serialize(copies, Options), Encoding.UTF8, token);
            token.ThrowIfCancellationRequested();
            File.Move(temporary, path, overwrite: true);
        }
        finally { if (File.Exists(temporary)) File.Delete(temporary); }
    }
}
