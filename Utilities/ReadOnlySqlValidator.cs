namespace c_lan.Utilities
{
    public sealed class ReadOnlySqlValidator
    {
        public string? Validate(string sql)
        {
            //这里只做第一版的基本拦截，不把它当成完整的SQL语法分析器。
            if (string.IsNullOrWhiteSpace(sql)) return "请输入要执行的 SQL";
            string text = sql.Trim();
            if (text.EndsWith(';')) text = text[..^1].TrimEnd();
            //允许用户在语句末尾写一个分号，但不允许用分号拼接多条语句。
            if (text.Contains(';')) return "一次只能执行一条 SQL";
            string upperSql = text.ToUpperInvariant();
            if (!(upperSql.StartsWith("SELECT ") || upperSql == "SELECT" || upperSql.StartsWith("WITH ")))
                return "只允许执行 SELECT 查询";
            string[] forbiddenWords = { "INSERT", "UPDATE", "DELETE", "DROP", "ALTER", "CREATE", "ATTACH", "DETACH", "REPLACE" };
            foreach (string word in forbiddenWords)
            {
                if (upperSql.Contains(word, StringComparison.Ordinal)) return $"只读模式不允许执行 {word} 操作";
            }
            return null;
        }
    }
}
