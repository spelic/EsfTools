namespace EsfParser.Parser.Logic.Statements
{
    public class UnknownStatement : IStatement
    {
        public StatementType Type => StatementType.Unknown;
        public string OriginalCode { get; set; } = string.Empty;
        public int LineNumber { get; set; }
        public int NestingLevel { get; set; } = 0;

        public string ToCSharp()
        {
            return " // throw new NotImplementedException();" + this.ToString();
        }

        // tostring pretty print
        public override string ToString()
            => $"UnknownStatement: (Line: {LineNumber}, Nesting: {NestingLevel}){(string.IsNullOrEmpty(OriginalCode) ? string.Empty : $" // {OriginalCode}")}";

    }

}