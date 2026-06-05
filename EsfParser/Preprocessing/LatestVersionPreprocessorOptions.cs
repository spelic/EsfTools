namespace EsfParser.Preprocessing;

/// <summary>How selected files are laid out under the output directory.</summary>
public enum EsfCopyMode
{
    /// <summary>All selected files dropped flat into the output directory (default).</summary>
    Flat,

    /// <summary>Mirror each file's path relative to the input directory.</summary>
    PreserveRelativePath,
}

/// <summary>Options for <see cref="EsfLatestVersionPreprocessor.Run"/>.</summary>
public sealed class LatestVersionPreprocessorOptions
{
    public required string InputDirectory { get; init; }
    public required string OutputDirectory { get; init; }

    /// <summary>Recurse into subdirectories when scanning for *.esf. Default true.</summary>
    public bool Recursive { get; init; } = true;

    /// <summary>Delete existing contents of the output directory before copying. Ignored in dry-run.</summary>
    public bool CleanOutput { get; init; }

    /// <summary>Report only — perform no clean and no copy. Default false.</summary>
    public bool DryRun { get; init; }

    /// <summary>Output layout. Default <see cref="EsfCopyMode.Flat"/>.</summary>
    public EsfCopyMode CopyMode { get; init; } = EsfCopyMode.Flat;
}
