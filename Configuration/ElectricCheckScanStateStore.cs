using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using c_lan.Models;

namespace c_lan.Configuration;

public sealed class ElectricCheckScanStateStore
{
    private readonly string _directory;

    public ElectricCheckScanStateStore(string? directory = null) =>
        _directory = directory ?? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "c-lan");

    public async Task<ElectricCheckScanState?> LoadAsync(string key, CancellationToken token)
    {
        string path = PathFor(key);
        if (!File.Exists(path)) return null;
        return JsonSerializer.Deserialize<ElectricCheckScanState>(await File.ReadAllTextAsync(path, token))
            ?? throw new InvalidDataException("扫码状态文件为空，请核实后恢复检测记录。");
    }

    public async Task SaveAsync(string key, ElectricCheckScanState state, CancellationToken token)
    {
        Directory.CreateDirectory(_directory);
        string path = PathFor(key);
        await File.WriteAllTextAsync(path + ".tmp", JsonSerializer.Serialize(state), token);
        File.Move(path + ".tmp", path, overwrite: true);
    }

    private string PathFor(string key) => Path.Combine(_directory,
        $"electric-scan-{Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(key)))[..16]}.json");
}
