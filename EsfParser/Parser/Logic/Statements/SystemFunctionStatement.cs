using EsfParser.Parser.Logic.Statements;

public class SystemFunctionStatement : IStatement
{
    public StatementType Type => StatementType.SystemFunction;
    public string OriginalCode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public List<string> Parameters { get; set; } = new();
    public int LineNumber { get; set; }
    public int NestingLevel { get; set; } = 0;

    public string ToCSharp()
    {
        return " // SystemFunction: throw new NotImplementedException();";
    }

    // tostring pretty print   
    public override string ToString()
    {
        return $"SystemFunctionStatement: {Name}({string.Join(", ", Parameters)}) (Line: {LineNumber}, Nesting: {NestingLevel})";
    }
}
