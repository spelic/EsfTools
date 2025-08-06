namespace EsfParser.Parser.Logic.Statements
{
    public class WhileStatement : IStatement
    {
        public StatementType Type => StatementType.While;
        public string OriginalCode { get; set; } = string.Empty;
        public string Condition { get; set; } = string.Empty;
        public List<IStatement> BodyStatements { get; set; } = new();

        public int LineNumber { get; set; }
        public int NestingLevel { get; set; } = 0;
    }
}