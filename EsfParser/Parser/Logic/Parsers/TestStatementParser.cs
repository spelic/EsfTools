using EsfParser.Parser.Logic.Statements;

namespace EsfParser.Parser.Logic.Parsers
{
    public class TestStatementParser : IStatementParser
    {
        public bool CanParse(string line) => line.StartsWith("TEST ", StringComparison.OrdinalIgnoreCase);

        public IStatement Parse(List<PreprocessedLine> lines, ref int index, int currentLevel = 0)
        {
            var line = lines[index];
            var clean = line.CleanLine[5..].TrimEnd(';');

            return new TestStatement
            {
                OriginalCode = line.OriginalBlock,
                Expression = clean
            };
        }
    }
}