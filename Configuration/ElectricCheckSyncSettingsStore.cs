using System.Text.Json;
using c_lan.Models;

namespace c_lan.Configuration;

public sealed class ElectricCheckSyncSettingsStore
{
    private readonly string _path;

    public ElectricCheckSyncSettingsStore(string? directory = null)
    {
        _path = Path.Combine(directory ?? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "c-lan"), "electric-check-settings.json");
    }

    public ElectricCheckSyncSettings? Load()
    {
        if (!File.Exists(_path)) return null;
        var settings = JsonSerializer.Deserialize<ElectricCheckSyncSettings>(File.ReadAllText(_path))
            ?? throw new JsonException("同步配置为空。");
        if (settings.DeviceNo is null || settings.SqliteFilePath is null || settings.ServerBaseUrl is null ||
            string.IsNullOrWhiteSpace(settings.TableName) || settings.PollIntervalSeconds <= 0 || settings.BatchSize <= 0)
            throw new JsonException("同步配置字段无效。");
        return settings;
    }

    public void Save(ElectricCheckSyncSettings settings)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(_path)!);
        string temporaryPath = _path + ".tmp";
        // 完整写入后再替换，避免中途退出破坏上次保存的配置。
        File.WriteAllText(temporaryPath, JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true }));
        File.Move(temporaryPath, _path, overwrite: true);
    }
}
