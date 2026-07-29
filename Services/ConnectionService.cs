using c_lan.Models;
using System;
using System.Collections.Generic;
using System.Text;
using MySqlConnector;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Windows.Forms.VisualStyles;
using System.Data.SqlTypes;

namespace c_lan.Services
{
    public class ConnectionService : IConnectionService
    {
        public async Task<bool> TestConnectionAsync(
            ConnectionProfile profile,CancellationToken cancellationToken
            )
        {
            if(profile == null || String.IsNullOrWhiteSpace(profile.Host) || 0 >= profile.Port || profile.Port >65535 || profile.ConnectionTimeout <= 0) { return false; }

            String connectionstring = BuildConnectionString(profile);

            //使用usinglauguaue管理连接

            using var conn = new MySqlConnection(connectionstring);

                try
                {
                    
                    await conn.OpenAsync(cancellationToken);

                    Debug.WriteLine("连接成功");

                    return true;
                }
                catch (OperationCanceledException cancel)
                {

                    Debug.WriteLine("操作被取消");

                    return false;
                }

                catch (MySqlException sqlerror)
                {
                    Debug.WriteLine("sql错误，请检查连接、账号、网络等问题");

                    Debug.WriteLine(sqlerror.Message);

                    return false;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("发生未知错误");

                    Debug.WriteLine(ex.Message);

                    return false;
                }

        }

        public static String BuildConnectionString(ConnectionProfile profile)
        {
            MySqlConnectionStringBuilder builder = new MySqlConnectionStringBuilder();

            builder.Server = profile.Host;
            builder.Port = profile.Port;
            builder.UserID = profile.UserName;
            builder.Password = profile.Password;
            builder.ConnectionTimeout = profile.ConnectionTimeout;

            if (profile.DefaultDatabase != null)
            {  builder.Database = profile.DefaultDatabase; 
               }
            if(profile.CharacterSet != null)
            {
                builder.CharacterSet = profile.CharacterSet;
            }

            return builder.ToString();
        }
        public async Task<ConnectionProfile> ReadallConfigurationAsync(
            CancellationToken cancellationToken)
        {
            
        }

        public async Task<bool> SaveConnectionConfigurationAsync(
            ConnectionProfile profile, bool keeporNot, CancellationToken token)
        {

        }

        public async Task<bool> DeleteConnectionConfigurationAsync(
            int configid, CancellationToken token)
        {

        }
    }
}
