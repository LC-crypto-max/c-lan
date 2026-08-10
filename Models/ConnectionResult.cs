using System;
using System.Collections.Generic;
using System.Text;

namespace c_lan.Models
{
    //用来表示测试连接和正式连接的结果，主要处理报错
    public class ConnectionResult
    {
        public bool IsSuccess {  get; set; }

        public string? ErrorMessage { get; set; }
    }
}
