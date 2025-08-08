using EsfParser.CodeGen;
using System.Text;

namespace EsfParser.Parser.Logic.Statements
{
    public class MoveAStatement : IStatement
    {
        public StatementType Type => StatementType.MoveA;
        public string OriginalCode { get; set; } = string.Empty;
        public string Source { get; set; } = string.Empty;
        public string Target { get; set; } = string.Empty;
        public string Occurrence { get; set; }
        public int LineNumber { get; set; }
        public int NestingLevel { get; set; } = 0;

        /// <summary>
        /// Emits C# code for ESF <c>MOVEA src TO dst, FOR n;</c><br/>
        ///   – <c>src</c> and <c>dst</c> are treated as 1-based arrays in ESF;<br/>
        ///   – generated C# copies <c>n</c> elements using zero-based indexing.
        /// </summary>
        public string ToCSharp()
        {
            string srcExpr = CSharpUtils.ConvertOperand(Source);
            string dstExpr = CSharpUtils.ConvertOperand(Target);
            string occExpr = string.IsNullOrWhiteSpace(Occurrence)
                                ? "1"
                                : CSharpUtils.ConvertOperand(Occurrence);

            string indent = CSharpUtils.Indent(NestingLevel);
            var sb = new StringBuilder();

            sb.AppendLine($"{indent}{{");
            sb.AppendLine($"{indent}    int __count = {occExpr};");
            sb.AppendLine($"{indent}    for (int __i = 0; __i < __count; __i++)");
            sb.AppendLine($"{indent}        {dstExpr}[__i] = {srcExpr}[__i];");
            sb.AppendLine($"{indent}}}");

            sb.AppendLine("// Org: " + OriginalCode);
            return sb.ToString();
        }

        public override string ToString()
            => $"MoveAStatement: {Source} -> {Target} (For: {Occurrence}) (Line: {LineNumber}, Nesting: {NestingLevel})";
    }
}