namespace EsfParser.Parser.Logic.Statements
{
    public class UnknownStatement : IStatement
    {
        public StatementType Type => StatementType.Unknown;
        public string OriginalCode { get; set; } = string.Empty;
        public int LineNumber { get; set; }

    }

}