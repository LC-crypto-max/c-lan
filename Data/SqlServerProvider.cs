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
            dc.definition
        FROM sys.columns c JOIN sys.objects o ON o.object_id=c.object_id
        JOIN sys.schemas s ON s.schema_id=o.schema_id
        JOIN sys.types t ON t.user_type_id=c.user_type_id
        LEFT JOIN sys.default_constraints dc ON dc.object_id=c.default_object_id
        WHERE s.name=@scope AND o.name=@objectName ORDER BY c.column_id
        """;
}
