// EsfParser/Parser/SqlClausePlanner.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EsfParser.Esf;         // EsfFunctionPart, EsfSqlClause, EsfFuncOption, SqlClauseKind

namespace EsfParser.Parsing
{
    /// <summary>
    /// Normalizes :func … :efunc SQL clauses into a neutral DbPlan
    /// (does NOT build final SQL; it keeps clause texts + host var bindings).
    /// </summary>
    internal static class SqlClausePlanner
    {
        public sealed class DbPlan
        {
            public EsfFuncOption Option { get; init; }

            public char HostVarPrefix { get; init; }   // '?' or '@'

            // Clause texts (raw, trimmed). WHERE and ORDERBY may include their keywords per ESF spec.
            public string SelectText { get; init; } = "";
            public string IntoText { get; init; } = "";
            public string WhereText { get; init; } = "";
            public string OrderByText { get; init; } = "";
            public string SetText { get; init; } = "";
            public string ValuesText { get; init; } = "";
            public string ForUpdateOfText { get; init; } = "";
            public string SqlExecText { get; init; } = "";
            public string InsertColumnNamesText { get; init; } = "";

            // Parsed lists
            public List<string> SelectList { get; init; } = new();                // e.g. ["STPOSZAHT","USTREZA"]
            public List<string> IntoVars { get; init; } = new();                  // e.g. ["STPOSZAHT","USTREZA"] (without prefix)
            public List<string> InputHostVars { get; init; } = new();             // distinct, from WHERE/SET/VALUES/SQLEXEC (without prefix)

            // Cursor-related
            public bool IsCursor { get; init; }
            public bool WithHold { get; init; }
            public string CursorKey { get; init; } = "";                          // usually object name or function name
        }

        public static DbPlan Plan(EsfFunctionPart f)
        {
            char prefix = (f.HostVarPrefix == '?' || f.HostVarPrefix == '@') ? f.HostVarPrefix : '?';

            string selectText = GetClauseText(f, SqlClauseKind.SELECT) ?? "";
            string intoText = GetClauseText(f, SqlClauseKind.INTO) ?? "";
            string whereText = GetClauseText(f, SqlClauseKind.WHERE) ?? "";
            string orderByText = GetClauseText(f, SqlClauseKind.ORDERBY) ?? "";
            string setText = GetClauseText(f, SqlClauseKind.SET) ?? "";
            string valuesText = GetClauseText(f, SqlClauseKind.VALUES) ?? "";
            string fuoText = GetClauseText(f, SqlClauseKind.FORUPDATEOF) ?? "";
            string sqlexecText = GetClauseText(f, SqlClauseKind.SQLEXEC) ?? "";
            string insertCols = GetClauseText(f, SqlClauseKind.INSERTCOLNAME) ?? "";

            var selectList = SplitCsv(selectText).ToList();
            var intoVars = SplitCsv(intoText).Select(v => TrimHost(v, prefix)).ToList();

            // Inputs come from WHERE, SET, VALUES, and SQLEXEC texts (hostvar-scanned).
            var inputs = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var hv in ExtractHostVars(whereText, prefix)) inputs.Add(hv);
            foreach (var hv in ExtractHostVars(setText, prefix)) inputs.Add(hv);
            foreach (var hv in ExtractHostVars(valuesText, prefix)) inputs.Add(hv);
            foreach (var hv in ExtractHostVars(sqlexecText, prefix)) inputs.Add(hv);

            bool isCursor = f.Option == EsfFuncOption.SETINQ || f.Option == EsfFuncOption.SETUPD;
            bool withHold = f.WithHold ?? true;
            string cursorKey = f.ObjectName ?? f.Name;

            return new DbPlan
            {
                Option = f.Option,
                HostVarPrefix = prefix,

                SelectText = selectText,
                IntoText = intoText,
                WhereText = whereText,
                OrderByText = orderByText,
                SetText = setText,
                ValuesText = valuesText,
                ForUpdateOfText = fuoText,
                SqlExecText = sqlexecText,
                InsertColumnNamesText = insertCols,

                SelectList = selectList,
                IntoVars = intoVars,
                InputHostVars = inputs.ToList(),

                IsCursor = isCursor,
                WithHold = withHold,
                CursorKey = cursorKey
            };
        }

        private static string? GetClauseText(EsfFunctionPart f, SqlClauseKind kind)
            => f.SqlClauses.FirstOrDefault(c => c.Kind == kind)?.Text;

        /// <summary>
        /// CSV split with quotes and parentheses awareness.
        /// Accepts null/empty -> empty enumeration.
        /// </summary>
        private static IEnumerable<string> SplitCsv(string? s)
        {
            var result = new List<string>();
            if (string.IsNullOrWhiteSpace(s)) return result;

            int i = 0; var cur = new StringBuilder(); char? q = null; int paren = 0;

            void Flush()
            {
                var t = cur.ToString().Trim();
                if (t.Length > 0) result.Add(t);
                cur.Clear();
            }

            while (i < s!.Length)
            {
                var c = s[i++];

                if (q != null)
                {
                    cur.Append(c);
                    if (c == q) q = null;            // end of quoted segment
                    continue;
                }

                if (c == '\'' || c == '"') { q = c; cur.Append(c); continue; }
                if (c == '(') { paren++; cur.Append(c); continue; }
                if (c == ')') { paren--; cur.Append(c); continue; }

                if (c == ',' && paren == 0) { Flush(); continue; }

                cur.Append(c);
            }

            Flush();
            return result;
        }

        /// <summary>
        /// Remove hostvar prefix ('?' or '@') and surrounding whitespace.
        /// </summary>
        private static string TrimHost(string v, char prefix)
        {
            if (string.IsNullOrWhiteSpace(v)) return string.Empty;
            v = v.Trim();

            // Be tolerant: strip either prefix if present
            if (v.Length > 0 && (v[0] == prefix || v[0] == '?' || v[0] == '@'))
                v = v.Substring(1);

            return v.Trim();
        }

        /// <summary>
        /// Extract distinct host variable names (without prefix) from a clause.
        /// Respects quotes so '?NAME' inside strings is ignored.
        /// Valid name chars: letters, digits, '_', '$', '#'.
        /// </summary>
        private static IEnumerable<string> ExtractHostVars(string? clauseText, char prefix)
        {
            var res = new List<string>();
            if (string.IsNullOrEmpty(clauseText)) return res;

            int i = 0; char? q = null;
            ReadOnlySpan<char> s = clauseText.AsSpan();

            while (i < s.Length)
            {
                char c = s[i++];

                if (q != null)
                {
                    if (c == q) q = null;
                    continue;
                }

                if (c == '\'' || c == '"') { q = c; continue; }

                if (c == prefix)
                {
                    int start = i;
                    while (i < s.Length)
                    {
                        char d = s[i];
                        if (char.IsLetterOrDigit(d) || d == '_' || d == '$' || d == '#')
                            i++;
                        else break;
                    }
                    if (i > start)
                    {
                        string name = clauseText.Substring(start, i - start);
                        if (!string.IsNullOrWhiteSpace(name))
                            res.Add(name);
                    }
                    continue;
                }
            }

            // Distinct by OrdIgnoreCase
            return res.Distinct(StringComparer.OrdinalIgnoreCase)
                      .Select(n => n.Trim());
        }
    }
}
