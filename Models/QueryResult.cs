using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace c_lan.Models
{
    public sealed class QueryResult
    {
        public bool IsSuccess {  get; set; }
        public List<ColumnInfo>? Columns { get; set; }
        public int? RowCount {  get; set; }
        public DataTable? Rows { get; set; }
        public int ExecutionTime {  get; set; }
        public String? ErrorMessage { get; set; }
        public bool IsTruncated { get; set; }

    }
}
