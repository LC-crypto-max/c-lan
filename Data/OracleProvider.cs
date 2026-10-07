using System.Data.Common;
using c_lan.Models;
using Oracle.ManagedDataAccess.Client;

namespace c_lan.Data;

public sealed class OracleProvider : ServerDatabaseProvider
{
    public override DatabaseType SupportedDatabaseType => DatabaseType.Oracle;
    protected override string ParameterPrefix => "";
    public override string? ValidateProfile(ConnectionProfile profile)
    {
        if (string.IsNullOrWhiteSpace(profile.Host)) return "请填写 Oracle 主机";
        if (profile.Port is 0 or > 65535) return "端口范围必须在 1～65535 之间";
        if (string.IsNullOrWhiteSpace(profile.ServiceName)) return "请填写 Oracle Service Name";
        if (string.IsNullOrWhiteSpace(profile.UserName)) return "请填写 Oracle 用户名";
        if (profile.Host.IndexOfAny(['(', ')', '=', '/', ';']) >= 0 || profile.ServiceName.IndexOfAny(['(', ')', '=', '/', ';']) >= 0)
            return "主机和 Service Name 不可包含连接描述符分隔符";
        return null;
    }
    public static string BuildConnectionString(ConnectionProfile profile) => new OracleConnectionStringBuilder
    {
        DataSource = $"{profile.Host}:{profile.Port}/{profile.ServiceName}",
        UserID = profile.UserName, Password = profile.Password,
        ConnectionTimeout = (int)Math.Clamp(profile.ConnectionTimeout, 1u, 3600u)
    }.ConnectionString;
    protected override DbConnection CreateConnection(ConnectionProfile profile, string? database) => new OracleConnection(BuildConnectionString(profile));
    protected override DbCommand CreateCommand(DbConnection connection, string sql, uint timeout)
    {
        var command = (OracleCommand)base.CreateCommand(connection, sql, timeout);
        command.BindByName = true;
        return command;
    }
    public static string QuoteIdentifier(string name) => "\"" + name.Replace("\"", "\"\"") + "\"";
    protected override string PreviewSql(string schema, string name, int rows) =>
        $"SELECT * FROM {QuoteIdentifier(schema)}.{QuoteIdentifier(name)} WHERE ROWNUM <= {rows}";
    protected override string DatabasesSql => "SELECT DISTINCT OWNER FROM ALL_OBJECTS WHERE OBJECT_TYPE IN ('TABLE','VIEW') UNION SELECT USER FROM DUAL ORDER BY 1";
    protected override string ObjectsSql => """
        SELECT OBJECT_NAME, OWNER, CASE OBJECT_TYPE WHEN 'VIEW' THEN 'View' ELSE 'Table' END
        FROM ALL_OBJECTS WHERE OWNER=:scope AND OBJECT_TYPE IN ('TABLE','VIEW') ORDER BY OBJECT_NAME
        """;
    protected override string ColumnsSql => """
        SELECT c.COLUMN_NAME, c.DATA_TYPE, c.COLUMN_ID,
          CASE c.NULLABLE WHEN 'Y' THEN 1 ELSE 0 END,
          CASE WHEN EXISTS (SELECT 1 FROM ALL_CONSTRAINTS k JOIN ALL_CONS_COLUMNS kc
            ON kc.OWNER=k.OWNER AND kc.CONSTRAINT_NAME=k.CONSTRAINT_NAME
            WHERE k.CONSTRAINT_TYPE='P' AND k.OWNER=c.OWNER AND k.TABLE_NAME=c.TABLE_NAME
              AND kc.COLUMN_NAME=c.COLUMN_NAME) THEN 1 ELSE 0 END, c.DATA_DEFAULT,
          c.DATA_LENGTH, c.CHAR_LENGTH, c.CHAR_USED, c.DATA_PRECISION, c.DATA_SCALE
        FROM ALL_TAB_COLUMNS c WHERE c.OWNER=:scope AND c.TABLE_NAME=:objectName ORDER BY c.COLUMN_ID
        """;

    protected override ColumnInfo ReadColumn(DbDataReader reader)
    {
        var column = base.ReadColumn(reader);
        column.NumericPrecision = ReadNullableInt(reader, "DATA_PRECISION");
        column.NumericScale = ReadNullableInt(reader, "DATA_SCALE");
        string type = column.DataType.ToUpperInvariant();
        if (type is "CHAR" or "VARCHAR2" or "NCHAR" or "NVARCHAR2" or "RAW")
        {
            bool national = type is "NCHAR" or "NVARCHAR2";
            bool characters = national || Convert.ToString(reader["CHAR_USED"]) == "C";
            column.MaxLength = ReadNullableInt(reader, characters ? "CHAR_LENGTH" : "DATA_LENGTH");
            string semantics = national || type == "RAW" ? "" : characters ? " CHAR" : " BYTE";
            if (column.MaxLength.HasValue)
                column.FullColumnType = FormattableString.Invariant($"{column.DataType}({column.MaxLength}{semantics})");
        }
        else if (type == "NUMBER")
        {
            // 无精度、无 scale 的 NUMBER 不等同于 NUMBER(38,0)。
            if (column.NumericPrecision.HasValue)
                column.FullColumnType = FormattableString.Invariant($"{column.DataType}({column.NumericPrecision},{column.NumericScale ?? 0})");
            else if (column.NumericScale.HasValue)
                column.FullColumnType = FormattableString.Invariant($"{column.DataType}(*,{column.NumericScale})");
        }
        else if (type == "FLOAT" && column.NumericPrecision.HasValue)
            column.FullColumnType = FormattableString.Invariant($"{column.DataType}({column.NumericPrecision})");
        // TIMESTAMP / INTERVAL 的 DATA_TYPE 自带参数，直接保留。
        return column;
    }
}
