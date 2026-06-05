using EsfParser.Analytics;
using EsfParser.Builder;
using EsfParser.CodeGen;
using EsfParser.Esf;
using EsfParser.Parser;
using EsfParser.Parser.Logic.Statements;
using EsfParser.Tags;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace EsfParser.Analysis;

/// <summary>
/// Scans a folder of (already-latest) ESF files and produces a portfolio coverage/risk result.
/// Per-file failures are isolated (every file appears in the result). Parse + generation run
/// sequentially (generation mutates the global <see cref="CSharpUtils.Program"/>); builds run
/// in a bounded-parallel phase afterwards.
/// </summary>
public sealed class PortfolioCoverageAnalyzer
{
    private static readonly ItemType[] PackedZonedTypes = { ItemType.PACK, ItemType.PACF, ItemType.NUM, ItemType.NUMC };
    // EZE/EY token NOT preceded by '.' — a leading dot means it's a qualified field
    // access (e.g. MAP.EZEMSG), which is a normal field, not a system function.
    private static readonly Regex EzeToken = new(@"(?<![.\w])E[ZY]E[A-Z0-9]+\b", RegexOptions.Compiled | RegexOptions.IgnoreCase);
    private static readonly Regex XferWord = new(@"\bXFER\b", RegexOptions.Compiled | RegexOptions.IgnoreCase);
    private static readonly Regex DisplayWord = new(@"\bDISPLAY\b", RegexOptions.Compiled | RegexOptions.IgnoreCase);

    private static readonly HashSet<string> ImplementedEze = BuildImplementedEzeSet();

    public PortfolioCoverageResult Analyze(PortfolioCoverageOptions options)
    {
        if (options is null) throw new ArgumentNullException(nameof(options));
        var input = Path.GetFullPath(options.InputDirectory);
        if (!Directory.Exists(input)) throw new DirectoryNotFoundException($"Input directory not found: {input}");

        var output = Path.GetFullPath(options.OutputDirectory);
        Directory.CreateDirectory(output);

        var realOut = Console.Out;

        var files = Directory
            .EnumerateFiles(input, "*.esf", options.Recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly)
            .OrderBy(f => f, StringComparer.OrdinalIgnoreCase)
            .ToList();

        // Which files are eligible for generate/build (filter + limit gate only this phase).
        var selectedForGen = SelectForGeneration(files, options);

        var usedNames = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        var results = new List<ProgramCoverageResult>(files.Count);

        int index = 0;
        foreach (var file in files)
        {
            index++;
            var programName = UniqueProgramName(file, usedNames);
            bool generateThis = options.Generate && selectedForGen.Contains(file);

            var r = AnalyzeOne(file, programName, output, generateThis);
            results.Add(r);

            realOut.WriteLine($"[{index}/{files.Count}] {programName}  " +
                              $"stmts={r.TotalStatements} unk={r.UnknownStatements} " +
                              $"({r.UnknownPercent:0.#}%)  " +
                              (r.ParseFailed ? "PARSE-FAIL" : r.ModelBuildFailed ? "MODEL-FAIL" : "ok"));
        }

        // Build phase (bounded parallel). Only generated, non-failed, selected programs.
        if (options.Build)
        {
            var buildable = results.Where(r => r.GeneratedProjectDir != null).ToList();
            realOut.WriteLine($"Building {buildable.Count} project(s) (max-parallel={options.MaxParallel}, timeout={options.MaxBuildSeconds}s)...");
            var po = new ParallelOptions { MaxDegreeOfParallelism = Math.Max(1, options.MaxParallel) };
            var builder = new GeneratedProjectBuilder();
            Parallel.ForEach(buildable, po, r =>
            {
                var logPath = Path.Combine(output, "build-logs", r.ProgramName + ".build.log");
                r.Build = builder.Build(r.GeneratedProjectDir!, logPath, options.MaxBuildSeconds);
            });
        }

        // Final risk pass (needs build results).
        foreach (var r in results) ScoreRisk(r);

        return new PortfolioCoverageResult
        {
            InputDirectory = input,
            OutputDirectory = output,
            GenerateEnabled = options.Generate,
            BuildEnabled = options.Build,
            Programs = results.OrderByDescending(r => r.RiskScore).ThenBy(r => r.ProgramName, StringComparer.OrdinalIgnoreCase).ToList(),
        };
    }

    private ProgramCoverageResult AnalyzeOne(string file, string programName, string output, bool generate)
    {
        var r = new ProgramCoverageResult
        {
            FileName = Path.GetFileName(file),
            FilePath = file,
            ProgramName = programName,
        };

        var buffer = new StringWriter();
        var realOut = Console.Out;
        Console.SetOut(buffer);
        try
        {
            // ── Stage 1: parse ──
            List<TagNode> nodes;
            try
            {
                var lines = File.ReadAllLines(file, Encoding.GetEncoding(1250));
                nodes = MyEsfParser.Parse(lines);
            }
            catch (Exception ex)
            {
                r.ParseFailed = true;
                r.FailureMessage = ex.Message;
                return r;
            }

            // ── Stage 2: build model ──
            EsfProgram program;
            try
            {
                program = EsfProgramBuilder.GenerateEsfProgram(nodes);
            }
            catch (Exception ex)
            {
                r.ModelBuildFailed = true;
                r.FailureMessage = ex.Message;
                return r;
            }

            // ── Stage 3: counts + findings ──
            PopulateCounts(r, program);
            PopulateUnsupported(r, program);
            PopulateFeatures(r, program);

            // ── Stage 4: generation (optional) ──
            if (generate)
            {
                var genDir = Path.Combine(output, "generated", programName);
                var diags = new List<string>();
                try
                {
                    CSharpUtils.Program = program;
                    RoslynExporter.WriteProjectFiles(program, genDir, programName + "_ConsoleApp", default, diags);
                    r.GenerationSkippedFunctions = diags.Count;
                    r.GeneratedProjectDir = genDir;
                }
                catch (Exception ex)
                {
                    r.GenerationFailed = true;
                    r.FailureMessage ??= ex.Message;
                }
            }

            return r;
        }
        finally
        {
            Console.SetOut(realOut);
            var diag = MyEsfParser.Diagnostics.Count > 0
                ? "Parser diagnostics:\n  " + string.Join("\n  ", MyEsfParser.Diagnostics) + "\n"
                : "";
            r.ConsoleOutput = diag + buffer.ToString();
        }
    }

    private static void PopulateCounts(ProgramCoverageResult r, EsfProgram program)
    {
        r.FunctionCount = program.Functions.Functions.Count;
        r.MapCount = program.Maps.Maps.Count;
        r.RecordCount = program.Records.Records.Count;
        r.ItemCount = program.Items.Items.Count;
        r.TableCount = program.Tables.Tables.Count;

        var all = EsfProgramAnalytics.GetAllStatementsRecursive(program);
        r.TotalStatements = all.Count;
        r.UnknownStatements = all.Count(s => s.Type == StatementType.Unknown);
        int structural = all.Count(s => s.Type is StatementType.Comment or StatementType.Else or StatementType.End);
        r.TranslatableStatements = r.TotalStatements - structural;
        r.UnknownPercent = r.TranslatableStatements > 0
            ? Math.Round(100.0 * r.UnknownStatements / r.TranslatableStatements, 2)
            : 0;

        r.DistinctUnknownTexts = all
            .Where(s => s.Type == StatementType.Unknown)
            .Select(s => (s.OriginalCode ?? "").Trim())
            .Where(t => t.Length > 0)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(t => t, StringComparer.OrdinalIgnoreCase)
            .ToList();

        foreach (var f in program.Functions.Functions)
        {
            var fAll = EsfProgramAnalytics.GetAllStatementsRecursive(f);
            r.Functions.Add(new FunctionCoverageResult
            {
                FunctionName = f.Name ?? "",
                Option = f.Option ?? "",
                TotalStatements = fAll.Count,
                UnknownStatements = fAll.Count(s => s.Type == StatementType.Unknown),
            });
        }
    }

    private static void PopulateUnsupported(ProgramCoverageResult r, EsfProgram program)
    {
        var all = EsfProgramAnalytics.GetAllStatementsRecursive(program);

        // Names the program defines itself (items + record fields). An EZE* token that is a
        // declared field (e.g. an "EZEMSG" message field on a map record) is handled by the
        // normal field path, not the EZE runtime — so it must not count as "unsupported EZE".
        var definedNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var i in program.Items.Items) definedNames.Add(i.Name.ToUpperInvariant());
        foreach (var rec in program.Records.Records)
            foreach (var it in rec.Items) definedNames.Add(it.Name.ToUpperInvariant());

        var ezeTokens = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var s in all)
        {
            if (s is SystemFunctionStatement sf && !string.IsNullOrWhiteSpace(sf.Name))
                ezeTokens.Add(Normalize(sf.Name));
            foreach (Match m in EzeToken.Matches(s.OriginalCode ?? ""))
                ezeTokens.Add(Normalize(m.Value));
        }
        r.UnsupportedEzeWords = ezeTokens
            .Where(t => !ImplementedEze.Contains(t) && !definedNames.Contains(t))
            .OrderBy(t => t, StringComparer.OrdinalIgnoreCase)
            .ToList();

        r.UnsupportedSqlOptions = program.Functions.Functions
            .Select(f => f.Option)
            .Where(SqlSupport.IsUnsupported)
            .Select(o => o!.ToUpperInvariant())
            .Distinct()
            .OrderBy(o => o, StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    private static void PopulateFeatures(ProgramCoverageResult r, EsfProgram program)
    {
        var fx = r.Features;
        var funcs = program.Functions.Functions;
        var all = EsfProgramAnalytics.GetAllStatementsRecursive(program);

        fx.UsesConverse = funcs.Any(f => string.Equals(f.Option, "CONVERSE", StringComparison.OrdinalIgnoreCase));
        fx.UsesSqlCursorFlow = funcs.Any(f => f.Option != null && SqlSupport.CursorFlowOptions.Contains(f.Option));
        fx.UsesSql = funcs.Any(f => f.SqlClauses.Count > 0
                                    || (f.Option != null && SqlSupport.SupportedSqlOptions.Contains(f.Option)));

        fx.UsesDxfr = all.Any(s => s.Type == StatementType.Dxfr);
        fx.UsesCall = all.Any(s => s.Type == StatementType.Call);
        fx.UsesMoveA = all.Any(s => s.Type == StatementType.MoveA);
        fx.UsesXfer = all.Any(s => XferWord.IsMatch(s.OriginalCode ?? ""));
        fx.UsesDisplay = all.Any(s => DisplayWord.IsMatch(s.OriginalCode ?? ""));

        fx.UsesPackedOrZoned =
            program.Items.Items.Any(i => PackedZonedTypes.Contains(i.Type))
            || program.Records.Records.SelectMany(rec => rec.Items)
                   .Any(it => PackedZonedTypes.Any(t => string.Equals(it.Type, t.ToString(), StringComparison.OrdinalIgnoreCase)));

        fx.UsesOccursArrays = program.Records.Records.SelectMany(rec => rec.Items)
            .Any(it => int.TryParse(it.Occurs, out var n) && n > 1);

        fx.UsesRedefinedRecords = program.Records.Records
            .Any(rec => string.Equals(rec.Org, "REDEFREC", StringComparison.OrdinalIgnoreCase));

        fx.UsesMapEditRoutines = program.Items.Items
            .Any(i => i.MapEdits != null && !string.IsNullOrWhiteSpace(i.MapEdits.EditRtn));
    }

    private static void ScoreRisk(ProgramCoverageResult r)
    {
        bool buildFailed = r.Build.Status is BuildStatus.Failed or BuildStatus.TimedOut;
        var inputs = new RiskInputs(
            ParseFailed: r.ParseFailed,
            ModelBuildFailed: r.ModelBuildFailed,
            GenerationFailed: r.GenerationFailed,
            BuildFailed: buildFailed,
            UnknownPercent: r.UnknownPercent,
            HasUnsupportedEze: r.UnsupportedEzeWords.Count > 0,
            HasUnsupportedSql: r.UnsupportedSqlOptions.Count > 0,
            UsesConverse: r.Features.UsesConverse,
            UsesDxfrOrXfer: r.Features.UsesDxfr || r.Features.UsesXfer,
            UsesMoveA: r.Features.UsesMoveA,
            UsesPackedOrZoned: r.Features.UsesPackedOrZoned,
            UsesOccurs: r.Features.UsesOccursArrays,
            UsesRedefinedRecords: r.Features.UsesRedefinedRecords,
            UsesSqlCursorFlow: r.Features.UsesSqlCursorFlow);

        r.RiskScore = RiskScoreCalculator.Score(inputs);
        r.RiskLevel = RiskScoreCalculator.Level(r.RiskScore);
        r.RecommendedAction = RiskScoreCalculator.RecommendAction(inputs, r.Build.Status, r.RiskScore, r.UnknownStatements > 0);
    }

    // ── helpers ──────────────────────────────────────────────────────────

    private static HashSet<string> SelectForGeneration(List<string> files, PortfolioCoverageOptions o)
    {
        IEnumerable<string> q = files;
        if (!string.IsNullOrWhiteSpace(o.Filter))
        {
            var rx = GlobToRegex(o.Filter!);
            q = q.Where(f => rx.IsMatch(Path.GetFileNameWithoutExtension(f)));
        }
        if (o.Limit > 0) q = q.Take(o.Limit);
        return new HashSet<string>(q, StringComparer.OrdinalIgnoreCase);
    }

    private static Regex GlobToRegex(string glob)
    {
        var pattern = "^" + Regex.Escape(glob).Replace("\\*", ".*").Replace("\\?", ".") + "$";
        return new Regex(pattern, RegexOptions.IgnoreCase | RegexOptions.Compiled);
    }

    private static string UniqueProgramName(string file, Dictionary<string, int> used)
    {
        var baseName = Path.GetFileNameWithoutExtension(file).Replace("-", "_").ToUpperInvariant();
        if (used.TryGetValue(baseName, out var n))
        {
            used[baseName] = n + 1;
            return $"{baseName}_{n + 1}";
        }
        used[baseName] = 1;
        return baseName;
    }

    private static string Normalize(string token)
    {
        var t = token.Trim().ToUpperInvariant();
        if (t.StartsWith("EY", StringComparison.Ordinal)) t = "EZE" + t.Substring(2);
        return t;
    }

    private static HashSet<string> BuildImplementedEzeSet()
    {
        var t = typeof(EzFunctions);
        var set = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        const BindingFlags F = BindingFlags.Public | BindingFlags.Static;
        foreach (var p in t.GetProperties(F)) set.Add(p.Name.ToUpperInvariant());
        foreach (var fi in t.GetFields(F)) set.Add(fi.Name.ToUpperInvariant());
        foreach (var m in t.GetMethods(F).Where(m => !m.IsSpecialName)) set.Add(m.Name.ToUpperInvariant());
        foreach (var nt in t.GetNestedTypes(BindingFlags.Public)) set.Add(nt.Name.ToUpperInvariant());
        return set;
    }
}
