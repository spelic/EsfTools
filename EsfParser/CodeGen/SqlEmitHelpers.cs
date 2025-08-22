// ────────────────────────────────────────────────────────────────
// Functions — split per function: LOGIC (SqlClauses.Count==0) → Logic/, SQL (SqlClauses.Count>0) → Sql/
// One file per function, named <FuncName>.cs
// ────────────────────────────────────────────────────────────────

// Helpers for SQL emission (comment stripping, WHERE parsing, parameters)
static class SqlEmitHelpers
{
    // Robust SQL comment stripper:
    // - '--' to end-of-line (outside quotes)
    // - '/* ... */' blocks (outside quotes)
    // - '/*' to end-of-line when no '*/' appears before newline (VAGen/DB2 style)
    public static string StripSqlComments(string sql)
    {
        if (string.IsNullOrWhiteSpace(sql)) return string.Empty;

        var sb = new System.Text.StringBuilder(sql.Length);
        bool inSingle = false, inDouble = false;

        for (int i = 0; i < sql.Length;)
        {
            char c = sql[i];

            // quote toggles (do not strip inside strings)
            if (c == '\'' && !inDouble) { inSingle = !inSingle; sb.Append(c); i++; continue; }
            if (c == '"' && !inSingle) { inDouble = !inDouble; sb.Append(c); i++; continue; }

            if (!inSingle && !inDouble)
            {
                // -- line comment
                if (c == '-' && i + 1 < sql.Length && sql[i + 1] == '-')
                {
                    i += 2;
                    while (i < sql.Length && sql[i] != '\n') i++;
                    continue;
                }

                // /* ... */ block OR /* to EOL (VAGen allows comment line starting with /*):contentReference[oaicite:1]{index=1}
                if (c == '/' && i + 1 < sql.Length && sql[i + 1] == '*')
                {
                    int j = i + 2;
                    bool closed = false;
                    while (j < sql.Length)
                    {
                        if (sql[j] == '*' && j + 1 < sql.Length && sql[j + 1] == '/')
                        {
                            j += 2; closed = true; break;
                        }
                        if (sql[j] == '\n') break; // treat as single-line comment if no */ before newline
                        j++;
                    }
                    // if not closed before newline, swallow to EOL
                    if (!closed)
                    {
                        // skip to EOL
                        i += 2;
                        while (i < sql.Length && sql[i] != '\n') i++;
                        continue;
                    }
                    // closed /* ... */ block
                    i = j;
                    continue;
                }
            }

            sb.Append(c);
            i++;
        }

        // normalize: trim trailing spaces per line and drop empty comment-only lines
        var lines = sb.ToString()
                      .Split(new[] { "\r\n", "\n" }, StringSplitOptions.None)
                      .Select(l => l.TrimEnd())
                      .Where(l => l.Length > 0)
                      .ToArray();

        return string.Join("\n", lines).Trim();
    }

    private static string RemoveLeadingWhere(string s)
    {
        if (string.IsNullOrWhiteSpace(s)) return string.Empty;
        var t = s.TrimStart();
        if (t.StartsWith("WHERE", StringComparison.OrdinalIgnoreCase))
            t = t.Substring(5).TrimStart();
        return t;
    }

    // WHERE + parameters (returns cleaned WHERE without the 'WHERE' keyword,
    // plus the original for the C# comment block)
    public static (string cleanedWhereNoKeyword, string parametersInitCode, string originalWhere) BuildWhereAndParams(
        string rawWhere,
        string cleanSqlRowRecord,
        Func<string, string> convertOperand)
    {
        string originalWhere = rawWhere ?? string.Empty;

        // strip all comment forms first
        string text = StripSqlComments(originalWhere);

        if (string.IsNullOrWhiteSpace(text))
            return (string.Empty, string.Empty, originalWhere);

        // scan for ?/ @ host vars (outside quotes) and build Dapper params
        var sb = new System.Text.StringBuilder(text.Length + 16);
        var paramMap = new Dictionary<string, string>(StringComparer.Ordinal);
        bool inSingle = false, inDouble = false;

        static bool IsIdentStart(char c) => char.IsLetter(c) || c == '_';
        static bool IsIdentChar(char c) => char.IsLetterOrDigit(c) || c == '_' || c == '.';

        for (int i = 0; i < text.Length;)
        {
            char ch = text[i];

            if (ch == '\'' && !inDouble) { inSingle = !inSingle; sb.Append(ch); i++; continue; }
            if (ch == '"' && !inSingle) { inDouble = !inDouble; sb.Append(ch); i++; continue; }

            if (!inSingle && !inDouble && (ch == '?' || ch == '@') && i + 1 < text.Length && IsIdentStart(text[i + 1]))
            {
                int j = i + 1;
                while (j < text.Length && IsIdentChar(text[j])) j++;
                string token = text.Substring(i + 1, j - (i + 1));   // e.g. IDZAPST or IS00W01.IDZAPST
                string normalized = token.Replace('.', '_');         // param name for Dapper
                string valueExpr = token.Contains(".")
                    ? convertOperand(token)
                    : $"{cleanSqlRowRecord}.{token}";

                if (!paramMap.ContainsKey(normalized))
                    paramMap[normalized] = valueExpr;

                sb.Append('@').Append(normalized);
                i = j;
                continue;
            }

            sb.Append(ch);
            i++;
        }

        // drop leading WHERE to avoid "WHERE WHERE"
        string cleanedWhere = RemoveLeadingWhere(sb.ToString().Trim());

        string parametersInitCode = string.Empty;
        if (paramMap.Count > 0)
        {
            var parts = paramMap.Select(kv => $"{kv.Key} = {kv.Value}");
            parametersInitCode = $"var parameters = new {{ {string.Join(", ", parts)} }};";
        }

        return (cleanedWhere, parametersInitCode, originalWhere);
    }

    public static string ComposeSql(string selectText, string tableName, string whereNoKeyword, string orderByText)
    {
        var sql = "SELECT " + (string.IsNullOrWhiteSpace(selectText) ? "*" : selectText) + " FROM " + tableName;
        if (!string.IsNullOrWhiteSpace(whereNoKeyword)) sql += " WHERE " + whereNoKeyword;
        if (!string.IsNullOrWhiteSpace(orderByText)) sql += " ORDER BY " + orderByText;
        return sql;
    }
}