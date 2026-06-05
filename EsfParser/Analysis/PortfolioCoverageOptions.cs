namespace EsfParser.Analysis;

/// <summary>Options for <see cref="PortfolioCoverageAnalyzer"/> + report orchestration.</summary>
public sealed class PortfolioCoverageOptions
{
    public required string InputDirectory { get; init; }
    public required string OutputDirectory { get; init; }
    public bool Recursive { get; init; } = true;

    /// <summary>Generate a C# project per program (cheap-ish; default true).</summary>
    public bool Generate { get; init; } = true;

    /// <summary>Run <c>dotnet build</c> on generated projects. Default false (expensive).</summary>
    public bool Build { get; init; }

    /// <summary>Build/generate only the first N files (after filtering). 0 = no limit.</summary>
    public int Limit { get; init; }

    /// <summary>Optional file-name glob (e.g. <c>NR11*</c>) restricting which programs build/generate.</summary>
    public string? Filter { get; init; }

    /// <summary>Max concurrent builds. Default = max(1, CPU-2).</summary>
    public int MaxParallel { get; init; } = Math.Max(1, Environment.ProcessorCount - 2);

    /// <summary>Per-build timeout in seconds.</summary>
    public int MaxBuildSeconds { get; init; } = 60;

    /// <summary>Required to build the whole portfolio (no limit/filter) — guards against accidental hours-long runs.</summary>
    public bool BuildAll { get; init; }
}
