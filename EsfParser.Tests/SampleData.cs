using System.Text;

namespace EsfParser.Tests;

/// <summary>
/// Shared access to the sample ESF files copied next to the test assembly.
/// </summary>
public static class SampleData
{
    static SampleData() => Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

    public static string SamplesDir => Path.Combine(AppContext.BaseDirectory, "Samples");

    public static string[] AllSamples =>
        Directory.Exists(SamplesDir)
            ? Directory.GetFiles(SamplesDir, "*.esf").OrderBy(f => f).ToArray()
            : Array.Empty<string>();

    public static string Path_NR11 => Path.Combine(SamplesDir, "NR11av28.esf");

    // ESF host sources are code page 1250.
    public static string[] ReadLines(string path) =>
        File.ReadAllLines(path, Encoding.GetEncoding(1250));

    /// <summary>Sample file names for the [Theory] data source.</summary>
    public static IEnumerable<object[]> SampleFileNames()
    {
        foreach (var f in AllSamples)
            yield return new object[] { System.IO.Path.GetFileName(f) };
    }
}
