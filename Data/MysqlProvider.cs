
using c_lan.Models;

using MySqlConnector;

using System.Data;
using System.Diagnostics;
namespace c_lan.Data
{
    public class MysqlProvider : IDatabaseProvider
    {
        //此处学习了枚举类的引用
        public DatabaseType SupportedDatabaseType => DatabaseType.MySQL;

        public string? ValidateProfile(ConnectionProfile profile)
        {
            if (string.IsNullOrWhiteSpace(profile.Host))
            {
                return "请填写连接主机";
            }

            if (profile.Port == 0 || profile.Port > 65535)
            {
                return "端口范围必须在 1～65535 之间";
            }

            if (string.IsNullOrWhiteSpace(profile.UserName))
            {
                return "连接用户名不能为空";
            }

            if (string.IsNullOrWhiteSpace(profile.Password))
            {
                return "连接密码不能为空";
            }

            return null;
        }
        //测试连接方法
        public async Task<ConnectionResult> TestConnectionAsync(ConnectionProfile profile, CancellationToken token)
        {
            //先进行配置错误的具体判断
            string? validationError = ValidateProfile(profile);
            if (validationError is not null)
            {
                return new ConnectionResult{IsSuccess = false,ErrorMessage = validationError};
            }

            ConnectionResult result = new ConnectionResult();
            var connectionString = MysqlConnectionStringBuilder(profile);
            using (var conn = new MySqlConnection(connectionString))
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

                catch (MySqlException myex)
                {
                    result.IsSuccess = false;
                    result.ErrorMessage = "MySQL连接失败" + myex.Message;
                }

                return result;
            }
        }
        //接触了c#调用MySQL命令
        public async Task<List<string>> GetDatabasesAsync(ConnectionProfile profile, CancellationToken token)
        {
            var connectionString = MysqlConnectionStringBuilder(profile);
            using (var conn = new MySqlConnection(connectionString))
            {
                await conn.OpenAsync(token);
                var cmd = new MySqlCommand("SHOW DATABASES", conn);
                var reader = await cmd.ExecuteReaderAsync(token);
                var databases = new List<string>();
                while (reader.Read())
                {
                    databases.Add(reader["Database"].ToString());
                }
                reader.Close();
                return databases;
            }
        }
        //执行查询操作
        public async Task<QueryResult> ExecuteQueryAsync(ConnectionProfile profile, QueryRequest request, CancellationToken token)
        {
            var connectionString = MysqlConnectionStringBuilder(profile);
            QueryResult queryresult = new QueryResult();
            Stopwatch stopwatch = Stopwatch.StartNew();
            try {
                using var conn = new MySqlConnection(connectionString);
                await conn.OpenAsync(token);
                using var cmd = new MySqlCommand(request.SqlText, conn);
                cmd.CommandTimeout = request.TimeoutSeconds;
                using var reader = await cmd.ExecuteReaderAsync(token);
                var datatable = new DataTable();
                datatable.Load(reader);
                queryresult.IsSuccess = true;
                queryresult.RowCount = datatable.Rows.Count;
                queryresult.Rows = datatable;
            }
            catch (OperationCanceledException ex) {
                queryresult.IsSuccess = false;
                queryresult.ErrorMessage = "查询已被取消";
            }
            catch (MySqlException ex) {
                queryresult.IsSuccess = false;
                queryresult.ErrorMessage = "MySQL错误: " + ex.Message;
            }
            catch (Exception ex) {
                queryresult.IsSuccess = false;
                queryresult.ErrorMessage = "未知错误: " + ex.Message;
            }
            finally
            {
                stopwatch.Stop();
                queryresult.ExecutionTime = (int)stopwatch.ElapsedMilliseconds;
            }
            return queryresult;
        }
        //连接器需要，构建连接字符串
        private string MysqlConnectionStringBuilder(ConnectionProfile profile){
            MySqlConnectionStringBuilder builder = new MySqlConnectionStringBuilder();
            builder.Server = profile.Host;
            builder.Port = profile.Port;
            builder.UserID = profile.UserName;
            builder.Password = profile.Password;
            builder.ConnectionTimeout = profile.ConnectionTimeout;
            return builder.ToString();
        }
    }
}
