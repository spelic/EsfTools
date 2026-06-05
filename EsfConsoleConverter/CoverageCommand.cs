using EsfParser.Analysis;
using System.Linq;

namespace EsfConsoleConverter;

/// <summary>
/// Verb: <c>coverage</c>. Analyzes a folder of (already-latest) ESF files for translation
/// coverage and migration risk, optionally generating + building a C# project per program.
/// </summary>
internal static class CoverageCommand
{
    private const string Usage = """
        Usage:
          coverage --input <folder> --output <folder>
                   [--recursive true|false]      (default true)
                   [--generate true|false]       (default true)
                   [--build true|false]          (default false)
                   [--limit N]                   (generate/build only first N after filter)
                   [--filter <glob>]             (e.g. NR11*)
                   [--max-parallel N]            (default CPU-2; build phase)
                   [--max-build-seconds N]       (default 60)
                   [--build-all]                 (required to build the whole portfolio)
        """;

    public static int Run(string[] args)
    {
        string? input = null, output = null, filter = null;
        bool recursive = true, generate = true, build = false, buildAll = false;
        int limit = 0, maxParallel = System.Math.Max(1, System.Environment.ProcessorCount - 2), maxBuildSeconds = 60;

        for (int i = 0; i < args.Length; i++)
        {
            string a = args[i];
            switch (a)
            {
                case "--input" when i + 1 < args.Length: input = args[++i]; break;
                case "--output" when i + 1 < args.Length: output = args[++i]; break;
                case "--filter" when i + 1 < args.Length: filter = args[++i]; break;
                case "--limit" when i + 1 < args.Length: if (!int.TryParse(args[++i], out limit)) return Fail("--limit must be an integer."); break;
                case "--max-parallel" when i + 1 < args.Length: if (!int.TryParse(args[++i], out maxParallel)) return Fail("--max-parallel must be an integer."); break;
                case "--max-build-seconds" when i + 1 < args.Length: if (!int.TryParse(args[++i], out maxBuildSeconds)) return Fail("--max-build-seconds must be an integer."); break;
                case "--recursive": recursive = ReadBool(args, ref i); break;
                case "--generate": generate = ReadBool(args, ref i); break;
                case "--build": build = ReadBool(args, ref i); break;
                case "--build-all": buildAll = ReadBool(args, ref i); break;
                default: return Fail($"Unknown or incomplete option: '{a}'.");
            }
        }

        if (string.IsNullOrWhiteSpace(input)) return Fail("--input is required.");
        if (string.IsNullOrWhiteSpace(output)) return Fail("--output is required.");

        // Guard: building the whole portfolio (no limit/filter) is hours + huge disk.
        if (build && limit <= 0 && string.IsNullOrWhiteSpace(filter) && !buildAll)
            return Fail("--build with no --limit/--filter builds the ENTIRE portfolio (hours, 100s of GB). " +
                        "Re-run with --limit/--filter, or pass --build-all to confirm.");

        var options = new PortfolioCoverageOptions
        {
            InputDirectory = input!,
            OutputDirectory = output!,
            Recursive = recursive,
            Generate = generate || build,   // build implies generate
            Build = build,
            Limit = limit,
            Filter = filter,
            MaxParallel = maxParallel,
            MaxBuildSeconds = maxBuildSeconds,
            BuildAll = buildAll,
        };

        PortfolioCoverageResult result;
        try
        {
            result = new PortfolioCoverageAnalyzer().Analyze(options);
        }
        catch (System.Exception ex)
        {
            System.Console.Error.WriteLine($"❌  {ex.Message}");
            return 2;
        }

        var reports = CoverageReportWriter.WriteAll(result, result.OutputDirectory);

        var p = result.Programs;
        System.Console.WriteLine();
        System.Console.WriteLine($"Analyzed {p.Count} program(s). " +
            $"Parse-fail {p.Count(x => x.ParseFailed)}, model-fail {p.Count(x => x.ModelBuildFailed)}, " +
            $"LOW {p.Count(x => x.RiskLevel == RiskLevel.Low)}, MEDIUM {p.Count(x => x.RiskLevel == RiskLevel.Medium)}, " +
            $"HIGH {p.Count(x => x.RiskLevel == RiskLevel.High)}, BLOCKED {p.Count(x => x.RiskLevel == RiskLevel.Blocked)}.");
        foreach (var rep in reports) System.Console.WriteLine($"  report: {rep}");
        return 0;
    }

    private static bool ReadBool(string[] args, ref int i)
    {
        if (i + 1 < args.Length && bool.TryParse(args[i + 1], out var parsed)) { i++; return parsed; }
        return true; // bare flag ⇒ true
    }

    private static int Fail(string message)
    {
        System.Console.Error.WriteLine(message);
        System.Console.Error.WriteLine();
        System.Console.Error.WriteLine(Usage);
        return 2;
    }
}
