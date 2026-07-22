using System;
using System.Collections.Generic;
using System.Text;

namespace c_lan.Models
{
    public sealed class ConnectionProfile
    {
        public String ConnectionName { get; set; }
        public String DatabaseType { get; set; }
        public String Host {  get; set; }
        public uint Port { get; set; }
        public String? DefaultDatabase { get; set; }
        public bool SavePassword { get; set; } = false;
        public uint ConnectionTimeout { get; set; } = 10;
        public String? CharacterSet { get; set; }
        public String SSLmode { get; set; }
        public String UserName { get; set; }
        public String Password { get; set; }

        public bool IsComplete()
        {
            return !String.IsNullOrWhiteSpace(UserName) && !String.IsNullOrWhiteSpace(Password);
        }

    }
}
