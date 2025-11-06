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

    // --- param scanning: @A or @A.B[.C...] (collect unique), preserve original name with dots
    public static IReadOnlyList<(string Raw, string Name, bool Linked, string PropName)> ExtractHostVars(string sql)
    {
        var list = new List<(string Raw, string Name, bool Linked, string PropName)>();
        if (string.IsNullOrEmpty(sql)) return list;

        // Find tokens that START with @ and allow one or more .parts (so @A.B or @A.B.C ...)
        var rx = new System.Text.RegularExpressions.Regex(@"@([A-Za-z_][A-Za-z0-9_]*(?:\.[A-Za-z_][A-Za-z0-9_]*)*)");
        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (System.Text.RegularExpressions.Match m in rx.Matches(sql))
        {
            var raw = m.Value;            // e.g. @IS00W01.IDZAPST
            var name = m.Groups[1].Value;  //     IS00W01.IDZAPST
            if (seen.Add(raw))
            {
                bool linked = name.IndexOf('.') >= 0;
                string prop = System.Text.RegularExpressions.Regex.Replace(name, @"[^A-Za-z0-9_]", "_");
                list.Add((raw, name, linked, prop));
            }
        }

        return list;
    }

    // Normalize dotted names in SQL text so Dapper sees valid @param identifiers.
    // e.g.  @IS00W01.IDZAPST → @IS00W01_IDZAPST, @A.B.C → @A_B_C
    public static string NormalizeDottedParams(string sql)
    {
        if (string.IsNullOrEmpty(sql)) return sql;
        return System.Text.RegularExpressions.Regex.Replace(
            sql,
            @"@([A-Za-z_][A-Za-z0-9_]*(?:\.[A-Za-z_][A-Za-z0-9_]*)+)",
            m => "@" + m.Groups[1].Value.Replace('.', '_'));
    }

    // Merge & uniq param lists from multiple clauses
    public static IReadOnlyList<(string Raw, string Name, bool Linked, string PropName)>
        MergeTokens(params IReadOnlyList<(string Raw, string Name, bool Linked, string PropName)>[] lists)
    {
        var dict = new Dictionary<string, (string Raw, string Name, bool Linked, string PropName)>(StringComparer.OrdinalIgnoreCase);
        foreach (var lst in lists)
            if (lst != null)
                foreach (var t in lst)
                    dict[t.Raw] = t;
        return new List<(string Raw, string Name, bool Linked, string PropName)>(dict.Values);
    }

    // Build 'var parameters = new { ... };' from tokens (dots in PropName already removed)
    public static string BuildParametersDecl(
        IReadOnlyList<(string Raw, string Name, bool Linked, string PropName)> tokens,
        string cleanSqlRowRecord,
        Func<string, string> convertOperand) // e.g., op => CSharpUtils.ConvertOperand(op)
    {
        if (tokens == null || tokens.Count == 0) return string.Empty;

        var sb = new System.Text.StringBuilder();
        sb.Append("var parameters = new { ");
        for (int i = 0; i < tokens.Count; i++)
        {
            var t = tokens[i];
            string valueExpr = t.Linked ? convertOperand(t.Name) : $"{cleanSqlRowRecord}.{t.Name}";
            sb.Append($"{t.PropName} = {valueExpr}");
            if (i < tokens.Count - 1) sb.Append(", ");
        }
        sb.Append(" };");
        return sb.ToString();
    }

    public static string ComposeSql(string selectText, string tableName, string whereNoKeyword, string orderByText)
    {
        var table = (tableName ?? "").Replace("'", "");
        var sel = string.IsNullOrWhiteSpace(selectText) ? "*" : selectText.Trim();

        var sql = "SELECT " + sel + " FROM " + table;
        if (!string.IsNullOrWhiteSpace(whereNoKeyword)) sql += " WHERE " + whereNoKeyword.Trim();
        if (!string.IsNullOrWhiteSpace(orderByText)) sql += " ORDER BY " + orderByText.Trim();
        return sql;
    }

    public static (string clean, string original, IReadOnlyList<(string Raw, string Name, bool Linked, string PropName)> tokens)
        PrepClause(string raw, bool normalizeHostVars = true)
    {
        raw ??= string.Empty;
        var original = raw;

        // 1) strip comments
        var stripped = StripSqlComments(raw);

        // 2) normalize ? → @ (if requested)
        var withAt = normalizeHostVars ? stripped.Replace('?', '@') : stripped;

        // 3) collect tokens BEFORE replacing dots (so Name keeps original A.B)
        var tokens = ExtractHostVars(withAt);

        // 4) now make SQL Dapper-friendly (remove dots from @param names)
        var clean = NormalizeDottedParams(withAt);

        return (clean, original, tokens);
    }

    // Build a full SELECT, with optional trailing clause (e.g., "FOR UPDATE OF ...")
    public static string ComposeSelect(
        string selectText,
        string tableName,
        string whereNoKeyword,
        string orderByText,
        string trailingClause = null)
    {
        var sql = ComposeSql(selectText, tableName, whereNoKeyword, orderByText);
        if (!string.IsNullOrWhiteSpace(trailingClause))
            sql += " " + trailingClause.Trim();
        return sql;
    }

    // INSERT INTO <table> (<cols>) VALUES (<values>)
    public static string ComposeInsert(
        string tableName,
        string cols,
        string values)
    {
        var table = (tableName ?? "").Replace("'", "").Trim();
        var c = (cols ?? "").Trim();
        var v = (values ?? "").Trim();
        return $"INSERT INTO {table} ({c}) VALUES ({v})";
    }

    // UPDATE <table> SET <setClause> [WHERE <where>]
    public static string ComposeUpdateSet(
        string tableName,
        string setClause,
        string whereNoKeyword)
    {
        var table = (tableName ?? "").Replace("'", "").Trim();
        var set = (setClause ?? "").Trim();
        var sql = $"UPDATE {table} SET {set}";
        if (!string.IsNullOrWhiteSpace(whereNoKeyword))
            sql += " WHERE " + whereNoKeyword.Trim();
        return sql;
    }

    // DELETE FROM <table> [WHERE <where>]
    public static string ComposeDelete(
        string tableName,
        string whereNoKeyword)
    {
        var table = (tableName ?? "").Replace("'", "").Trim();
        var sql = $"DELETE FROM {table}";
        if (!string.IsNullOrWhiteSpace(whereNoKeyword))
            sql += " WHERE " + whereNoKeyword.Trim();
        return sql;
    }
}