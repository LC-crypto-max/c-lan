using c_lan.Models;
using Microsoft.Data.Sqlite;

namespace c_lan.Data
{
    public class SQLiteProvider : IDatabaseProvider
    {
        public DatabaseType SupportedDatabaseType => DatabaseType.SQLite;
        //连接配置有效性判断
        public string? ValidateProfile(ConnectionProfile profile)
        {
            if (string.IsNullOrWhiteSpace(profile.DatabaseFilePath))
            {
                return "请输入 SQLite 数据库文件路径";
            }

            if (!File.Exists(profile.DatabaseFilePath))
            {
                return "SQLite 数据库文件不存在";
            }

            return null;
        }

        public async Task<ConnectionResult> TestConnectionAsync(ConnectionProfile profile, CancellationToken token)
        {
            string? validationError = ValidateProfile(profile);
            if (validationError is not null)
            {
                return new ConnectionResult{IsSuccess = false,ErrorMessage = validationError};
            }

            ConnectionResult result = new ConnectionResult();
            var connectionString = SQLiteConnectionStringBuilder(profile);
            using (var conn = new SqliteConnection(connectionString))
            {
                try
                {
                    await conn.OpenAsync(token);
                    //此处加入成功判断
                    result.IsSuccess = true;
                    result.ErrorMessage = null;
                }

                catch (OperationCanceledException ex)
                {
                    result.IsSuccess = false;
                    result.ErrorMessage = "连接被取消" + ex.Message;
                }

                catch (SqliteException ex)
                {
                    result.IsSuccess = false;
                    result.ErrorMessage = "SQLite连接失败" + ex.Message;
                }

                return result;
            }
        }
        public Task<List<string>> GetDatabasesAsync(ConnectionProfile profile, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            //使用main来实现逻辑
            return Task.FromResult(new List<string> { "main" });
        }
        public async Task<List<DatabaseObjectInfo>> GetObjectsAsync(ConnectionProfile profile, string databaseName, CancellationToken token)
        {
            //实现sqlite元数据读取
            token.ThrowIfCancellationRequested();
            //判断现在是不是main数据库范围
            if(!string.Equals(databaseName, "main", StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException("SQLite当前仅支持main数据库范围", nameof(databaseName));
            }

            string? validationError = ValidateProfile(profile);
            //检查是否存在有效性错误
            if (validationError is not null)
            {
                throw new ArgumentException(validationError, nameof(profile));
            }
            const string sql = """
                SELECT name,type
                FROM sqlite_schema
                WHERE type IN ('table','view')
                	AND name NOT LIKE 'sqlite_%'
                	ORDER BY type,name
                """;
            //这里仍然使用using进行连接管理
            using var conn = new SqliteConnection(SQLiteConnectionStringBuilder(profile));

            await conn.OpenAsync(token);
            //使用sql命令行查询
            using var cmd = new SqliteCommand(sql, conn);
            using var reader = await cmd.ExecuteReaderAsync(token);

            var objects = new List<DatabaseObjectInfo>();
            while(await reader.ReadAsync(token))
            {
                string sqliteObjectType = reader.GetString(1);
                //实际查询结果构建
                objects.Add(new DatabaseObjectInfo { ObjectName = reader.GetString(0), ObjectType = string.Equals(sqliteObjectType, "view", StringComparison.OrdinalIgnoreCase) ? "View" : "Table",
                DatabaseName = "main",SchemaName = "main",IsSystemObject = false,Description = string.Empty
                });
            }

            return objects;
        }
        public Task<List<ColumnInfo>> GetColumnsAsync(ConnectionProfile profile, string databaseName, string objectName, CancellationToken token)
        {
            //SQLite元数据留到下一阶段。
            throw new NotImplementedException();
        }
        public Task<QueryResult> PreviewAsync(ConnectionProfile profile, string databaseName, string objectName,int maxRows, CancellationToken token)
        {
            //SQLite预览留到下一阶段。
            throw new NotImplementedException();
        }
        public Task<QueryResult> ExecuteQueryAsync(ConnectionProfile profile, QueryRequest request, CancellationToken token)
        {
            //暂时不实现该功能
            throw new NotImplementedException();
        }
        //与MySQL类似的连接字符串拼接
        private string SQLiteConnectionStringBuilder(ConnectionProfile profile)
        {
            SqliteConnectionStringBuilder builder = new SqliteConnectionStringBuilder();

            builder.DataSource = profile.DatabaseFilePath;
            //使用读写模式
            builder.Mode = SqliteOpenMode.ReadWrite;

            return builder.ToString();
        }
    }
}
