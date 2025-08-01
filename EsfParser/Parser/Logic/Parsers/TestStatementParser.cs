using EsfParser.Parser.Logic.Statements;

namespace EsfParser.Parser.Logic.Parsers
{
    public class TestStatementParser : IStatementParser
    {
        public bool CanParse(string line) => line.StartsWith("TEST ", StringComparison.OrdinalIgnoreCase);

        public IStatement Parse(List<PreprocessedLine> lines, ref int index)
        {
            var line = lines[index];
            return new TestStatement
            {
                OriginalCode = line.OriginalBlock,
                Expression = line.CleanLine[5..].TrimEnd(';')
            };
        }
    }
}