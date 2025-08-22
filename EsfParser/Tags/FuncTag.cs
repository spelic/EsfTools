using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;
using EsfParser.CodeGen;
using EsfParser.Esf;
using EsfParser.Parser.Logic;
using EsfParser.Parser.Logic.Statements;

namespace EsfParser.Tags
{
    public class FuncTag : IEsfTagModel
    {
        [JsonIgnore] public string TagName => "FUNC";

        // --- ATTRIBUTES OF :FUNC
        public string Name { get; set; }
        public DateTime? Date { get; set; }
        public TimeSpan? Time { get; set; }
        public string Option { get; set; }
        public string ObjectName { get; set; }
        public string ErrRtn { get; set; }
        public bool ExecBld { get; set; }
        public SqlModelType Model { get; set; }
        public bool Refine { get; set; }
        public bool SingRow { get; set; }
        public string UpdFunc { get; set; }
        public bool WithHold { get; set; }
        public string Desc { get; set; }

        // --- CHILD PARTS
        public List<ParmTag> Parameters { get; } = new();
        public List<StorageTag> Storages { get; } = new();
        public ReturnTag Return { get; set; }
        public List<string> BeforeLogic { get; } = new();
        public List<string> AfterLogic { get; } = new();
        [JsonIgnore] public List<IStatement> BeforeLogicStatements { get; set; } = new();
        [JsonIgnore] public List<IStatement> AfterLogicStatements { get; set; } = new();

        [JsonPropertyName("sqlClauses")]
        public List<SqlClause> SqlClauses { get; } = new();
        public DlicallTag DliCall { get; set; }
        public List<SsaTag> Ssas { get; } = new();
        public List<QualTag> Quals { get; } = new();

        public override string ToString()
        {
            var hdr = $"FUNC {Name} objects→{ObjectName}, {SqlClauses.Count} SQL clauses";
            return hdr + "\n  " + string.Join("\n  ", SqlClauses.Select(c => c.ToString()));
        }

        public static FuncTag Parse(TagNode node)
        {
            // simple helpers:
            string getAttr(string k) => node.Attributes.TryGetValue(k, out var vs) ? vs.First() : null;
            bool getBool(string k, bool def = false) =>
                bool.TryParse(getAttr(k), out var b) ? b : def;
            TEnum getEnum<TEnum>(string k, TEnum def = default) where TEnum : struct =>
                Enum.TryParse(getAttr(k), true, out TEnum e) ? e : def;
            DateTime? parseDate(string s) =>
                DateTime.TryParseExact(s, "MM/dd/yyyy", CultureInfo.InvariantCulture,
                                       DateTimeStyles.None, out var d) ? d : (DateTime?)null;
            TimeSpan? parseTime(string s) =>
                TimeSpan.TryParseExact(s, "hh\\:mm\\:ss", CultureInfo.InvariantCulture,
                                       out var ts) ? ts : (TimeSpan?)null;

            var t = new FuncTag
            {
                Name = getAttr("NAME"),
                Date = parseDate(getAttr("DATE")),
                Time = parseTime(getAttr("TIME")),
                Option = getAttr("OPTION"),
                ObjectName = getAttr("OBJECT"),
                ErrRtn = getAttr("ERRRTN"),
                ExecBld = getBool("EXECBLD"),
                Model = getEnum<SqlModelType>("MODEL", SqlModelType.NONE),
                Refine = getBool("REFINE"),
                SingRow = getBool("SINGROW", true),
                UpdFunc = getAttr("UPDFUNC"),
                WithHold = getBool("WITHHOLD", true),
                Desc = getAttr("DESC")
            };

            // children
            foreach (var child in node.Children)
            {
                switch (child.TagName)
                {
                    case "PARM": t.Parameters.Add(ParmTag.Parse(child)); break;
                    case "STORAGE": t.Storages.Add(StorageTag.Parse(child)); break;
                    case "RETURN": t.Return = ReturnTag.Parse(child); break;

                    case "BEFORE":
                        t.BeforeLogic.AddRange(child.Content
                            .Split('\n', StringSplitOptions.RemoveEmptyEntries)
                            .Select(l => l.Trim()));
                        break;

                    case "AFTER":
                        t.AfterLogic.AddRange(child.Content
                            .Split('\n', StringSplitOptions.RemoveEmptyEntries)
                            .Select(l => l.Trim()));
                        break;

                    case "SQL": t.SqlClauses.Add(SqlClause.Parse(child)); break;
                    case "DLICALL": t.DliCall = DlicallTag.Parse(child); break;
                    case "SSA": t.Ssas.Add(SsaTag.Parse(child)); break;
                    case "QUAL": t.Quals.Add(QualTag.Parse(child)); break;
                }
            }

            // keep your current AFTER handling exactly as-is
            if (t.AfterLogic.Count >= 1)
            {
                var al = t.AfterLogic
                    .Where(l => !string.IsNullOrWhiteSpace(l))
                    .ToList();

                if (al.Count > 0 && al[0].Trim().Equals("END;", StringComparison.OrdinalIgnoreCase))
                {
                    t.BeforeLogic.Add(al[0]);
                    t.AfterLogic.Clear();
                }
                else
                {
                    t.AfterLogic.Clear();
                }
            }

            // your existing BEFORE/AFTER conversion to statements
            if (t.BeforeLogic.Count > 0)
            {
                var preprocessedLines = EsfLogicPreprocessor.Preprocess(t.BeforeLogic);
                var parser = new VisualAgeLogicParser(preprocessedLines);
                t.BeforeLogicStatements = parser.Parse();
            }

            if (t.AfterLogic.Count > 0)
            {
                var preprocessedLines = EsfLogicPreprocessor.Preprocess(t.AfterLogic);
                var parser = new VisualAgeLogicParser(preprocessedLines);
                t.AfterLogicStatements = parser.Parse();
            }

            return t;
        }

        public string ToCSharp()
        {
            int indentSpaces = 0;
            var sb = new StringBuilder();
            var indent = new string(' ', indentSpaces);

            // Precompute a sanitized map class name for CONVERSE
            string? mapCsName = null;
            if (!string.IsNullOrWhiteSpace(ObjectName))
            {
                try { mapCsName = CSharpUtils.CleanName(ObjectName); } catch { }
            }

            // Keep your working BEFORE path untouched, but append CONVERSE call if applicable
            if (BeforeLogic.Count > 0)
            {
                sb.AppendLine(indent + "// -- BEFORE logic --");
                sb.Append(EsfLogicToCs.GenerateCSharpFromLogic(this, BeforeLogic, indentSpaces));
                // If this function is a CONVERSE, call the map conversation after BEFORE logic
                if (!string.IsNullOrWhiteSpace(Option) && Option.Equals("CONVERSE", StringComparison.OrdinalIgnoreCase))
                {
                    if (!string.IsNullOrWhiteSpace(mapCsName))
                    {
                        sb.AppendLine(indent + $"// Render and edit map {ObjectName}");
                        // Call runtime CONVERSE editor with constant and variable fields; capture AID and store in GlobalWorkstor
                        sb.AppendLine(indent + $"var aid = Runtime.ConverseConsole.RenderAndEdit(24, 80, GlobalMaps.{mapCsName}.Current.Cfields, GlobalMaps.{mapCsName}.Current.Vfields, null, null);");
                        sb.AppendLine(indent + $"EzFunctions.EZEAID = aid;");
                    }
                }
                return sb.ToString();
            }

            // Precompute the cursor key we will embed as a literal in generated code
            var keyLiteral = !string.IsNullOrWhiteSpace(ObjectName) ? ObjectName : Name;

            // SQL/DATABASE block (split per OPTION)
            if (SqlClauses.Count > 0)
            {
                sb.AppendLine(indent + "// -- SQL / DATABASE logic --");
                sb.AppendLine(indent + $"// OPTION={Option ?? "SQLEXEC"}, MODEL={Model}, SINGLE_ROW={SingRow}, WITH_HOLD={WithHold}");
                sb.AppendLine(indent + "// Clauses are preserved in source order.");

                // Build SQL and collect INTO + host variables (at generation time)
                var sql = BuildSqlFromClauses(out var intoTargets, out var inputHostVars);
                var sqlLiteral = sql.Replace("\"", "\"\""); // for verbatim string @"" in diagnostics

                // Heuristic at generation-time: is this a SELECT?
                bool isSelect =
                    SqlClauses.Any(c => c.ClauseType.Equals("SELECT", StringComparison.OrdinalIgnoreCase)) ||
                    SqlClauses.Any(c => c.ClauseType.Equals("SQLEXEC", StringComparison.OrdinalIgnoreCase)
                                        && c.Text.TrimStart().StartsWith("SELECT", StringComparison.OrdinalIgnoreCase));

                // Emit composed SQL for diagnostics
                sb.AppendLine(indent + $"// Composed SQL (diagnostic):");
                sb.AppendLine(indent + $"// {sqlLiteral.Replace(Environment.NewLine, "\\n")}");

                // Emit concrete INTO targets array (so generated code has it available)
                if (intoTargets.Count > 0)
                    sb.AppendLine(indent + $"var __into = new string[] {{ {string.Join(", ", intoTargets.Select(t => $"\"{t}\""))} }};");
                else
                    sb.AppendLine(indent + "var __into = System.Array.Empty<string>();");

                // Normalize ESF host markers to named @params (Db2 + Dapper friendly)
                sb.AppendLine(indent + "var __norm = SqlHostVarNormalizer.Normalize(sql: @\"" + sqlLiteral + "\", style: SqlParamStyle.NamedAt);");
                sb.AppendLine(indent + "var __sqlToRun = __norm.sql;");
                sb.AppendLine(indent + "var __ordered = __norm.orderedNames; // host var names in SQL order");

                // Build Dapper parameters
                sb.AppendLine(indent + "var __p = new Dapper.DynamicParameters();");
                sb.AppendLine(indent + "foreach (var __n in __ordered) __p.Add(__n, EsfValueProvider.Get(__n));");
                sb.AppendLine();

                // Generate code **specific** to this function's OPTION (known at generation-time)
                var opt = (Option ?? "SQLEXEC").ToUpperInvariant();

                if (opt == "INQUIRY")
                {
                    // Single-row read + INTO assignment (by ordinal)
                    sb.AppendLine(indent + "using var __conn = DataAccess.GetConnection();");
                    sb.AppendLine(indent + "using var __reader = Dapper.SqlMapper.ExecuteReader(__conn, __sqlToRun, __p);");
                    sb.AppendLine(indent + "if (__reader.Read())");
                    sb.AppendLine(indent + "{");
                    sb.AppendLine(indent + "    int __cnt = Math.Min(__reader.FieldCount, __into.Length);");
                    sb.AppendLine(indent + "    for (int __i = 0; __i < __cnt; __i++)");
                    sb.AppendLine(indent + "    {");
                    sb.AppendLine(indent + "        object? __val = __reader.IsDBNull(__i) ? null : __reader.GetValue(__i);");
                    sb.AppendLine(indent + "        EsfValueProvider.Set(__into[__i], __val);");
                    sb.AppendLine(indent + "    }");
                    sb.AppendLine(indent + "}");
                    sb.AppendLine(indent + "return;");
                    return sb.ToString();
                }

                if (opt == "SETINQ" || opt == "SETUPD")
                {
                    // Open a cursor for later SCAN/CLOSE; remember INTO layout
                    sb.AppendLine(indent + $"CursorStore.Open(key: \"{keyLiteral}\", sql: __sqlToRun, param: __p, intoTargets: new System.Collections.Generic.List<string>(__into));");
                    sb.AppendLine(indent + "return;");
                    return sb.ToString();
                }

                if (opt == "SQLEXEC")
                {
                    if (isSelect)
                    {
                        // Free-form SELECT without INTO: iterate rows (no assignment by default)
                        sb.AppendLine(indent + "using var __conn = DataAccess.GetConnection();");
                        sb.AppendLine(indent + "using var __reader = Dapper.SqlMapper.ExecuteReader(__conn, __sqlToRun, __p);");
                        sb.AppendLine(indent + "while (__reader.Read())");
                        sb.AppendLine(indent + "{");
                        sb.AppendLine(indent + "    // TODO: handle row data if needed (no INTO targets were provided).");
                        sb.AppendLine(indent + "}");
                        sb.AppendLine(indent + "return;");
                    }
                    else
                    {
                        // Non-query SQLEXEC (INSERT/UPDATE/DELETE/DDL…)
                        sb.AppendLine(indent + "using var __conn = DataAccess.GetConnection();");
                        sb.AppendLine(indent + "var __affected = Dapper.SqlMapper.Execute(__conn, __sqlToRun, __p);");
                        sb.AppendLine(indent + "return;");
                    }
                    return sb.ToString();
                }

                // Default branch: treat as non-query (ADD/REPLACE/DELETE/UPDATE if you emitted a statement)
                sb.AppendLine(indent + "using (var __conn = DataAccess.GetConnection())");
                sb.AppendLine(indent + "{");
                sb.AppendLine(indent + "    var __affected = Dapper.SqlMapper.Execute(__conn, __sqlToRun, __p);");
                sb.AppendLine(indent + "}");
                sb.AppendLine(indent + "return;");
                return sb.ToString();
            }

            // No SQL clauses present — still handle SCAN/CLOSE here (VAGen separates them)
            if (!string.IsNullOrWhiteSpace(Option))
            {
                var optUpper = Option.ToUpperInvariant();
                if (optUpper == "SCAN")
                {
                    sb.AppendLine(indent + "// SCAN current cursor and assign into targets captured at SETINQ/SETUPD");
                    sb.AppendLine(indent + $"CursorStore.FetchAssign(key: \"{keyLiteral}\");");
                    return sb.ToString();
                }
                if (optUpper == "CLOSE")
                {
                    sb.AppendLine(indent + "// CLOSE cursor opened by SETINQ/SETUPD");
                    sb.AppendLine(indent + $"CursorStore.Close(key: \"{keyLiteral}\");");
                    return sb.ToString();
                }
                // Handle CONVERSE option without BEFORE or SQL logic
                if (optUpper == "CONVERSE")
                {
                    // If a map name is available, emit conversation call
                    if (!string.IsNullOrWhiteSpace(mapCsName))
                    {
                        sb.AppendLine(indent + $"// Render and edit map {ObjectName}");
                        // Call runtime CONVERSE editor with constant and variable fields; capture AID and store in GlobalWorkstor
                        sb.AppendLine(indent + $"var aid = Runtime.ConverseConsole.RenderAndEdit(24, 80, GlobalMaps.{mapCsName}.Current.Cfields, GlobalMaps.{mapCsName}.Current.Vfields, null, null);");
                        sb.AppendLine(indent + $"EzFunctions.EZEAID = aid;");
                        return sb.ToString();
                    }
                }
            }

            // fallback (no BEFORE and no SQL)
            sb.AppendLine(indent + $"// {Option} {Desc}");
            sb.AppendLine(indent + $"// in {ObjectName}");
            return sb.ToString();
        }

        /// <summary>
        /// Generates a Dapper-based async method for this function’s SQL (unchanged).
        /// </summary>
        // in FuncTag.cs
        public string GenerateDapperMethod(string @namespace = "GeneratedFunctions")
        {
            var sqlSb = new StringBuilder();
            foreach (var c in SqlClauses) sqlSb.AppendLine(c.Text);

            var sqlLiteral = sqlSb.ToString().Replace("\"", "\"\""); // escape quotes

            var safeClass = Name; // keep your original function name as the class name

            var sb = new StringBuilder();
            sb.AppendLine("using System;");
            sb.AppendLine("using System.Threading.Tasks;");
            sb.AppendLine("using Dapper;");          // keep Dapper
                                                     // sb.AppendLine($"using {@namespace}.Data;");  // REMOVE this line

            sb.AppendLine();
            sb.AppendLine($"namespace {@namespace}");
            sb.AppendLine("{");
            sb.AppendLine($"    public static class {safeClass}"); // <— no 'Function' suffix
            sb.AppendLine("    {");
            sb.AppendLine("        /// <summary>");
            sb.AppendLine($"        /// Auto-generated Dapper method for ESF function “{Name}”.");
            sb.AppendLine("        /// </summary>");
            sb.AppendLine("        public static async Task ExecuteAsync(object? param = null)");
            sb.AppendLine("        {");
            sb.AppendLine("            using var conn = DataAccess.GetConnection();");
            sb.AppendLine($"            var sql = @\"{sqlLiteral}\";");
            sb.AppendLine("            var rows = await conn.QueryAsync(sql, param);");
            sb.AppendLine("            foreach (var row in rows) Console.WriteLine(row);");
            sb.AppendLine("            // for non-query: await conn.ExecuteAsync(sql, param);");
            sb.AppendLine("        }");
            sb.AppendLine("    }");
            sb.AppendLine("}");
            return sb.ToString();
        }


        // ─────────────────────────────────────────────────────────────
        // Helpers for SQL composition (new)
        // ─────────────────────────────────────────────────────────────

        private bool LooksLikeSelect()
        {
            var first = SqlClauses?.FirstOrDefault();
            if (first == null) return false;
            if (!string.IsNullOrWhiteSpace(first.ClauseType))
                return first.ClauseType.Equals("SELECT", StringComparison.OrdinalIgnoreCase);

            var t = (first.Text ?? "").TrimStart();
            return t.StartsWith("SELECT", StringComparison.OrdinalIgnoreCase) ||
                   t.StartsWith("WITH", StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Compose a runnable SQL string from ESF clauses while preserving order.
        /// We don't try to infer FROM; we keep your text verbatim.
        /// INTO targets are returned separately in <paramref name="intoTargets"/>.
        /// </summary>
        private string BuildSqlFromClauses(out List<string> intoTargets, out List<string> inputHostVars)
        {
            var parts = new List<string>();
            intoTargets = new List<string>();

            // gather inputs by scanning both '?' and '@' prefixes, respecting quotes
            var inputs = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (var c in SqlClauses)
            {
                var t = (c?.Text ?? string.Empty).Trim();
                if (t.Length == 0) continue;

                var kind = (c?.ClauseType ?? string.Empty).Trim().ToUpperInvariant();

                if (kind == "INTO")
                {
                    // Parse into list but do NOT put it into SQL text (generic SQL engines don't accept host-var INTO)
                    foreach (var name in SplitCsv(t))
                    {
                        var n = TrimHostVar(name);
                        if (!string.IsNullOrWhiteSpace(n)) intoTargets.Add(n);
                    }
                    continue;
                }

                if (kind == "SELECT")
                {
                    // If the text already starts with SELECT/WITH, keep it; else prefix SELECT
                    if (t.StartsWith("SELECT", StringComparison.OrdinalIgnoreCase) ||
                        t.StartsWith("WITH", StringComparison.OrdinalIgnoreCase))
                        parts.Add(t);
                    else
                        parts.Add("SELECT " + t);
                }
                else
                {
                    // WHERE/ORDER BY/SET/VALUES/SQLEXEC/etc. — keep verbatim (spec sometimes includes keywords in Text)
                    parts.Add(t);
                }

                // discover host vars for this clause (ignore INTO outputs)
                foreach (var hv in ExtractHostVars(t))
                    inputs.Add(hv);
            }

            inputHostVars = inputs.ToList();
            return string.Join(Environment.NewLine, parts).Trim();
        }

        // Split CSV with simple quote/paren awareness (no iterators)
        private static IEnumerable<string> SplitCsv(string s)
        {
            var result = new List<string>();
            if (string.IsNullOrWhiteSpace(s)) return result;

            var cur = new StringBuilder();
            char? q = null;
            int paren = 0;

            void Flush()
            {
                var t = cur.ToString().Trim();
                if (t.Length > 0) result.Add(t);
                cur.Clear();
            }

            for (int i = 0; i < s.Length; i++)
            {
                var c = s[i];

                if (q != null)
                {
                    cur.Append(c);
                    if (c == q) q = null;
                    continue;
                }

                if (c == '\'' || c == '"') { q = c; cur.Append(c); continue; }
                if (c == '(') { paren++; cur.Append(c); continue; }
                if (c == ')') { paren--; cur.Append(c); continue; }

                if (c == ',' && paren == 0)
                {
                    Flush();
                    continue;
                }

                cur.Append(c);
            }

            Flush();
            return result;
        }

        private static string TrimHostVar(string v)
        {
            if (string.IsNullOrWhiteSpace(v)) return string.Empty;
            v = v.Trim();
            if (v.Length > 0 && (v[0] == '?' || v[0] == '@')) v = v.Substring(1);
            return v.Trim();
        }

        // Extract '?NAME' or '@NAME' identifiers (ignore quoted strings)
        private static IEnumerable<string> ExtractHostVars(string clause)
        {
            var res = new List<string>();
            if (string.IsNullOrEmpty(clause)) return res;

            char? q = null;
            var sb = new StringBuilder();

            for (int i = 0; i < clause.Length; i++)
            {
                var c = clause[i];

                if (q != null)
                {
                    if (c == q) q = null;
                    continue;
                }

                if (c == '\'' || c == '"') { q = c; continue; }

                if (c == '?' || c == '@')
                {
                    sb.Clear();
                    int j = i + 1;
                    while (j < clause.Length)
                    {
                        var d = clause[j];
                        if (char.IsLetterOrDigit(d) || d == '_' || d == '$' || d == '#')
                        {
                            sb.Append(d); j++;
                        }
                        else break;
                    }
                    if (sb.Length > 0)
                    {
                        res.Add(sb.ToString());
                        i = j - 1;
                    }
                }
            }

            // distinct (case-insensitive)
            return res.Distinct(StringComparer.OrdinalIgnoreCase);
        }
    }

    // enums for MODEL, PARAMTYPE, DATATYPE
    public enum SqlModelType { NONE, DELETE, UPDATE }
    public enum ParamType { RECORD, ITEM, MAPITEM, SQLITEM }
}
