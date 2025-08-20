// EsfParser/CodeGen/RoslynExporter.cs
using EsfParser.Esf;
using System;
using System.IO;
using System.Threading;

namespace EsfParser.CodeGen;

public static partial class RoslynExporter
{
    /// <summary>
    /// Structured project output. Creates folder tree, multiple files, and a ready-to-run console project.
    /// </summary>
    public static void WriteProjectFiles(
        EsfProgram program,
        string outputFolder,
        string @namespace = "EsfConsoleApp",
        CancellationToken cancel = default)
    {
        if (program is null) throw new ArgumentNullException(nameof(program));
        if (string.IsNullOrWhiteSpace(outputFolder))
            throw new ArgumentException("Output folder is required.", nameof(outputFolder));

        Console.WriteLine("── RoslynExporter (structured) ──");
        Console.WriteLine($"Namespace : {@namespace}");
        Console.WriteLine($"Root     : {Path.GetFullPath(outputFolder)}");
        Console.WriteLine();

        Structured_Write(program, outputFolder, @namespace);

        Console.WriteLine("ℹ️  To run:");
        Console.WriteLine($"    cd \"{Path.GetFullPath(outputFolder)}\"");
        Console.WriteLine("    dotnet restore");
        Console.WriteLine($"    dotnet run --project {SanitizeFileName(@namespace)}.csproj");
    }
}
