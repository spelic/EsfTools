using EsfParser.Parser.Logic.Parsers;
using EsfParser.Parser.Logic.Statements;

using System;

public class AssignStatementParser : IStatementParser
{
    public bool CanParse(string line)
        => line.Contains("=") && !line.TrimStart().StartsWith("MOVE", StringComparison.OrdinalIgnoreCase);

    public IStatement Parse(List<PreprocessedLine> lines, ref int index, int currentLevel = 0)
    {
        var line = lines[index];

        var parts = line.CleanLine.TrimEnd(';').Split('=', 2, StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length != 2)
        {
            return new UnknownStatement
            {
                OriginalCode = line.OriginalBlock,
                LineNumber = line.StartLineNumber,
            };
        }

        return new AssignStatement
        {
            OriginalCode = line.OriginalBlock,
            LineNumber = line.StartLineNumber,
            Left = parts[0].Trim(),
            Right = parts[1].Trim()
        };
    }
}
