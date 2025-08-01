using EsfParser.Parser.Logic.Statements;

public class EndStatement : IStatement
{
    public StatementType Type => StatementType.End;
    public string OriginalCode { get; set; } = string.Empty;
    public int LineNumber { get; set; }

}