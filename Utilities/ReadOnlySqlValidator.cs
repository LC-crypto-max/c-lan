using System.Text;
using System.Text.RegularExpressions;

namespace c_lan.Utilities;

public sealed class ReadOnlySqlValidator
{
    public string? Validate(string sql) => Validate(sql, out _);

    public string? Validate(string sql, out string preparedSql)
    {
        preparedSql = sql;
        int terminator = -1;
        if (string.IsNullOrWhiteSpace(sql)) return "请输入要执行的 SQL";
        // 扫描语句结构，字符串、引用列名及注释不参与关键字判断。
        // 客户端检查不能替代数据库只读账号。
        StringBuilder code = new();
        for (int i = 0; i < sql.Length; i++)
        {
            char c = sql[i];
            if (c == '#') return "暂不支持 # 注释或临时表，请使用 -- 注释";
            if (c == '-' && i + 2 < sql.Length && sql[i + 1] == '-' && char.IsWhiteSpace(sql[i + 2]))
            {
                while (i < sql.Length && sql[i] != '\n' && sql[i] != '\r') i++;
                code.Append(' ');
            }
            else if (c == '/' && i + 1 < sql.Length && sql[i + 1] == '*')
            {
                if (i + 2 < sql.Length && (sql[i + 2] == '!' || sql[i + 2] == '+'))
                    return "暂不支持可执行注释或查询提示";
                int end = sql.IndexOf("*/", i + 2, StringComparison.Ordinal);
                if (end < 0 || sql.IndexOf("/*", i + 2, end - i - 2, StringComparison.Ordinal) >= 0)
                    return "注释未结束或包含嵌套注释";
                i = end + 1;
                code.Append(' ');
            }
            else if (c is '\'' or '"' or '`' or '[')
            {
                char close = c == '[' ? ']' : c;
                bool closed = false;
                while (++i < sql.Length)
                {
                    if (sql[i] == '\\') return "暂不支持反斜杠转义，请使用 SQL 标准引号转义";
                    if (sql[i] != close) continue;
                    if (i + 1 < sql.Length && sql[i + 1] == close) { i++; continue; }
                    closed = true;
                    break;
                }
                if (!closed) return "字符串或标识符引号未结束";
                code.Append(" literal ");
            }
            else { if (c == ';') terminator = i; code.Append(c); }
        }
        string text = code.ToString().Trim();
        if (text.EndsWith(';')) { text = text[..^1].TrimEnd(); preparedSql = sql.Remove(terminator, 1); }
        if (text.Contains(';')) return "一次只能执行一条 SQL";
        string[] words = Regex.Matches(text, @"[\p{L}_][\p{L}\p{N}_$]*")
            .Select(m => m.Value.ToUpperInvariant()).ToArray();
        if (words.Length == 0 || words[0] is not ("SELECT" or "WITH")) return "只允许执行 SELECT 查询";
        string[] forbidden = { "INSERT", "UPDATE", "DELETE", "DROP", "ALTER", "CREATE", "ATTACH", "DETACH",
            "REPLACE", "INTO", "MERGE", "EXEC", "EXECUTE", "CALL", "TRUNCATE", "GRANT", "REVOKE",
            "PRAGMA", "VACUUM", "REINDEX", "LOAD_FILE", "OUTFILE", "DUMPFILE", "LOCK", "UNLOCK", "NEXTVAL", "NEXT" };
        string? rejected = words.FirstOrDefault(w => forbidden.Contains(w));
        return rejected is null ? null : $"只读模式不允许执行 {rejected} 操作";
    }
}
