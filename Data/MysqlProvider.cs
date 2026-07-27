using System;
using System.Collections.Generic;
using System.Text;
using c_lan.Models;
using System.Threading;
using MySqlConnector;
using System.Threading.Tasks;
using c_lan.Services;
using System.Linq.Expressions;

namespace c_lan.Data
{
    public class MysqlProvider : IDatabaseProvider
    {
        public DatabaseType SupportedDatabaseType => DatabaseType.MySQL;

        public bool ValidateProfile(ConnectionProfile profile){
            return profile.IsValid();
        }

        public async Task<bool> TestConnectionAsync(ConnectionProfile profile, CancellationToken token)
        {
            if(string.IsNullOrWhiteSpace(profile.ToString())){
                return false;
            }
            if(!ValidateProfile(profile)){
                return false;
            }
            if(profile.DatabaseType != SupportedDatabaseType){
                return false;
            }
            var connectionString = MysqlConnectionStringBuilder(profile);
            using (var conn = new MySqlConnection(connectionString))
            {
                try{
                    await conn.OpenAsync(token);
                }
                catch(OperationCanceledException ex){
                    MessageBox.Show(ex.Message);
                    return false;
                }
                catch(MySqlException ex){
                    MessageBox.Show(ex.Message);
                    return false;
                }
                catch(Exception ex){
                    MessageBox.Show(ex.Message);
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
            var conn = new MySqlConnection(connectionString);
            using (conn)
            {
                try{
                await conn.OpenAsync(token);
                var cmd = new MySqlCommand(request.SqlText, conn);
                cmd.CommandTimeout = request.TimeoutSeconds;
                var reader = await cmd.ExecuteReaderAsync(token);
                var result = new QueryResult();
                var datatable = new DataTable();
                datatable.Load(reader);
                var queryresult = new QueryResult();
                queryresult.IsSuccess = true;
                queryresult.RowCount = datatable.Rows.Count;
                queryresult.Rows = datatable.Rows;
                }
                catch(OperationCanceledException ex){
                    MessageBox.Show(ex.Message);
                }
                catch(MySqlException ex){
                    MessageBox.Show(ex.Message);
                }
                catch(Exception ex){
                    MessageBox.Show(ex.Message);
                }
                return queryresult;
            }
        }
        private string MysqlConnectionStringBuilder(ConnectionProfile profile){
            MySqlConnectionStringBuilder builder = new MySqlConnectionStringBuilder();
            builder.Server = profile.Host;
            builder.Port = profile.Port;
            builder.UserID = profile.UserName;
            builder.Password = profile.Password;
            builder.ConnectionIdleTimeout = profile.ConnectionTimeout;
            return builder.ToString();
        }
    }
}
