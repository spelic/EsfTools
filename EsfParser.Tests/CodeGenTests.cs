using EsfParser.Builder;
using EsfParser.CodeGen;
using EsfParser.Esf;
using EsfParser.Parser;
using Xunit;

namespace EsfParser.Tests;

// CSharpUtils.Program is static, so keep these tests in one (serially-run) class.
public class CodeGenTests
{
    private static EsfProgram BuildNr11()
    {
        var nodes = MyEsfParser.Parse(SampleData.ReadLines(SampleData.Path_NR11));
        return EsfProgramBuilder.GenerateEsfProgram(nodes);
    }

    [Fact]
    public void ConvertOperand_QualifiesSystemVar()
    {
        Assert.StartsWith("EzFunctions.", CSharpUtils.ConvertOperand("EZEAID"));
    }

    [Fact]
    public void ConvertOperand_PassesThroughLiterals()
    {
        Assert.Equal("5", CSharpUtils.ConvertOperand("5"));
        Assert.Equal("\"IS00\"", CSharpUtils.ConvertOperand("'IS00'"));
    }

    [Fact]
    public void ConvertOperand_IsNullSafe_WithProgramSet()
    {
        // §3: RecordOrg / lookups must not throw for records with a null Org, etc.
        CSharpUtils.Program = BuildNr11();
        var ex = Record.Exception(() =>
        {
            CSharpUtils.ConvertOperand("NR11W01.STEVAPPL");
            CSharpUtils.ConvertOperand("EZEAID");
            CSharpUtils.ConvertOperand("SomeUnknownThing");
        });
        Assert.Null(ex);
    }

    [Fact]
    public void WriteProjectFiles_EmitsExpectedSkeleton()
    {
        var program = BuildNr11();
        CSharpUtils.Program = program;

        var outDir = Path.Combine(Path.GetTempPath(), "esf_test_" + Guid.NewGuid().ToString("N"));
        try
        {
            RoslynExporter.WriteProjectFiles(program, outDir, "GoldenApp");

            Assert.True(File.Exists(Path.Combine(outDir, "Program.cs")));
            Assert.True(File.Exists(Path.Combine(outDir, "GoldenApp.csproj")));
            Assert.True(File.Exists(Path.Combine(outDir, "Startup.json")));
            Assert.True(File.Exists(Path.Combine(outDir, "EsfRuntime", "EzFunctions.cs")));
            Assert.True(File.Exists(Path.Combine(outDir, "EsfRuntime", "ConverseConsole.cs")));

            var logicFiles = Directory.GetFiles(Path.Combine(outDir, "Functions", "Logic"), "*.cs");
            Assert.NotEmpty(logicFiles);
        }
        finally
        {
            if (Directory.Exists(outDir)) Directory.Delete(outDir, recursive: true);
        }
    }

    [Fact]
    public void RuntimeHelpers_GetCallerNamespace_NotAHardcodedOne()
    {
        // §6 regression: the runtime helpers must be rewritten to the *requested*
        // namespace, never leak a hardcoded "NR11AV28_ConsoleApp".
        var program = BuildNr11();
        CSharpUtils.Program = program;

        var outDir = Path.Combine(Path.GetTempPath(), "esf_test_" + Guid.NewGuid().ToString("N"));
        try
        {
            RoslynExporter.WriteProjectFiles(program, outDir, "AcmeApp");

            foreach (var helper in new[] { "ConverseConsole.cs", "ConsoleMapRenderer.cs" })
            {
                var text = File.ReadAllText(Path.Combine(outDir, "EsfRuntime", helper));
                Assert.Contains("namespace AcmeApp.Runtime", text);
                Assert.DoesNotContain("NR11AV28_ConsoleApp", text);
            }
        }
        finally
        {
            if (Directory.Exists(outDir)) Directory.Delete(outDir, recursive: true);
        }
    }
}
