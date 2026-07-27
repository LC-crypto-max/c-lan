namespace c_lan.Models
{
    /// <summary>
    /// 表示应用支持的数据库种类。
    /// 使用枚举而不是字符串，可以避免 "mysql"、"MySQL" 等拼写差异，
    /// 编译器也能帮助检查赋值和比较是否使用了正确的类型。
    /// </summary>
    public enum DatabaseType
    {
        /// <summary>
        /// 尚未选择数据库类型。
        /// 枚举的默认值是 0，保留 Unknown 可以避免未赋值时被误认为 MySQL。
        /// </summary>
        Unknown = 0,

        /// <summary>MySQL 数据库。</summary>
        MySQL = 1,

        /// <summary>Microsoft SQL Server 数据库。</summary>
        SqlServer = 2,

        /// <summary>Oracle 数据库。</summary>
        Oracle = 3
    }
}
