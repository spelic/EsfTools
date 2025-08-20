// EsfParser/CodeGen/RoslynExporter.Structured.cs
using EsfParser.Esf;
using EsfParser.Tags;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace EsfParser.CodeGen;

public static partial class RoslynExporter
{
    private static void Structured_Write(EsfProgram program, string root, string ns)
    {
        // folder tree
        var runtimeDir = Path.Combine(root, "EsfRuntime");
        var itemsDir = Path.Combine(root, "Items");
        var workstorDir = Path.Combine(root, "Workstor");
        var recordsDir = Path.Combine(root, "Records");
        var tablesDir = Path.Combine(root, "Tables");
        var mapsDir = Path.Combine(root, "Maps");
        var ezeeDir = Path.Combine(root, "Ezee");
        var functionsLogicDir = Path.Combine(root, "Functions", "Logic");
        var functionsSqlDir = Path.Combine(root, "Functions", "Sql");

        Directory.CreateDirectory(root);
        Directory.CreateDirectory(runtimeDir);
        Directory.CreateDirectory(itemsDir);
        Directory.CreateDirectory(workstorDir);
        Directory.CreateDirectory(recordsDir);
        Directory.CreateDirectory(tablesDir);
        Directory.CreateDirectory(mapsDir);
        Directory.CreateDirectory(ezeeDir);
        Directory.CreateDirectory(functionsLogicDir);
        Directory.CreateDirectory(functionsSqlDir);

        // top-level program, project, startup.json
        WriteProgramCs(root, ns, program.Program.Mainfun.Name);
        WriteProjectFile(root, ns);
        TryCopyStartupJson(root);

        // runtime helpers
        CopyEzFunctionsWithNamespaceRewrite(EzFunctionsPath, Path.Combine(runtimeDir, "EzFunctions.cs"), ns);
        CopyFileIfExists(SqlHelpersPath, Path.Combine(runtimeDir, "SqlHelpers.cs"));
        CopyFileIfExists(CursorStorePath, Path.Combine(runtimeDir, "CursorStore.cs"));

        // existence flags for sub-namespaces
        bool hasItems = program.Items.Items.Count > 0;
        bool hasRecords = program.Records.Records.Count > 0;
        bool hasTables = program.Tables.Tables.Count > 0;
        bool hasMaps = program.Maps.Maps.Count > 0;

        // NEW: detect any WORKSTOR by metadata (Org), fallback to Program.WorkstorRecord
        bool hasWorkstor = program.Records.Records.Any(r =>
                                r.Org != null && (r.Org.Equals("WORKSTOR", StringComparison.OrdinalIgnoreCase)
                                               || r.Org.Equals("WORKSTORAGE", StringComparison.OrdinalIgnoreCase)))
                           || program.WorkstorRecord != null;

        var subUsings = BuildSubNsUsings(ns, hasItems, hasWorkstor, hasRecords, hasTables, hasMaps).ToArray();
        
        static IEnumerable<MemberDeclarationSyntax> SnipAll(string csharp)
        {
            var cu = CSharpSyntaxTree.ParseText(csharp).GetCompilationUnitRoot();
            return RoslynExporter_FlattenProxy(cu);
        }

        // Items -> <AppNs>.Items
        if (hasItems)
        {
            var itemsCode = program.Items.ToCSharp();
            foreach (var m in SnipAll(itemsCode))
            {
                var name = MemberName(m);
                WriteMembersFile(itemsDir, $"{SanitizeFileName(name)}.cs", ns + ".Items", new[] { m }, null, subUsings);
            }
        }

        // Records (route WORKSTOR record + GlobalWorkstor to Workstor/)
        var workstorName = program.WorkstorRecord?.Name; // e.g., IS00W01
                                                         // Records (route WORKSTOR record + GlobalWorkstor to Workstor/)
                                                         // Records (route WORKSTOR record + GlobalWorkstor to Workstor/)
        if (hasRecords)
        {
            var recIndex = program.Records.Records
                .ToDictionary(r => r.Name ?? string.Empty, StringComparer.OrdinalIgnoreCase);
            var fallbackWorkstorName = program.WorkstorRecord?.Name;

            var recCode = program.Records.ToCSharp();
            foreach (var m in SnipAll(recCode))
            {
                var name = MemberName(m);
                var file = $"{SanitizeFileName(name)}.cs";

                recIndex.TryGetValue(name, out var meta);
                var org = meta?.Org?.Trim();

                bool isWorkstorRecord =
                       (!string.IsNullOrEmpty(org) &&
                           (org.Equals("WORKSTOR", StringComparison.OrdinalIgnoreCase)
                         || org.Equals("WORKSTORAGE", StringComparison.OrdinalIgnoreCase)))
                    || (!string.IsNullOrWhiteSpace(fallbackWorkstorName)
                         && name.Equals(fallbackWorkstorName, StringComparison.OrdinalIgnoreCase));

                bool isGlobalWorkstor = name.Equals("GlobalWorkstor", StringComparison.Ordinal);

                if (isWorkstorRecord)
                {
                    WriteMembersFile(workstorDir, file, ns + ".Workstor", new[] { m }, null, subUsings);
                }
                else if (isGlobalWorkstor)
                {
                    // Route to Workstor namespace AND rewrite type uses from Records.* → Workstor.*
                    WriteMembersFile(
                        workstorDir,
                        file,
                        ns + ".Workstor",
                        new[] { m },
                        extraUsings: null,
                        appSubNamespaceUsings: subUsings,
                        contentFilter: s =>
                            // keep 'using <AppNs>.Records;' intact, but fix type references:
                            s.Replace(" new Records.", " new Workstor.")
                             .Replace(" Records.", " Workstor.")
                    );
                }
                else
                {
                    WriteMembersFile(recordsDir, file, ns + ".Records", new[] { m }, null, subUsings);
                }
            }
        }

        // Tables -> <AppNs>.Tables
        if (hasTables)
        {
            var tblCode = program.Tables.ToCSharp();
            foreach (var m in SnipAll(tblCode))
            {
                var name = MemberName(m);
                WriteMembersFile(tablesDir, $"{SanitizeFileName(name)}.cs", ns + ".Tables", new[] { m }, null, subUsings);
            }
        }

        // Maps -> <AppNs>.Maps
        if (hasMaps)
        {
            var mapCode = program.Maps.ToCSharp();
            foreach (var m in SnipAll(mapCode))
            {
                var name = MemberName(m);
                WriteMembersFile(mapsDir, $"{SanitizeFileName(name)}.cs", ns + ".Maps", new[] { m }, null, subUsings);
            }
        }

        // Ezee placeholder -> <AppNs>.Ezee
        if (program.Ezee != null)
        {
            var ezeeCode = @"namespace " + ns + @".Ezee;
internal static class EzeeInfo
{
    // TODO: emit real EZEE content if available
}";
            File.WriteAllText(Path.Combine(ezeeDir, "Ezee.cs"), ezeeCode);
        }
      
        // ────────────────────────────────────────────────────────────────
        // Functions — split per function: LOGIC (SqlClauses.Count==0) → Logic/, SQL (SqlClauses.Count>0) → Sql/
        // One file per function, named <FuncName>.cs
        // ────────────────────────────────────────────────────────────────
        if (program.Functions.Functions.Count > 0)
        {
            var usedSqlNames = new HashSet<string>(System.StringComparer.OrdinalIgnoreCase);

            foreach (var f in program.Functions.Functions)
            {
                bool isSql = (f.SqlClauses?.Count ?? 0) > 0;

                if (!isSql)
                {
                    // LOGIC → Functions/Logic/<FuncName>.cs
                    var methodName = f.Name;
                    var body = f.ToCSharp() ?? string.Empty; // your FuncTag.ToCSharp emits BEFORE-logic body
                    var summary = string.IsNullOrWhiteSpace(f.Desc) ? "" :
                                     $"    /// <summary>\n    /// {EscapeXml(f.Desc)}\n    /// </summary>\n";

                    var logicCode = $@"
public static partial class GlobalFunctions
{{
{summary}    public static void {methodName}()
    {{
{IndentText(body, 8)}
    }}
}}";

                    var cu = Microsoft.CodeAnalysis.CSharp.CSharpSyntaxTree.ParseText(logicCode).GetCompilationUnitRoot();
                    var cls = cu.Members.OfType<Microsoft.CodeAnalysis.CSharp.Syntax.ClassDeclarationSyntax>().First();

                    WriteMembersFile(
                        functionsLogicDir,
                        $"{SanitizeFileName(methodName)}.cs",
                        ns,                                    // root app namespace
                        new[] { cls },
                        extraUsings: null,
                        appSubNamespaceUsings: subUsings);
                }
                else
                {
                    // SQL → Functions/Sql/<FuncName>.cs (method in partial GlobalFunctions)
                    {
                        // Compose SQL text from ESF clauses in source order
                        var sqlText = string.Join("\n", f.SqlClauses.Select(c => c?.Text ?? string.Empty));
                        var sqlLiteral = sqlText.Replace("\"", "\"\""); // escape for verbatim @""

                        var methodName = f.Name; // use the ESF function name as method name

                        // Generate a partial GlobalFunctions with a void method
                        var sqlMethodCode = $@"
public static partial class GlobalFunctions
{{
    /// <summary>Auto-generated SQL for ESF function “{EscapeXml(f.Name)}”.</summary>
    public static void {methodName}(object? param = null)
    {{
        using var conn = DataAccess.GetConnection();
        var sql = @""{sqlLiteral}"";
        var rows = conn.Query(sql, param);
        foreach (var row in rows)
            System.Console.WriteLine(row);
        // For non-query: conn.Execute(sql, param);
    }}
}}";

                        var cu = Microsoft.CodeAnalysis.CSharp.CSharpSyntaxTree.ParseText(sqlMethodCode).GetCompilationUnitRoot();
                        var cls = cu.Members.OfType<Microsoft.CodeAnalysis.CSharp.Syntax.ClassDeclarationSyntax>().First();

                        // Ensure Dapper + app sub-namespace usings are present; no phantom *.Data usings
                        var extraUsings = new HashSet<string>(new[]
                        {
        "System", "System.Collections.Generic", "System.Linq", "Dapper"
    }, System.StringComparer.Ordinal);

                        WriteMembersFile(
                            functionsSqlDir,
                            $"{SanitizeFileName(methodName)}.cs",
                            ns,                       // root app namespace (matches Logic)
                            new[] { cls },
                            extraUsings: extraUsings,
                            appSubNamespaceUsings: subUsings);
                    }

                }
            }
        }

        System.Console.WriteLine("✅  Structured project created.");
    }

    // proxy for local call to shared helper
    private static IEnumerable<MemberDeclarationSyntax> RoslynExporter_FlattenProxy(CompilationUnitSyntax cu) => FlattenMembers(cu);
       
    private static string MemberName(MemberDeclarationSyntax m) =>
        m switch
        {
            ClassDeclarationSyntax c => c.Identifier.Text,
            StructDeclarationSyntax s => s.Identifier.Text,
            RecordDeclarationSyntax r => r.Identifier.Text,
            InterfaceDeclarationSyntax i => i.Identifier.Text,
            EnumDeclarationSyntax e => e.Identifier.Text,
            _ => "Generated"
        };

    // csproj writer shared by both modes
    private static void WriteProjectFile(string projectDir, string projectName)
    {
        var csprojPath = Path.Combine(projectDir, $"{SanitizeFileName(projectName)}.csproj");
        var csproj = $@"<Project Sdk=""Microsoft.NET.Sdk"">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net8.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>disable</Nullable>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include=""Dapper"" Version=""2.*"" />
    <PackageReference Include=""Net.IBM.Data.Db2"" Version=""2.*"" />
  </ItemGroup>

  <ItemGroup>
    <Content Include=""Startup.json"">
      <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
    </Content>
  </ItemGroup>
</Project>";
        File.WriteAllText(csprojPath, csproj);
        System.Console.WriteLine($"🧩  Project file created: {csprojPath}");
    }

    // Program.cs for structured output
    private static void WriteProgramCs(string root, string ns, string mainProgramName)
    {
        var src = $@"using System;
using System.IO;
using System.Text.Json;

namespace {ns};

internal sealed class StartupConfig
{{
    public string DB2_CONN_STR {{ get; set; }} = string.Empty;
    public string? PARAM_STYLE {{ get; set; }}
}}

internal static class StartupConfigLoader
{{
    public static StartupConfig Load(string? path = null)
    {{
        var baseDir = AppContext.BaseDirectory;
        var file = path ?? Path.Combine(baseDir, ""Startup.json"");
        if (!File.Exists(file))
        {{
            return new StartupConfig
            {{
                DB2_CONN_STR = Environment.GetEnvironmentVariable(""DB2_CONN_STR"") ?? string.Empty
            }};
        }}
        var json = File.ReadAllText(file);
        var cfg = JsonSerializer.Deserialize<StartupConfig>(json,
            new JsonSerializerOptions {{ PropertyNameCaseInsensitive = true }})
            ?? new StartupConfig();
        return cfg;
    }}
}}

public static class Program
{{
    public static void Main()
    {{
        var cfg = StartupConfigLoader.Load();
        if (!string.IsNullOrWhiteSpace(cfg.DB2_CONN_STR))
        {{
            Environment.SetEnvironmentVariable(""DB2_CONN_STR"", cfg.DB2_CONN_STR);
            DataAccess.Configure(cfg.DB2_CONN_STR);
        }}
        else
        {{
            DataAccess.Configure();
        }}

        Console.WriteLine(""ESF program initialized."");
        GlobalFunctions.{mainProgramName}(); // call the main function
    }}
}}";
        File.WriteAllText(Path.Combine(root, "Program.cs"), src);
        System.Console.WriteLine($"✏️  {Path.Combine(root, "Program.cs")}");
    }
}
