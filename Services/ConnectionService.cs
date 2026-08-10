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
using c_lan.Configuration;
using c_lan.Data;

namespace c_lan.Services
{
    public class ConnectionService : IConnectionService
    {
        private readonly ConnectionProfileStore _store;
        private readonly DatabaseProviderFactory _factory;
        //初始化Service，引入store
/*        未引入依赖注入
        public ConnectionService()
        {
            _store = new ConnectionProfileStore();
            _factory = new DatabaseProviderFactory();
        }*/
        public ConnectionService(ConnectionProfileStore store,DatabaseProviderFactory factory)
        {
            _store = store;
            _factory = factory;
        }
        public async Task<ConnectionResult> TestConnectionAsync(ConnectionProfile profile,CancellationToken cancellationToken)
        {
            if(profile == null) { return new ConnectionResult() { IsSuccess = false,ErrorMessage = "连接信息为空"}; }

            DatabaseType databaseType = profile.DatabaseType;

            if(databaseType == DatabaseType.MySQL)
            {
                //使用工厂管理
                IDatabaseProvider provider = _factory.CreateProvider(databaseType);
                //异步等待连接
                return await provider.TestConnectionAsync(profile, cancellationToken);
            }

        }
        //读取配置文件方法
        public async Task<List<ConnectionProfile>> ReadallConfigurationAsync(CancellationToken cancellationToken)
        {
            //调用ConnectionProfileStore类
            List<ConnectionProfile> profiles = await _store.LoadAsync(cancellationToken);
            
            return profiles;
        }

/*        public async Task<bool> SaveConnectionConfigurationAsync(ConnectionProfile profile, bool keeporNot, CancellationToken token)
        {

        }

        public async Task<bool> DeleteConnectionConfigurationAsync(int configid, CancellationToken token)
        {

        }*/
    }
}
