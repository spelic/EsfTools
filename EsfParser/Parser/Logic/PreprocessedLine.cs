public class PreprocessedLine
{
    public int StartLineNumber { get; set; }
    public int EndLineNumber { get; set; }

    public string OriginalBlock { get; set; } = string.Empty;
    public string CleanLine { get; set; } = string.Empty;

    public string? InlineComment { get; set; } = null;
    public string? FullLineComment { get; set; } = null;
}
