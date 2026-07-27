using System;
using System.Collections.Generic;
using System.Text;
using c_lan.Models;
using System.Threading;
using MySqlConnector;
using System.Threading.Tasks;
using c_lan.Services;
using System.Linq.Expressions;
using System.Data;
using System.Diagnostics;

namespace c_lan.Data
{
    public class MysqlProvider : IDatabaseProvider
    {
        public DatabaseType SupportedDatabaseType => DatabaseType.MySQL;

        public bool ValidateProfile(ConnectionProfile profile) {
            return profile.IsValid();
        }

        public async Task<bool> TestConnectionAsync(ConnectionProfile profile, CancellationToken token)
        {
            if (string.IsNullOrWhiteSpace(profile.ToString())) {
                return false;
            }
            if (!ValidateProfile(profile)) {
                return false;
            }
            if (profile.DatabaseType != SupportedDatabaseType) {
                return false;
            }
            var connectionString = MysqlConnectionStringBuilder(profile);
            using (var conn = new MySqlConnection(connectionString))
            {
                try {
                    await conn.OpenAsync(token);
                }
                catch (OperationCanceledException ex) {
                    return false;
                }
                catch (MySqlException ex) {
                    return false;
                }
                catch (Exception ex) {
                    return false;
                }
                return true;
            }

        }
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
