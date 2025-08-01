using EsfParser.Parser.Logic.Statements;

namespace EsfParser.Parser.Logic.Parsers
{
    public interface IStatementParser
    {
        bool CanParse(string cleanLine);
        IStatement Parse(List<PreprocessedLine> preLine, ref int index);
    }
}