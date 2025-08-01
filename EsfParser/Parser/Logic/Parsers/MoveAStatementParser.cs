using EsfParser.Parser.Logic.Parsers;
using EsfParser.Parser.Logic.Statements;

public class MoveAStatementParser : IStatementParser
{
    public bool CanParse(string line) => line.TrimStart().StartsWith("MOVEA ", StringComparison.OrdinalIgnoreCase);

    public IStatement Parse(List<PreprocessedLine> lines, ref int index)
    {
        var line = lines[index];
        index++; // advance

        string body = line.CleanLine.Trim()[6..].Trim(); // remove "MOVEA "
        string source = "", target = "", forClause = null;

        var tokens = body.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

        if (tokens.Length >= 3 && string.Equals(tokens[^2], "FOR", StringComparison.OrdinalIgnoreCase))
        {
            // Has FOR clause
            source = tokens[0];
            target = string.Join(" ", tokens.Skip(1).Take(tokens.Length - 3));
            forClause = tokens[^1];
        }
        else if (tokens.Length >= 2)
        {
            source = tokens[0];
            target = string.Join(" ", tokens.Skip(1));
        }
        else
        {
            return new UnknownStatement
            {
                OriginalCode = line.OriginalBlock,
                LineNumber = line.StartLineNumber,
            };
        }

        return new MoveAStatement
        {
            OriginalCode = line.OriginalBlock,
            LineNumber = line.StartLineNumber,
            Source = source,
            Target = target,
            ForClause = forClause
        };
    }
}
