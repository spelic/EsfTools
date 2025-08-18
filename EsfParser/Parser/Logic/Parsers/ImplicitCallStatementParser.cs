using EsfParser.Parser.Logic.Parsers;
using EsfParser.Parser.Logic.Statements;
using System.Text.RegularExpressions;

public class ImplicitCallStatementParser : IStatementParser
{
    public bool CanParse(string line)
    {
        if (line.ToUpperInvariant().StartsWith("EZ")) return false;
        return Regex.IsMatch(line.Trim(), @"^[A-Z0-9_-]+\s*\(.*\)\s*;?$", RegexOptions.IgnoreCase);
    }

    public IStatement Parse(List<PreprocessedLine> lines, ref int index, int currentLevel = 0)
    {
        var line = lines[index];
        var clean = line.CleanLine.TrimEnd(';');

        var match = Regex.Match(clean, @"^(?<prog>[A-Z0-9_-]+)\s*\((?<args>[^)]*)\)\s*;?$", RegexOptions.IgnoreCase);

        var program = match.Groups["prog"].Value.Trim();
        var argText = match.Groups["args"].Value.Trim();

        var parameters = new List<CallParameter>();

        if (!string.IsNullOrWhiteSpace(argText))
        {
            var args = argText.Split(',').Select(a => a.Trim()).Where(a => !string.IsNullOrWhiteSpace(a));
            foreach (var arg in args)
            {
                parameters.Add(new CallParameter
                {
                    Raw = arg,
                    Type = ClassifyParameter(arg)
                });
            }
        }

        return new CallStatement
        {
            OriginalCode = line.OriginalBlock,
            ProgramName = program,
            Parameters = parameters,
            LineNumber = line.StartLineNumber
        };
    }

    private static CallParameterType ClassifyParameter(string raw)
    {
        if (string.IsNullOrWhiteSpace(raw))
            return CallParameterType.Unknown;

        if (raw.StartsWith("'") || raw.StartsWith("\""))
            return CallParameterType.Literal;

        if (raw.EndsWith("A") || raw.EndsWith("B"))
            return CallParameterType.Map;

        if (raw.All(char.IsUpper) && raw.Length >= 6 && raw.StartsWith("MAP"))
            return CallParameterType.Map;

        if (raw.Contains('.'))
            return CallParameterType.Record;

        if (raw.StartsWith("EZE") || raw.StartsWith("EZERT"))
            return CallParameterType.SpecialVariable;

        return CallParameterType.DataItem;
    }
}
