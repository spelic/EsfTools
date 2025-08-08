using EsfParser.CodeGen;

namespace EsfParser.Parser.Logic.Statements
{
    public class CommentStatement : IStatement
    {
        public StatementType Type => StatementType.Comment;
        public int LineNumber { get; set; }

        public string OriginalCode { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;
        public int NestingLevel { get; set; } = 0;

        // tostring pretty print
        public override string ToString()
            => $"Comment: {Text} (Line: {LineNumber}, Nesting: {NestingLevel})";

        /// <summary>
        /// Emits a C# single-line comment.  Multi-line ESF comments are split
        /// on newline and each line is prefixed with <c>//</c>.
        /// </summary>
        public string ToCSharp()
        {
            var indent = CSharpUtils.Indent(NestingLevel);
            if (string.IsNullOrWhiteSpace(Text))
                return indent + "//";

            var lines = Text.Replace("\r\n", "\n")
                            .Replace("\r", "\n")
                            .Split('\n');

            return string.Join("\n", lines.Select(l => $"{indent}// {l.TrimEnd()}"));
        }
    }
}