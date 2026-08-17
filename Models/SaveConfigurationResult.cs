using System;
using System.Collections.Generic;
using System.Text;

namespace c_lan.Models
{
    // Service 的公开方法会返回这个类型，因此结果模型本身也必须是 public。
    // 使用结果对象而不是单独返回 string，可以明确区分成功状态和错误信息。
    public sealed class SaveConfigurationResult
    {
        public bool IsSuccess { get; set; }

        public string? Message { get; set; }

        public string? ErrorMessage { get; set; }
    }
}
