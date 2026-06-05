using System.Collections.Generic;

namespace EsfParser.Preprocessing;

/// <summary>
/// Two or more files map to the same program AND the same highest version.
/// One is selected (the <see cref="DuplicateLatestWarning"/> records all candidate paths,
/// winner first) and the rest are ignored.
/// </summary>
public sealed record DuplicateLatestWarning(
    string ProgramName,
    long Version,
    IReadOnlyList<string> Paths);

/// <summary>
/// Pure result of <see cref="EsfLatestVersionSelector.Select"/> — no I/O performed.
/// </summary>
public sealed class LatestVersionSelectionResult
{
    /// <summary>The single chosen latest file per logical program.</summary>
    public IReadOnlyList<EsfFileVersionInfo> Selected { get; init; } = new List<EsfFileVersionInfo>();

    /// <summary>Older versions, plus duplicate-latest losers, that will not be copied.</summary>
    public IReadOnlyList<EsfFileVersionInfo> Ignored { get; init; } = new List<EsfFileVersionInfo>();

    /// <summary>Programs that had more than one file at the highest version.</summary>
    public IReadOnlyList<DuplicateLatestWarning> DuplicateWarnings { get; init; } = new List<DuplicateLatestWarning>();
}
