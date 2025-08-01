using EsfParser.Parser.Logic.Statements;

namespace EsfParser.Parser.Logic.Parsers
{
    public class CommentStatementParser : IStatementParser
    {
        public bool CanParse(string line) =>
            line.TrimStart().StartsWith("/*") || line.TrimStart().StartsWith("//");

        public IStatement Parse(List<PreprocessedLine> lines, ref int index)
        {
            var line = lines[index];
            return new CommentStatement
            {
                OriginalCode = line.OriginalBlock,
                Text = line.OriginalBlock.TrimStart('/', '*').Trim(),
                LineNumber = line.StartLineNumber,
            };
        }
    }
}
