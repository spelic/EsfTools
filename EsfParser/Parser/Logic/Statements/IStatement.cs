namespace EsfParser.Parser.Logic.Statements
{
    // === Interfaces ===
    public interface IStatement
    {
        StatementType Type { get; }
        string OriginalCode { get; set; }
        public int LineNumber { get; set; }

        public int NestingLevel { get; set; }

    }
}