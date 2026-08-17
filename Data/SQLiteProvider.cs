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
            //暂时不实现该功能
            throw new NotImplementedException();
        }
        public Task<List<DatabaseObjectInfo>> GetObjectsAsync(
            ConnectionProfile profile, string databaseName, CancellationToken token)
        {
            //第三关只完成MySQL，SQLite元数据留到下一阶段。
            throw new NotImplementedException();
        }
        public Task<List<ColumnInfo>> GetColumnsAsync(
            ConnectionProfile profile, string databaseName, string objectName, CancellationToken token)
        {
            //第三关只完成MySQL，SQLite元数据留到下一阶段。
            throw new NotImplementedException();
        }
        public Task<QueryResult> PreviewAsync(
            ConnectionProfile profile, string databaseName, string objectName,
            int maxRows, CancellationToken token)
        {
            //第三关只完成MySQL，SQLite预览留到下一阶段。
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
