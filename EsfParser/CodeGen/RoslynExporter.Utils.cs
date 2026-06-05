// EsfParser/CodeGen/RoslynExporter.Utils.cs
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace EsfParser.CodeGen;

public static partial class RoslynExporter
{
    private static UsingDirectiveSyntax Using(string ns) => UsingDirective(ParseName(ns));

    private static string SanitizeFileName(string name)
    {
        foreach (var c in Path.GetInvalidFileNameChars()) name = name.Replace(c, '_');
        return name;
    }

    /// <summary>
    /// Load a helper source by file name. Prefers the file embedded in this assembly
    /// (so the tool works after publish / from any working directory) and falls back to
    /// the on-disk source path for local development. Returns null if neither is found.
    /// </summary>
    private static string? LoadHelperText(string fsFallbackPath)
    {
        var fileName = Path.GetFileName(fsFallbackPath);
        var asm = typeof(RoslynExporter).Assembly;
        var res = asm.GetManifestResourceNames()
                     .FirstOrDefault(n => n.EndsWith("." + fileName, StringComparison.OrdinalIgnoreCase));
        if (res != null)
        {
            using var s = asm.GetManifestResourceStream(res);
            if (s != null) { using var r = new StreamReader(s); return r.ReadToEnd(); }
        }
        return File.Exists(fsFallbackPath) ? File.ReadAllText(fsFallbackPath) : null;
    }

    private static void TryCopyStartupJson(string destFolder)
    {
        try
        {
            var destJson = Path.Combine(destFolder, "Startup.json");
            var text = LoadHelperText(StartupJsonPath);
            if (text != null)
            {
                File.WriteAllText(destJson, text);
                Console.WriteLine($"📄  Startup.json written to: {destJson}");
            }
            else Console.WriteLine($"⚠️  Startup.json not found (embedded or at {StartupJsonPath})");
        }
        catch (Exception ex) { Console.WriteLine($"⚠️  Failed to copy Startup.json: {ex.Message}"); }
    }

    private static void CopyFileIfExists(string src, string dst, Func<string, string>? post = null)
    {
        var text = LoadHelperText(src);
        if (text == null) { Console.WriteLine($"⚠️  Helper not found: {Path.GetFileName(src)}"); return; }
        if (post != null) text = post(text);
        File.WriteAllText(dst, text);
        Console.WriteLine($"📎  {Path.GetFileName(dst)} written.");
    }

    private static void CopyEzFunctionsWithNamespaceRewrite(string src, string dst, string newNamespace)
        => CopyFileIfExists(src, dst, t => RewriteNamespace(t, newNamespace));

    private static string RewriteNamespace(string sourceText, string? newNamespace)
    {
        var s = sourceText.Replace("\r\n", "\n");
        if (newNamespace == null)
        {
            if (s.Contains("namespace EsfParser.CodeGen"))
            {
                s = s.Replace("namespace EsfParser.CodeGen\n{", "");
                var i = s.LastIndexOf('}');
                if (i >= 0) s = s.Remove(i, 1);
            }
            return s;
        }
        s = s.Replace("namespace EsfParser.CodeGen", $"namespace {newNamespace}");
        s = s.Replace("namespace EsfParser.CodeGen;", $"namespace {newNamespace};");
        return s;
    }

    /// <summary>
    /// Build sub-namespace usings that actually exist (Items/Workstor/Records/Tables/Maps).
    /// </summary>
    private static IEnumerable<string> BuildSubNsUsings(
        string appNs, bool hasItems, bool hasWorkstor, bool hasRecords, bool hasTables, bool hasMaps)
    {
        if (hasItems) yield return $"{appNs}.Items";
        if (hasWorkstor) yield return $"{appNs}.Workstor";
        if (hasRecords) yield return $"{appNs}.Records";
        if (hasTables) yield return $"{appNs}.Tables";
        if (hasMaps) yield return $"{appNs}.Maps";
        // Always include Runtime namespace so conversation helpers and AidKey are accessible
        yield return $"{appNs}.Runtime";
    }

    /// <summary>
    /// Write a .cs file with a file-scoped namespace and provided members.
    /// Adds base usings, optional sub-namespace usings, and extra usings (deduped, sanitized).
    /// Strips 'EsfRuntime.' and removes any '*.Data' usings.
    /// </summary>
    private static void WriteMembersFile(
    string folder,
    string fileName,
    string targetNamespace,
    IEnumerable<MemberDeclarationSyntax> members,
    IEnumerable<string>? extraUsings = null,
    IEnumerable<string>? appSubNamespaceUsings = null,
    Func<string, string>? contentFilter = null)   // <-- new
    {
        Directory.CreateDirectory(folder);

        var usingSet = new HashSet<string>(StringComparer.Ordinal)
    {
        "System", "System.Collections.Generic", "System.Linq"
    };
        if (appSubNamespaceUsings != null)
            foreach (var u in appSubNamespaceUsings) usingSet.Add(u);
        if (extraUsings != null)
            foreach (var u in extraUsings) usingSet.Add(u);

        // remove any *.Data usings (phantom)
        usingSet.RemoveWhere(u => u.EndsWith(".Data", StringComparison.Ordinal) || u.EndsWith(".Data;", StringComparison.Ordinal));

        var file = CompilationUnit()
            .WithUsings(List(usingSet.Select(Using)))
            .AddMembers(FileScopedNamespaceDeclaration(ParseName(targetNamespace)).AddMembers(members.ToArray()))
            .NormalizeWhitespace(elasticTrivia: true, indentation: "    ")
            .ToString()
            .Replace("EsfRuntime.", ""); // strip any leftovers

        // apply optional content filter for special files (e.g., GlobalWorkstor)
        if (contentFilter != null)
            file = contentFilter(file);

        File.WriteAllText(Path.Combine(folder, fileName), file);
        Console.WriteLine($"✏️  {Path.Combine(folder, fileName)}");
    }


    private static MemberDeclarationSyntax[] LoadTopLevelMembers(string path)
    {
        if (!File.Exists(path)) { Console.WriteLine($"⚠️  Helper not found: {path}"); return Array.Empty<MemberDeclarationSyntax>(); }
        return CSharpSyntaxTree.ParseText(File.ReadAllText(path)).GetCompilationUnitRoot().Members.ToArray();
    }

    // Flatten top-level members so we never duplicate or nest namespaces.
    private static IEnumerable<MemberDeclarationSyntax> FlattenMembers(CompilationUnitSyntax cu)
    {
        foreach (var m in cu.Members)
        {
            if (m is NamespaceDeclarationSyntax nds) { foreach (var inner in nds.Members) yield return inner; }
            else if (m is FileScopedNamespaceDeclarationSyntax fnds) { foreach (var inner in fnds.Members) yield return inner; }
            else yield return m;
        }
    }

    private static string IndentText(string text, int spaces)
    {
        if (string.IsNullOrEmpty(text)) return string.Empty;
        var pad = new string(' ', spaces);
        using var sr = new System.IO.StringReader(text.Replace("\r\n", "\n"));
        using var sw = new System.IO.StringWriter();
        string? line;
        while ((line = sr.ReadLine()) is not null) sw.WriteLine(pad + line);
        return sw.ToString();
    }

    private static string EscapeXml(string? s)
    {
        if (string.IsNullOrEmpty(s)) return string.Empty;
        return s.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;");
    }

}
