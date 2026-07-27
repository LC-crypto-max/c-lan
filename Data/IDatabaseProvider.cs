using System;
using System.Collections.Generic;
using System.Text;
using c_lan.Models;

namespace c_lan.Data
{
    public interface IDatabaseProvider
    {
        DatabaseType SupportedDatabaseType { get; }

        bool ValidateProfile(ConnectionProfile profile);
        Task<bool> TestConnectionAsync(ConnectionProfile profile, CancellationToken token);
        Task<List<string>> GetDatabasesAsync(ConnectionProfile profile, CancellationToken token);
        Task<QueryResult> ExecuteQueryAsync(ConnectionProfile profile, QueryRequest request, CancellationToken token);
    }
}
