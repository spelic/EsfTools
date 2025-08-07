namespace EsfParser.Parser.Logic.Statements
{

    public enum CallParameterType
    {
        Unknown,
        DataItem,
        Literal,
        Record,
        Map,
        SpecialVariable
    }

    public class CallParameter
    {
        public string Raw { get; set; } = string.Empty;
        public CallParameterType Type { get; set; } = CallParameterType.Unknown;

        public override string ToString()
        {
            return $"{Raw} [{Type}]";
        }
    }

    public class CallStatement : IStatement
    {
        public StatementType Type => StatementType.Call;
        public string OriginalCode { get; set; } = string.Empty;

        public string ProgramName { get; set; } = string.Empty;
        public bool IsServiceRoutine { get; set; } = false; // e.g., COMMIT, RESET, etc.
        public List<CallParameter> Parameters { get; set; } = new();
        public List<string> Options { get; set; } = new(); // NOMAPS, NONCSP, REPLY
        public int LineNumber { get; set; }
        public int NestingLevel { get; set; } = 0;

        public override string ToString()
        {
            var options = Options.Count > 0 ? $" [{string.Join(", ", Options)}]" : string.Empty;
            return $"CallStatement: {ProgramName}({string.Join(", ", Parameters)}){options} (Line: {LineNumber}, Nesting: {NestingLevel})";
        }

        // Implement IStatement interface method
        public string ToCSharp()
        {
            var args = Parameters.Any() ? string.Join(", ", Parameters.Select(p => p.Raw)) : "";
            return $"{ProgramName}({args});";
        }
    }

}