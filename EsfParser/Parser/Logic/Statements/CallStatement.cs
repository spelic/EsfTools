using System.Collections.Generic;
using System.Linq;
using System.Text;
using EsfParser.CodeGen;

namespace EsfParser.Parser.Logic.Statements
{
    public enum CallParameterType
    {
        Unknown,
        DataItem,
        Literal,
        Record,
        Map,
        SpecialVariable
    }

    public class CallParameter
    {
        public string Raw { get; set; } = string.Empty;
        public CallParameterType Type { get; set; } = CallParameterType.Unknown;
        public override string ToString() => $"{Raw} [{Type}]";
    }

    public class CallStatement : IStatement
    {
        public StatementType Type => StatementType.Call;

        public string OriginalCode { get; set; } = string.Empty;
        public string ProgramName { get; set; } = string.Empty;
        public bool IsServiceRoutine { get; set; }

        public List<CallParameter> Parameters { get; set; } = new();
        public List<string> Options { get; set; } = new();

        public int LineNumber { get; set; }
        public int NestingLevel { get; set; }

        public override string ToString()
        {
            var opts = Options.Count > 0 ? $" [{string.Join(", ", Options)}]" : string.Empty;
            return $"CallStatement: {ProgramName}({string.Join(", ", Parameters)}){opts}";
        }

        // ────────────────────────────────────────────────────────────────
        //  C# emitter
        // ────────────────────────────────────────────────────────────────
        public string ToCSharp()
        {
            string indent = CSharpUtils.Indent(NestingLevel);
            var sb = new StringBuilder();

            // 1️⃣  Map service routines to EzFunctions helpers
            if (IsServiceRoutine)
            {
                switch (ProgramName.ToUpperInvariant())
                {
                    case "COMMIT":
                        sb.AppendLine($"{indent}EzFunctions.EZECOMIT.Execute();");
                        break;

                    case "RESET":
                        sb.AppendLine($"{indent}EzFunctions.EZEROLLB.Execute();");
                        break;

                    case "AUDIT":
                        sb.AppendLine($"{indent}EzFunctions.Audit();   // TODO implement");
                        break;

                    case "CREATX":
                        sb.AppendLine($"{indent}EzFunctions.Creatx();  // TODO implement");
                        break;

                    case "CSPTDLI":
                        sb.AppendLine($"{indent}EzFunctions.CsptDli(); // TODO implement");
                        break;

                    default:
                        sb.AppendLine($"{indent}// TODO Unsupported service routine CALL {ProgramName}");
                        break;
                }
                HandleOptions(sb, indent);
                return sb.ToString().TrimEnd();
            }

            // 2️⃣  Regular program call
            string csProgramId = CSharpUtils.CleanName(ProgramName);
            string paramList = string.Join(", ",
                                   Parameters.Select(p =>
                                         CSharpUtils.ConvertOperand(p.Raw)));

            sb.AppendLine($"{indent}{csProgramId}({paramList});");
            HandleOptions(sb, indent);
            return sb.ToString().TrimEnd()+ $" // Org: {OriginalCode}";
        }

        // ────────────────────────────────────────────────────────────────
        //  Emit comments for options (NOMAPS, REPLY, NONCSP)
        // ────────────────────────────────────────────────────────────────
        private void HandleOptions(StringBuilder sb, string indent)
        {
            if (Options == null || Options.Count == 0) return;

            foreach (var opt in Options)
            {
                string up = opt.Trim().ToUpperInvariant();
                sb.AppendLine($"{indent}// OPTION {up} not yet implemented");
            }
        }
    }
}
