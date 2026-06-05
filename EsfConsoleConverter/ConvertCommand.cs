using EsfParser.Builder;
using EsfParser.CodeGen;
using EsfParser.Parser;
using System.Text;

namespace EsfConsoleConverter;

/// <summary>
/// The original behavior: parse one ESF file and emit a structured C# console project.
/// Invoked via the explicit <c>convert</c> verb or as the default when no verb is given.
///
/// Usage:
///   EsfConsoleConverter [convert] &lt;input.esf&gt; [--out &lt;folder&gt;] [--namespace &lt;ns&gt;]
/// </summary>
internal static class ConvertCommand
{
    public static int Run(string[] args)
    {
        var options = ConvertOptions.Parse(args);

        var inputPath = ResolveInput(options.Input);
        if (inputPath is null)
        {
            Console.Error.WriteLine($"File not found: {options.Input}");
            return 1;
        }

        string name = Path.GetFileNameWithoutExtension(inputPath).Replace("-", "_").ToUpperInvariant();
        string @namespace = options.Namespace ?? $"{name}_ConsoleApp";
        string outputFolder = Path.Combine(
            options.OutputRoot ?? Path.Combine(Directory.GetCurrentDirectory(), "generated"),
            name);

        Console.WriteLine($"Input     : {inputPath}");
        Console.WriteLine($"Output    : {outputFolder}");
        Console.WriteLine($"Namespace : {@namespace}");
        Console.WriteLine();

        // ESF host sources are code page 1250.
        var lines = File.ReadAllLines(inputPath, Encoding.GetEncoding(1250));

        var nodes = MyEsfParser.Parse(lines);
        if (MyEsfParser.Diagnostics.Count > 0)
        {
            Console.WriteLine($"⚠️  Parser reported {MyEsfParser.Diagnostics.Count} diagnostic(s):");
            foreach (var d in MyEsfParser.Diagnostics) Console.WriteLine($"    {d}");
            Console.WriteLine();
        }

        var program = EsfProgramBuilder.GenerateEsfProgram(nodes);
        CSharpUtils.Program = program;

        RoslynExporter.WriteProjectFiles(program, outputFolder, @namespace);

        Console.WriteLine("Done.");
        return 0;
    }

    // Resolve the input path: as given, or relative to the executable directory.
    private static string? ResolveInput(string input)
    {
        if (File.Exists(input)) return Path.GetFullPath(input);
        if (!Path.IsPathRooted(input))
        {
            var beside = Path.Combine(AppContext.BaseDirectory, input);
            if (File.Exists(beside)) return beside;
        }
        return null;
    }

    private sealed class ConvertOptions
    {
        public string Input { get; private set; } = "NR11av28.esf";
        public string? OutputRoot { get; private set; }
        public string? Namespace { get; private set; }

        public static ConvertOptions Parse(string[] args)
        {
            var o = new ConvertOptions();
            bool inputSet = false;

            for (int i = 0; i < args.Length; i++)
            {
                switch (args[i])
                {
                    case "--out" or "-o" when i + 1 < args.Length:
                        o.OutputRoot = args[++i];
                        break;
                    case "--namespace" or "-n" when i + 1 < args.Length:
                        o.Namespace = args[++i];
                        break;
                    default:
                        if (!inputSet && !args[i].StartsWith('-'))
                        {
                            o.Input = args[i];
                            inputSet = true;
                        }
                        break;
                }
            }
            return o;
        }
    }
}
