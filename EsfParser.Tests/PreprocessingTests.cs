using EsfParser.Preprocessing;
using Xunit;

namespace EsfParser.Tests;

public class LatestVersionSelectorTests
{
    private static LatestVersionSelectionResult Select(params string[] names) =>
        EsfLatestVersionSelector.Select(names);

    [Fact]
    public void SelectLatest_ShouldPickHighestNumericVersion()
    {
        var r = Select("NR11A-V1.esf", "NR11A-V3.esf", "NR11A-V2.esf");

        var picked = Assert.Single(r.Selected);
        Assert.Equal("NR11A", picked.ProgramName);
        Assert.Equal(3, picked.Version);
    }

    [Fact]
    public void SelectLatest_ShouldTreatMissingVersionAsZero()
    {
        // Plain name and a strict-mismatch (hyphen-less) name are both version 0.
        Assert.Equal(0, EsfFileVersionInfo.Parse("IS00.esf").Version);
        Assert.False(EsfFileVersionInfo.Parse("IS00.esf").HasExplicitVersion);
        Assert.Equal(0, EsfFileVersionInfo.Parse("IN72AV71.esf").Version);
        Assert.False(EsfFileVersionInfo.Parse("IN72AV71.esf").HasExplicitVersion);

        // A v0 plain file loses to an explicit -V1 of the same program.
        var r = Select("IS00.esf", "IS00-V1.esf");
        var picked = Assert.Single(r.Selected);
        Assert.Equal(1, picked.Version);
    }

    [Fact]
    public void SelectLatest_ShouldSupportLowercaseV()
    {
        var info = EsfFileVersionInfo.Parse("NR11A-v10.esf");
        Assert.Equal("NR11A", info.ProgramName);
        Assert.Equal(10, info.Version);
        Assert.True(info.HasExplicitVersion);
    }

    [Fact]
    public void SelectLatest_ShouldSupportProgramNamesWithHyphen()
    {
        var info = EsfFileVersionInfo.Parse("AB-CD-EF-V7.esf");
        Assert.Equal("AB-CD-EF", info.ProgramName);
        Assert.Equal(7, info.Version);
    }

    [Fact]
    public void SelectLatest_ShouldCompareVersionsNumerically()
    {
        // Lexicographic ordering would wrongly pick V9 over V10.
        var r = Select("NA70A-V9.esf", "NA70A-V10.esf");

        var picked = Assert.Single(r.Selected);
        Assert.Equal(10, picked.Version);
    }

    [Fact]
    public void SelectLatest_ShouldIgnoreOlderVersions()
    {
        var r = Select("D133A-V1.esf", "D133A-V2.esf", "D133A-V68.esf");

        Assert.Single(r.Selected);
        Assert.Equal(2, r.Ignored.Count);
        Assert.All(r.Ignored, i => Assert.NotEqual(68, i.Version));
    }

    [Fact]
    public void SelectLatest_ShouldGroupCaseInsensitively()
    {
        var r = Select("is00a-V1.esf", "IS00A-V2.esf");
        Assert.Single(r.Selected);
        Assert.Equal(2, r.Selected[0].Version);
    }

    [Fact]
    public void SelectLatest_ShouldReportDuplicateLatestVersionWarning()
    {
        // Same program, same highest version, two different paths.
        var r = Select(
            Path.Combine("dirB", "IS00A-V2.esf"),
            Path.Combine("dirA", "IS00A-V2.esf"));

        Assert.Single(r.Selected);
        var warn = Assert.Single(r.DuplicateWarnings);
        Assert.Equal("IS00A", warn.ProgramName);
        Assert.Equal(2, warn.Version);
        Assert.Equal(2, warn.Paths.Count);
        // Deterministic tie-break: ordinal-first path wins (dirA before dirB).
        Assert.Equal(Path.Combine("dirA", "IS00A-V2.esf"), r.Selected[0].FullPath);
        Assert.Single(r.Ignored);
    }
}

public sealed class LatestVersionPreprocessorTests : IDisposable
{
    private readonly string _root;
    private readonly string _input;
    private readonly string _output;

    public LatestVersionPreprocessorTests()
    {
        _root = Path.Combine(Path.GetTempPath(), "esf_prep_" + Guid.NewGuid().ToString("N"));
        _input = Path.Combine(_root, "in");
        _output = Path.Combine(_root, "out");
        Directory.CreateDirectory(_input);
    }

    public void Dispose()
    {
        if (Directory.Exists(_root)) Directory.Delete(_root, recursive: true);
    }

    private void WriteEsf(string relativeName, string content = "ESF")
    {
        var path = Path.Combine(_input, relativeName);
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        File.WriteAllText(path, content);
    }

    private LatestVersionCopyResult Run(LatestVersionPreprocessorOptions options) =>
        new EsfLatestVersionPreprocessor().Run(options);

    private LatestVersionPreprocessorOptions Options(bool dryRun = false, EsfCopyMode mode = EsfCopyMode.Flat) => new()
    {
        InputDirectory = _input,
        OutputDirectory = _output,
        Recursive = true,
        DryRun = dryRun,
        CopyMode = mode,
    };

    [Fact]
    public void Preprocessor_ShouldCopyOnlySelectedLatestFiles()
    {
        WriteEsf("NR11A-V1.esf");
        WriteEsf("NR11A-V2.esf");
        WriteEsf("IS00A-V26.esf");

        var result = Run(Options());

        var copied = Directory.GetFiles(_output, "*.esf").Select(Path.GetFileName).OrderBy(x => x).ToArray();
        Assert.Equal(new[] { "IS00A-V26.esf", "NR11A-V2.esf" }, copied);
        Assert.DoesNotContain(copied, f => f == "NR11A-V1.esf");
        Assert.Equal(2, result.Copied.Count(c => c.Status == CopyStatus.Copied));
    }

    [Fact]
    public void Preprocessor_ShouldNotCopyFilesInDryRun()
    {
        WriteEsf("NR11A-V1.esf");
        WriteEsf("NR11A-V2.esf");

        var result = Run(Options(dryRun: true));

        Assert.True(Directory.Exists(_output));                       // created for reports
        Assert.Empty(Directory.GetFiles(_output, "*.esf"));           // but nothing copied
        Assert.All(result.Copied, c => Assert.Equal(CopyStatus.DryRun, c.Status));
    }

    [Fact]
    public void Preprocessor_ShouldPreserveRelativePathWhenRequested()
    {
        WriteEsf(Path.Combine("sub", "deep", "NR11A-V2.esf"));

        Run(Options(mode: EsfCopyMode.PreserveRelativePath));

        var expected = Path.Combine(_output, "sub", "deep", "NR11A-V2.esf");
        Assert.True(File.Exists(expected), $"Expected mirrored path: {expected}");
    }

    [Fact]
    public void Preprocessor_ShouldRejectSameInputAndOutputDirectory()
    {
        var options = new LatestVersionPreprocessorOptions
        {
            InputDirectory = _input,
            OutputDirectory = _input,
        };

        Assert.Throws<ArgumentException>(() => Run(options));
    }
}
