namespace EsfParser.Parser.Logic.Statements
{
    public class SetStatement : IStatement
    {
        public StatementType Type => StatementType.Set;
        public string OriginalCode { get; set; } = string.Empty;
        public string Target { get; set; } = string.Empty;
        public List<string> Attributes { get; set; } = new();
        public int LineNumber { get; set; }
        public int NestingLevel { get; set; } = 0;

        public override string ToString()
        {
            return $"SetStatement: {Target} with attributes [{string.Join(", ", Attributes)}] (Line: {LineNumber}, Nesting: {NestingLevel})";
        }


    }
}