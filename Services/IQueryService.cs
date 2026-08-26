using c_lan.Models;

namespace c_lan.Services
{
    public interface IQueryService
    {
        Task<QueryResult> ExecuteAsync(ConnectionProfile profile, QueryRequest request, CancellationToken token);
    }
}
