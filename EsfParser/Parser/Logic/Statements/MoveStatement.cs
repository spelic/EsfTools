namespace EsfParser.Parser.Logic.Statements
{
    public class MoveStatement : IStatement
    {
        public StatementType Type => StatementType.Move;
        public string OriginalCode { get; set; } = string.Empty;
        public string Source { get; set; } = string.Empty;
        public string Destination { get; set; } = string.Empty;

        public int LineNumber { get; set; }
        public int NestingLevel { get; set; } = 0;

        public override string ToString()
               => $"MoveStatement: {Source} -> {Destination} (Line: {LineNumber}, Nesting: {NestingLevel})";

        // add method to convert this statement to C# code


        public string ToCSharp()
        {
            return $"{Source} = {Destination};";
        }
    }

}   

   
   
