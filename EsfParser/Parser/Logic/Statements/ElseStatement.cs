using EsfParser.Parser.Logic.Statements;

public class ElseStatement : IStatement
{
    public StatementType Type => StatementType.Else;
    public string OriginalCode { get; set; } = string.Empty;
    public int LineNumber { get; set; }
    public int NestingLevel { get; set; } = 0;
    public string InlineComent { get; set; } = string.Empty;


}