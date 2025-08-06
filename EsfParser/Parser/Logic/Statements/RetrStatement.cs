using EsfParser.Parser.Logic.Statements;

public class RetrStatement : IStatement
{
    public StatementType Type => StatementType.Retr;
    public string OriginalCode { get; set; } = string.Empty;

    public string SourceItem { get; set; } = string.Empty;      // ITEM
    public string TableName { get; set; } = string.Empty;       // INFO
    public string SearchColumn { get; set; } = string.Empty;    // INFO.STATE
    public string TargetItem { get; set; } = string.Empty;      // AMOUNT
    public string ReturnColumn { get; set; } = string.Empty;    // AREA

    public int LineNumber { get; set; }
    public int NestingLevel { get; set; } = 0;
}
