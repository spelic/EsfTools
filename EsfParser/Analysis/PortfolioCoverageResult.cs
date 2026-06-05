using System.Collections.Generic;

namespace EsfParser.Analysis;

/// <summary>Aggregate result over all scanned programs.</summary>
public sealed class PortfolioCoverageResult
{
    public required string InputDirectory { get; init; }
    public required string OutputDirectory { get; init; }
    public bool GenerateEnabled { get; init; }
    public bool BuildEnabled { get; init; }

    public List<ProgramCoverageResult> Programs { get; init; } = new();
}
