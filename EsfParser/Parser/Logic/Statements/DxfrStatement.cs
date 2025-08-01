namespace EsfParser.Parser.Logic.Statements
{
    public class DxfrStatement : IStatement
    {
        public StatementType Type => StatementType.Dxfr;
        public string OriginalCode { get; set; } = string.Empty;
        public string TargetApp { get; set; } = string.Empty;
        public string TargetScreen { get; set; } = string.Empty;
        public string TargetField { get; set; } = string.Empty;
        public string SourceApp { get; set; } = string.Empty;
        public int LineNumber { get; set; }

    }
}