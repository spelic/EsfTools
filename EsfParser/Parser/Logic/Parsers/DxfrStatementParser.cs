using EsfParser.Parser.Logic.Statements;
using System.Text.RegularExpressions;

namespace EsfParser.Parser.Logic.Parsers
{
    public class DxfrStatementParser : IStatementParser
    {
        public bool CanParse(string line) =>
            line.TrimStart().StartsWith("DXFR ", StringComparison.OrdinalIgnoreCase);

        public IStatement Parse(List<PreprocessedLine> lines, ref int index, int currentLevel = 0)
        {
            var line = lines[index];
            var clean = line.CleanLine.TrimEnd(';');

            var match = Regex.Match(clean, @"^DXFR\s+(\S+)\s+(\S+);?", RegexOptions.IgnoreCase);

            if (!match.Success)
            {

                var parts = clean.Split(' ');
                if (parts.Length == 2 && parts[0].Trim() == "DXFR")
                {
                    return new DxfrStatement
                    {
                        OriginalCode = line.OriginalBlock,
                        ProgramName = parts[1],
                        ProgramStartScreen = "",
                        LineNumber = line.StartLineNumber
                    };
                }
                else


                    return new UnknownStatement
                    {
                        OriginalCode = line.OriginalBlock,
                        LineNumber = line.StartLineNumber
                    };
            }

            return new DxfrStatement
            {
                OriginalCode = line.OriginalBlock,
                ProgramName = match.Groups[1].Value,
                ProgramStartScreen = match.Groups[2].Value,
                LineNumber = line.StartLineNumber
            };
        }
    }
}
