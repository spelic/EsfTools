namespace EsfParser.Analysis;

public enum BuildStatus
{
    NotAttempted,
    Succeeded,
    Failed,
    TimedOut,
}

/// <summary>Outcome of running <c>dotnet build</c> on one generated project.</summary>
public sealed class BuildResult
{
    public BuildStatus Status { get; init; } = BuildStatus.NotAttempted;
    public int ErrorCount { get; init; }
    public int WarningCount { get; init; }

    /// <summary>Path to the full captured build log (null when not attempted).</summary>
    public string? LogPath { get; init; }

    public double Seconds { get; init; }
}
