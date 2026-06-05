using EsfParser.Builder;
using EsfParser.Parser;
using Xunit;

namespace EsfParser.Tests;

public class ParserTests
{
    [Fact]
    public void Samples_AreDeployed()
    {
        Assert.True(SampleData.AllSamples.Length > 0,
            $"No sample .esf files found in {SampleData.SamplesDir}");
    }

    [Theory]
    [MemberData(nameof(SampleData.SampleFileNames), MemberType = typeof(SampleData))]
    public void Parse_ProducesNodes_AndDoesNotThrow(string fileName)
    {
        var path = Path.Combine(SampleData.SamplesDir, fileName);
        var lines = SampleData.ReadLines(path);

        // §8: parsing must never throw, even on unexpected lines.
        var nodes = MyEsfParser.Parse(lines);

        Assert.NotNull(nodes);
        Assert.NotEmpty(nodes);
    }

    [Fact]
    public void Parse_DoesNotThrow_OnMalformedLine_AndRecordsDiagnostic()
    {
        // A PROGRAM tag whose body line is neither attribute nor recognizable content.
        var lines = new[]
        {
            ":program   name      = BADP",
            "this line is not a valid attribute or content @@@",
            ":eprogram"
        };

        var ex = Record.Exception(() => MyEsfParser.Parse(lines));

        Assert.Null(ex); // previously threw InvalidOperationException
        Assert.NotEmpty(MyEsfParser.Diagnostics);
    }

    [Fact]
    public void GenerateEsfProgram_PopulatesModel_ForNr11()
    {
        var nodes = MyEsfParser.Parse(SampleData.ReadLines(SampleData.Path_NR11));
        var program = EsfProgramBuilder.GenerateEsfProgram(nodes);

        Assert.NotNull(program.Program);
        Assert.NotEmpty(program.Functions.Functions);
        Assert.NotEmpty(program.Records.Records);
    }
}
