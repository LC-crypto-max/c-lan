using System;
using System.Collections.Generic;
using System.Text;
using c_lan.Models;

namespace c_lan.Data
{
    public class DatabaseProviderFactory
    {
        // 工厂后续应使用同一个 DatabaseType 枚举选择 Provider，
        // 不要再接收 "MySQL" 之类需要手工比较的字符串。
        public IDatabaseProvider CreateProvider(DatabaseType databaseType)
        {
            return databaseType switch
            {
                DatabaseType.MySQL => new MysqlProvider(),
                //DatabaseType.SqlServer => new SqlServerDatabaseProvider(),
                //DatabaseType.Oracle => new OracleDatabaseProvider(),
                _ => throw new NotSupportedException($"Unsupported database type: {databaseType}"),
            };
        }
    }
}
