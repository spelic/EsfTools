using EsfParser.Preprocessing;

namespace EsfConsoleConverter;

/// <summary>
/// Verb: <c>prepare-latest-esf</c>. Selects the latest version of each ESF program from an input
/// folder and copies the winners to an output folder, writing selection reports. Thin wiring only —
/// all logic lives in <see cref="EsfParser.Preprocessing"/>.
/// </summary>
internal static class PrepareLatestCommand
{
    private const string Usage = """
        Usage:
          prepare-latest-esf --input <folder> --output <folder>
                             [--recursive true|false]      (default true)
                             [--clean-output true|false]   (default false)
                             [--copy-mode flat|preserve-relative-path]  (default flat)
                             [--dry-run true|false]        (default false)
        """;

    public static int Run(string[] args)
    {
        string? input = null, output = null;
        bool recursive = true, cleanOutput = false, dryRun = false;
        var copyMode = EsfCopyMode.Flat;

        for (int i = 0; i < args.Length; i++)
        {
            string a = args[i];
            switch (a)
            {
                case "--input" when i + 1 < args.Length: input = args[++i]; break;
                case "--output" when i + 1 < args.Length: output = args[++i]; break;
                case "--recursive": recursive = ReadBool(args, ref i, a, out var rOk); if (!rOk) return Fail($"Invalid value for {a}."); break;
                case "--clean-output": cleanOutput = ReadBool(args, ref i, a, out var cOk); if (!cOk) return Fail($"Invalid value for {a}."); break;
                case "--dry-run": dryRun = ReadBool(args, ref i, a, out var dOk); if (!dOk) return Fail($"Invalid value for {a}."); break;
                case "--copy-mode" when i + 1 < args.Length:
                    var mode = args[++i];
                    switch (mode.ToLowerInvariant())
                    {
                        case "flat": copyMode = EsfCopyMode.Flat; break;
                        case "preserve-relative-path": copyMode = EsfCopyMode.PreserveRelativePath; break;
                        default: return Fail($"Unknown --copy-mode '{mode}'. Use flat or preserve-relative-path.");
                    }
                    break;
                default:
                    return Fail($"Unknown or incomplete option: '{a}'.");
            }
        }

        if (string.IsNullOrWhiteSpace(input)) return Fail("--input is required.");
        if (string.IsNullOrWhiteSpace(output)) return Fail("--output is required.");

        var options = new LatestVersionPreprocessorOptions
        {
            InputDirectory = input!,
            OutputDirectory = output!,
            Recursive = recursive,
            CleanOutput = cleanOutput,
            DryRun = dryRun,
            CopyMode = copyMode,
        };

        LatestVersionCopyResult result;
        try
        {
            result = new EsfLatestVersionPreprocessor().Run(options);
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"❌  {ex.Message}");
            return 2;
        }

        var reports = LatestSelectionReportWriter.Write(result, result.OutputDirectory);

        int copiedCount = result.Copied.Count(c => c.Status == CopyStatus.Copied);
        Console.WriteLine($"{(result.DryRun ? "DRY RUN — " : "")}Selected {result.Copied.Count} program(s); "
                          + $"copied {copiedCount}; ignored {result.IgnoredOlder.Count} older; "
                          + $"{result.DuplicateWarnings.Count} duplicate-latest; {result.Errors.Count} error(s).");
        foreach (var rep in reports) Console.WriteLine($"  report: {rep}");

        // Non-fatal per-file errors still succeed overall; surface them in the exit code only if everything failed.
        return 0;
    }

    private static bool ReadBool(string[] args, ref int i, string flag, out bool ok)
    {
        // Accept "--flag" (presence ⇒ true) or "--flag true|false".
        if (i + 1 < args.Length && bool.TryParse(args[i + 1], out var parsed))
        {
            i++;
            ok = true;
            return parsed;
        }
        // Bare flag, or followed by another option → treat as true.
        ok = true;
        return true;
    }

    private static int Fail(string message)
    {
        Console.Error.WriteLine(message);
        Console.Error.WriteLine();
        Console.Error.WriteLine(Usage);
        return 2;
    }
}
