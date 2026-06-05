using EsfParser.Analysis;
using Xunit;

namespace EsfParser.Tests;

public class RiskScoreCalculatorTests
{
    private static RiskInputs Clean() => new(
        ParseFailed: false, ModelBuildFailed: false, GenerationFailed: false, BuildFailed: false,
        UnknownPercent: 0,
        HasUnsupportedEze: false, HasUnsupportedSql: false,
        UsesConverse: false, UsesDxfrOrXfer: false, UsesMoveA: false,
        UsesPackedOrZoned: false, UsesOccurs: false, UsesRedefinedRecords: false, UsesSqlCursorFlow: false);

    [Fact]
    public void RiskScoreCalculator_ShouldReturnLowRiskForCleanProgram()
    {
        int score = RiskScoreCalculator.Score(Clean());
        Assert.Equal(0, score);
        Assert.Equal(RiskLevel.Low, RiskScoreCalculator.Level(score));
    }

    [Fact]
    public void RiskScoreCalculator_ShouldReturnBlockedForParseFailure()
    {
        var inputs = Clean() with { ParseFailed = true };
        int score = RiskScoreCalculator.Score(inputs);
        Assert.Equal(100, score);
        Assert.Equal(RiskLevel.Blocked, RiskScoreCalculator.Level(score));
    }

    [Fact]
    public void RiskScoreCalculator_ShouldCapAt100()
    {
        // Pile on every signal; raw sum far exceeds 100.
        var inputs = new RiskInputs(
            ParseFailed: true, ModelBuildFailed: true, GenerationFailed: true, BuildFailed: true,
            UnknownPercent: 99,
            HasUnsupportedEze: true, HasUnsupportedSql: true,
            UsesConverse: true, UsesDxfrOrXfer: true, UsesMoveA: true,
            UsesPackedOrZoned: true, UsesOccurs: true, UsesRedefinedRecords: true, UsesSqlCursorFlow: true);

        Assert.Equal(100, RiskScoreCalculator.Score(inputs));
    }

    [Fact]
    public void RiskScoreCalculator_LevelsMapToBands()
    {
        Assert.Equal(RiskLevel.Low, RiskScoreCalculator.Level(20));
        Assert.Equal(RiskLevel.Medium, RiskScoreCalculator.Level(21));
        Assert.Equal(RiskLevel.Medium, RiskScoreCalculator.Level(50));
        Assert.Equal(RiskLevel.High, RiskScoreCalculator.Level(51));
        Assert.Equal(RiskLevel.High, RiskScoreCalculator.Level(79));
        Assert.Equal(RiskLevel.Blocked, RiskScoreCalculator.Level(80));
    }
}

public sealed class PortfolioCoverageAnalyzerTests : IDisposable
{
    private readonly string _root;
    private readonly string _input;
    private readonly string _output;

    public PortfolioCoverageAnalyzerTests()
    {
        _root = Path.Combine(Path.GetTempPath(), "esf_cov_" + Guid.NewGuid().ToString("N"));
        _input = Path.Combine(_root, "in");
        _output = Path.Combine(_root, "out");
        Directory.CreateDirectory(_input);

        // A real, well-formed sample (copied into the test output as Samples\*.esf).
        File.Copy(SampleData.Path_NR11, Path.Combine(_input, "NR11av28.esf"));
        // A deliberately broken file (binary-ish garbage that the parser cannot handle as tags).
        File.WriteAllText(Path.Combine(_input, "BROKEN-V1.esf"), ":program name = X\n\x01\x02 not valid \n");
    }

    public void Dispose()
    {
        if (Directory.Exists(_root)) Directory.Delete(_root, recursive: true);
    }

    private PortfolioCoverageResult Analyze(bool generate = false) =>
        new PortfolioCoverageAnalyzer().Analyze(new PortfolioCoverageOptions
        {
            InputDirectory = _input,
            OutputDirectory = _output,
            Recursive = true,
            Generate = generate,
            Build = false,
        });

    [Fact]
    public void PortfolioCoverageAnalyzer_ShouldContinueAfterFailedFile()
    {
        var result = Analyze();
        // Both files present; the analyzer did not throw despite the broken file.
        Assert.Equal(2, result.Programs.Count);
        Assert.Contains(result.Programs, p => p.ProgramName.Contains("NR11", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void PortfolioCoverageAnalyzer_ShouldIncludeEveryInputFileInReport()
    {
        var result = Analyze();
        var names = result.Programs.Select(p => p.FileName).OrderBy(x => x).ToArray();
        Assert.Equal(new[] { "BROKEN-V1.esf", "NR11av28.esf" }, names);
    }

    [Fact]
    public void PortfolioCoverageAnalyzer_ShouldComputeCountsForGoodProgram()
    {
        var result = Analyze();
        var nr11 = result.Programs.First(p => p.FileName == "NR11av28.esf");

        Assert.False(nr11.ParseFailed);
        Assert.False(nr11.ModelBuildFailed);
        Assert.True(nr11.FunctionCount > 0);
        Assert.True(nr11.TotalStatements > 0);
    }

    [Fact]
    public void CoverageReportWriter_ShouldWriteMarkdownCsvJson()
    {
        var result = Analyze();
        var reports = CoverageReportWriter.WriteAll(result, _output);

        Assert.True(File.Exists(Path.Combine(_output, "migration-coverage-report.md")));
        Assert.True(File.Exists(Path.Combine(_output, "migration-coverage-report.csv")));
        Assert.True(File.Exists(Path.Combine(_output, "migration-coverage-report.json")));
        Assert.True(File.Exists(Path.Combine(_output, "migration-risk-summary.md")));
        Assert.Equal(4, reports.Count);
    }
}
