using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace EsfParser.Preprocessing;

/// <summary>
/// Scans an input directory for *.esf files, selects the latest version of each program via
/// <see cref="EsfLatestVersionSelector"/>, and copies the winners to an output directory.
/// I/O errors on individual files are captured (one bad file never crashes the run); option
/// validation problems throw <see cref="ArgumentException"/> before any file is touched.
/// </summary>
public sealed class EsfLatestVersionPreprocessor
{
    public LatestVersionCopyResult Run(LatestVersionPreprocessorOptions options)
    {
        if (options is null) throw new ArgumentNullException(nameof(options));

        var input = Validate(options.InputDirectory, options.OutputDirectory);
        var output = Path.GetFullPath(options.OutputDirectory);

        // Scan input, but never treat files already inside the output directory as input.
        var files = Directory
            .EnumerateFiles(input, "*.esf",
                options.Recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly)
            .Select(Path.GetFullPath)
            .Where(f => !IsUnder(output, f))
            .ToList();

        var selection = EsfLatestVersionSelector.Select(files);

        // Output is always created so reports can be written, even in dry-run.
        Directory.CreateDirectory(output);

        if (!options.DryRun && options.CleanOutput)
            CleanInside(output);

        var copied = new List<CopiedEsfFile>();
        var errors = new List<string>();
        var usedTargets = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var sel in selection.Selected)
        {
            string target = ComputeTarget(sel, input, output, options.CopyMode);
            try
            {
                if (options.DryRun)
                {
                    copied.Add(new CopiedEsfFile(sel.ProgramName, sel.Version, sel.FullPath, target, CopyStatus.DryRun));
                    continue;
                }

                // Flat-mode leaf-name collision between two different selected files.
                if (!usedTargets.Add(target))
                {
                    var msg = $"Target '{target}' already produced by another program; skipped '{sel.FullPath}'.";
                    errors.Add(msg);
                    copied.Add(new CopiedEsfFile(sel.ProgramName, sel.Version, sel.FullPath, target, CopyStatus.Skipped, msg));
                    continue;
                }

                Directory.CreateDirectory(Path.GetDirectoryName(target)!);
                File.Copy(sel.FullPath, target, overwrite: true);
                copied.Add(new CopiedEsfFile(sel.ProgramName, sel.Version, sel.FullPath, target, CopyStatus.Copied));
            }
            catch (Exception ex)
            {
                var msg = $"{sel.FullPath}: {ex.Message}";
                errors.Add(msg);
                copied.Add(new CopiedEsfFile(sel.ProgramName, sel.Version, sel.FullPath, target, CopyStatus.Error, ex.Message));
            }
        }

        return new LatestVersionCopyResult
        {
            InputDirectory = input,
            OutputDirectory = output,
            DryRun = options.DryRun,
            Copied = copied,
            IgnoredOlder = selection.Ignored,
            DuplicateWarnings = selection.DuplicateWarnings,
            Errors = errors,
        };
    }

    // ── Safety validation ──────────────────────────────────────────────────
    private static string Validate(string inputDir, string outputDir)
    {
        if (string.IsNullOrWhiteSpace(inputDir)) throw new ArgumentException("Input directory is required.");
        if (string.IsNullOrWhiteSpace(outputDir)) throw new ArgumentException("Output directory is required.");

        var input = Path.GetFullPath(inputDir);
        var output = Path.GetFullPath(outputDir);

        if (!Directory.Exists(input))
            throw new DirectoryNotFoundException($"Input directory not found: {input}");

        if (PathsEqual(input, output))
            throw new ArgumentException("Input and output directories must be different.");

        if (Directory.GetParent(output) is null)
            throw new ArgumentException($"Output directory must not be a filesystem root: {output}");

        if (IsUnder(output, input))
            throw new ArgumentException("Output directory must not be an ancestor of the input directory.");

        return input;
    }

    // Delete files/subdirectories strictly inside <paramref name="dir"/> (never the dir itself).
    private static void CleanInside(string dir)
    {
        foreach (var f in Directory.EnumerateFiles(dir)) File.Delete(f);
        foreach (var d in Directory.EnumerateDirectories(dir)) Directory.Delete(d, recursive: true);
    }

    private static string ComputeTarget(EsfFileVersionInfo file, string input, string output, EsfCopyMode mode) =>
        mode == EsfCopyMode.PreserveRelativePath
            ? Path.GetFullPath(Path.Combine(output, Path.GetRelativePath(input, file.FullPath)))
            : Path.Combine(output, file.FileName);

    private static bool PathsEqual(string a, string b) =>
        string.Equals(
            Path.TrimEndingDirectorySeparator(Path.GetFullPath(a)),
            Path.TrimEndingDirectorySeparator(Path.GetFullPath(b)),
            StringComparison.OrdinalIgnoreCase);

    // True when <paramref name="path"/> is the same as, or nested under, <paramref name="ancestor"/>.
    private static bool IsUnder(string ancestor, string path)
    {
        var a = Path.TrimEndingDirectorySeparator(Path.GetFullPath(ancestor));
        var p = Path.TrimEndingDirectorySeparator(Path.GetFullPath(path));
        if (string.Equals(a, p, StringComparison.OrdinalIgnoreCase)) return true;
        return p.StartsWith(a + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase);
    }
}
