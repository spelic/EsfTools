using System.Collections.Generic;
using System.Text;

namespace EsfParser.Parser.Logic.Statements
{
    public class WhileStatement : IStatement
    {
        public StatementType Type => StatementType.While;
        public string OriginalCode { get; set; } = string.Empty;
        public string Condition { get; set; } = string.Empty;
        public List<IStatement> BodyStatements { get; set; } = new();

        public int LineNumber { get; set; }
        public int NestingLevel { get; set; } = 0;

        public string ToCSharp()
        {
            var sb = new StringBuilder();
            var indent = Indent(NestingLevel);
            var bodyIndent = Indent(NestingLevel + 1);

            var cond = string.IsNullOrWhiteSpace(Condition) ? "true" : Condition.Trim();

            // while header
            sb.Append(indent)
              .Append("while (")
              .Append(cond)
              .AppendLine(")")
              .Append(indent).AppendLine("{");

            // body
            if (BodyStatements.Count == 0)
            {
                sb.Append(bodyIndent).AppendLine("// no-op");
            }
            else
            {
                foreach (var stmt in BodyStatements)
                {
                    if (stmt == null) continue;

                    // Generate child code, then indent it one extra level so it nests nicely
                    var childCode = stmt.ToCSharp() ?? string.Empty;
                    AppendIndentedBlock(sb, childCode, bodyIndent);
                }
            }

            // closing brace
            sb.Append(indent).Append('}');

            return sb.ToString();
        }

        public override string ToString()
        {
            return $"WhileStatement: {Condition} (Line: {LineNumber}, Nesting: {NestingLevel}) with body lines: [{BodyStatements.Count}]";
        }

        // ---- helpers ----

        private static string Indent(int level) => new string(' ', level * 4);

        private static void AppendIndentedBlock(StringBuilder sb, string code, string indent)
        {
            if (string.IsNullOrWhiteSpace(code))
                return;

            // Normalize newlines, then indent each line
            var lines = code.Replace("\r\n", "\n").Replace("\r", "\n").Split('\n');
            foreach (var line in lines)
            {
                if (line.Length == 0)
                {
                    sb.AppendLine(); // preserve blank lines
                }
                else
                {
                    sb.Append(indent).AppendLine(line);
                }
            }
        }
    }
}
