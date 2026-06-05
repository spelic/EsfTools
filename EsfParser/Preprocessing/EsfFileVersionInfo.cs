using System.IO;
using System.Text.RegularExpressions;

namespace EsfParser.Preprocessing;

/// <summary>
/// The logical program name and version parsed from an ESF file name.
/// Version is taken only from a strict <c>-V&lt;digits&gt;</c> suffix (hyphen required);
/// file names without that suffix are treated as version 0 (see <see cref="HasExplicitVersion"/>).
/// </summary>
public sealed class EsfFileVersionInfo
{
    /// <summary>Absolute or original path of the file.</summary>
    public required string FullPath { get; init; }

    /// <summary>Leaf file name including extension (e.g. <c>IS00A-V26.esf</c>).</summary>
    public required string FileName { get; init; }

    /// <summary>Logical program name (e.g. <c>IS00A</c>), original casing preserved.</summary>
    public required string ProgramName { get; init; }

    /// <summary>Numeric version (0 when no <c>-V</c> suffix was present).</summary>
    public required long Version { get; init; }

    /// <summary>True when a <c>-V&lt;digits&gt;</c> suffix was found.</summary>
    public required bool HasExplicitVersion { get; init; }

    // Strict hyphen-V pattern, applied to the file name WITHOUT extension.
    // Lazy program capture so the version anchors at the end of the stem.
    private static readonly Regex VersionRegex =
        new(@"^(?<program>.+?)-[vV](?<version>\d+)$", RegexOptions.Compiled);

    /// <summary>Parse a file path's name into program + version.</summary>
    public static EsfFileVersionInfo Parse(string path)
    {
        var fileName = Path.GetFileName(path);
        var stem = Path.GetFileNameWithoutExtension(path);

        var m = VersionRegex.Match(stem);
        if (m.Success && long.TryParse(m.Groups["version"].Value, out var version))
        {
            return new EsfFileVersionInfo
            {
                FullPath = path,
                FileName = fileName,
                ProgramName = m.Groups["program"].Value,
                Version = version,
                HasExplicitVersion = true,
            };
        }

        // No strict -V suffix → whole stem is the program, version 0.
        return new EsfFileVersionInfo
        {
            FullPath = path,
            FileName = fileName,
            ProgramName = stem,
            Version = 0,
            HasExplicitVersion = false,
        };
    }
}
