namespace EsfParser.Parser.Logic.Statements
{
    public class MoveAStatement : IStatement
    {
        public StatementType Type => StatementType.MoveA;
        public string OriginalCode { get; set; } = string.Empty;
        public string Source { get; set; } = string.Empty;
        public string Target { get; set; } = string.Empty;
        public string Occurrence { get; set; }
        public int LineNumber { get; set; }
        public int NestingLevel { get; set; } = 0;

       public string ToCSharp()
{
    if (!string.IsNullOrEmpty(Occurrence))
        return $"for (int i = 0; i < {Occurrence}; i++) {Target}[i] = {Source};";
    
    return $"{Target} = Enumerable.Repeat({Source}, {Target}.Length).ToArray();";
}

        public override string ToString()
            => $"MoveAStatement: {Source} -> {Target} (For: {Occurrence}) (Line: {LineNumber}, Nesting: {NestingLevel})";
    }
}