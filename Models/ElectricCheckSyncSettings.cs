namespace c_lan.Models;

public sealed class ElectricCheckSyncSettings
{
    public bool Enabled { get; set; }
    public string DeviceNo { get; set; } = "";
    public string SqliteFilePath { get; set; } = "";
    public string TableName { get; set; } = "ElectricCheck";
    public string ServerBaseUrl { get; set; } = "";
    public int PollIntervalSeconds { get; set; } = 5;
    public int BatchSize { get; set; } = 200;
}
