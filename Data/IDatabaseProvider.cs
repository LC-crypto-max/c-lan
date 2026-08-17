using c_lan.Models;

namespace c_lan.Data
{
    public interface IDatabaseProvider
    {
        //定义支持的数据库类型
        DatabaseType SupportedDatabaseType { get; }
        //校验当前数据库专用的连接参数；返回 null 表示通过。
        string? ValidateProfile(ConnectionProfile profile);
        Task<ConnectionResult> TestConnectionAsync(ConnectionProfile profile, CancellationToken token);
        Task<List<string>> GetDatabasesAsync(ConnectionProfile profile, CancellationToken token);
        //获取某个数据库下的表和视图，UI不需要知道MySQL元数据SQL。
        Task<List<DatabaseObjectInfo>> GetObjectsAsync(
            ConnectionProfile profile, string databaseName, CancellationToken token);
        //获取指定表或视图的字段信息。
        Task<List<ColumnInfo>> GetColumnsAsync(
            ConnectionProfile profile, string databaseName, string objectName, CancellationToken token);
        //预览由元数据树选中的对象，并限制最大返回行数。
        Task<QueryResult> PreviewAsync(
            ConnectionProfile profile, string databaseName, string objectName,
            int maxRows, CancellationToken token);
        Task<QueryResult> ExecuteQueryAsync(ConnectionProfile profile, QueryRequest request, CancellationToken token);
    }
}
