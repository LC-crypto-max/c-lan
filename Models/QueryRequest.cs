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
        public bool IsReadOnly {  get; set; }

    }
}
