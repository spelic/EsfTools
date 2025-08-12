using System.Text;
using EsfParser.CodeGen;

namespace EsfParser.Parser.Logic.Statements
{
    public class DxfrStatement : IStatement
    {
        public StatementType Type => StatementType.Dxfr;

        public string OriginalCode { get; set; } = string.Empty;
        public string ProgramName { get; set; } = string.Empty;   // NEWAPP1  /  EZEAPP
        public string ProgramStartScreen { get; set; } = string.Empty; // rarely used in practice
        public string RecordName { get; set; } = string.Empty;   // WORKSTOR record to pass
        public string SourceApp { get; set; } = string.Empty;   // rarely used in batch

        public int LineNumber { get; set; }
        public int NestingLevel { get; set; }

        public override string ToString() =>
            $"DxfrStatement: {ProgramName}.{ProgramStartScreen}.{RecordName} <- {SourceApp} " +
            $"(Line {LineNumber}, Nest {NestingLevel})";

        // ────────────────────────────────────────────────────────────────
        //  Emit C#
        // ────────────────────────────────────────────────────────────────
        public string ToCSharp()
        {
            string indent = CSharpUtils.Indent(NestingLevel);
            var sb = new StringBuilder();

            // 1️⃣  Target program expression
            string progExpr = ProgramName.Trim().Equals("EZEAPP", System.StringComparison.OrdinalIgnoreCase)
                                ? "EzFunctions.EZEAPP"
                                : $"\"{ProgramName}\"";

            // 2️⃣  Optional record to pass
            string recExpr = string.IsNullOrWhiteSpace(RecordName)
                                ? "\"\""
                                : CSharpUtils.ConvertOperand(RecordName.Trim())+".ToJson()";

            // 3️⃣  Start-screen rarely used; we still honour if supplied
            string screenExpr = string.IsNullOrWhiteSpace(ProgramStartScreen)
                                ? ""
                                : $"\"{ProgramStartScreen}\"";

            sb.AppendLine($"{indent}EzFunctions.ExternalCallProgram({progExpr},{recExpr});");
            sb.AppendLine($"{indent}return;   // DXFR terminates current routine");

            return sb.ToString().TrimEnd() + $" // Org: {OriginalCode}";
        }
    }
}
