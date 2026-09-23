using c_lan.Data;
using c_lan.Models;

namespace c_lan.Services
{
    public sealed class SchemaService : ISchemaService
    {
        private readonly DatabaseProviderFactory _factory;

        public SchemaService(DatabaseProviderFactory factory)
        {
            _factory = factory;
        }

        public Task<List<string>> GetDatabasesAsync(ConnectionProfile profile, CancellationToken token)
        {
            // 先统一校验连接配置，再把元数据查询交给对应数据库驱动。
            IDatabaseProvider provider = GetValidatedProvider(profile);
            return provider.GetDatabasesAsync(profile, token);
        }

        public Task<List<DatabaseObjectInfo>> GetObjectsAsync(ConnectionProfile profile, string databaseName, CancellationToken token)
        {
            // 数据库名称来自界面选择，空值直接拒绝，避免发出无意义的元数据请求。
            if (string.IsNullOrWhiteSpace(databaseName))
            {
                throw new ArgumentException("数据库名称不能为空", nameof(databaseName));
            }

            IDatabaseProvider provider = GetValidatedProvider(profile);
            return provider.GetObjectsAsync(profile, databaseName, token);
        }

        public Task<List<ColumnInfo>> GetColumnsAsync(ConnectionProfile profile, string databaseName, string objectName,CancellationToken token)
        {
            // 表和视图名称都必须明确，驱动层才能安全地定位对象。
            if (string.IsNullOrWhiteSpace(databaseName))
            {
                throw new ArgumentException("数据库名称不能为空", nameof(databaseName));
            }

            if (string.IsNullOrWhiteSpace(objectName))
            {
                throw new ArgumentException("对象名称不能为空", nameof(objectName));
            }

            IDatabaseProvider provider = GetValidatedProvider(profile);
            return provider.GetColumnsAsync(profile, databaseName, objectName, token);
        }

        public Task<QueryResult> PreviewAsync(ConnectionProfile profile, string databaseName, string objectName,int maxRows, CancellationToken token)
        {
            // 预览只允许有限行数，避免用户点开对象时一次性加载过大的结果集。
            if (maxRows <= 0 || maxRows > 200)
            {
                throw new ArgumentOutOfRangeException(nameof(maxRows), "预览行数必须在1到200之间");
            }

            IDatabaseProvider provider = GetValidatedProvider(profile);

            return provider.PreviewAsync(profile, databaseName, objectName, maxRows, token);
        }

        private IDatabaseProvider GetValidatedProvider(ConnectionProfile profile)
        {
            // 所有元数据入口共用这段校验，保证不同操作使用同一套连接规则。
            if (profile is null)
            {
                throw new ArgumentNullException(nameof(profile));
            }

            //工厂负责选择策略，Service不需要判断MySQL或SQLite的具体类型。
            IDatabaseProvider provider = _factory.CreateProvider(profile.DatabaseType);

            string? validationError = provider.ValidateProfile(profile);
            if (validationError is not null)
            {
                throw new InvalidOperationException(validationError);
            }

            return provider;
        }
    }
}
