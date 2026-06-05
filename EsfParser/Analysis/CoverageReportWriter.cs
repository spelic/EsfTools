using System.Collections.Generic;
using System.IO;

namespace EsfParser.Analysis;

/// <summary>Writes all coverage reports (markdown, csv, json, risk summary) to the output dir.</summary>
public static class CoverageReportWriter
{
    public static IReadOnlyList<string> WriteAll(PortfolioCoverageResult result, string outputDir)
    {
        Directory.CreateDirectory(outputDir);
        return new[]
        {
            CoverageMarkdownWriter.WriteFullReport(result, outputDir),
            CoverageMarkdownWriter.WriteRiskSummary(result, outputDir),
            CoverageCsvWriter.Write(result, outputDir),
            CoverageJsonWriter.Write(result, outputDir),
        };
    }
}
