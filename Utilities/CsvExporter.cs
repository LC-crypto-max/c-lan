using System.Data;
using System.Globalization;
using System.Text;

namespace c_lan.Utilities;

public static class CsvExporter
{
    public static async Task WriteAsync(DataTable table, string path, CancellationToken token)
    {
        string temporary = path + "." + Guid.NewGuid().ToString("N") + ".tmp";
        try
        {
            await using (StreamWriter writer = new(temporary, false, new UTF8Encoding(true)))
            {
                await writer.WriteLineAsync(string.Join(",", table.Columns.Cast<DataColumn>().Select(c => Cell(c.ColumnName))).AsMemory(), token);
                foreach (DataRow row in table.Rows)
                {
                    token.ThrowIfCancellationRequested();
                    await writer.WriteLineAsync(string.Join(",", row.ItemArray.Select(Cell)).AsMemory(), token);
                }
            }
            token.ThrowIfCancellationRequested();
            File.Move(temporary, path, true);
        }
        finally { if (File.Exists(temporary)) File.Delete(temporary); }
    }

    private static string Cell(object? value)
    {
        string text = value is byte[] bytes ? Convert.ToHexString(bytes) : Convert.ToString(value, CultureInfo.InvariantCulture) ?? "";
        // Excel 打开 CSV 时，不将文本型内容当作公式执行。
        if (value is string && text.TrimStart() is { Length: > 0 } trimmed && "=+-@".Contains(trimmed[0])) text = "'" + text;
        return "\"" + text.Replace("\"", "\"\"") + "\"";
    }
}
