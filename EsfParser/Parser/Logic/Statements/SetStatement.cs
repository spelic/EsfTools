using System.Collections.Generic;
using System.Linq;
using System.Text;
using EsfParser.CodeGen;

namespace EsfParser.Parser.Logic.Statements
{
    public class SetStatement : IStatement
    {
        public StatementType Type => StatementType.Set;

        public string OriginalCode { get; set; } = string.Empty;
        public string Target { get; set; } = string.Empty;
        public List<string> Attributes { get; set; } = new();

        public int LineNumber { get; set; }
        public int NestingLevel { get; set; } = 0;

        public override string ToString() =>
            $"SetStatement: {Target} [{string.Join(", ", Attributes)}] " +
            $"(Line {LineNumber}, Nest {NestingLevel})";

        public string ToCSharp()
        {
            var prog = CSharpUtils.Program;
            string tgt = CSharpUtils.ConvertOperand(Target);
            string indent = CSharpUtils.Indent(NestingLevel);
            var sb = new StringBuilder();

            void Attr(string a) => sb.AppendLine($"{indent}{tgt}.SetAttribute(\"{a}\");");

            bool isMap = false, isMapField = false, isRecord = false, isSqlItem = false;

            if (prog != null)
            {
                int dot = Target.IndexOf('.');
                string first = dot > 0 ? Target[..dot] : Target;

                isMap = prog.Maps.Maps.Any(m =>
                        CSharpUtils.CleanName(m.MapName)
                                   .Equals(CSharpUtils.CleanName(first),
                                           System.StringComparison.OrdinalIgnoreCase));

                isRecord = prog.Records.Records.Any(r =>
                        CSharpUtils.CleanName(r.Name)
                                   .Equals(CSharpUtils.CleanName(first),
                                           System.StringComparison.OrdinalIgnoreCase));

                isSqlItem = isRecord &&
                            prog.Records.Records
                                .First(r => CSharpUtils.CleanName(r.Name)
                                                .Equals(CSharpUtils.CleanName(first),
                                                        System.StringComparison.OrdinalIgnoreCase))
                                .Org.Equals("SQLROW",
                                            System.StringComparison.OrdinalIgnoreCase);

                isMapField = isMap && dot > 0;
            }
            int bracket = 0;
            foreach (var raw in Attributes)
            {
                string attr = raw.Trim().ToUpperInvariant();

                switch (attr)
                {
                    // ── Record / Map "EMPTY" ─────────────────────────────────────
                    case "EMPTY" when isMap:
                        sb.AppendLine($"{indent}{tgt}.SetClear();");
                        break;

                    case "EMPTY" when isRecord:
                        sb.AppendLine($"{indent}{tgt}.SetEmpty();");
                        break;

                    // ── Map "CLEAR"  → SetClear() ───────────────────────────────
                    case "CLEAR" when isMap:
                        sb.AppendLine($"{indent}{tgt}.SetClear();");
                        break;

                    // ── Map "PAGE" / "ALARM" (unchanged) ───────────────────────
                    case "PAGE" when isMap:
                        sb.AppendLine($"{indent}MapServices.NextPage();");
                        break;

                    case "ALARM" when isMap:
                        sb.AppendLine($"{indent}MapServices.Alarm();");
                        break;

                    // ── SQL-row item NULL ───────────────────────────────────────
                    case "NULL" when isSqlItem:
                        sb.AppendLine($"{indent}{tgt}_NULL = -1;");
                        break;

                    // ── Map field visual / cursor / attribute ──────────────────────────
                    case "CURSOR" when isMapField:
                        bracket = tgt.IndexOf('[');
                        if (bracket >= 0)
                        {
                            sb.AppendLine($"{indent}{tgt.Insert(bracket, "Tag")}.SetCursor();");  // if you support it
                        }
                        else
                        {
                            sb.AppendLine($"{indent}{tgt}Tag.SetCursor();");          // if you support it
                        }
                        break;

                    case "DEFINED" when isMapField:                        // <<< NEW
                        {
                            bracket = tgt.IndexOf('[');
                            if (bracket >= 0)
                            {
                                sb.AppendLine($"{indent}{tgt.Insert(bracket, "Tag")}.Defined();");  // if you support it
                            }
                            else
                            {
                                sb.AppendLine($"{indent}{tgt}Tag.Defined();");
                            }
                            break;
                        }

                    case "RED" when isMapField:                        // <<< NEW
                        sb.AppendLine($"{indent}{tgt}Tag.SetRed();");
                        break;

                    case "BLINK" when isMapField:                        // <<< NEW
                        sb.AppendLine($"{indent}{tgt}Tag.SetBlink();");
                        break;

                    case "PROTECT" when isMapField:                        // <<< NEW
                        sb.AppendLine($"{indent}{tgt}Tag.SetProtect();");
                        break;


                    case var a when isMapField:
                        sb.AppendLine($"{indent}{tgt} = \"{a}\";");         // assign literal
                        break;

                    // ── Anything else left as TODO comment ──────────────────────
                    default:
                        sb.AppendLine($"{indent}// TODO SET {attr} on {tgt}");
                        break;
                }
            }
            return sb.ToString().TrimEnd() + $" // Org: {OriginalCode}";
        }
    }
}
