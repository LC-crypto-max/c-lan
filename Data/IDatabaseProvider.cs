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
        Task<QueryResult> ExecuteQueryAsync(ConnectionProfile profile, QueryRequest request, CancellationToken token);
    }
}
