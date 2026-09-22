namespace c_lan.Models;

public sealed class ElectricCheckRecordPayload
{
    public long SourceRowId { get; set; }
    public DateTime TestDateTime { get; set; }
    public string LightName { get; set; } = "";
    public string TestItemName { get; set; } = "";
    public string? HighLimit { get; set; }
    public string? LowerLimit { get; set; }
    public string? TestValue { get; set; }
    public string TestResult { get; set; } = "";
    public string LightType { get; set; } = "";
    public string TestId { get; set; } = "";
    public int TemplateItemId { get; set; }
    public string? TemplateItemName { get; set; }
    public string? Remark { get; set; }
    public string? SnCode { get; set; }
}
