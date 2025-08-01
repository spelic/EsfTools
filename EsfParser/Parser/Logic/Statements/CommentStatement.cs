namespace EsfParser.Parser.Logic.Statements
{
    public class CommentStatement : IStatement
    {
        public StatementType Type => StatementType.Comment;
        public int LineNumber { get; set; }

        public string OriginalCode { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;
    }
}