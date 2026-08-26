using System;
using System.Collections.Generic;
using System.Text;

namespace c_lan.Models
{
    public sealed class QueryRequest
    {
        public long ConnectionId {  get; set; }
        public String? DatabaseName { get; set; }
        public String? SchemaName {  get; set; }
        public String SqlText {  get; set; } = String.Empty;
        public int TimeoutSeconds { get; set; } = 30;
        //查询结果最多返回多少行，Provider会再多读一行来判断是否截断。
        public int MaxRows { get; set; } = 200;
        public bool IsReadOnly {  get; set; }

    }
}
