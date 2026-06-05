using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace EsfParser.Analysis;

public enum RiskLevel { Low, Medium, High, Blocked }

/// <summary>Full coverage/risk result for a single ESF program. Every scanned file produces one,
/// even on failure (so the report is complete).</summary>
public sealed class ProgramCoverageResult
{
    public required string FileName { get; init; }
    public required string FilePath { get; init; }
    public required string ProgramName { get; init; }

    // ── Stage status (cascade: a failed earlier stage skips later signals) ──
    public bool ParseFailed { get; set; }
    public bool ModelBuildFailed { get; set; }
    public bool GenerationFailed { get; set; }   // WriteProjectFiles threw
    public string? FailureMessage { get; set; }

    // ── Counts ──
    public int FunctionCount { get; set; }
    public int MapCount { get; set; }
    public int RecordCount { get; set; }
    public int ItemCount { get; set; }
    public int TableCount { get; set; }

    public int TotalStatements { get; set; }
    public int TranslatableStatements { get; set; }   // total − Comment/Else/End
    public int UnknownStatements { get; set; }
    public double UnknownPercent { get; set; }        // unknown / translatable (0 if denom 0)

    /// <summary>Functions skipped during C# generation (from RoslynExporter diagnostics).</summary>
    public int GenerationSkippedFunctions { get; set; }

    /// <summary>Path to the generated project (null when generation was not run or failed).</summary>
    public string? GeneratedProjectDir { get; set; }

    // ── Findings ──
    public List<string> DistinctUnknownTexts { get; set; } = new();
    public List<string> UnsupportedEzeWords { get; set; } = new();
    public List<string> UnsupportedSqlOptions { get; set; } = new();
    public List<FunctionCoverageResult> Functions { get; set; } = new();
    public FeatureUsageResult Features { get; set; } = new();
    public BuildResult Build { get; set; } = new();

    // ── Risk ──
    public int RiskScore { get; set; }
    public RiskLevel RiskLevel { get; set; }
    public string RecommendedAction { get; set; } = string.Empty;

    /// <summary>Captured console output (parser diagnostics + generation messages) for this file.</summary>
    [JsonIgnore]
    public string ConsoleOutput { get; set; } = string.Empty;
}
