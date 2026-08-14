namespace c_lan.Models
{
    /// 表示应用支持的数据库种类。
    /// 编译器也能帮助检查赋值和比较是否使用了正确的类型。
    public enum DatabaseType
    {

        Unknown = 0,

        /// MySQL 数据库。
        MySQL = 1,

        /// Microsoft SQL Server 数据库。
        SqlServer = 2,

        /// Oracle 数据库。
        Oracle = 3,

        //SQLlite也支持
        SQLite = 4
    }
}
