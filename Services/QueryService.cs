using c_lan.Data;
using c_lan.Models;
using c_lan.Utilities;

namespace c_lan.Services
{
    public sealed class QueryService : IQueryService
    {
        private readonly DatabaseProviderFactory _factory;
        private readonly ReadOnlySqlValidator _validator;

        public QueryService(DatabaseProviderFactory factory, ReadOnlySqlValidator validator)
        {
            _factory = factory;
            _validator = validator;
        }

        public async Task<QueryResult> ExecuteAsync(ConnectionProfile profile, QueryRequest request, CancellationToken token)
        {
            //先在Service层检查输入，Provider只处理已经整理过的查询请求。
            if (profile is null) return Failed("连接信息为空");
            if (request is null) return Failed("查询请求为空");
            request.SqlText = SqlTextNormalizer.Normalize(request.SqlText);
            string? sqlError = _validator.Validate(request.SqlText);
            if (sqlError is not null) return Failed(sqlError);
            if (request.TimeoutSeconds <= 0) return Failed("查询超时时间必须大于 0 秒");
            try
            {
                //工厂负责选择具体数据库，查询流程本身不需要判断数据库类型。
                IDatabaseProvider provider = _factory.CreateProvider(profile.DatabaseType);
                string? profileError = provider.ValidateProfile(profile);
                if (profileError is not null) return Failed(profileError);
                return await provider.ExecuteQueryAsync(profile, request, token);
            }
            catch (NotSupportedException ex) { return Failed(ex.Message); }
            catch (OperationCanceledException) { return Failed("查询已取消"); }
            catch (Exception ex) { return Failed("查询失败：" + ex.Message); }
        }
        //整合报错方法Failed
        private static QueryResult Failed(string message) => new QueryResult { IsSuccess = false, ErrorMessage = message };
    }
}
