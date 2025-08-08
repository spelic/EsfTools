using EsfParser.Parser.Logic.Parsers;
using EsfParser.Parser.Logic.Statements;

public class MoveStatementParser : IStatementParser
{
    public bool CanParse(string line) => line.TrimStart().StartsWith("MOVE ", StringComparison.OrdinalIgnoreCase);

    public IStatement Parse(List<PreprocessedLine> lines, ref int index, int currentLevel = 0)
    {
        var line = lines[index];
        string clean = line.CleanLine.TrimEnd(';')[5..].Trim(); // remove "MOVE "

        string[] parts;

        // Handle ' in clean and splint after second '
        if (clean.Contains('\''))
        {
            // search for second ' and split on it
            int firstQuote = clean.IndexOf('\'');
            int secondQuote = clean.IndexOf('\'', firstQuote + 1);
            if (secondQuote > firstQuote)
            {
                parts = new string[2];
                parts[0] = clean.Substring(0, secondQuote + 1).Trim(); // source
                parts[1] = clean.Substring(secondQuote + 1).Trim(); // destination

                // if destination starts with "TO ", remove it
                if (parts[1].StartsWith("TO ", StringComparison.OrdinalIgnoreCase))
                    parts[1] = parts[1][3..].Trim(); // remove "TO " prefix
            }
            else
                throw new FormatException($"Malformed MOVE statement: {line.OriginalBlock}");
        }
        else if (clean.Contains('"'))
        {
            // search for second " and split on it
            int firstQuote = clean.IndexOf('"');
            int secondQuote = clean.IndexOf('"', firstQuote + 1);
            if (secondQuote > firstQuote)
            {
                parts = new string[2];
                parts[0] = clean.Substring(0, secondQuote + 1).Trim(); // source
                parts[1] = clean.Substring(secondQuote + 1).Trim(); // destination

                // if destination starts with "TO ", remove it
                if (parts[1].StartsWith("TO ", StringComparison.OrdinalIgnoreCase))
                    parts[1] = parts[1][3..].Trim(); // remove "TO " prefix
            }
            else
                throw new FormatException($"Malformed MOVE statement: {line.OriginalBlock}");
        } else if (clean.Contains(" TO "))
        {
            // Handle optional TO keyword
            parts = clean.Split(new[] { " TO " }, 2, StringSplitOptions.None);
        }
        else 
        {
            // split on ' ' and limit to 2 parts
            parts = clean.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
        }

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
