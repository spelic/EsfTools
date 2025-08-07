namespace EsfParser.Parser.Logic.Statements
{
    public class DxfrStatement : IStatement
    {
        public StatementType Type => StatementType.Dxfr;
        public string OriginalCode { get; set; } = string.Empty;
        public string ProgramName { get; set; } = string.Empty;
        public string ProgramStartScreen { get; set; } = string.Empty;
        public string RecordName { get; set; } = string.Empty;
        public string SourceApp { get; set; } = string.Empty;
        public int LineNumber { get; set; }
        public int NestingLevel { get; set; } = 0;

        public override string ToString()
        {
            return $"DxfrStatement: {ProgramName}.{ProgramStartScreen}.{RecordName} <- {SourceApp} (Line: {LineNumber}, Nesting: {NestingLevel})";
        }

        public string ToCSharp()
        {
            var args = !string.IsNullOrEmpty(RecordName) ? $"({RecordName})" : "";
            return $"TransferToProgram(\"{ProgramName}\"{(string.IsNullOrEmpty(RecordName) ? "" : $", {RecordName}")});";

        }
    }
}