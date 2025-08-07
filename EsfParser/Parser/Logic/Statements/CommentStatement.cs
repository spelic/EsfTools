namespace EsfParser.Parser.Logic.Statements
{
    public class CommentStatement : IStatement
    {
        public StatementType Type => StatementType.Comment;
        public int LineNumber { get; set; }

        public string OriginalCode { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;
        public int NestingLevel { get; set; } = 0;

        // tostring pretty print
        public override string ToString()
            => $"Comment: {Text} (Line: {LineNumber}, Nesting: {NestingLevel})";

        // add method to convert this statement to C# code
        public string ToCSharpCode()
        {
            // C# comments are prefixed with //
            return $"// {Text}";
        }

        public string ToCSharp()
        {
            return "throw new NotImplementedException();";  
        }
    }
}