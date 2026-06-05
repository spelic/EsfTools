using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace EsfParser.Analysis;

internal static class CoverageMarkdownWriter
{
    private static readonly Encoding Utf8NoBom = new UTF8Encoding(false);
    private const int TopN = 25;

    public static string WriteFullReport(PortfolioCoverageResult result, string outputDir)
    {
        var path = Path.Combine(outputDir, "migration-coverage-report.md");
        var p = result.Programs;
        var sb = new StringBuilder();

        sb.AppendLine("# ESF migration coverage report");
        sb.AppendLine();
        sb.AppendLine($"- Input: `{result.InputDirectory}`");
        sb.AppendLine($"- Programs analyzed: **{p.Count}**");
        sb.AppendLine($"- Generation: {(result.GenerateEnabled ? "on" : "off")} · Build: {(result.BuildEnabled ? "on" : "off")}");
        sb.AppendLine();

        // 1. Executive Summary
        sb.AppendLine("## 1. Executive Summary");
        sb.AppendLine();
        sb.AppendLine($"- Parse failed: {p.Count(x => x.ParseFailed)} · Model failed: {p.Count(x => x.ModelBuildFailed)} · Generation failed: {p.Count(x => x.GenerationFailed)}");
        sb.AppendLine($"- Functions skipped during generation (total): {p.Sum(x => x.GenerationSkippedFunctions)}");
        if (result.BuildEnabled)
            sb.AppendLine($"- Builds: passed {p.Count(x => x.Build.Status == BuildStatus.Succeeded)} · failed {p.Count(x => x.Build.Status == BuildStatus.Failed)} · timed out {p.Count(x => x.Build.Status == BuildStatus.TimedOut)}");
        sb.AppendLine($"- Avg unknown-statement %: {(p.Count > 0 ? p.Average(x => x.UnknownPercent) : 0):0.##}");
        sb.AppendLine();
        sb.AppendLine("| Risk level | Programs |");
        sb.AppendLine("|---|---:|");
        foreach (RiskLevel lvl in Enum.GetValues(typeof(RiskLevel)))
            sb.AppendLine($"| {lvl} | {p.Count(x => x.RiskLevel == lvl)} |");
        sb.AppendLine();

        // 2. Top Pilot Candidates
        sb.AppendLine("## 2. Top Pilot Candidates");
        sb.AppendLine();
        var pilots = p.Where(x => !x.ParseFailed && !x.ModelBuildFailed && !x.GenerationFailed
                                  && x.RecommendedAction.StartsWith("Pilot", StringComparison.Ordinal))
                      .OrderBy(x => x.RiskScore).ThenBy(x => x.ProgramName).Take(TopN).ToList();
        AppendProgramTable(sb, pilots);

        // 3. Highest Risk Programs
        sb.AppendLine("## 3. Highest Risk Programs");
        sb.AppendLine();
        AppendProgramTable(sb, p.OrderByDescending(x => x.RiskScore).ThenBy(x => x.ProgramName).Take(TopN).ToList());

        // 4. Status summary
        sb.AppendLine("## 4. Parse / Model / Generation / Build Status Summary");
        sb.AppendLine();
        sb.AppendLine("| Stage | OK | Failed |");
        sb.AppendLine("|---|---:|---:|");
        sb.AppendLine($"| Parse | {p.Count(x => !x.ParseFailed)} | {p.Count(x => x.ParseFailed)} |");
        sb.AppendLine($"| Model build | {p.Count(x => !x.ParseFailed && !x.ModelBuildFailed)} | {p.Count(x => x.ModelBuildFailed)} |");
        if (result.GenerateEnabled)
            sb.AppendLine($"| Generation | {p.Count(x => x.GeneratedProjectDir != null)} | {p.Count(x => x.GenerationFailed)} |");
        if (result.BuildEnabled)
            sb.AppendLine($"| Build | {p.Count(x => x.Build.Status == BuildStatus.Succeeded)} | {p.Count(x => x.Build.Status is BuildStatus.Failed or BuildStatus.TimedOut)} |");
        sb.AppendLine();

        // 5. Unsupported Statement Summary
        sb.AppendLine("## 5. Unsupported Statement Summary");
        sb.AppendLine();
        AppendFrequencyTable(sb, "Unknown statement text",
            p.SelectMany(x => x.DistinctUnknownTexts), TopN);

        // 6. Unsupported EZE Word Summary
        sb.AppendLine("## 6. Unsupported EZE Word Summary");
        sb.AppendLine();
        AppendFrequencyTable(sb, "EZE word", p.SelectMany(x => x.UnsupportedEzeWords), TopN);

        // 7. Unsupported SQL Option Summary
        sb.AppendLine("## 7. Unsupported SQL Option Summary");
        sb.AppendLine();
        AppendFrequencyTable(sb, "SQL option", p.SelectMany(x => x.UnsupportedSqlOptions), TopN);

        // 8. Feature Usage Summary
        sb.AppendLine("## 8. Feature Usage Summary");
        sb.AppendLine();
        sb.AppendLine("| Feature | Programs |");
        sb.AppendLine("|---|---:|");
        sb.AppendLine($"| CONVERSE | {p.Count(x => x.Features.UsesConverse)} |");
        sb.AppendLine($"| DXFR | {p.Count(x => x.Features.UsesDxfr)} |");
        sb.AppendLine($"| XFER | {p.Count(x => x.Features.UsesXfer)} |");
        sb.AppendLine($"| DISPLAY | {p.Count(x => x.Features.UsesDisplay)} |");
        sb.AppendLine($"| CALL | {p.Count(x => x.Features.UsesCall)} |");
        sb.AppendLine($"| MOVEA | {p.Count(x => x.Features.UsesMoveA)} |");
        sb.AppendLine($"| SQL | {p.Count(x => x.Features.UsesSql)} |");
        sb.AppendLine($"| SQL cursor flow | {p.Count(x => x.Features.UsesSqlCursorFlow)} |");
        sb.AppendLine($"| Packed/zoned decimals | {p.Count(x => x.Features.UsesPackedOrZoned)} |");
        sb.AppendLine($"| OCCURS arrays | {p.Count(x => x.Features.UsesOccursArrays)} |");
        sb.AppendLine($"| Redefined records | {p.Count(x => x.Features.UsesRedefinedRecords)} |");
        sb.AppendLine($"| Map edit routines | {p.Count(x => x.Features.UsesMapEditRoutines)} |");
        sb.AppendLine();

        // 9. Full Program Table
        sb.AppendLine("## 9. Full Program Table");
        sb.AppendLine();
        AppendProgramTable(sb, p);

        // 10. Build Failure Details
        sb.AppendLine("## 10. Build Failure Details");
        sb.AppendLine();
        var failed = p.Where(x => x.Build.Status is BuildStatus.Failed or BuildStatus.TimedOut).ToList();
        if (failed.Count == 0)
        {
            sb.AppendLine(result.BuildEnabled ? "_No build failures._" : "_Build phase not run._");
        }
        else
        {
            foreach (var f in failed)
            {
                sb.AppendLine($"- **{f.ProgramName}** — {f.Build.Status}, {f.Build.ErrorCount} error(s), {f.Build.WarningCount} warning(s). Log: `{f.Build.LogPath}`");
            }
        }
        sb.AppendLine();

        File.WriteAllText(path, sb.ToString(), Utf8NoBom);
        return path;
    }

    public static string WriteRiskSummary(PortfolioCoverageResult result, string outputDir)
    {
        var path = Path.Combine(outputDir, "migration-risk-summary.md");
        var p = result.Programs;
        var sb = new StringBuilder();

        sb.AppendLine("# ESF migration risk summary");
        sb.AppendLine();
        sb.AppendLine($"Programs: **{p.Count}**  ·  " +
                      string.Join("  ·  ", Enum.GetValues(typeof(RiskLevel)).Cast<RiskLevel>()
                          .Select(l => $"{l}: {p.Count(x => x.RiskLevel == l)}")));
        sb.AppendLine();

        sb.AppendLine("## Recommended actions (counts)");
        sb.AppendLine();
        sb.AppendLine("| Action | Programs |");
        sb.AppendLine("|---|---:|");
        foreach (var g in p.GroupBy(x => x.RecommendedAction).OrderByDescending(g => g.Count()))
            sb.AppendLine($"| {g.Key} | {g.Count()} |");
        sb.AppendLine();

        sb.AppendLine("## Top pilot candidates");
        sb.AppendLine();
        AppendProgramTable(sb, p.Where(x => x.RecommendedAction.StartsWith("Pilot", StringComparison.Ordinal))
                                .OrderBy(x => x.RiskScore).ThenBy(x => x.ProgramName).Take(TopN).ToList());

        sb.AppendLine("## Highest-risk programs");
        sb.AppendLine();
        AppendProgramTable(sb, p.OrderByDescending(x => x.RiskScore).Take(TopN).ToList());

        File.WriteAllText(path, sb.ToString(), Utf8NoBom);
        return path;
    }

    private static void AppendProgramTable(StringBuilder sb, IReadOnlyList<ProgramCoverageResult> rows)
    {
        if (rows.Count == 0) { sb.AppendLine("_None._"); sb.AppendLine(); return; }
        sb.AppendLine("| Program | Risk | Level | Action | Fns | Stmts | Unk% | Build |");
        sb.AppendLine("|---|---:|---|---|---:|---:|---:|---|");
        foreach (var r in rows)
            sb.AppendLine($"| {r.ProgramName} | {r.RiskScore} | {r.RiskLevel} | {r.RecommendedAction} | " +
                          $"{r.FunctionCount} | {r.TotalStatements} | {r.UnknownPercent:0.#} | {r.Build.Status} |");
        sb.AppendLine();
    }

    private static void AppendFrequencyTable(StringBuilder sb, string label, IEnumerable<string> items, int top)
    {
        var freq = items
            .GroupBy(t => t, StringComparer.OrdinalIgnoreCase)
            .Select(g => (Text: g.Key, Count: g.Count()))
            .OrderByDescending(x => x.Count).ThenBy(x => x.Text, StringComparer.OrdinalIgnoreCase)
            .ToList();

        if (freq.Count == 0) { sb.AppendLine("_None._"); sb.AppendLine(); return; }

        sb.AppendLine($"| {label} | Programs |");
        sb.AppendLine("|---|---:|");
        foreach (var (text, count) in freq.Take(top))
            sb.AppendLine($"| {Escape(text)} | {count} |");
        if (freq.Count > top) sb.AppendLine($"| _…and {freq.Count - top} more_ | |");
        sb.AppendLine();
    }

    private static string Escape(string s) => s.Replace("|", "\\|").Replace("\n", " ").Replace("\r", " ");
}
