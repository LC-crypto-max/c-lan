namespace c_lan.Models;

/// <summary>一次扫码及其观察进度，网络重试和程序重启沿用同一份报文。</summary>
public sealed class ElectricCheckScanState
{
    public string ScanId { get; set; } = "";
    public string DeviceNo { get; set; } = "";
    public string SnCode { get; set; } = "";
    public string ScanTime { get; set; } = "";
    public long SourceRowIdBeforeScan { get; set; }
    public string Status { get; set; } = "PENDING_ENTRY";
    public string? ObservedFingerprint { get; set; }
    public DateTimeOffset? LastChangedAt { get; set; }
    public long ObservedLastRowId { get; set; }
    public int ObservedRecordCount { get; set; }
    public ElectricCheckCompletionPayload? Completion { get; set; }
    public string Message { get; set; } = "";
}

public sealed class ElectricCheckCompletionPayload
{
    public string DeviceNo { get; set; } = "";
    public string CompletedAt { get; set; } = "";
    public long LastSourceRowId { get; set; }
    public int ExpectedRecordCount { get; set; }
}
