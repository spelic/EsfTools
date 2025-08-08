// RoslynExporter.cs ──────────────────────────────────────────────────────
using EsfParser.Esf;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace EsfParser.CodeGen;

public static class RoslynExporter
{
    // Path to the helper class that contains EzFunctions
    private static readonly string EzFunctionsPath =
        Path.GetFullPath(Path.Combine(AppContext.BaseDirectory,
            @"..\..\..\..\EsfParser\CodeGen\EzFunctions.cs"));

    // ────────────────────────────────────────────────────────────────
    //  PUBLIC API — write only the generated code to a file
    // ────────────────────────────────────────────────────────────────
    public static void WriteSourceFile(
        EsfProgram program,
        string outputFile,
        string @namespace = "EsfConsoleApp",
        CancellationToken cancel = default)
    {
        if (program is null) throw new ArgumentNullException(nameof(program));
        if (string.IsNullOrWhiteSpace(outputFile))
            throw new ArgumentException("Output file path is required.", nameof(outputFile));

        Console.WriteLine("── RoslynExporter ──");
        Console.WriteLine($"Namespace  : {@namespace}");
        Console.WriteLine($"Output     : {Path.GetFullPath(outputFile)}");
        Console.WriteLine();

        var cu = BuildCompilationUnit(program, @namespace)
                 .NormalizeWhitespace(elasticTrivia: true, indentation: "    ");

        var source = cu.ToFullString();

        Directory.CreateDirectory(Path.GetDirectoryName(outputFile)!);
        File.WriteAllText(outputFile, source);

        Console.WriteLine("✅  Source written.");
    }

    // ────────────────────────────────────────────────────────────────
    //  Syntax-tree construction
    // ────────────────────────────────────────────────────────────────
    private static CompilationUnitSyntax BuildCompilationUnit(EsfProgram p, string ns) =>
        CompilationUnit()
            .WithUsings(List(new[]
            {
                Using("System"),
                Using("System.Collections.Generic"),
                Using("System.Linq"),
                Using("System.Reflection"),
            }))
            .AddMembers(EzFunctionsNode())                  // helper class
            .AddMembers(
                NamespaceDeclaration(ParseName(ns))
                    .WithMembers(List(BuildSections(p))));

    // BuildSections streams *every* member from each generated snippet
    private static IEnumerable<MemberDeclarationSyntax> BuildSections(EsfProgram p)
    {
        // helper that returns *all* top-level members in a snippet
        static IEnumerable<MemberDeclarationSyntax> SnipAll(string csharp) =>
            ParseCompilationUnit(csharp).Members;

        // helper for guaranteed single-member snippets (used below)
        static MemberDeclarationSyntax SnipOne(string csharp) =>
            ParseCompilationUnit(csharp).Members.First();

        // ── Items ────────────────────────────────────────────────
        Console.WriteLine($"Items           {p.Items.Items.Count}");
        if (p.Items.Items.Count > 0)
            foreach (var m in SnipAll(p.Items.ToCSharp()))
                yield return m;

        // ── Records ──────────────────────────────────────────────
        Console.WriteLine($"Records         {p.Records.Records.Count}");
        if (p.Records.Records.Count > 0)
            foreach (var m in SnipAll(p.Records.ToCSharp()))
                yield return m;

        // ── Maps ─────────────────────────────────────────────────
        Console.WriteLine($"Maps            {p.Maps.Maps.Count}");
        if (p.Maps.Maps.Count > 0)
        {
            var mapCode = p.Maps.ToCSharp();

            DumpRegion("⏪  Raw MAP region", mapCode);

            foreach (var m in SnipAll(mapCode))
                yield return m;

            // Just a visual confirmation that everything made it through
            DumpRegion("⏩  Final MAP region (preview only)", mapCode);
        }

        // ── Tables ───────────────────────────────────────────────
        Console.WriteLine($"Tables          {p.Tables.Tables.Count}");
        if (p.Tables.Tables.Count > 0)
            foreach (var m in SnipAll(p.Tables.ToCSharp()))
                yield return m;

        // ── Functions ────────────────────────────────────────────
        Console.WriteLine($"Functions       {p.Functions.Functions.Count}");
        if (p.Functions.Functions.Count > 0)
            foreach (var m in SnipAll(p.Functions.ToCSharp()))
                yield return m;

        // ── Entry-point stub ─────────────────────────────────────
        yield return
            ClassDeclaration("Program")
                .AddModifiers(Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.StaticKeyword))
                .AddMembers(
                    MethodDeclaration(PredefinedType(Token(SyntaxKind.VoidKeyword)), "Main")
                        .AddModifiers(Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.StaticKeyword))
                        .WithBody(
                            Block(
                                ParseStatement("""Console.WriteLine("ESF program initialized.");"""))));
    }

    // ────────────────────────────────────────────────────────────────
    //  Region preview helper (diagnostics only)
    // ────────────────────────────────────────────────────────────────
    private static void DumpRegion(string title, string code)
    {
        var header = code.IndexOf("#region", StringComparison.Ordinal);
        var tail = code.IndexOf("#endregion", StringComparison.Ordinal);

        Console.WriteLine($"── {title} ────────────────────────────────────────────────────");

        if (header < 0 || tail < 0)
        {
            Console.WriteLine("(region not found)");
        }
        else
        {
            int len = Math.Min(320, tail - header);
            var preview = code.Substring(header, len)
                              .Replace("\r", "");
            Console.WriteLine(preview);
            Console.WriteLine("…");
        }

        Console.WriteLine("──────────────────────────────────────────────────────────────");
        Console.WriteLine();
    }

    // ────────────────────────────────────────────────────────────────
    //  EzFunctions loader (kept outside any namespace)
    // ────────────────────────────────────────────────────────────────
    private static MemberDeclarationSyntax EzFunctionsNode()
    {
        var cu = ParseCompilationUnit(File.ReadAllText(EzFunctionsPath));
        return cu.DescendantNodes()
                 .OfType<ClassDeclarationSyntax>()
                 .First(c => c.Identifier.Text == "EzFunctions");
    }

    // simple helper for `using` creation
    private static UsingDirectiveSyntax Using(string ns) =>
        UsingDirective(ParseName(ns));
}
