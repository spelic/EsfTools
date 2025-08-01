using EsfParser.Parser.Logic.Statements;
using System.Text.RegularExpressions;

namespace EsfParser.Parser.Logic.Parsers
{
    public class DxfrStatementParser : IStatementParser
    {
        public bool CanParse(string line) =>
            line.TrimStart().StartsWith("DXFR ", StringComparison.OrdinalIgnoreCase);

        public IStatement Parse(List<PreprocessedLine> lines, ref int index)
        {
            var line = lines[index];
            var clean = line.CleanLine.Trim();

            var match = Regex.Match(clean, @"^DXFR\s+(\S+)\s+(\S+);?", RegexOptions.IgnoreCase);

            if (!match.Success)
            {
                return new UnknownStatement
                {
                    OriginalCode = line.OriginalBlock,
                    LineNumber = line.StartLineNumber
                };
            }

            return new DxfrStatement
            {
                OriginalCode = line.OriginalBlock,
                TargetApp = match.Groups[1].Value,
                TargetScreen = match.Groups[2].Value,
                LineNumber = line.StartLineNumber
            };
        }
    }
}
