namespace EsfParser.Parser.Logic.Statements
{
    // === Statements ===
    public class IfStatement : IStatement
    {
        public StatementType Type => StatementType.If;
        public string OriginalCode { get; set; } = string.Empty;
        public string Condition { get; set; } = string.Empty;
        public List<IStatement> TrueStatements { get; set; } = new();
        public List<IStatement> ElseStatements { get; set; } = new();
        public int LineNumber { get; set; }

        public int NestingLevel { get; set; } = 0; 

        // tostring pretty print
        public override string ToString()
        {
            var trueNumberOfStatemets = TrueStatements.Count;     
            var elseNumberOfStatemets = ElseStatements.Count;
            return $"If: {Condition} (Line: {LineNumber}, Nesting: {NestingLevel}) If:{trueNumberOfStatemets} E:{elseNumberOfStatemets}";
        }
    }
}