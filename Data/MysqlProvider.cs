
using c_lan.Models;

using MySqlConnector;

using System.Data;
using System.Diagnostics;
namespace c_lan.Data
{
    public class MysqlProvider : IDatabaseProvider
    {
        //此处学习了枚举类的引用
        public DatabaseType SupportedDatabaseType => DatabaseType.MySQL;

        public string? ValidateProfile(ConnectionProfile profile)
        {
            if (string.IsNullOrWhiteSpace(profile.Host))
            {
                return "请填写连接主机";
            }

            if (profile.Port == 0 || profile.Port > 65535)
            {
                return "端口范围必须在 1～65535 之间";
            }

            if (string.IsNullOrWhiteSpace(profile.UserName))
            {
                return "连接用户名不能为空";
            }

            if (string.IsNullOrWhiteSpace(profile.Password))
            {
                return "连接密码不能为空";
            }

            return null;
        }
        //测试连接方法
        public async Task<ConnectionResult> TestConnectionAsync(ConnectionProfile profile, CancellationToken token)
        {
            //先进行配置错误的具体判断
            string? validationError = ValidateProfile(profile);
            if (validationError is not null)
            {
                return new ConnectionResult{IsSuccess = false,ErrorMessage = validationError};
            }

            ConnectionResult result = new ConnectionResult();
            var connectionString = MysqlConnectionStringBuilder(profile);
            using (var conn = new MySqlConnection(connectionString))
            {
                try
                {
                    await conn.OpenAsync(token);
                    //此处加入成功判断
                    result.IsSuccess = true;
                    result.ErrorMessage = null;
                }

                catch (OperationCanceledException ex)
                {
                    result.IsSuccess = false;
                    result.ErrorMessage = "连接被取消" + ex.Message;
                }

                catch (MySqlException myex)
                {
                    result.IsSuccess = false;
                    result.ErrorMessage = "MySQL连接失败" + myex.Message;
                }

                return result;
            }
        }
        //接触了c#调用MySQL命令
        public async Task<List<string>> GetDatabasesAsync(ConnectionProfile profile, CancellationToken token)
        {
            var connectionString = MysqlConnectionStringBuilder(profile);
            using (var conn = new MySqlConnection(connectionString))
            {
                await conn.OpenAsync(token);
                using var cmd = new MySqlCommand("SHOW DATABASES", conn);
                using var reader = await cmd.ExecuteReaderAsync(token);
                var databases = new List<string>();
                while (reader.Read())
                {
                    //第一列就是数据库名称，GetString可以避免把null加入集合。
                    databases.Add(reader.GetString(0));
                }
                return databases;
            }
        }

        //获取指定数据库下的表和视图
        public async Task<List<DatabaseObjectInfo>> GetObjectsAsync(
            ConnectionProfile profile, string databaseName, CancellationToken token)
        {
            if (string.IsNullOrWhiteSpace(databaseName))
            {
                throw new ArgumentException("数据库名称不能为空", nameof(databaseName));
            }

            const string sql = """
                SELECT TABLE_NAME, TABLE_TYPE, TABLE_COMMENT
                FROM INFORMATION_SCHEMA.TABLES
                WHERE TABLE_SCHEMA = @databaseName
                ORDER BY TABLE_TYPE, TABLE_NAME
                """;

            using var conn = new MySqlConnection(MysqlConnectionStringBuilder(profile));
            await conn.OpenAsync(token);
            using var cmd = new MySqlCommand(sql, conn);
            //数据库名称属于数据值，可以使用参数，不能手工拼接到元数据SQL中。
            cmd.Parameters.AddWithValue("@databaseName", databaseName);
            using var reader = await cmd.ExecuteReaderAsync(token);

            List<DatabaseObjectInfo> objects = new List<DatabaseObjectInfo>();
            while (await reader.ReadAsync(token))
            {
                string mysqlObjectType = reader.GetString("TABLE_TYPE");
                objects.Add(new DatabaseObjectInfo
                {
                    ObjectName = reader.GetString("TABLE_NAME"),
                    //Provider在这里把MySQL原始类型映射成UI使用的统一名称。
                    ObjectType = mysqlObjectType == "VIEW" ? "View" : "Table",
                    DatabaseName = databaseName,
                    SchemaName = databaseName,
                    IsSystemObject = IsSystemDatabase(databaseName),
                    Description = reader.IsDBNull("TABLE_COMMENT")? String.Empty : reader.GetString("TABLE_COMMENT")
                });
            }

            return objects;
        }

        //读取表或视图的字段信息
        public async Task<List<ColumnInfo>> GetColumnsAsync(ConnectionProfile profile, string databaseName, string objectName,CancellationToken token)
        {
            if (string.IsNullOrWhiteSpace(databaseName))
            {
                throw new ArgumentException("数据库名称不能为空", nameof(databaseName));
            }

            if (string.IsNullOrWhiteSpace(objectName))
            {
                throw new ArgumentException("对象名称不能为空", nameof(objectName));
            }

            const string sql = """
                SELECT COLUMN_NAME, DATA_TYPE, COLUMN_TYPE,
                       CHARACTER_MAXIMUM_LENGTH, NUMERIC_PRECISION, NUMERIC_SCALE,
                       COLUMN_KEY, EXTRA, IS_NULLABLE, COLUMN_DEFAULT,
                       COLUMN_COMMENT, ORDINAL_POSITION, CHARACTER_SET_NAME, COLLATION_NAME
                FROM INFORMATION_SCHEMA.COLUMNS
                WHERE TABLE_SCHEMA = @databaseName AND TABLE_NAME = @objectName
                ORDER BY ORDINAL_POSITION
                """;

            using var conn = new MySqlConnection(MysqlConnectionStringBuilder(profile));
            await conn.OpenAsync(token);
            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@databaseName", databaseName);
            cmd.Parameters.AddWithValue("@objectName", objectName);
            using var reader = await cmd.ExecuteReaderAsync(token);

            List<ColumnInfo> columns = new List<ColumnInfo>();
            while (await reader.ReadAsync(token))
            {
                string extra = GetNullableString(reader, "EXTRA") ?? String.Empty;
                columns.Add(new ColumnInfo
                {
                    ColumnName = reader.GetString("COLUMN_NAME"),
                    DataType = reader.GetString("DATA_TYPE"),
                    FullColumnType = reader.GetString("COLUMN_TYPE"),
                    MaxLength = GetNullableInt32(reader, "CHARACTER_MAXIMUM_LENGTH"),
                    NumericPrecision = GetNullableInt32(reader, "NUMERIC_PRECISION"),
                    NumericScale = GetNullableInt32(reader, "NUMERIC_SCALE"),
                    IsPrimaryKey = String.Equals(GetNullableString(reader, "COLUMN_KEY"), "PRI",StringComparison.OrdinalIgnoreCase),
                    IsAutoIncrement = extra.Contains("auto_increment", StringComparison.OrdinalIgnoreCase),
                    IsNullable = String.Equals(reader.GetString("IS_NULLABLE"), "YES",StringComparison.OrdinalIgnoreCase),
                    DefaultValue = GetNullableString(reader, "COLUMN_DEFAULT"),
                    Comment = GetNullableString(reader, "COLUMN_COMMENT"),
                    OrdinalPosition = Convert.ToInt32(reader["ORDINAL_POSITION"]),
                    CharacterSet = GetNullableString(reader, "CHARACTER_SET_NAME"),
                    Collation = GetNullableString(reader, "COLLATION_NAME") ?? String.Empty
                });
            }

            return columns;
        }

        //预览从元数据树中选择的表或视图，最多向UI返回maxRows行
        public async Task<QueryResult> PreviewAsync(
            ConnectionProfile profile, string databaseName, string objectName,
            int maxRows, CancellationToken token)
        {
            QueryResult result = new QueryResult();
            Stopwatch stopwatch = Stopwatch.StartNew();

            try
            {
                if (string.IsNullOrWhiteSpace(databaseName) || string.IsNullOrWhiteSpace(objectName))
                {
                    return new QueryResult{IsSuccess = false,ErrorMessage = "数据库名称和对象名称不能为空"};
                }

                int safeMaxRows = Math.Clamp(maxRows, 1, 200);
                //多读取一行，用来判断结果是否真的被截断。
                int fetchRows = safeMaxRows + 1;
                string quotedDatabase = QuoteIdentifier(databaseName);
                string quotedObject = QuoteIdentifier(objectName);
                string sql = $"SELECT * FROM {quotedDatabase}.{quotedObject} LIMIT @fetchRows";

                using var conn = new MySqlConnection(MysqlConnectionStringBuilder(profile));
                await conn.OpenAsync(token);
                using var cmd = new MySqlCommand(sql, conn);
                cmd.CommandTimeout = (int)profile.ConnectionTimeout;
                cmd.Parameters.AddWithValue("@fetchRows", fetchRows);
                using var reader = await cmd.ExecuteReaderAsync(token);
                DataTable table = new DataTable();
                table.Load(reader);

                bool isTruncated = table.Rows.Count > safeMaxRows;
                if (isTruncated)
                {
                    table.Rows.RemoveAt(table.Rows.Count - 1);
                }

                result.IsSuccess = true;
                result.Rows = table;
                result.RowCount = table.Rows.Count;
                result.IsTruncated = isTruncated;
            }
            catch (OperationCanceledException)
            {
                result.IsSuccess = false;
                result.ErrorMessage = "预览已被取消";
            }
            catch (MySqlException ex)
            {
                result.IsSuccess = false;
                result.ErrorMessage = "MySQL预览失败: " + ex.Message;
            }
            finally
            {
                stopwatch.Stop();
                result.ExecutionTime = (int)stopwatch.ElapsedMilliseconds;
            }

            return result;
        }
        //执行查询操作
        public async Task<QueryResult> ExecuteQueryAsync(ConnectionProfile profile, QueryRequest request, CancellationToken token)
        {
            var connectionString = MysqlConnectionStringBuilder(profile);
            QueryResult queryresult = new QueryResult();
            Stopwatch stopwatch = Stopwatch.StartNew();
            try {
                using var conn = new MySqlConnection(connectionString);
                await conn.OpenAsync(token);
                using var cmd = new MySqlCommand(request.SqlText, conn);
                cmd.CommandTimeout = request.TimeoutSeconds;
                using var reader = await cmd.ExecuteReaderAsync(token);
                var datatable = new DataTable();
                datatable.Load(reader);
                queryresult.IsSuccess = true;
                queryresult.RowCount = datatable.Rows.Count;
                queryresult.Rows = datatable;
            }
            catch (OperationCanceledException) {
                queryresult.IsSuccess = false;
                queryresult.ErrorMessage = "查询已被取消";
            }
            catch (MySqlException ex) {
                queryresult.IsSuccess = false;
                queryresult.ErrorMessage = "MySQL错误: " + ex.Message;
            }
            catch (Exception ex) {
                queryresult.IsSuccess = false;
                queryresult.ErrorMessage = "未知错误: " + ex.Message;
            }
            finally
            {
                stopwatch.Stop();
                queryresult.ExecutionTime = (int)stopwatch.ElapsedMilliseconds;
            }
            return queryresult;
        }
        //连接器需要，构建连接字符串
        private string MysqlConnectionStringBuilder(ConnectionProfile profile){
            MySqlConnectionStringBuilder builder = new MySqlConnectionStringBuilder();
            builder.Server = profile.Host;
            builder.Port = profile.Port;
            builder.UserID = profile.UserName;
            builder.Password = profile.Password;
            builder.ConnectionTimeout = profile.ConnectionTimeout;
            return builder.ToString();
        }

        //参数只能保护数据值，表名和数据库名需要按照MySQL标识符规则进行引用。
        private static string QuoteIdentifier(string identifier)
        {
            return "`" + identifier.Replace("`", "``") + "`";
        }

        private static string? GetNullableString(MySqlDataReader reader, string columnName)
        {
            int ordinal = reader.GetOrdinal(columnName);
            return reader.IsDBNull(ordinal) ? null : Convert.ToString(reader.GetValue(ordinal));
        }

        private static int? GetNullableInt32(MySqlDataReader reader, string columnName)
        {
            int ordinal = reader.GetOrdinal(columnName);
            return reader.IsDBNull(ordinal) ? null : Convert.ToInt32(reader.GetValue(ordinal));
        }

        private static bool IsSystemDatabase(string databaseName)
        {
            return databaseName.Equals("information_schema", StringComparison.OrdinalIgnoreCase) || databaseName.Equals("mysql", StringComparison.OrdinalIgnoreCase) 
                || databaseName.Equals("performance_schema", StringComparison.OrdinalIgnoreCase) || databaseName.Equals("sys", StringComparison.OrdinalIgnoreCase);
        }
    }
}
