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
            if (profile is null)
            {
                return new ConnectionResult { IsSuccess = false, ErrorMessage = "连接信息为空" };
            }

            if (string.IsNullOrWhiteSpace(profile.ConnectionName))
            {
                return new ConnectionResult { IsSuccess = false, ErrorMessage = "连接名称不能为空" };
            }

            if (profile.DatabaseType == DatabaseType.Unknown)
            {
                return new ConnectionResult { IsSuccess = false, ErrorMessage = "请选择数据库类型" };
            }

            if (profile.ConnectionTimeout == 0)
            {
                return new ConnectionResult { IsSuccess = false, ErrorMessage = "连接超时时间必须大于 0" };
            }

            DatabaseType databaseType = profile.DatabaseType;
            //单独判断是否符合支持的数据库类型
            IDatabaseProvider provider;
            try
            {
                //使用工厂管理
                provider = _factory.CreateProvider(databaseType);
            }
            catch(NotSupportedException ex)
            {
                return new ConnectionResult() { IsSuccess = false,ErrorMessage = ex.Message };
            }
            //异步等待连接
            return await provider.TestConnectionAsync(profile, cancellationToken);
        }
        //读取配置文件方法
        public async Task<List<ConnectionProfile>> ReadallConfigurationAsync(CancellationToken cancellationToken)
        {
            //调用ConnectionProfileStore类
            List<ConnectionProfile> profiles = await _store.LoadAsync(cancellationToken);
            
            return profiles;
        }

        public async Task<SaveConfigurationResult> SaveConnectionConfigurationAsync(ConnectionProfile profile, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            if (profile == null) 
            {
                return new SaveConfigurationResult { IsSuccess = false, ErrorMessage = "保存失败" };
            }

            if (string.IsNullOrWhiteSpace(profile.ConnectionName))
            {
                return new SaveConfigurationResult { IsSuccess = false, ErrorMessage = "连接名称不能为空" };
            }

            List<ConnectionProfile> profiles = await _store.LoadAsync(token);

            
        }

/*        public async Task<bool> DeleteConnectionConfigurationAsync(int configid, CancellationToken token)
        {

        }*/
    }
}
