using EsfParser.Parser.Logic.Statements;

namespace EsfParser.Parser.Logic.Parsers
{
    public class SetStatementParser : IStatementParser
    {
        public bool CanParse(string line) =>
            line.TrimStart().StartsWith("SET ", StringComparison.OrdinalIgnoreCase);

        public IStatement Parse(List<PreprocessedLine> lines, ref int index, int currentLevel = 0)
        {
            var current = lines[index];
            var clean = current.CleanLine.TrimEnd(';');

            // Remove "SET "
            clean = clean.Substring(4).Trim();

            var parts = clean.Split(new[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 0)
            {
                return new SetStatement
                {
                    OriginalCode = current.OriginalBlock,
                    Target = "",
                    Attributes = new(),
                    LineNumber = current.StartLineNumber,
                };
            }

            return new SetStatement
            {
                OriginalCode = current.OriginalBlock,
                Target = parts[0],
                Attributes = parts.Skip(1).Select(p => p.ToUpperInvariant()).ToList(),
                LineNumber = current.StartLineNumber,
            };
        }
    }
}
