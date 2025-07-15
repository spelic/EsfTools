using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;
using EsfCore.Esf;

namespace EsfCore.Tags
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
        [JsonPropertyName("sqlClauses")]
        public List<SqlClause> SqlClauses { get; } = new();
        public DlicallTag DliCall { get; set; }
        public List<SsaTag> Ssas { get; } = new();
        public List<QualTag> Quals { get; } = new();

        public override string ToString()
        {
            var hdr = $"FUNC {Name} objects→{ObjectName}, {SqlClauses.Count} SQL clauses";
            return hdr + "\n  " +
                   string.Join("\n  ", SqlClauses.Select(c => c.ToString()));
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

            return t;
        }

        /// <summary>
        /// Generates the C# code lines for this function’s body,
        /// including the original ESF comments and the translated C#.
        /// Requires EsfLogicToCs.Configure(...) to have been called first.
        /// </summary>
        /// <param name="indentSpaces">How many spaces to indent each line.</param>
        public string GenerateCSharpBody(int indentSpaces = 6)
        {
            var sb = new StringBuilder();
            var indent = new string(' ', indentSpaces);

            if (BeforeLogic.Count > 0)
            {
                sb.AppendLine(indent + "// -- BEFORE logic --");
                sb.Append(
                    EsfLogicToCs.GenerateCSharpFromLogic(this, BeforeLogic, indentSpaces)
                );
            }
            else if (SqlClauses.Count > 0)
            {
                sb.Append(indent).AppendLine("// SQL Clauses:");
                switch (SqlClauses[0].ClauseType)
                {
                    case "SELECT":
                        sb.Append(indent)
                          .Append("string sql = $@\" SELECT ")
                          .Append(SqlClauses[0].Text.Replace("\n", " "))
                          .Append(" ")
                          .Append(SqlClauses[2].Text)
                          .Append(" ");
                        if (!SqlClauses[3].Text.StartsWith("/*"))
                            sb.Append(SqlClauses[3].Text).Append(" ");
                        sb.AppendLine("\";");
                        sb.AppendLine(indent + $"// {SqlClauses[1].Text} = SQL RESULT");
                        break;
                    default:
                        sb.AppendLine(indent + $"// UNSUPPORTED {SqlClauses[0].ClauseType}");
                        break;
                }
            }
            else
            {
                // other OPTIONS: INQUIRY, SCAN, DELETE, CONVERSE
                sb.AppendLine(indent + $"// {Option} {Desc}");
                sb.AppendLine(indent + $"// in {ObjectName}");
            }

            return sb.ToString();
        }

        /// <summary>
        /// Generates a Dapper-based async method for this function’s SQL.
        /// </summary>
        public string GenerateDapperMethod(string @namespace = "GeneratedFunctions")
        {
            var sqlSb = new StringBuilder();
            foreach (var c in SqlClauses) sqlSb.AppendLine(c.Text);

            var sqlLiteral = sqlSb
                .ToString()
                .Replace("\"", "\"\""); // escape quotes

            var sb = new StringBuilder();
            sb.AppendLine("using System;");
            sb.AppendLine("using System.Threading.Tasks;");
            sb.AppendLine("using Dapper;");
            sb.AppendLine($"using {@namespace}.Data;");
            sb.AppendLine();
            sb.AppendLine($"namespace {@namespace}");
            sb.AppendLine("{");
            sb.AppendLine($"    public static class {Name}Function");
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
    }

    // -------------------------------------------------

    // enums for MODEL, PARAMTYPE, DATATYPE
    public enum SqlModelType { NONE, DELETE, UPDATE }
    public enum ParamType { RECORD, ITEM, MAPITEM, SQLITEM }
}
