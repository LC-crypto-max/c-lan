using c_lan.Models;
using System;
using System.Collections.Generic;
using System.Text;
using MySqlConnector;

namespace c_lan.Services
{
    public class ConnectionService : IConnectionService
    {
        public async Task<bool> TestConnectionAsync(
            ConnectionProfile profile,CancellationToken cancellationToken
            )
        {
            if(profile == null || profile.Host == null || 0 >= profile.Port || profile.Port >=65535 || profile.ConnectionTimeout <= 0) { return false; }

            String connectionstring = BuildConnectionString(profile);

            //使用usinglaugh管理连接

            using var conn = new MySqlConnection(connectionstring);

            try
            {
                await conn.OpenAsync();
                Console.WriteLine("连接成功");

            }

        }

        private String BuildConnectionString(ConnectionProfile profile)
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
    }
}
