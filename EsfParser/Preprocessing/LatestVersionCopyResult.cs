using System.Collections.Generic;

namespace EsfParser.Preprocessing;

/// <summary>Outcome of attempting to place one selected file in the output.</summary>
public enum CopyStatus
{
    Copied,
    Skipped,   // e.g. flat-mode leaf-name collision
    DryRun,    // would have been copied
    Error,     // an exception was captured for this file
}

/// <summary>One selected file and what happened to it.</summary>
public sealed record CopiedEsfFile(
    string ProgramName,
    long Version,
    string SourcePath,
    string TargetPath,
    CopyStatus Status,
    string? Message = null);

/// <summary>Full result of <see cref="EsfLatestVersionPreprocessor.Run"/>.</summary>
public sealed class LatestVersionCopyResult
{
    public string InputDirectory { get; init; } = string.Empty;
    public string OutputDirectory { get; init; } = string.Empty;
    public bool DryRun { get; init; }

    public IReadOnlyList<CopiedEsfFile> Copied { get; init; } = new List<CopiedEsfFile>();
    public IReadOnlyList<EsfFileVersionInfo> IgnoredOlder { get; init; } = new List<EsfFileVersionInfo>();
    public IReadOnlyList<DuplicateLatestWarning> DuplicateWarnings { get; init; } = new List<DuplicateLatestWarning>();

    /// <summary>Per-file (non-fatal) errors captured during the run.</summary>
    public IReadOnlyList<string> Errors { get; init; } = new List<string>();
}
