using System;
using System.Collections.Generic;
using System.Text;

namespace c_lan.Models
{
    public sealed class DatabaseObjectInfo
    {
        public String ObjectName {  get; set; }
        public String ObjectType {  get; set; }
        public String DatabaseName {  get; set; }
        public String SchemaName {  get; set; }
        public bool IsSystemObject {  get; set; }
        public String Description {  get; set; }

    }


}
