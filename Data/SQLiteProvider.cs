using c_lan.Models;
using c_lan.Utilities;
using Microsoft.Data.Sqlite;
using System.Data;
using System.Diagnostics;

namespace c_lan.Data
{
    public class SQLiteProvider : IDatabaseProvider
    {
        public DatabaseType SupportedDatabaseType => DatabaseType.SQLite;

        //连接配置有效性判断
        public string? ValidateProfile(ConnectionProfile profile)
        {
            if (profile is null) return "连接信息为空";
            if (string.IsNullOrWhiteSpace(profile.DatabaseFilePath)) return "请输入 SQLite 数据库文件路径";
            if (!File.Exists(profile.DatabaseFilePath)) return "SQLite 数据库文件不存在";
            return null;
        }

        public async Task<ConnectionResult> TestConnectionAsync(ConnectionProfile profile, CancellationToken token)
        {
            string? validationError = ValidateProfile(profile);
            if (validationError is not null) return new ConnectionResult { IsSuccess = false, ErrorMessage = validationError };
            try
            {
                using var conn = new SqliteConnection(BuildConnectionString(profile));
                await conn.OpenAsync(token);
                return new ConnectionResult { IsSuccess = true };
            }
            catch (OperationCanceledException) { return new ConnectionResult { IsSuccess = false, ErrorMessage = "SQLite连接已取消" }; }
            catch (SqliteException ex) { return new ConnectionResult { IsSuccess = false, ErrorMessage = "SQLite连接失败：" + ex.Message }; }
        }

        public Task<List<string>> GetDatabasesAsync(ConnectionProfile profile, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            //读取main数据库的内容
            return Task.FromResult(new List<string> { "main" });
        }

        public async Task<List<DatabaseObjectInfo>> GetObjectsAsync(ConnectionProfile profile, string databaseName, CancellationToken token)
        {
            ValidateDatabaseRequest(profile, databaseName);
            //找出表和视图
            const string sql = """
                SELECT name,type FROM sqlite_schema
                WHERE type IN ('table','view') AND name NOT LIKE 'sqlite_%'
                ORDER BY type,name
                """;
            using var conn = new SqliteConnection(BuildConnectionString(profile));
            await conn.OpenAsync(token);
            using var cmd = new SqliteCommand(sql, conn);
            using var reader = await cmd.ExecuteReaderAsync(token);
            var objects = new List<DatabaseObjectInfo>();
            while (await reader.ReadAsync(token))
            {
                string sqliteType = reader.GetString(1);
                objects.Add(new DatabaseObjectInfo
                {
                    ObjectName = reader.GetString(0),
                    ObjectType = string.Equals(sqliteType, "view", StringComparison.OrdinalIgnoreCase) ? "View" : "Table",
                    DatabaseName = "main", SchemaName = "main", IsSystemObject = false, Description = string.Empty
                });
            }
            return objects;
        }

        public Task<List<ColumnInfo>> GetColumnsAsync(ConnectionProfile profile, string databaseName, string objectName, CancellationToken token)
        {
            return GetColumnsCoreAsync(profile, databaseName, objectName, token);
        }

        private async Task<List<ColumnInfo>> GetColumnsCoreAsync(ConnectionProfile profile, string databaseName, string objectName, CancellationToken token)
        {
            ValidateObjectRequest(profile, databaseName, objectName);
            string sql = $"PRAGMA table_info({QuoteIdentifier(objectName)})";
            using var conn = new SqliteConnection(BuildConnectionString(profile));
            await conn.OpenAsync(token);
            using var cmd = new SqliteCommand(sql, conn);
            using var reader = await cmd.ExecuteReaderAsync(token);
            var columns = new List<ColumnInfo>();
            while (await reader.ReadAsync(token))
            {
                //PRAGMA table_info返回：序号、名称、类型、非空、默认值、主键顺序。
                columns.Add(new ColumnInfo
                {
                    OrdinalPosition = reader.GetInt32(0) + 1,
                    ColumnName = reader.GetString(1),
                    DataType = reader.IsDBNull(2) ? String.Empty : reader.GetString(2),
                    FullColumnType = reader.IsDBNull(2) ? String.Empty : reader.GetString(2),
                    IsNullable = reader.GetInt32(3) == 0,
                    DefaultValue = reader.IsDBNull(4) ? null : reader.GetValue(4)?.ToString(),
                    IsPrimaryKey = reader.GetInt32(5) > 0
                });
            }
            return columns;
        }

        public async Task<QueryResult> PreviewAsync(ConnectionProfile profile, string databaseName, string objectName, int maxRows, CancellationToken token)
        {
            QueryResult result = new QueryResult();
            Stopwatch stopwatch = Stopwatch.StartNew();
            try
            {
                ValidateObjectRequest(profile, databaseName, objectName);
                int safeMaxRows = Math.Clamp(maxRows, 1, 200);
                string sql = $"SELECT * FROM {QuoteIdentifier(objectName)} LIMIT $fetchRows";
                using var conn = new SqliteConnection(BuildConnectionString(profile));
                await conn.OpenAsync(token);
                using var cmd = new SqliteCommand(sql, conn) { CommandTimeout = (int)Math.Clamp(profile.ConnectionTimeout, 1u, 3600u) };
                cmd.Parameters.AddWithValue("$fetchRows", safeMaxRows + 1);
                using var reader = await cmd.ExecuteReaderAsync(token);
                (DataTable table, bool truncated) =
                    await BoundedDataTableReader.LoadAsync(reader, safeMaxRows, token);
                result.IsSuccess = true; result.Rows = table; result.RowCount = table.Rows.Count; result.IsTruncated = truncated;
            }
            catch (OperationCanceledException) { result.ErrorMessage = "SQLite预览已取消"; }
            catch (SqliteException ex) { result.ErrorMessage = "SQLite预览失败：" + ex.Message; }
            catch (Exception ex) { result.ErrorMessage = "SQLite预览失败：" + ex.Message; }
            finally { stopwatch.Stop(); result.ExecutionTime = (int)stopwatch.ElapsedMilliseconds; }
            return result;
        }

        public async Task<QueryResult> ExecuteQueryAsync(ConnectionProfile profile, QueryRequest request, CancellationToken token)
        {
            QueryResult result = new QueryResult();
            Stopwatch stopwatch = Stopwatch.StartNew();
            try
            {
                string? validationError = ValidateProfile(profile);
                if (validationError is not null) { result.ErrorMessage = validationError; return result; }
                int safeMaxRows = Math.Clamp(request.MaxRows, 1, 2000);
                using var conn = new SqliteConnection(BuildConnectionString(profile, request.IsReadOnly));
                await conn.OpenAsync(token);
                using var cmd = new SqliteCommand(request.SqlText, conn) { CommandTimeout = request.TimeoutSeconds };
                if (request.SqlText.TrimStart().StartsWith("ALTER", StringComparison.OrdinalIgnoreCase))
                {
                    await cmd.ExecuteNonQueryAsync(token);
                    result.IsSuccess = true;
                    result.Rows = new DataTable();
                    result.RowCount = 0;
                    return result;
                }
                using var reader = await cmd.ExecuteReaderAsync(token);
                (DataTable table, bool truncated) =
                    await BoundedDataTableReader.LoadAsync(reader, safeMaxRows, token);
                result.IsSuccess = true; result.Rows = table; result.RowCount = table.Rows.Count; result.IsTruncated = truncated;
            }
            catch (OperationCanceledException) { result.ErrorMessage = "SQLite查询已取消"; }
            catch (SqliteException ex) { result.ErrorMessage = "SQLite查询失败：" + ex.Message; }
            catch (Exception ex) { result.ErrorMessage = "SQLite查询失败：" + ex.Message; }
            finally { stopwatch.Stop(); result.ExecutionTime = (int)stopwatch.ElapsedMilliseconds; }
            return result;
        }

        private static string BuildConnectionString(ConnectionProfile profile, bool isReadOnly = true)
        {
            SqliteConnectionStringBuilder builder = new SqliteConnectionStringBuilder
            {
                DataSource = profile.DatabaseFilePath,
                Mode = isReadOnly ? SqliteOpenMode.ReadOnly : SqliteOpenMode.ReadWrite,
                DefaultTimeout = (int)Math.Clamp(profile.ConnectionTimeout, 1u, 3600u)
            };
            return builder.ToString();
        }

        private static void ValidateDatabaseRequest(ConnectionProfile profile, string databaseName)
        {
            string? error = profile is null ? "连接信息为空" : new SQLiteProvider().ValidateProfile(profile);
            if (error is not null) throw new ArgumentException(error, nameof(profile));
            if (!string.Equals(databaseName, "main", StringComparison.OrdinalIgnoreCase)) throw new ArgumentException("SQLite当前仅支持main数据库范围", nameof(databaseName));
        }

        private static void ValidateObjectRequest(ConnectionProfile profile, string databaseName, string objectName)
        {
            ValidateDatabaseRequest(profile, databaseName);
            if (string.IsNullOrWhiteSpace(objectName)) throw new ArgumentException("对象名称不能为空", nameof(objectName));
        }

        private static string QuoteIdentifier(string identifier) => "\"" + identifier.Replace("\"", "\"\"") + "\"";
    }
}
