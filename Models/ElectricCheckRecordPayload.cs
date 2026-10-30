namespace c_lan.Models;

public sealed class ElectricCheckRecordPayload
{
    public long LastSourceRowId { get; set; }
    public int SourceRecordCount { get; set; }
    public long SourceRowId { get; set; }
    public string TestDateTime { get; set; } = "";
    public string LightName { get; set; } = "";
    [System.Text.Json.Serialization.JsonIgnore]
    public string TestItemName { get; set; } = "";
    [System.Text.Json.Serialization.JsonIgnore]
    public string? HighLimit { get; set; }
    [System.Text.Json.Serialization.JsonIgnore]
    public string? LowerLimit { get; set; }
    [System.Text.Json.Serialization.JsonIgnore]
    public string? TestValue { get; set; }
    public string TestResult { get; set; } = "";
    public string LightType { get; set; } = "";
    public string TestId { get; set; } = "";
    [System.Text.Json.Serialization.JsonIgnore]
    public int TemplateItemId { get; set; }
    [System.Text.Json.Serialization.JsonIgnore]
    public string? TemplateItemName { get; set; }
    [System.Text.Json.Serialization.JsonIgnore]
    public string? Remark { get; set; }
    public string? SnCode { get; set; }
}
