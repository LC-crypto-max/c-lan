namespace c_lan.Models;

public sealed record ElectricCheckSyncResult(int Sent, long LastRowId, string Message);
