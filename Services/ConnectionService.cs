using c_lan.Configuration;
using c_lan.Data;
using c_lan.Models;

namespace c_lan.Services
{
    public class ConnectionService : IConnectionService
    {
        private readonly ConnectionProfileStore _store;
        private readonly DatabaseProviderFactory _factory;

        public ConnectionService(ConnectionProfileStore store, DatabaseProviderFactory factory)
        {
            _store = store;
            _factory = factory;
        }

        public async Task<ConnectionResult> TestConnectionAsync(ConnectionProfile profile, CancellationToken cancellationToken)
        {
            string? commonValidationError = ValidateCommonFields(profile);
            if (commonValidationError is not null)
            {
                return new ConnectionResult { IsSuccess = false, ErrorMessage = commonValidationError };
            }

            IDatabaseProvider provider;
            try
            {
                provider = _factory.CreateProvider(profile.DatabaseType);
            }
            catch (NotSupportedException ex)
            {
                return new ConnectionResult { IsSuccess = false, ErrorMessage = ex.Message };
            }

            return await provider.TestConnectionAsync(profile, cancellationToken);
        }

        public Task<List<ConnectionProfile>> ReadAllConfigurationsAsync(CancellationToken cancellationToken)
        {
            // Service 只编排用例，JSON 的位置和反序列化细节仍由 Store 管理。
            return _store.LoadAsync(cancellationToken);
        }

        public async Task<SaveConfigurationResult> SaveConnectionConfigurationAsync(ConnectionProfile profile, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            string? commonValidationError = ValidateCommonFields(profile);
            if (commonValidationError is not null)
            {
                return Failed(commonValidationError);
            }

            IDatabaseProvider provider;
            try
            {
                provider = _factory.CreateProvider(profile.DatabaseType);
            }
            catch (NotSupportedException ex)
            {
                return Failed(ex.Message);
            }

            string? providerValidationError = provider.ValidateProfile(profile);
            if (providerValidationError is not null)
            {
                return Failed(providerValidationError);
            }

            try
            {
                List<ConnectionProfile> profiles = await _store.LoadAsync(token);
                ConnectionProfile profileForStorage = CreateStorageCopy(profile);

                // 当前阶段以连接名称作为唯一键，并忽略名称大小写差异。
                int existingIndex = profiles.FindIndex(item =>
                    string.Equals(item.ConnectionName.Trim(),profileForStorage.ConnectionName,StringComparison.OrdinalIgnoreCase));

                bool isUpdate = existingIndex >= 0;
                if (isUpdate)
                {
                    profiles[existingIndex] = profileForStorage;
                }
                else
                {
                    profiles.Add(profileForStorage);
                }

                await _store.SaveAsync(profiles, token);

                return new SaveConfigurationResult
                {
                    IsSuccess = true,
                    Message = isUpdate ? "连接配置已更新" : "连接配置已保存"
                };
            }
            catch (OperationCanceledException)
            {
                // 取消由 UI 单独识别，不能包装成普通的“保存失败”。
                throw;
            }
            catch (IOException ex)
            {
                return Failed(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return Failed(ex.Message);
            }
        }

        public async Task<SaveConfigurationResult> DeleteConnectionConfigurationAsync(string connectionName, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            if (string.IsNullOrWhiteSpace(connectionName))
            {
                return Failed("请先填写要删除的连接名称");
            }

            try
            {
                List<ConnectionProfile> profiles = await _store.LoadAsync(token);
                int existingIndex = profiles.FindIndex(item =>
                    string.Equals(item.ConnectionName.Trim(),connectionName.Trim(),StringComparison.OrdinalIgnoreCase));

                if (existingIndex < 0)
                {
                    return Failed("没有找到同名连接配置");
                }

                profiles.RemoveAt(existingIndex);
                await _store.SaveAsync(profiles, token);

                return new SaveConfigurationResult
                {
                    IsSuccess = true,
                    Message = "连接配置已删除"
                };
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (IOException ex)
            {
                return Failed(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return Failed(ex.Message);
            }
        }

        private static string? ValidateCommonFields(ConnectionProfile profile)
        {
            if (profile is null)
            {
                return "连接信息为空";
            }

            if (string.IsNullOrWhiteSpace(profile.ConnectionName))
            {
                return "连接名称不能为空";
            }

            if (profile.DatabaseType == DatabaseType.Unknown)
            {
                return "请选择数据库类型";
            }

            if (profile.ConnectionTimeout == 0)
            {
                return "连接超时时间必须大于 0";
            }

            return null;
        }

        private static ConnectionProfile CreateStorageCopy(ConnectionProfile source)
        {
            // 创建副本而不改动窗体传入的对象；否则未勾选“保存密码”时，
            // 保存动作会顺带清空当前内存中用于测试连接的密码。
            return new ConnectionProfile
            {
                ConnectionName = source.ConnectionName.Trim(),
                DatabaseType = source.DatabaseType,
                Host = source.Host.Trim(),
                Port = source.Port,
                DefaultDatabase = source.DefaultDatabase?.Trim(),
                SavePassword = source.SavePassword,
                ConnectionTimeout = source.ConnectionTimeout,
                CharacterSet = source.CharacterSet?.Trim(),
                SSLmode = source.SSLmode,
                UserName = source.UserName.Trim(),
                Password = source.SavePassword ? source.Password : String.Empty,
                DatabaseFilePath = source.DatabaseFilePath.Trim()
            };
        }

        //优化报错的统一方法
        private static SaveConfigurationResult Failed(string errorMessage)
        {
            return new SaveConfigurationResult
            {
                IsSuccess = false,
                ErrorMessage = errorMessage
            };
        }
    }
}
