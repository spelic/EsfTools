using EsfParser.Parser.Logic.Parsers;
using EsfParser.Parser.Logic.Statements;
using System.Text.RegularExpressions;

public class SystemFunctionStatementParser : IStatementParser
{
    private static readonly HashSet<string> KnownFunctions = new(StringComparer.OrdinalIgnoreCase)
    {
        "EZECLOS", "EZEWAIT", "EZECNVCM", "EZECONCT", "EZECOMIT",
        "EZEBYTES", "EZEAPP", "EZEAID", "EZEUSRID", "EZEDAY",
        "EZECONV", "EZEREPLY", "EZEFLO", "EZEROLLB"
        // Add more as needed
    };

    public bool CanParse(string line)
    {
        line = line.Trim().TrimEnd(';');
        var firstToken = line.Split(new[] { ' ', '(' }, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();
        return !string.IsNullOrWhiteSpace(firstToken) && KnownFunctions.Contains(firstToken.ToUpperInvariant());
    }

    public IStatement Parse(List<PreprocessedLine> lines, ref int index)
    {
        var line = lines[index];
        var clean = line.CleanLine.Trim().TrimEnd(';');

        var match = Regex.Match(clean, @"^(?<func>\w+)(?:\((?<params>.*?)\))?$");
        if (!match.Success)
        {
            return new UnknownStatement
            {
                OriginalCode = line.OriginalBlock,
                LineNumber = line.StartLineNumber,
            };
        }

        var name = match.Groups["func"].Value;
        var parameters = new List<string>();

        if (match.Groups["params"].Success)
        {
            parameters = match.Groups["params"].Value
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(p => p.Trim())
                .ToList();
        }

        return new SystemFunctionStatement
        {
            OriginalCode = line.OriginalBlock,
            Name = name,
            Parameters = parameters,
            LineNumber = line.StartLineNumber,
        };
    }
}
