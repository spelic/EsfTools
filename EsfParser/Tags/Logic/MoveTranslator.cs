using System.Text.RegularExpressions;

namespace EsfParser.Tags.Logic
{
    public class MoveTranslator : IStatementTranslator
    {
        private readonly IExpressionConverter _exprConv;
        private static readonly Regex _moveRx = new(@"^(?:MOVE\s+)?(?<src>.+?)\s+TO\s+(?<tgt>.+?);?$", RegexOptions.IgnoreCase);

        public MoveTranslator(IExpressionConverter exprConv) => _exprConv = exprConv;
        public bool CanTranslate(string line) => _moveRx.IsMatch(line);
        public string Translate(string line, out string? original)
        {
            original = line;
            var m = _moveRx.Match(line);
            var src = _exprConv.Convert(m.Groups["src"].Value);
            var tgt = _exprConv.Convert(m.Groups["tgt"].Value);
            return $"{tgt} = {src};";
        }
    }
}
