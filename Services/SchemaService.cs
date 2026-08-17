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

        public Task<List<string>> GetDatabasesAsync(
            ConnectionProfile profile, CancellationToken token)
        {
            IDatabaseProvider provider = GetValidatedProvider(profile);
            return provider.GetDatabasesAsync(profile, token);
        }

        public Task<List<DatabaseObjectInfo>> GetObjectsAsync(
            ConnectionProfile profile, string databaseName, CancellationToken token)
        {
            if (string.IsNullOrWhiteSpace(databaseName))
            {
                throw new ArgumentException("数据库名称不能为空", nameof(databaseName));
            }

            IDatabaseProvider provider = GetValidatedProvider(profile);
            return provider.GetObjectsAsync(profile, databaseName, token);
        }

        public Task<List<ColumnInfo>> GetColumnsAsync(
            ConnectionProfile profile, string databaseName, string objectName,
            CancellationToken token)
        {
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

        public Task<QueryResult> PreviewAsync(
            ConnectionProfile profile, string databaseName, string objectName,
            int maxRows, CancellationToken token)
        {
            if (maxRows <= 0 || maxRows > 200)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(maxRows), "预览行数必须在1到200之间");
            }

            IDatabaseProvider provider = GetValidatedProvider(profile);
            return provider.PreviewAsync(
                profile, databaseName, objectName, maxRows, token);
        }

        private IDatabaseProvider GetValidatedProvider(ConnectionProfile profile)
        {
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
