using EsfParser.Parser.Logic.Statements;

public class EndStatement : IStatement
{
    public StatementType Type => StatementType.End;
    public string OriginalCode { get; set; } = string.Empty;
    public int LineNumber { get; set; }
    public string InlineComent { get; set; } = string.Empty;
    public int NestingLevel { get; set; } = 0;
}