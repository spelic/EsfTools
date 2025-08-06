using EsfParser.Parser.Logic.Statements;

public class MarkerStatement : IStatement
{
    public StatementType Type { get; set; } = StatementType.Comment;
    public string OriginalCode { get; set; } = string.Empty;
    public int LineNumber { get; set; }
    public int NestingLevel { get; set; } = 0;
}
