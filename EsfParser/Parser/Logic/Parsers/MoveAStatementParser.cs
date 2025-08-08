using EsfParser.Parser.Logic.Parsers;
using EsfParser.Parser.Logic.Statements;
using System.IO;

public class MoveAStatementParser : IStatementParser
{
    public bool CanParse(string line) => line.TrimStart().StartsWith("MOVEA ", StringComparison.OrdinalIgnoreCase);

    public IStatement Parse(List<PreprocessedLine> lines, ref int index, int currentLevel = 0)
    {
        var line = lines[index];
        string clean = line.CleanLine.TrimEnd(';')[6..].Trim(); // remove "MOVEA "

        string source = "", target = "", forClause = null;

        if (clean.IndexOf(" TO ", StringComparison.OrdinalIgnoreCase) < 0)
        {
            string[] parts = new string[2];
            // Handle ' in clean and splint after second '
            if (clean.Contains('\''))
            {
                // search for second ' and split on it
                int firstQuote = clean.IndexOf('\'');
                int secondQuote = clean.IndexOf('\'', firstQuote + 1);
                if (secondQuote > firstQuote)
                {
                   
                    parts[0] = clean.Substring(0, secondQuote + 1).Trim(); // source
                    parts[1] = clean.Substring(secondQuote + 1).Trim(); // destination

                    // if destination starts with "FOR ", remove it
                    if (parts[1].IndexOf("FOR ", StringComparison.OrdinalIgnoreCase)>=0)
                    {
                        var spltit = parts[1].Split(" FOR ", StringSplitOptions.RemoveEmptyEntries);
                        if (spltit.Length == 2)
                        {
                            parts[1] = spltit[0].Trim(); // destination
                            forClause = spltit[1].Trim(); // occurrence
                        }
                        else
                            throw new FormatException($"Malformed MOVE statement: {line.OriginalBlock}");
                    } 
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
                    
                    parts[0] = clean.Substring(0, secondQuote + 1).Trim(); // source
                    parts[1] = clean.Substring(secondQuote + 1).Trim(); // destination

                    // if destination starts with "FOR ", remove it
                    if (parts[1].IndexOf("FOR ", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        var spltit = parts[1].Split(" FOR ", StringSplitOptions.RemoveEmptyEntries);
                        if (spltit.Length == 2)
                        {
                            parts[1] = spltit[0].Trim(); // destination
                            forClause = spltit[1].Trim(); // occurrence
                        }
                        else
                            throw new FormatException($"Malformed MOVE statement: {line.OriginalBlock}");
                    }
                    
                }
                else
                    throw new FormatException($"Malformed MOVE statement: {line.OriginalBlock}");
            }
            else
            {
                // split by ' ' 
                parts = clean.Split(' ');

            }

            if (parts[0] == null || parts[1] == null)
                throw new FormatException($"Malformed MOVE statement: {line.OriginalBlock}");

            source = parts[0].Trim();
            target = parts[1].Trim();

        }
        else
        {

            var tokens = clean.Split(" TO ", StringSplitOptions.RemoveEmptyEntries);

            if (tokens.Length >= 2 && string.Equals(tokens[1], " FOR ", StringComparison.OrdinalIgnoreCase))
            {
                // Has FOR clause
                var targetT = tokens[1].Split(" FOR ", StringSplitOptions.RemoveEmptyEntries);
                source = tokens[0].Trim();
                target = targetT[0].Trim();
                forClause = targetT[1].Trim();
            }
            else if (tokens.Length == 2)
            {
                source = tokens[0].Trim();
                target = tokens[1].Trim();
            }
            else
            {
                return new UnknownStatement
                {
                    OriginalCode = line.OriginalBlock,
                    LineNumber = line.StartLineNumber,
                };
            }
        }

        return new MoveAStatement
        {
            OriginalCode = line.OriginalBlock,
            LineNumber = line.StartLineNumber,
            Source = source,
            Target = target,
            Occurrence = forClause
        };
    }
}
