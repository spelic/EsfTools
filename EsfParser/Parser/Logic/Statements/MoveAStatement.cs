namespace EsfParser.Parser.Logic.Statements
{
    public class MoveAStatement : IStatement
    {
        public StatementType Type => StatementType.MoveA;
        public string OriginalCode { get; set; } = string.Empty;
        public string Source { get; set; } = string.Empty;
        public string Target { get; set; } = string.Empty;
        public string ForClause { get; set; }
        public int LineNumber { get; set; }
        public int NestingLevel { get; set; } = 0;

         public override string ToString()
            => $"MoveAStatement: {Source} -> {Target} (For: {ForClause}) (Line: {LineNumber}, Nesting: {NestingLevel})";
    }
}