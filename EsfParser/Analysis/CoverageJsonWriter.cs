using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace EsfParser.Analysis;

internal static class CoverageJsonWriter
{
    private static readonly Encoding Utf8NoBom = new UTF8Encoding(false);
    private static readonly JsonSerializerOptions Opts = new()
    {
        WriteIndented = true,
        Converters = { new JsonStringEnumConverter() },
    };

    public static string Write(PortfolioCoverageResult result, string outputDir)
    {
        var path = Path.Combine(outputDir, "migration-coverage-report.json");
        File.WriteAllText(path, JsonSerializer.Serialize(result, Opts), Utf8NoBom);
        return path;
    }
}
