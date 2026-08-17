using System;
using System.Collections.Generic;
using System.Text;

namespace c_lan.Models
{
    public sealed class DatabaseObjectInfo
    {
        public String ObjectName {  get; set; } = String.Empty;
        public String ObjectType {  get; set; } = String.Empty;
        public String DatabaseName {  get; set; } = String.Empty;
        public String SchemaName {  get; set; } = String.Empty;
        public bool IsSystemObject {  get; set; }
        public String Description {  get; set; } = String.Empty;

    }


}
