using EsfParser.Parser.Logic.Statements;

public class ElseStatement : IStatement
{
    public StatementType Type => StatementType.Else;
    public string OriginalCode { get; set; } = string.Empty;
    public int LineNumber { get; set; }
    public int NestingLevel { get; set; } = 0;
    public string InlineComent { get; set; } = string.Empty;

    public string ToCSharp()
    {
        return " // throw new NotImplementedException();" + this.ToString();
    }

    public override string ToString()
    {
        return $"ElseStatement: (Line: {LineNumber}, Nesting: {NestingLevel}){(string.IsNullOrEmpty(InlineComent) ? string.Empty : $" // {InlineComent}")}";
    }

}