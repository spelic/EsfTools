using EsfParser.Parser.Logic.Statements;

namespace EsfParser.Parser.Logic.Parsers
{
    public class CallStatementParser : IStatementParser
    {
        private static readonly HashSet<string> ServiceRoutines = new()
        {
            "AUDIT", "COMMIT", "CREATX", "CSPTDLI", "RESET"
        };

        private static readonly HashSet<string> SpecialItems = new()
        {
            "EZEDLPSB", "EZEDLPCB"
        };

        public bool CanParse(string line) =>
            line.TrimStart().StartsWith("CALL ", StringComparison.OrdinalIgnoreCase);

        public IStatement Parse(List<PreprocessedLine> lines, ref int index)
        {
            var line = lines[index];
            string clean = line.CleanLine.Trim()[4..].Trim(); // remove "CALL"
            var result = new CallStatement
            {
                OriginalCode = line.OriginalBlock,
                LineNumber = line.StartLineNumber,
            };

            // === Split on optional (OPTIONS) group
            var callParts = clean.Split('(', 2, StringSplitOptions.TrimEntries);
            string mainPart = callParts[0].Trim();
            string optionPart = callParts.Length > 1 ? callParts[1].TrimEnd(')') : "";

            // === Tokens (ProgramName, Parameters)
            var tokens = mainPart.Split(new[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries);
            if (tokens.Length == 0)
                return result;

            result.ProgramName = tokens[0];
            result.IsServiceRoutine = ServiceRoutines.Contains(result.ProgramName.ToUpperInvariant());

            foreach (var token in tokens.Skip(1))
            {
                result.Parameters.Add(new CallParameter
                {
                    Raw = token,
                    Type = InferParameterType(token)
                });
            }

            // === Options
            if (!string.IsNullOrEmpty(optionPart))
            {
                var opts = optionPart.Split(',', StringSplitOptions.RemoveEmptyEntries);
                foreach (var opt in opts)
                    result.Options.Add(opt.Trim().ToUpperInvariant());
            }

            return result;
        }

        private CallParameterType InferParameterType(string token)
        {
            if (SpecialItems.Contains(token.ToUpperInvariant()))
                return CallParameterType.SpecialVariable;

            if (token.StartsWith("\"") || token.StartsWith("'"))
                return CallParameterType.Literal;

            if (token.Contains("."))
                return CallParameterType.DataItem;

            if (token.Length >= 6 && token.EndsWith("01"))
                return CallParameterType.Record;

            if (token.Length >= 6 && token.EndsWith("02"))
                return CallParameterType.Map;

            return CallParameterType.DataItem;
        }
    }
}
