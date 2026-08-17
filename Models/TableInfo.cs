using System;
using System.Collections.Generic;
using System.Text;

namespace c_lan.Models
{
    public sealed class TableInfo
    {
        public String TableName { get; set; } = String.Empty;
        public String DatabaseName { get; set; } = String.Empty;
        //public String SchemaName { get; set; }
        public String ObjectType { get; set; } = String.Empty;
        public String? Comment { get; set; }
        public String? Engine { get; set; }
        public String? Collation { get; set; }
        public int ColumnCount {  get; set; }
        //public int PrimaryKeyColumnCount { get; set; }
        public long RowCount { get; set; }
        public long DataLength {  get; set; }
        public long IndexLength {  get; set; }
        public String CreateTime { get; set; } = String.Empty;

        //判断是否为视图
        public bool IsView()
        {
            if(ObjectType == "View") { return true; }
            return false;
        }

    }
}
