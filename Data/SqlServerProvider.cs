using System.Data.Common;
using c_lan.Models;
using Microsoft.Data.SqlClient;

namespace c_lan.Data;

public sealed class SqlServerProvider : ServerDatabaseProvider
{
    public override DatabaseType SupportedDatabaseType => DatabaseType.SqlServer;
    public override string? ValidateProfile(ConnectionProfile profile)
    {
        if (string.IsNullOrWhiteSpace(profile.Host)) return "请填写 SQL Server 主机或主机\\实例";
        if (profile.Port > 65535) return "端口不能超过 65535；命名实例可留空";
        if (!profile.IntegratedSecurity && string.IsNullOrWhiteSpace(profile.UserName)) return "请填写 SQL Server 用户名";
        return null;
    }
    public static string BuildConnectionString(ConnectionProfile profile, string? database = null)
    {
        SqlConnectionStringBuilder builder = new()
        {
            DataSource = profile.Port == 0 ? profile.Host : $"{profile.Host},{profile.Port}",
            InitialCatalog = database ?? profile.DefaultDatabase ?? "master",
            IntegratedSecurity = profile.IntegratedSecurity,
            ConnectTimeout = (int)Math.Clamp(profile.ConnectionTimeout, 1u, 3600u),
            TrustServerCertificate = profile.TrustServerCertificate,
            ApplicationName = "c-lan Database Browser"
        };
        if (!profile.IntegratedSecurity) { builder.UserID = profile.UserName; builder.Password = profile.Password; }
        return builder.ConnectionString;
    }
    protected override DbConnection CreateConnection(ConnectionProfile profile, string? database) => new SqlConnection(BuildConnectionString(profile, database));
    public static string QuoteIdentifier(string name) => "[" + name.Replace("]", "]]") + "]";
    protected override string PreviewSql(string schema, string name, int rows) =>
        $"SELECT TOP ({rows}) * FROM {QuoteIdentifier(schema)}.{QuoteIdentifier(name)}";
    protected override string DatabasesSql => "SELECT name FROM sys.databases WHERE state = 0 AND HAS_DBACCESS(name) = 1 ORDER BY name";
    protected override string ObjectsSql => """
        SELECT o.name, s.name, CASE o.type WHEN 'V' THEN 'View' ELSE 'Table' END
        FROM sys.objects o JOIN sys.schemas s ON s.schema_id = o.schema_id
        WHERE o.type IN ('U','V') AND o.is_ms_shipped = 0 ORDER BY s.name, o.name
        """;
    protected override string ColumnsSql => """
        SELECT c.name, t.name, c.column_id, CONVERT(int,c.is_nullable),
            CASE WHEN EXISTS (SELECT 1 FROM sys.indexes i JOIN sys.index_columns ic
              ON ic.object_id=i.object_id AND ic.index_id=i.index_id
              WHERE i.object_id=c.object_id AND i.is_primary_key=1 AND ic.column_id=c.column_id) THEN 1 ELSE 0 END,
            dc.definition, c.max_length, c.precision, c.scale, c.is_identity, t.is_user_defined
        FROM sys.columns c JOIN sys.objects o ON o.object_id=c.object_id
        JOIN sys.schemas s ON s.schema_id=o.schema_id
        JOIN sys.types t ON t.user_type_id=c.user_type_id
        LEFT JOIN sys.default_constraints dc ON dc.object_id=c.default_object_id
        WHERE s.name=@scope AND o.name=@objectName ORDER BY c.column_id
        """;

    protected override ColumnInfo ReadColumn(DbDataReader reader)
    {
        var column = base.ReadColumn(reader);
        int? length = ReadNullableInt(reader, "max_length");
        int? precision = ReadNullableInt(reader, "precision");
        int? scale = ReadNullableInt(reader, "scale");
        column.IsAutoIncrement = Convert.ToBoolean(reader["is_identity"]);
        // 别名类型保留原名，不能把内置类型的参数附加到别名后。
        if (Convert.ToBoolean(reader["is_user_defined"])) return column;
        string type = column.DataType.ToLowerInvariant();
        if (type is "char" or "varchar" or "nchar" or "nvarchar" or "binary" or "varbinary")
        {
            if (length > 0 && type is ("nchar" or "nvarchar")) length /= 2;
            column.MaxLength = length;
            if (length.HasValue)
                column.FullColumnType = length == -1 ? $"{column.DataType}(max)" : FormattableString.Invariant($"{column.DataType}({length})");
        }
        if (type is "decimal" or "numeric" or "float" or "real" or "money" or "smallmoney" or "bigint" or "int" or "smallint" or "tinyint")
        {
            column.NumericPrecision = precision;
            column.NumericScale = scale;
        }
        if (type is ("decimal" or "numeric") && precision.HasValue && scale.HasValue)
            column.FullColumnType = FormattableString.Invariant($"{column.DataType}({precision},{scale})");
        else if (type == "float" && precision.HasValue)
            column.FullColumnType = FormattableString.Invariant($"{column.DataType}({precision})");
        else if (type is ("time" or "datetime2" or "datetimeoffset") && scale.HasValue)
            column.FullColumnType = FormattableString.Invariant($"{column.DataType}({scale})");
        return column;
    }
}
