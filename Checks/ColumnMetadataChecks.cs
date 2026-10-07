using System.Data;
using System.Reflection;
using c_lan.Data;
using c_lan.Models;

internal static class ColumnMetadataChecks
{
    public static void Run()
    {
        CheckSqlServer("varchar", 100, 0, 0, "varchar(100)", 100);
        CheckSqlServer("nvarchar", 200, 0, 0, "nvarchar(100)", 100);
        CheckSqlServer("nchar", 20, 0, 0, "nchar(10)", 10);
        CheckSqlServer("nvarchar", -1, 0, 0, "nvarchar(max)", -1);
        CheckSqlServer("varbinary", -1, 0, 0, "varbinary(max)", -1);
        CheckSqlServer("decimal", 9, 18, 2, "decimal(18,2)", null, 18, 2);
        CheckSqlServer("numeric", 9, 18, 0, "numeric(18,0)", null, 18, 0);
        CheckSqlServer("float", 8, 53, 0, "float(53)", null, 53, 0);
        CheckSqlServer("datetime2", 8, 27, 7, "datetime2(7)");
        CheckSqlServer("time", 3, 8, 0, "time(0)");
        CheckSqlServer("xml", -1, 0, 0, "xml");
        CheckSqlServer("text", 16, 0, 0, "text");
        CheckSqlServer("int", 4, 10, 0, "int", null, 10, 0, identity: true);
        CheckSqlServer("decimal", 9, 18, 2, "decimal", userDefined: true);

        CheckOracle("VARCHAR2", 400, 100, "C", null, null, "VARCHAR2(100 CHAR)", 100);
        CheckOracle("VARCHAR2", 100, 100, "B", null, null, "VARCHAR2(100 BYTE)", 100);
        CheckOracle("NVARCHAR2", 200, 100, "C", null, null, "NVARCHAR2(100)", 100);
        CheckOracle("RAW", 16, 0, null, null, null, "RAW(16)", 16);
        CheckOracle("NUMBER", 22, 0, null, 18, 2, "NUMBER(18,2)");
        CheckOracle("NUMBER", 22, 0, null, 10, -2, "NUMBER(10,-2)");
        CheckOracle("NUMBER", 22, 0, null, null, null, "NUMBER");
        CheckOracle("NUMBER", 22, 0, null, null, 0, "NUMBER(*,0)");
        CheckOracle("FLOAT", 22, 0, null, 126, null, "FLOAT(126)");
        CheckOracle("TIMESTAMP(6) WITH TIME ZONE", 13, 0, null, null, 6, "TIMESTAMP(6) WITH TIME ZONE");
        CheckOracle("CLOB", 4000, 0, null, null, null, "CLOB");
        Console.WriteLine("SQL Server / Oracle 字段映射样例检查通过（不替代实机元数据查询）。");
    }

    private static void CheckSqlServer(string type, int bytes, int precision, int scale, string expected,
        int? length = null, int? expectedPrecision = null, int? expectedScale = null,
        bool identity = false, bool userDefined = false)
    {
        using var table = MetadataTable("max_length", "precision", "scale", "is_identity", "is_user_defined");
        // 使用驱动实际对应的 smallint / tinyint / bit CLR 类型。
        table.Rows.Add("sample", type, 1, 0, 1, DBNull.Value,
            (short)bytes, (byte)precision, (byte)scale, identity, userDefined);
        var column = ReadColumn(new SqlServerProvider(), table);
        Verify(column.FullColumnType == expected && column.MaxLength == length &&
            column.NumericPrecision == expectedPrecision && column.NumericScale == expectedScale &&
            column.IsAutoIncrement == identity, $"SQL Server {expected}");
    }

    private static void CheckOracle(string type, int bytes, int characters, string? semantics,
        int? precision, int? scale, string expected, int? length = null)
    {
        using var table = MetadataTable("DATA_LENGTH", "CHAR_LENGTH", "CHAR_USED", "DATA_PRECISION", "DATA_SCALE");
        // Oracle NUMBER 元数据可由驱动返回 decimal；同时覆盖 DBNull。
        table.Rows.Add("sample", type, 1m, 0m, 1m, DBNull.Value, (decimal)bytes, (decimal)characters,
            (object?)semantics ?? DBNull.Value,
            precision.HasValue ? (object)(decimal)precision.Value : DBNull.Value,
            scale.HasValue ? (object)(decimal)scale.Value : DBNull.Value);
        var column = ReadColumn(new OracleProvider(), table);
        Verify(column.FullColumnType == expected && column.MaxLength == length &&
            column.NumericPrecision == precision && column.NumericScale == scale, $"Oracle {expected}");
    }

    private static DataTable MetadataTable(params string[] extras)
    {
        DataTable table = new();
        foreach (string name in new[] { "name", "type", "ordinal", "nullable", "primaryKey", "default" }.Concat(extras))
            table.Columns.Add(name, typeof(object));
        return table;
    }

    private static ColumnInfo ReadColumn(ServerDatabaseProvider provider, DataTable table)
    {
        using var reader = table.CreateDataReader();
        reader.Read();
        var method = provider.GetType().GetMethod("ReadColumn", BindingFlags.Instance | BindingFlags.NonPublic)!;
        var column = (ColumnInfo)method.Invoke(provider, new object[] { reader })!;
        Verify(column.ColumnName == "sample" && column.OrdinalPosition == 1 &&
            column.IsPrimaryKey && !column.IsNullable && column.DefaultValue is null, "公共字段映射");
        return column;
    }

    private static void Verify(bool condition, string name)
    {
        if (!condition) throw new InvalidOperationException("字段元数据检查失败：" + name);
    }
}
