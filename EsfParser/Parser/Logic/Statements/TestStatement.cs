namespace EsfParser.Parser.Logic.Statements
{
    public class TestStatement : IStatement
    {
        public StatementType Type => StatementType.Test;
        public string OriginalCode { get; set; } = string.Empty;
        public string Expression { get; set; } = string.Empty;

        public int LineNumber { get; set; }

        public int NestingLevel { get; set; } = 0;

        public string ToCSharp()
        {
            throw new NotImplementedException();
        }

        // tostring pretty print
        public override string ToString()
            => $"TestStatement: {Expression} (Line: {LineNumber}, Nesting: {NestingLevel})";
    }
}