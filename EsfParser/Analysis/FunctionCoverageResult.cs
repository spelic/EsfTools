namespace EsfParser.Analysis;

/// <summary>Per-function statement coverage within a program.</summary>
public sealed class FunctionCoverageResult
{
    public required string FunctionName { get; init; }
    public required string Option { get; init; }
    public int TotalStatements { get; init; }
    public int UnknownStatements { get; init; }
}
