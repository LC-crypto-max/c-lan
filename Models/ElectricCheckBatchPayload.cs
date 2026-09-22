namespace c_lan.Models;

public sealed class ElectricCheckBatchPayload
{
    public string RequestId { get; set; } = "";
    public string DeviceNo { get; set; } = "";
    public List<ElectricCheckRecordPayload> Records { get; set; } = [];
}
