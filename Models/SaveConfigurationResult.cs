using System;
using System.Collections.Generic;
using System.Text;

namespace c_lan.Models
{
    //用于存放配置保存结果
    internal class SaveConfigurationResult
    {
        public bool IsSuccess { get; set; }

        public string? ErrorMessage { get; set; }
    }
}
