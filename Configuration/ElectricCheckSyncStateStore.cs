using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace c_lan.Configuration;

public sealed class ElectricCheckSyncStateStore
{
    private readonly string _directory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "c-lan");

    public async Task<long> LoadAsync(string key, CancellationToken token)
    {
        string path = PathFor(key);
        if (!File.Exists(path)) return 0;
        string json = await File.ReadAllTextAsync(path, token);
        return JsonSerializer.Deserialize<State>(json)?.LastRowId ?? 0;
    }

    public async Task SaveAsync(string key, long lastRowId, CancellationToken token)
    {
        Directory.CreateDirectory(_directory);
        string path = PathFor(key);
        string json = JsonSerializer.Serialize(new State(lastRowId));
        await File.WriteAllTextAsync(path, json, token);
    }

    private string PathFor(string key)
    {
        byte[] hash = SHA256.HashData(Encoding.UTF8.GetBytes(key));
        return Path.Combine(_directory, $"electric-check-{Convert.ToHexString(hash)[..16]}.json");
    }

    private sealed record State(long LastRowId);
}
