using EsfParser.Parser.Logic.Parsers;
using EsfParser.Parser.Logic.Statements;

public class MoveStatementParser : IStatementParser
{
    public bool CanParse(string line) => line.TrimStart().StartsWith("MOVE ", StringComparison.OrdinalIgnoreCase);

    public IStatement Parse(List<PreprocessedLine> lines, ref int index)
    {
        var line = lines[index];
        index++; // advance

        string body = line.CleanLine.Trim()[5..].Trim(); // remove "MOVE "
        string[] parts;

        // Handle optional TO keyword
        if (body.Contains(" TO "))
            parts = body.Split(new[] { " TO " }, 2, StringSplitOptions.None);
        else
            parts = body.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries); // fallback

        if (parts.Length != 2)
        {
            return new UnknownStatement
            {
                OriginalCode = line.OriginalBlock,
                LineNumber = line.StartLineNumber,
            };
        }

        return new MoveStatement
        {
            OriginalCode = line.OriginalBlock,
            LineNumber = line.StartLineNumber,
            Source = parts[0].Trim(),
            Destination = parts[1].Trim()
        };
    }
}
