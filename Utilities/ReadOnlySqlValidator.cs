namespace c_lan.Utilities
{
    public sealed class ReadOnlySqlValidator
    {
        public string? Validate(string sql, bool isReadOnly = true)
        {
            // 校验顺序从输入清理到关键字检查，尽早返回可以避免后续逻辑处理无效 SQL。
            //这里只做第一版的基本拦截，不把它当成完整的SQL语法分析器。
            if (string.IsNullOrWhiteSpace(sql)) return "请输入要执行的 SQL";
            string text = RemoveLeadingComments(sql).Trim();
            if (string.IsNullOrWhiteSpace(text)) return "请输入要执行的 SQL";
            if (text.EndsWith(';')) text = text[..^1].TrimEnd();
            //允许用户在语句末尾写一个分号，但不允许用分号拼接多条语句。
            if (text.Contains(';')) return "一次只能执行一条 SQL";
            string upperSql = text.ToUpperInvariant();
            string firstKeyword = text.Split(new[] { ' ', '\t', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)[0].ToUpperInvariant();
            if (firstKeyword == "ALTER" && !isReadOnly)
                return null;
            if (firstKeyword is not ("SELECT" or "WITH"))
                return "只允许执行 SELECT 查询";
            string[] forbiddenWords = { "INSERT", "UPDATE", "DELETE", "DROP", "ALTER", "CREATE", "ATTACH", "DETACH", "REPLACE" };
            foreach (string word in forbiddenWords)
            {
                // 只读模式下命中任一写操作关键字就停止执行，交由上层展示明确错误。
                if (upperSql.Contains(word, StringComparison.Ordinal)) return $"只读模式不允许执行 {word} 操作";
            }
            return null;
        }

        private static string RemoveLeadingComments(string sql)
        {
            // 查询前允许存在说明性注释，但注释不能绕过后面的首关键字校验。
            int start = 0;
            while (start < sql.Length)
            {
                while (start < sql.Length && char.IsWhiteSpace(sql[start])) start++;

                if (start + 1 < sql.Length && sql[start] == '-' && sql[start + 1] == '-')
                {
                    int lineEnd = sql.IndexOfAny(new[] { '\r', '\n' }, start + 2);
                    start = lineEnd < 0 ? sql.Length : lineEnd;
                    continue;
                }

                if (start + 1 < sql.Length && sql[start] == '/' && sql[start + 1] == '*')
                {
                    int commentEnd = sql.IndexOf("*/", start + 2, StringComparison.Ordinal);
                    start = commentEnd < 0 ? sql.Length : commentEnd + 2;
                    continue;
                }

                break;
            }

            return sql[start..];
        }
    }
}
