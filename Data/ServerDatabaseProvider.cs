using System.Data.Common;
using System.Diagnostics;
using c_lan.Models;
using c_lan.Utilities;

namespace c_lan.Data;

// SQL Server 和 Oracle 共用 ADO.NET 的资源释放、取消、限行和结果映射。
public abstract class ServerDatabaseProvider : IDatabaseProvider
{
    public abstract DatabaseType SupportedDatabaseType { get; }
    public abstract string? ValidateProfile(ConnectionProfile profile);
    protected abstract DbConnection CreateConnection(ConnectionProfile profile, string? database);
    protected abstract string DatabasesSql { get; }
    protected abstract string ObjectsSql { get; }
    protected abstract string ColumnsSql { get; }
    protected abstract string PreviewSql(string schema, string name, int rows);
    protected virtual string ParameterPrefix => "@";
    protected virtual string DefaultSchema(ConnectionProfile profile) => "dbo";

    public async Task<ConnectionResult> TestConnectionAsync(ConnectionProfile profile, CancellationToken token)
    {
        string? error = ValidateProfile(profile);
        if (error is not null) return new() { ErrorMessage = error };
        try
        {
            using var connection = CreateConnection(profile, profile.DefaultDatabase);
            await connection.OpenAsync(token).ConfigureAwait(false);
            return new() { IsSuccess = true };
        }
        catch (OperationCanceledException) { throw; }
        catch (Exception ex) { return new() { ErrorMessage = $"{SupportedDatabaseType} 连接失败：{ex.Message}" }; }
    }

    public async Task<List<string>> GetDatabasesAsync(ConnectionProfile profile, CancellationToken token)
    {
        using var connection = CreateConnection(profile, profile.DefaultDatabase);
        await connection.OpenAsync(token).ConfigureAwait(false);
        using var command = CreateCommand(connection, DatabasesSql, profile.ConnectionTimeout);
        using var reader = await command.ExecuteReaderAsync(token).ConfigureAwait(false);
        List<string> result = new();
        while (await reader.ReadAsync(token).ConfigureAwait(false)) result.Add(reader.GetString(0));
        return result;
    }

    public async Task<List<DatabaseObjectInfo>> GetObjectsAsync(ConnectionProfile profile, string databaseName, CancellationToken token)
    {
        using var connection = CreateConnection(profile, databaseName);
        await connection.OpenAsync(token).ConfigureAwait(false);
        using var command = CreateCommand(connection, ObjectsSql, profile.ConnectionTimeout);
        if (SupportedDatabaseType == DatabaseType.Oracle) AddParameter(command, "scope", databaseName);
        using var reader = await command.ExecuteReaderAsync(token).ConfigureAwait(false);
        List<DatabaseObjectInfo> result = new();
        while (await reader.ReadAsync(token).ConfigureAwait(false))
            result.Add(new() { ObjectName = reader.GetString(0), SchemaName = reader.GetString(1),
                DatabaseName = databaseName, ObjectType = reader.GetString(2) });
        return result;
    }

    public async Task<List<ColumnInfo>> GetColumnsAsync(ConnectionProfile profile, string databaseName, string objectName,
        CancellationToken token, string? schemaName = null)
    {
        using var connection = CreateConnection(profile, databaseName);
        await connection.OpenAsync(token).ConfigureAwait(false);
        using var command = CreateCommand(connection, ColumnsSql, profile.ConnectionTimeout);
        AddParameter(command, "scope", schemaName ?? (SupportedDatabaseType == DatabaseType.Oracle ? databaseName : DefaultSchema(profile)));
        AddParameter(command, "objectName", objectName);
        using var reader = await command.ExecuteReaderAsync(token).ConfigureAwait(false);
        List<ColumnInfo> result = new();
        while (await reader.ReadAsync(token).ConfigureAwait(false))
        {
            result.Add(ReadColumn(reader));
        }
        return result;
    }

    // 前六列为公共元数据，其余字段由各数据库按自己的类型规则解释。
    protected virtual ColumnInfo ReadColumn(DbDataReader reader) => new()
    {
        ColumnName = reader.GetString(0), DataType = reader.GetString(1),
        FullColumnType = reader.GetString(1), OrdinalPosition = Convert.ToInt32(reader.GetValue(2)),
        IsNullable = Convert.ToInt32(reader.GetValue(3)) == 1,
        IsPrimaryKey = Convert.ToInt32(reader.GetValue(4)) == 1,
        DefaultValue = reader.IsDBNull(5) ? null : Convert.ToString(reader.GetValue(5))
    };

    protected static int? ReadNullableInt(DbDataReader reader, string name) =>
        reader[name] is DBNull ? null : Convert.ToInt32(reader[name]);

    public Task<QueryResult> PreviewAsync(ConnectionProfile profile, string databaseName, string objectName,
        int maxRows, CancellationToken token, string? schemaName = null)
    {
        int rows = Math.Clamp(maxRows, 1, 200);
        string schema = schemaName ?? (SupportedDatabaseType == DatabaseType.Oracle ? databaseName : DefaultSchema(profile));
        return ExecuteQueryAsync(profile, new QueryRequest { DatabaseName = databaseName,
            SqlText = PreviewSql(schema, objectName, rows + 1), MaxRows = rows,
            TimeoutSeconds = (int)Math.Clamp(profile.ConnectionTimeout, 1u, 3600u) }, token);
    }

    public async Task<QueryResult> ExecuteQueryAsync(ConnectionProfile profile, QueryRequest request, CancellationToken token)
    {
        Stopwatch timer = Stopwatch.StartNew();
        try
        {
            using var connection = CreateConnection(profile, request.DatabaseName);
            await connection.OpenAsync(token).ConfigureAwait(false);
            if (SupportedDatabaseType == DatabaseType.Oracle && !string.IsNullOrWhiteSpace(request.DatabaseName))
            {
                using var scope = CreateCommand(connection,
                    "ALTER SESSION SET CURRENT_SCHEMA = " + OracleProvider.QuoteIdentifier(request.DatabaseName), (uint)request.TimeoutSeconds);
                await scope.ExecuteNonQueryAsync(token).ConfigureAwait(false);
            }
            string sql = request.SqlText.TrimEnd().TrimEnd(';');
            using var command = CreateCommand(connection, sql, (uint)request.TimeoutSeconds);
            using var reader = await command.ExecuteReaderAsync(token).ConfigureAwait(false);
            var (table, truncated) = await BoundedDataTableReader.LoadAsync(reader, Math.Clamp(request.MaxRows, 1, 2000), token).ConfigureAwait(false);
            return new() { IsSuccess = true, Rows = table, RowCount = table.Rows.Count, IsTruncated = truncated,
                ExecutionTime = (int)timer.ElapsedMilliseconds };
        }
        catch (OperationCanceledException) { throw; }
        catch (Exception ex) when (!token.IsCancellationRequested)
        { return new() { ErrorMessage = $"{SupportedDatabaseType} 查询失败：{ex.Message}", ExecutionTime = (int)timer.ElapsedMilliseconds }; }
        catch (Exception) { throw new OperationCanceledException(token); }
    }

    protected virtual DbCommand CreateCommand(DbConnection connection, string sql, uint timeout)
    {
        var command = connection.CreateCommand();
        command.CommandText = sql;
        command.CommandTimeout = (int)Math.Clamp(timeout, 1u, 3600u);
        return command;
    }

    private void AddParameter(DbCommand command, string name, string value)
    {
        var parameter = command.CreateParameter();
        parameter.ParameterName = ParameterPrefix + name;
        parameter.Value = value;
        command.Parameters.Add(parameter);
    }
}
