using System.Data;
using System.Data.Common;

namespace c_lan.Utilities
{
    public static class BoundedDataTableReader
    {
        public static async Task<(DataTable Table, bool IsTruncated)> LoadAsync(
            DbDataReader reader,
            int maxRows,
            CancellationToken token)
        {
            ArgumentNullException.ThrowIfNull(reader);
            if (maxRows < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(maxRows));
            }

            DataTable table = new DataTable();
            for (int i = 0; i < reader.FieldCount; i++)
            {
                table.Columns.Add(reader.GetName(i), typeof(object));
            }

            while (table.Rows.Count <= maxRows && await reader.ReadAsync(token))
            {
                object[] values = new object[reader.FieldCount];
                reader.GetValues(values);
                for (int i = 0; i < values.Length; i++)
                {
                    values[i] = values[i] is null ? DBNull.Value : values[i];
                }

                table.Rows.Add(values);
            }

            bool isTruncated = table.Rows.Count > maxRows;
            if (isTruncated)
            {
                table.Rows.RemoveAt(table.Rows.Count - 1);
            }

            return (table, isTruncated);
        }
    }
}
