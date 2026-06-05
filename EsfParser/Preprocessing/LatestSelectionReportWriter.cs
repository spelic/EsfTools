using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace EsfParser.Preprocessing;

/// <summary>
/// Writes the latest-version selection result as Markdown, CSV and JSON reports
/// (all UTF-8, no BOM) into the output directory.
/// </summary>
public static class LatestSelectionReportWriter
{
    public const string MarkdownFile = "_latest-selection-report.md";
    public const string CsvFile = "_latest-selection-report.csv";
    public const string JsonFile = "_latest-selection-report.json";

    private static readonly Encoding Utf8NoBom = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false);

    private static readonly JsonSerializerOptions JsonOpts = new()
    {
        WriteIndented = true,
        Converters = { new System.Text.Json.Serialization.JsonStringEnumConverter() },
    };

    public static IReadOnlyList<string> Write(LatestVersionCopyResult result, string outputDir)
    {
        Directory.CreateDirectory(outputDir);

        var md = Path.Combine(outputDir, MarkdownFile);
        var csv = Path.Combine(outputDir, CsvFile);
        var json = Path.Combine(outputDir, JsonFile);

        File.WriteAllText(md, BuildMarkdown(result), Utf8NoBom);
        File.WriteAllText(csv, BuildCsv(result), Utf8NoBom);
        File.WriteAllText(json, JsonSerializer.Serialize(result, JsonOpts), Utf8NoBom);

        return new[] { md, csv, json };
    }

    private static string BuildMarkdown(LatestVersionCopyResult r)
    {
        var sb = new StringBuilder();
        sb.AppendLine("# ESF latest-version selection report");
        sb.AppendLine();
        sb.AppendLine($"- Input: `{r.InputDirectory}`");
        sb.AppendLine($"- Output: `{r.OutputDirectory}`");
        sb.AppendLine($"- Mode: {(r.DryRun ? "DRY RUN (no files copied)" : "copy")}");
        sb.AppendLine($"- Selected: {r.Copied.Count} | Ignored older: {r.IgnoredOlder.Count} | "
                      + $"Duplicate-latest: {r.DuplicateWarnings.Count} | Errors: {r.Errors.Count}");
        sb.AppendLine();

        sb.AppendLine("## Selected (latest per program)");
        sb.AppendLine();
        sb.AppendLine("| Program | Version | Status | Source | Target |");
        sb.AppendLine("|---|---:|---|---|---|");
        foreach (var c in r.Copied.OrderBy(c => c.ProgramName, StringComparer.OrdinalIgnoreCase))
            sb.AppendLine($"| {c.ProgramName} | {c.Version} | {c.Status} | `{c.SourcePath}` | `{c.TargetPath}` |");
        sb.AppendLine();

        if (r.IgnoredOlder.Count > 0)
        {
            sb.AppendLine("## Ignored older versions");
            sb.AppendLine();
            sb.AppendLine("| Program | Version | File |");
            sb.AppendLine("|---|---:|---|");
            foreach (var i in r.IgnoredOlder
                         .OrderBy(i => i.ProgramName, StringComparer.OrdinalIgnoreCase)
                         .ThenByDescending(i => i.Version))
                sb.AppendLine($"| {i.ProgramName} | {i.Version} | `{i.FullPath}` |");
            sb.AppendLine();
        }

        if (r.DuplicateWarnings.Count > 0)
        {
            sb.AppendLine("## Duplicate-latest warnings");
            sb.AppendLine();
            foreach (var d in r.DuplicateWarnings)
            {
                sb.AppendLine($"- **{d.ProgramName}** v{d.Version}: {d.Paths.Count} files at the same highest version");
                foreach (var p in d.Paths) sb.AppendLine($"  - `{p}`");
            }
            sb.AppendLine();
        }

        if (r.Errors.Count > 0)
        {
            sb.AppendLine("## Errors");
            sb.AppendLine();
            foreach (var e in r.Errors) sb.AppendLine($"- {e}");
            sb.AppendLine();
        }

        return sb.ToString();
    }

    private static string BuildCsv(LatestVersionCopyResult r)
    {
        var sb = new StringBuilder();
        sb.AppendLine("category,program,version,status,source,target");

        foreach (var c in r.Copied)
            sb.AppendLine(Row("selected", c.ProgramName, c.Version, c.Status.ToString(), c.SourcePath, c.TargetPath));

        foreach (var i in r.IgnoredOlder)
            sb.AppendLine(Row("ignored-older", i.ProgramName, i.Version, "Ignored", i.FullPath, ""));

        foreach (var d in r.DuplicateWarnings)
            foreach (var p in d.Paths)
                sb.AppendLine(Row("duplicate-latest", d.ProgramName, d.Version, "Duplicate", p, ""));

        return sb.ToString();
    }

    private static string Row(string category, string program, long version, string status, string source, string target)
        => string.Join(",", new[] { category, program, version.ToString(), status, source, target }.Select(Csv));

    private static string Csv(string field)
    {
        field ??= string.Empty;
        if (field.Contains(',') || field.Contains('"') || field.Contains('\n'))
            return "\"" + field.Replace("\"", "\"\"") + "\"";
        return field;
    }
}
