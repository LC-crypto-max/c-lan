using System.Net;
using System.Text;

namespace c_lan.Utilities
{
    public static class SqlTextNormalizer
    {
        public static string Normalize(string sql)
        {
            string text = sql;
            if (sql.Contains("&#", StringComparison.Ordinal) || sql.Contains("&nbsp;", StringComparison.OrdinalIgnoreCase))
            {
                text = WebUtility.HtmlDecode(text);
            }

            text = text.Replace('\u00A0', ' ').TrimStart('\uFEFF');
            StringBuilder result = new(text.Length);
            bool inSingleQuote = false;
            bool inDoubleQuote = false;

            for (int i = 0; i < text.Length; i++)
            {
                char current = text[i];
                if (current == '\'' && !inDoubleQuote)
                {
                    inSingleQuote = !inSingleQuote;
                    result.Append(current);
                    continue;
                }

                if (current == '"' && !inSingleQuote)
                {
                    inDoubleQuote = !inDoubleQuote;
                    result.Append(current);
                    continue;
                }

                if (!inSingleQuote && !inDoubleQuote && current == '\\')
                {
                    int slashStart = i;
                    while (i + 1 < text.Length && text[i + 1] == '\\') i++;
                    if (i + 1 < text.Length && text[i + 1] == '_')
                    {
                        result.Append('_');
                        i++;
                        continue;
                    }

                    result.Append(text.AsSpan(slashStart, i - slashStart + 1));
                    continue;
                }

                result.Append(current);
            }

            return result.ToString();
        }
    }
}
