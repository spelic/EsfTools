using EsfParser.Parser.Logic.Statements;

public class SystemFunctionStatement : IStatement
{
    public StatementType Type => StatementType.SystemFunction;
    public string OriginalCode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public List<string> Parameters { get; set; } = new();
    public int LineNumber { get; set; }


    public override string ToString()
    {
        return Parameters.Count > 0
            ? $"{Name}({string.Join(", ", Parameters)})"
            : Name;
    }
}
