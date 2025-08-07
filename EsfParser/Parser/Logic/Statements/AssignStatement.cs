namespace EsfParser.Parser.Logic.Statements
{


    public class AssignStatement : IStatement
    {
        public StatementType Type => StatementType.Assign;
        public string OriginalCode { get; set; } = string.Empty;
        public string Left { get; set; } = string.Empty;
        public string Right { get; set; } = string.Empty;
        public int LineNumber { get; set; }
        public int NestingLevel { get; set; } = 0;

        public string ToCSharp()
        {
            return "throw new NotImplementedException();";
        }

        public override string ToString()
            => $"AssignStatement: {Left} = {Right} (Line: {LineNumber}, Nesting: {NestingLevel})";
    }
}