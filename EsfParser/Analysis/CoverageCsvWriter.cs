using System.IO;
using System.Linq;
using System.Text;

namespace EsfParser.Analysis;

internal static class CoverageCsvWriter
{
    private static readonly Encoding Utf8NoBom = new UTF8Encoding(false);

    public static string Write(PortfolioCoverageResult result, string outputDir)
    {
        var path = Path.Combine(outputDir, "migration-coverage-report.csv");
        var sb = new StringBuilder();
        sb.AppendLine(string.Join(",",
            "program", "file", "riskScore", "riskLevel", "action",
            "parseFailed", "modelFailed", "genFailed", "genSkipped",
            "buildStatus", "buildErrors", "buildWarnings",
            "functions", "maps", "records", "items", "tables",
            "totalStmts", "translatableStmts", "unknownStmts", "unknownPct",
            "unsupportedEzeCount", "unsupportedSqlOptions",
            "converse", "dxfr", "xfer", "display", "call", "moveA",
            "sql", "cursorFlow", "packedZoned", "occurs", "redefrec", "mapEdits"));

        foreach (var r in result.Programs)
        {
            var fx = r.Features;
            sb.AppendLine(string.Join(",", new[]
            {
                C(r.ProgramName), C(r.FileName), r.RiskScore.ToString(), r.RiskLevel.ToString(), C(r.RecommendedAction),
                B(r.ParseFailed), B(r.ModelBuildFailed), B(r.GenerationFailed), r.GenerationSkippedFunctions.ToString(),
                r.Build.Status.ToString(), r.Build.ErrorCount.ToString(), r.Build.WarningCount.ToString(),
                r.FunctionCount.ToString(), r.MapCount.ToString(), r.RecordCount.ToString(), r.ItemCount.ToString(), r.TableCount.ToString(),
                r.TotalStatements.ToString(), r.TranslatableStatements.ToString(), r.UnknownStatements.ToString(), r.UnknownPercent.ToString("0.##"),
                r.UnsupportedEzeWords.Count.ToString(), C(string.Join(" ", r.UnsupportedSqlOptions)),
                B(fx.UsesConverse), B(fx.UsesDxfr), B(fx.UsesXfer), B(fx.UsesDisplay), B(fx.UsesCall), B(fx.UsesMoveA),
                B(fx.UsesSql), B(fx.UsesSqlCursorFlow), B(fx.UsesPackedOrZoned), B(fx.UsesOccursArrays), B(fx.UsesRedefinedRecords), B(fx.UsesMapEditRoutines),
            }));
        }

        File.WriteAllText(path, sb.ToString(), Utf8NoBom);
        return path;
    }

    private static string B(bool b) => b ? "1" : "0";

    private static string C(string? field)
    {
        field ??= string.Empty;
        if (field.Contains(',') || field.Contains('"') || field.Contains('\n'))
            return "\"" + field.Replace("\"", "\"\"") + "\"";
        return field;
    }
}
