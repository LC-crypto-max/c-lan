using System.Text.Json;
using c_lan.Models;
using Microsoft.Data.Sqlite;

namespace c_lan.Configuration;

/// <summary>独立于仪器源库保存扫码队列；完成记录与来源水位在同一事务提交。</summary>
public sealed class ElectricCheckScanQueueStore
{
    private readonly string _path;

    public ElectricCheckScanQueueStore(string? directory = null)
    {
        directory ??= Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "c-lan");
        Directory.CreateDirectory(directory);
        _path = Path.Combine(directory, "electric-scan-queue.db");
        using SqliteConnection connection = Open();
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText = """
            CREATE TABLE IF NOT EXISTS scan_queue (
                sequence INTEGER PRIMARY KEY AUTOINCREMENT,
                server_key TEXT NOT NULL,
                source_key TEXT NOT NULL,
                scan_id TEXT NOT NULL UNIQUE,
                sn_code TEXT COLLATE BINARY NOT NULL,
                payload TEXT NOT NULL,
                UNIQUE(server_key, sn_code)
            );
            CREATE TABLE IF NOT EXISTS scan_progress (
                source_key TEXT PRIMARY KEY,
                last_row INTEGER NOT NULL CHECK(last_row >= 0)
            );
            CREATE INDEX IF NOT EXISTS idx_scan_queue_source ON scan_queue(source_key, sequence);
            """;
        command.ExecuteNonQuery();
    }

    public List<ElectricCheckScanState> Load(string sourceKey)
    {
        using SqliteConnection connection = Open();
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText = "SELECT sequence,payload FROM scan_queue WHERE source_key=$source ORDER BY sequence";
        command.Parameters.AddWithValue("$source", sourceKey);
        using SqliteDataReader reader = command.ExecuteReader();
        List<ElectricCheckScanState> states = [];
        while (reader.Read())
        {
            ElectricCheckScanState state = JsonSerializer.Deserialize<ElectricCheckScanState>(reader.GetString(1))
                ?? throw new InvalidDataException("扫码队列记录为空，请恢复缓存后重试。");
            if (string.IsNullOrWhiteSpace(state.ScanId) || string.IsNullOrWhiteSpace(state.SnCode))
                throw new InvalidDataException("扫码队列记录不完整，请恢复缓存后重试。");
            state.Sequence = reader.GetInt64(0);
            states.Add(state);
        }
        return states;
    }

    public ElectricCheckScanState? Find(string serverKey, string sn)
    {
        using SqliteConnection connection = Open();
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText = "SELECT sequence,payload FROM scan_queue WHERE server_key=$server AND sn_code=$sn";
        command.Parameters.AddWithValue("$server", serverKey);
        command.Parameters.AddWithValue("$sn", sn);
        using SqliteDataReader reader = command.ExecuteReader();
        if (!reader.Read()) return null;
        ElectricCheckScanState state = JsonSerializer.Deserialize<ElectricCheckScanState>(reader.GetString(1))
            ?? throw new InvalidDataException("扫码历史记录为空。");
        state.Sequence = reader.GetInt64(0);
        return state;
    }

    public long? Watermark(string sourceKey)
    {
        using SqliteConnection connection = Open();
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText = "SELECT last_row FROM scan_progress WHERE source_key=$source";
        command.Parameters.AddWithValue("$source", sourceKey);
        object? value = command.ExecuteScalar();
        return value is null ? null : Convert.ToInt64(value);
    }

    public void Add(string serverKey, string sourceKey, ElectricCheckScanState state, long initialWatermark)
    {
        using SqliteConnection connection = Open();
        using SqliteTransaction transaction = connection.BeginTransaction();
        using SqliteCommand command = connection.CreateCommand();
        command.Transaction = transaction;
        command.CommandText = """
            INSERT INTO scan_queue(server_key,source_key,scan_id,sn_code,payload)
            VALUES($server,$source,$scan,$sn,$payload);
            SELECT last_insert_rowid();
            """;
        command.Parameters.AddWithValue("$server", serverKey);
        command.Parameters.AddWithValue("$source", sourceKey);
        command.Parameters.AddWithValue("$scan", state.ScanId);
        command.Parameters.AddWithValue("$sn", state.SnCode);
        command.Parameters.AddWithValue("$payload", JsonSerializer.Serialize(state));
        state.Sequence = Convert.ToInt64(command.ExecuteScalar());
        command.Parameters.Clear();
        command.CommandText = "INSERT OR IGNORE INTO scan_progress(source_key,last_row) VALUES($source,$row)";
        command.Parameters.AddWithValue("$source", sourceKey);
        command.Parameters.AddWithValue("$row", initialWatermark);
        command.ExecuteNonQuery();
        transaction.Commit();
    }

    public void Save(string sourceKey, ElectricCheckScanState state, long? completedThrough = null)
    {
        using SqliteConnection connection = Open();
        using SqliteTransaction transaction = connection.BeginTransaction();
        using SqliteCommand command = connection.CreateCommand();
        command.Transaction = transaction;
        command.CommandText = "UPDATE scan_queue SET payload=$payload WHERE source_key=$source AND scan_id=$scan";
        command.Parameters.AddWithValue("$payload", JsonSerializer.Serialize(state));
        command.Parameters.AddWithValue("$source", sourceKey);
        command.Parameters.AddWithValue("$scan", state.ScanId);
        if (command.ExecuteNonQuery() != 1) throw new InvalidDataException("扫码队列记录丢失，已暂停处理。");
        if (completedThrough is not null)
        {
            command.Parameters.Clear();
            command.CommandText = "UPDATE scan_progress SET last_row=MAX(last_row,$row) WHERE source_key=$source";
            command.Parameters.AddWithValue("$source", sourceKey);
            command.Parameters.AddWithValue("$row", completedThrough.Value);
            if (command.ExecuteNonQuery() != 1) throw new InvalidDataException("扫码来源水位丢失，已暂停处理。");
        }
        transaction.Commit();
    }

    private SqliteConnection Open()
    {
        SqliteConnection connection = new(new SqliteConnectionStringBuilder
        { DataSource = _path, Mode = SqliteOpenMode.ReadWriteCreate, Pooling = false }.ToString());
        try { connection.Open(); return connection; }
        catch { connection.Dispose(); throw; }
    }
}
