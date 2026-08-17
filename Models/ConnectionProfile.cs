namespace c_lan.Models
{
    public sealed class ConnectionProfile
    {
        // 给字符串属性提供安全默认值，避免刚创建模型时出现空引用。
        public String ConnectionName { get; set; } = String.Empty;
        // 使用 DatabaseType 枚举保存数据库种类，不再使用容易出现拼写差异的字符串。
        // 没有显式赋值时会得到 DatabaseType.Unknown，便于校验遗漏的类型选择。
        public DatabaseType DatabaseType { get; set; } = DatabaseType.Unknown;
        public String Host {  get; set; } = String.Empty;
        public uint Port { get; set; }
        public String? DefaultDatabase { get; set; }
        public bool SavePassword { get; set; } = false;
        public uint ConnectionTimeout { get; set; } = 10;
        public String? CharacterSet { get; set; }
        public String SSLmode { get; set; } = String.Empty;
        public String UserName { get; set; } = String.Empty;
        public String Password { get; set; } = String.Empty;

        //增加SQLite相关字段
        public String DatabaseFilePath {  get; set; } = String.Empty;
/*        public bool IsComplete()
        {
            return !String.IsNullOrWhiteSpace(UserName) && !String.IsNullOrWhiteSpace(Password);
        }
        public bool IsValid()
        {
            return !String.IsNullOrWhiteSpace(ConnectionName) && !String.IsNullOrWhiteSpace(Host) && Port > 0 && Port<=65535 && !String.IsNullOrWhiteSpace(UserName) && !String.IsNullOrWhiteSpace(Password) && ConnectionTimeout > 0;
        }*/
    }
}
