using System;
using System.Collections.Generic;
using System.Text;

namespace c_lan.Models
{
    public sealed class ColumnInfo
    {
        public String ColumnName { get; set; } = String.Empty;
        public String DataType { get; set; } = String.Empty;
        public String FullColumnType { get; set; } = String.Empty;
        public int? MaxLength { get; set; }
        public int? NumericPrecision { get; set; }
        public int? NumericScale {  get; set; }
        public bool IsPrimaryKey { get; set; }
        public bool IsAutoIncrement { get; set; }
        public bool IsNullable { get; set; }
        public String? DefaultValue { get; set; } 
        public String? Comment {  get; set; }
        public int OrdinalPosition {  get; set; }
        public String? CharacterSet { get; set; }
        public String Collation { get; set; } = String.Empty;
        public bool IsNormalColumn(String ColumnName)
        {
            return !(IsPrimaryKey || IsAutoIncrement);
        }
    }
}
