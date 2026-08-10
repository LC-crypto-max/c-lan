using System;
using System.Collections.Generic;
using System.Text;
using c_lan.Models;

namespace c_lan.Data
{
    public interface IDatabaseProvider
    {
        //定义支持的数据库类型
        DatabaseType SupportedDatabaseType { get; }
        //定义特定的有效性校验
        bool ValidateProfile(ConnectionProfile profile);
        Task<ConnectionResult> TestConnectionAsync(ConnectionProfile profile, CancellationToken token);
        Task<List<string>> GetDatabasesAsync(ConnectionProfile profile, CancellationToken token);
        Task<QueryResult> ExecuteQueryAsync(ConnectionProfile profile, QueryRequest request, CancellationToken token);
    }
}
