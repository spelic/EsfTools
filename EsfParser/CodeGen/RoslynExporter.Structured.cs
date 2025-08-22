// EsfParser/CodeGen/RoslynExporter.Structured.cs
using EsfParser.Esf;
using EsfParser.Tags;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
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

        // conversation runtime helpers (AidKey, ConsoleMapRenderer, ConverseConsole)
        // These files live under EsfParser/Runtime and use the EsfParser.Runtime namespace.
        // We rewrite their namespaces and using directives to the current app namespace (ns)
        // so they can be included directly in the generated console project without
        // referencing EsfParser.  We also replace references to EsfParser.Tags with the
        // local Maps namespace.
        CopyRuntimeHelperWithNamespaceRewrite(AidKeyPath, Path.Combine(runtimeDir, "AidKey.cs"), ns);
        CopyRuntimeHelperWithNamespaceRewrite(ConsoleMapRendererPath, Path.Combine(runtimeDir, "ConsoleMapRenderer.cs"), ns);
        CopyRuntimeHelperWithNamespaceRewrite(ConverseConsolePath, Path.Combine(runtimeDir, "ConverseConsole.cs"), ns);

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

        if (program.Functions.Functions.Count > 0)
        {
            var usedSqlNames = new HashSet<string>(System.StringComparer.OrdinalIgnoreCase);

            foreach (var f in program.Functions.Functions)
            {         
                if (f.Option == "EXECUTE" || f.Option == "CONVERSE")
                {
                    // LOGIC → Functions/Logic/<FuncName>.cs
                    var methodName = f.Name;
                    var body = f.ToCSharp() ?? string.Empty;
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
                        ns,
                        new[] { cls },
                        extraUsings: null,
                        appSubNamespaceUsings: subUsings);
                }
                else
                {
                    // SQL → Functions/Sql/<FuncName>.cs
                    var selectRaw = f.SqlClauses?.FirstOrDefault(c => c?.ClauseType == "SELECT")?.Text ?? " * ";
                    var whereRaw = f.SqlClauses?.FirstOrDefault(c => c?.ClauseType == "WHERE")?.Text ?? string.Empty;
                    var orderRaw = f.SqlClauses?.FirstOrDefault(c => c?.ClauseType == "ORDERBY")?.Text ?? string.Empty;

                    var methodName = f.Name;
                    var sqlRecordName = f.ObjectName;

                    var sqlRecord = program.Records.Records
                        .FirstOrDefault(r => r.Name != null &&
                                             r.Name.Equals(sqlRecordName, System.StringComparison.OrdinalIgnoreCase));
                    if (sqlRecord == null)
                    {
                        System.Console.WriteLine($"⚠️  Warning: SQL function '{f.Name}' references record '{sqlRecordName}' which does not exist in the program.");
                        throw new InvalidOperationException(
                            $"SQL function '{f.Name}' references record '{sqlRecordName}' which does not exist in the program.");
                    }

                    var cleanSqlRowRecord = CSharpUtils.ConvertOperand(sqlRecordName);
                    var cleanSqlRowRecordType = cleanSqlRowRecord;
                    var typeIdx = cleanSqlRowRecord.LastIndexOf('.');
                    if (typeIdx >= 0) cleanSqlRowRecordType = cleanSqlRowRecord[(typeIdx + 1)..];

                    var dbTableName = sqlRecord.SqlTables.FirstOrDefault()?.TableId;
                    if (string.IsNullOrWhiteSpace(dbTableName))
                    {
                        System.Console.WriteLine($"⚠️  Warning: SQL function '{f.Name}' has no valid table name.");
                        dbTableName = "UnknownTable";
                    }

                    string sqlMethodCode = "";

                    switch (f.Option)
                    {
                        case "INQUIRY":
                            {
                                // WHERE + params
                                var (whereClean, paramDecl, originalWhere) =
                                    SqlEmitHelpers.BuildWhereAndParams(whereRaw, cleanSqlRowRecord, op => CSharpUtils.ConvertOperand(op));

                                // Clean SELECT/ORDERBY (remove comments)
                                var selectClean = SqlEmitHelpers.StripSqlComments(selectRaw);
                                if (string.IsNullOrWhiteSpace(selectClean))
                                {
                                    System.Console.WriteLine($"⚠️  Warning: SQL function '{f.Name}' has no SELECT clause.");
                                    selectClean = "*";
                                }
                                var orderClean = SqlEmitHelpers.StripSqlComments(orderRaw);

                                // Compose originals (with comments) and executed (no comments)
                                var originalSqlWithComments = SqlEmitHelpers.ComposeSql(selectRaw, dbTableName, originalWhere, orderRaw);
                                var executedSql = SqlEmitHelpers.ComposeSql(selectClean, dbTableName, whereClean, orderClean);

                                // 🚫 Final hard strip to guarantee NO comments reach Dapper
                                executedSql = SqlEmitHelpers.StripSqlComments(executedSql);

                                // Build method
                                var callLine = string.IsNullOrEmpty(paramDecl)
                                    ? $"var result = conn.QueryFirstOrDefault<{cleanSqlRowRecordType}>(sql);"
                                    : $"var result = conn.QueryFirstOrDefault<{cleanSqlRowRecordType}>(sql, parameters);";

                                sqlMethodCode = $@"
public static partial class GlobalFunctions
{{
    /// <summary>Auto-generated SQL for ESF function “{EscapeXml(f.Name)}”.</summary>
    public static void {methodName}()
    {{
        using var conn = DataAccess.GetConnection();

        // Original SQL (with comments) — preserved for review/debug:
        /*
{originalSqlWithComments}
        */

        {(string.IsNullOrEmpty(paramDecl) ? "" : paramDecl)}
        var sql = @""{executedSql}"";
        {callLine}
        if (result != null)
        {{
            {cleanSqlRowRecord}.Current.CopyFrom(result);
        }}
        else
        {{
            {cleanSqlRowRecord}.Current.SetEmpty();
        }}
    }}
}}";
                                break;
                            }
                        case "SETINQ":
                            {
                                // SELECT ... WHERE ... ORDER BY ... (returns a set)
                                var (whereClean, paramDecl, originalWhere) =
                                    SqlEmitHelpers.BuildWhereAndParams(whereRaw, cleanSqlRowRecord, op => CSharpUtils.ConvertOperand(op));

                                var selectClean = SqlEmitHelpers.StripSqlComments(selectRaw);
                                if (string.IsNullOrWhiteSpace(selectClean))
                                {
                                    System.Console.WriteLine($"⚠️  Warning: SQL function '{f.Name}' has no SELECT clause.");
                                    selectClean = "*";
                                }
                                var orderClean = SqlEmitHelpers.StripSqlComments(orderRaw);

                                // Compose originals (with comments) and executed (no comments)
                                var originalSqlWithComments = SqlEmitHelpers.ComposeSql(selectRaw, dbTableName, originalWhere, orderRaw);
                                var executedSql = SqlEmitHelpers.ComposeSql(selectClean, dbTableName, whereClean, orderClean);
                                executedSql = SqlEmitHelpers.StripSqlComments(executedSql);

                                // Generate method that returns a set
                                sqlMethodCode = $@"
                                public static partial class GlobalFunctions
                                {{
                                    /// <summary>Auto-generated SQL SETINQ for ESF function “{EscapeXml(f.Name)}”.</summary>
                                    public static IEnumerable<{cleanSqlRowRecordType}> {methodName}()
                                    {{
                                        using var conn = DataAccess.GetConnection();

                                        // Original SQL (with comments) — preserved for review/debug:
                                        /*
                                            {originalSqlWithComments}
                                        */

                                            {(string.IsNullOrEmpty(paramDecl) ? "" : paramDecl)}
                                            var sql = @""{executedSql}"";
                                            var rows = {(string.IsNullOrEmpty(paramDecl) ? 
                                                        $"conn.Query<{cleanSqlRowRecordType}>(sql)" : $"conn.Query<{cleanSqlRowRecordType}>(sql, parameters)")}.ToList();

                                            // Keep Current in sync with first row (VAGen ""current row"" convention)
                                            if (rows.Count > 0)
                                                                    {cleanSqlRowRecord}.Current.CopyFrom(rows[0]);
                                            else
                                                                    {cleanSqlRowRecord}.Current.SetEmpty();

                                            return rows;
                                        
                                    }}
                                }}
                                ";
                                break;
                            }

                        case "SETUPD":
                            {
                                // SELECT ... WHERE ... ORDER BY ... [FOR UPDATE OF ...] (returns a set that can be updated)
                                var (whereClean, paramDecl, originalWhere) =
                                    SqlEmitHelpers.BuildWhereAndParams(whereRaw, cleanSqlRowRecord, op => CSharpUtils.ConvertOperand(op));

                                var selectClean = SqlEmitHelpers.StripSqlComments(selectRaw);
                                if (string.IsNullOrWhiteSpace(selectClean))
                                {
                                    System.Console.WriteLine($"⚠️  Warning: SQL function '{f.Name}' has no SELECT clause.");
                                    selectClean = "*";
                                }
                                var orderClean = SqlEmitHelpers.StripSqlComments(orderRaw);

                                // Optional FOR UPDATE OF <cols> clause
                                var forUpdateRaw = f.SqlClauses?.FirstOrDefault(c => c?.ClauseType == "FORUPDATEOF")?.Text ?? string.Empty;
                                var forUpdateClean = SqlEmitHelpers.StripSqlComments(forUpdateRaw);
                                string forUpdateOrigTail = string.IsNullOrWhiteSpace(forUpdateRaw) ? "" : $" FOR UPDATE OF {forUpdateRaw.Trim()}";
                                string forUpdateCleanTail = string.IsNullOrWhiteSpace(forUpdateClean) ? "" : $" FOR UPDATE OF {forUpdateClean.Trim()}";

                                // Compose originals (with comments) and executed (no comments)
                                var originalSqlWithComments = SqlEmitHelpers.ComposeSql(selectRaw, dbTableName, originalWhere, orderRaw) + forUpdateOrigTail;
                                var executedSql = SqlEmitHelpers.ComposeSql(selectClean, dbTableName, whereClean, orderClean) + forUpdateCleanTail;
                                executedSql = SqlEmitHelpers.StripSqlComments(executedSql);

                                // Generate method that returns a set
                                sqlMethodCode = $@"
                                public static partial class GlobalFunctions
                                {{
                                    /// <summary>Auto-generated SQL SETUPD for ESF function “{EscapeXml(f.Name)}”.</summary>
                                    public static IEnumerable<{cleanSqlRowRecordType}> {methodName}()
                                    {{
                                        using var conn = DataAccess.GetConnection();

                                        // Original SQL (with comments) — preserved for review/debug:
                                        /*
                                            {originalSqlWithComments}
                                        */

                                        {(string.IsNullOrEmpty(paramDecl) ? "" : paramDecl)}
                                        var sql = @""{executedSql}"";
                                        var rows = {(string.IsNullOrEmpty(paramDecl) ? $"conn.Query<{cleanSqlRowRecordType}>(sql)" : $"conn.Query<{cleanSqlRowRecordType}>(sql, parameters)")}.ToList();

                                        // Keep Current in sync with first row
                                        if (rows.Count > 0)
                                            {cleanSqlRowRecord}.Current.CopyFrom(rows[0]);
                                        else
                                            {cleanSqlRowRecord}.Current.SetEmpty();

                                        return rows;
                                    }}
                                }}";
                                break;
                            }
                        case "SCAN":
                            {
                                // SELECT ... WHERE ... ORDER BY ... ; return next row on each call
                                var (whereClean, paramDecl, originalWhere) =
                                    SqlEmitHelpers.BuildWhereAndParams(whereRaw, cleanSqlRowRecord, op => CSharpUtils.ConvertOperand(op));

                                var selectClean = SqlEmitHelpers.StripSqlComments(selectRaw);
                                if (string.IsNullOrWhiteSpace(selectClean))
                                {
                                    System.Console.WriteLine($"⚠️  Warning: SQL function '{f.Name}' has no SELECT clause.");
                                    selectClean = "*";
                                }
                                var orderClean = SqlEmitHelpers.StripSqlComments(orderRaw);

                                // Compose originals (with comments) and executed (no comments)
                                var originalSqlWithComments = SqlEmitHelpers.ComposeSql(selectRaw, dbTableName, originalWhere, orderRaw);
                                var executedSql = SqlEmitHelpers.ComposeSql(selectClean, dbTableName, whereClean, orderClean);
                                executedSql = SqlEmitHelpers.StripSqlComments(executedSql);

                                // Give the per-function static fields unique, safe names
                                var safe = SanitizeFileName(methodName);

                                sqlMethodCode = $@"
                                public static partial class GlobalFunctions
                                {{
                                    // SCAN state for {EscapeXml(methodName)}
                                    private static System.Collections.Generic.List<{cleanSqlRowRecordType}>? __{safe}_SCAN_ROWS;
                                    private static int __{safe}_SCAN_POS;
                                    private static string? __{safe}_SCAN_KEY;

                                    /// <summary>Auto-generated SQL SCAN for ESF function “{EscapeXml(f.Name)}”.</summary>
                                    /// <returns>true if a row was fetched & copied to Current; false if no more rows</returns>
                                    public static bool {methodName}()
                                    {{
                                        using var conn = DataAccess.GetConnection();

                                        // Original SQL (with comments) — preserved for review/debug:
                                        /*
                                            {originalSqlWithComments}
                                        */

                                        {(string.IsNullOrEmpty(paramDecl) ? "" : paramDecl)}
                                        var sql = @""{executedSql}"";

                                        // Build a simple key from SQL + bound parameter values to detect changes between calls
                                        string key = sql;
                                        {(string.IsNullOrEmpty(paramDecl) ? "" :
                                                                    @"if (parameters is object p)
                                        {
                                            var props = p.GetType().GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                                            if (props != null && props.Length > 0)
                                            {
                                                var parts = new System.Collections.Generic.List<string>(props.Length);
                                                foreach (var pr in props)
                                                {
                                                    var val = pr.GetValue(p);
                                                    parts.Add(pr.Name + ""="" + (val?.ToString() ?? """"));
                                                }
                                                key += ""|"" + string.Join("","", parts);
                                            }
                                        }")}

                                        // (Re)load the set if first time or the key changed
                                        if (__{safe}_SCAN_ROWS == null || !string.Equals(__{safe}_SCAN_KEY, key, System.StringComparison.Ordinal))
                                        {{
                                            var rows = {(string.IsNullOrEmpty(paramDecl)
                                                                            ? $"conn.Query<{cleanSqlRowRecordType}>(sql)"
                                                                            : $"conn.Query<{cleanSqlRowRecordType}>(sql, parameters)")}?.ToList()
                                                ?? new System.Collections.Generic.List<{cleanSqlRowRecordType}>();

                                            __{safe}_SCAN_ROWS = rows;
                                            __{safe}_SCAN_POS  = 0;
                                            __{safe}_SCAN_KEY  = key;
                                        }}

                                        // Yield next row (if any)
                                        if (__{safe}_SCAN_ROWS != null && __{safe}_SCAN_POS < __{safe}_SCAN_ROWS.Count)
                                        {{
                                            var row = __{safe}_SCAN_ROWS[__{safe}_SCAN_POS++];
                                            {cleanSqlRowRecord}.Current.CopyFrom(row);
                                            return true;
                                        }}

                                        {cleanSqlRowRecord}.Current.SetEmpty();
                                        return false;
                                    }}
                                }}";
                                break;
                            }

                        default:
                            {
                                sqlMethodCode = $@"
                                public static partial class GlobalFunctions
                                {{
                                    /// <summary>Auto-generated SQL for ESF function “{EscapeXml(f.Name)}”.</summary>
                                    public static void {methodName}()
                                    {{
                                        // TODO: Implement SQL function logic for {f.Name}
                                        // {f.Option} option is not yet supported in structured mode.
                                    }}
                                }}";
                                break;
                            }
                    }

                    var cu = Microsoft.CodeAnalysis.CSharp.CSharpSyntaxTree.ParseText(sqlMethodCode).GetCompilationUnitRoot();
                    var cls = cu.Members.OfType<Microsoft.CodeAnalysis.CSharp.Syntax.ClassDeclarationSyntax>().First();

                    var extraUsings = new HashSet<string>(new[]
                    {
                "System", "System.Collections.Generic", "System.Linq", "Dapper"
            }, System.StringComparer.Ordinal);

                    WriteMembersFile(
                        functionsSqlDir,
                        $"{SanitizeFileName(methodName)}.cs",
                        ns,
                        new[] { cls },
                        extraUsings: extraUsings,
                        appSubNamespaceUsings: subUsings);
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

    /// <summary>
    /// Copy a runtime helper file (AidKey, ConsoleMapRenderer, ConverseConsole) into the
    /// output runtime directory, rewriting its namespace and using directives to the
    /// specified application namespace.  This allows the runtime helpers to be
    /// embedded in the generated console project without depending on the EsfParser
    /// assembly.  We replace:
    ///   namespace EsfParser.Runtime → namespace {appNs}.Runtime
    ///   using EsfParser.Tags;      → using {appNs}.Maps;
    /// Fully qualified EsfParser.Tags references are stripped by the global
    /// replacement of EsfRuntime. later in WriteMembersFile.
    /// </summary>
    private static void CopyRuntimeHelperWithNamespaceRewrite(string src, string dst, string appNs)
    {
        CopyFileIfExists(src, dst, t =>
        {
            var s = t.Replace("\r\n", "\n");
            // rewrite namespace declarations (both file-scoped and block)
            s = s.Replace("namespace EsfParser.Runtime", $"namespace {appNs}.Runtime");
            // rewrite using directives for Tags
            s = s.Replace("using EsfParser.Tags;", $"using {appNs}.Maps;");
            s = s.Replace("using EsfParser.Tags", $"using {appNs}.Maps");
            return s;
        });
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
