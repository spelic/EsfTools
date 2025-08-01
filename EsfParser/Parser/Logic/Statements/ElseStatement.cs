using EsfParser.Parser.Logic.Statements;

public class ElseStatement : IStatement
{
    public StatementType Type => StatementType.Else;
    public string OriginalCode { get; set; } = string.Empty;
    public int LineNumber { get; set; }

}