using EsfParser.Parser.Logic.Statements;

public class MarkerStatement : IStatement
{
    public StatementType Type { get; set; } = StatementType.Comment;
    public string OriginalCode { get; set; } = string.Empty;
    public int LineNumber { get; set; }
    public int NestingLevel { get; set; } = 0;

    public string ToCSharp()
    {
        throw new NotImplementedException();
    }

    // ToString pretty print
    public override string ToString()
        => $"MarkerStatement: (Line: {LineNumber}, Nesting: {NestingLevel}){(string.IsNullOrEmpty(OriginalCode) ? string.Empty : $" // {OriginalCode}")}";
}
