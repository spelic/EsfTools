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
    private static void Structured_Write(EsfProgram program, string root, string ns,
        System.Collections.Generic.IList<string>? generationDiagnostics = null)
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
                // §9: isolate per-function failures so one unsupported function
                // doesn't abort the entire conversion.
                try { EmitOneFunction(f, program, functionsLogicDir, functionsSqlDir, ns, subUsings); }
                catch (Exception ex)
                {
                    var msg = $"Skipped function '{f.Name}' ({f.Option}): {ex.Message}";
                    System.Console.WriteLine($"⚠️  {msg}");
                    generationDiagnostics?.Add(msg);
                }
            }
        }

        System.Console.WriteLine("✅  Structured project created.");
    }

    // §4: one ESF function → one C# method file (LOGIC or SQL). Extracted from the
    // former ~550-line Structured_Write so orchestration and per-function emission
    // are separable.
    private static void EmitOneFunction(
        FuncTag f,
        EsfProgram program,
        string functionsLogicDir,
        string functionsSqlDir,
        string ns,
        string[] subUsings)
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

                    // TODO: System.InvalidOperationException: 'SQL function 'NA70P30' references record 'NA70M05' which does not exist in the program.'
                    // ADD SUPPORT FOR MAP OBJECT AND VFIELDS

                    if (sqlRecord == null)
                    {
                        throw new EsfTranslationException(
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
                        case "ADD":
                            {
                                var insertColsRaw = f.SqlClauses?.FirstOrDefault(c => c?.ClauseType == "INSERTCOLNAME")?.Text ?? "";
                                var valuesRaw = f.SqlClauses?.FirstOrDefault(c => c?.ClauseType == "VALUES")?.Text ?? "";

                                var (colsClean, colsOrig, colsTokens) = SqlEmitHelpers.PrepClause(insertColsRaw);
                                var (valsClean, valsOrig, valsTokens) = SqlEmitHelpers.PrepClause(valuesRaw);
                                var tokens = SqlEmitHelpers.MergeTokens(colsTokens, valsTokens);

                                var originalSqlWithComments = $"INSERT INTO {dbTableName} ({colsOrig}) VALUES ({valsOrig})";
                                var executedSql = SqlEmitHelpers.ComposeInsert(dbTableName, colsClean, valsClean);
                                executedSql = SqlEmitHelpers.StripSqlComments(executedSql);

                                var paramDecl = SqlEmitHelpers.BuildParametersDecl(tokens, cleanSqlRowRecord, op => CSharpUtils.ConvertOperand(op));

                                sqlMethodCode = $@"
public static partial class GlobalFunctions
{{
    /// <summary>Auto-generated SQL ADD for ESF function “{EscapeXml(f.Name)}”.</summary>
    public static int {methodName}()
    {{
        using var conn = DataAccess.GetConnection();
        /*
{originalSqlWithComments}
        */
        {(string.IsNullOrEmpty(paramDecl) ? "" : paramDecl)}
        var sql = @""{executedSql}"";
        var rows = {(string.IsNullOrEmpty(paramDecl) ? "conn.Execute(sql)" : "conn.Execute(sql, parameters)")};
        return rows;
    }}
}}";
                                break;
                            }

                        case "UPDATE":
                            {
                                // Read for update (single row) — SELECT ... [FOR UPDATE OF ...]
                                var (whereClean, whereOrig, whereTokens) = SqlEmitHelpers.PrepClause(whereRaw);
                                var (selClean, selOrig, selTokens) = SqlEmitHelpers.PrepClause(selectRaw); if (string.IsNullOrWhiteSpace(selClean)) selClean = "*";
                                var (ordClean, ordOrig, ordTokens) = SqlEmitHelpers.PrepClause(orderRaw);
                                var forUpdRaw = f.SqlClauses?.FirstOrDefault(c => c?.ClauseType == "FORUPDATEOF")?.Text ?? "";
                                var (forUpdClean, forUpdOrig, _) = SqlEmitHelpers.PrepClause(forUpdRaw, normalizeHostVars: false);

                                var tokens = SqlEmitHelpers.MergeTokens(selTokens, whereTokens, ordTokens);

                                var originalSqlWithComments = SqlEmitHelpers.ComposeSelect(selOrig, dbTableName, whereOrig, ordOrig, forUpdOrig);
                                var executedSql = SqlEmitHelpers.ComposeSelect(selClean, dbTableName, whereClean, ordClean, forUpdClean);
                                executedSql = SqlEmitHelpers.StripSqlComments(executedSql);

                                var paramDecl = SqlEmitHelpers.BuildParametersDecl(tokens, cleanSqlRowRecord, op => CSharpUtils.ConvertOperand(op));
                                var callLine = string.IsNullOrEmpty(paramDecl)
                                    ? $"var result = conn.QueryFirstOrDefault<{cleanSqlRowRecordType}>(sql);"
                                    : $"var result = conn.QueryFirstOrDefault<{cleanSqlRowRecordType}>(sql, parameters);";

                                sqlMethodCode = $@"
public static partial class GlobalFunctions
{{
    /// <summary>Auto-generated SQL UPDATE (read-for-update) for ESF function “{EscapeXml(f.Name)}”.</summary>
    public static void {methodName}()
    {{
        using var conn = DataAccess.GetConnection();
        /*
{originalSqlWithComments}
        */
        {(string.IsNullOrEmpty(paramDecl) ? "" : paramDecl)}
        var sql = @""{executedSql}"";
        {callLine}
        if (result != null) {cleanSqlRowRecord}.Current.CopyFrom(result);
        else {cleanSqlRowRecord}.Current.SetEmpty();
    }}
}}";
                                break;
                            }

                        case "REPLACE":
                            {
                                // UPDATE ... SET <SET> WHERE <WHERE>
                                var setRaw = f.SqlClauses?.FirstOrDefault(c => c?.ClauseType == "SET")?.Text ?? "";
                                var (setClean, setOrig, setTokens) = SqlEmitHelpers.PrepClause(setRaw);
                                var (whereClean, whereOrig, whereTokens) = SqlEmitHelpers.PrepClause(whereRaw);
                                var tokens = SqlEmitHelpers.MergeTokens(setTokens, whereTokens);

                                var originalSqlWithComments = SqlEmitHelpers.ComposeUpdateSet(dbTableName, setOrig, whereOrig);
                                var executedSql = SqlEmitHelpers.ComposeUpdateSet(dbTableName, setClean, whereClean);
                                executedSql = SqlEmitHelpers.StripSqlComments(executedSql);

                                var paramDecl = SqlEmitHelpers.BuildParametersDecl(tokens, cleanSqlRowRecord, op => CSharpUtils.ConvertOperand(op));

                                sqlMethodCode = $@"
public static partial class GlobalFunctions
{{
    /// <summary>Auto-generated SQL REPLACE for ESF function “{EscapeXml(f.Name)}”.</summary>
    public static int {methodName}()
    {{
        using var conn = DataAccess.GetConnection();
        /*
{originalSqlWithComments}
        */
        {(string.IsNullOrEmpty(paramDecl) ? "" : paramDecl)}
        var sql = @""{executedSql}"";
        var rows = {(string.IsNullOrEmpty(paramDecl) ? "conn.Execute(sql)" : "conn.Execute(sql, parameters)")};
        return rows;
    }}
}}";
                                break;
                            }

                        case "DELETE":
                            {
                                var (whereClean, whereOrig, whereTokens) = SqlEmitHelpers.PrepClause(whereRaw);

                                var originalSqlWithComments = SqlEmitHelpers.ComposeDelete(dbTableName, whereOrig);
                                var executedSql = SqlEmitHelpers.ComposeDelete(dbTableName, whereClean);
                                executedSql = SqlEmitHelpers.StripSqlComments(executedSql);

                                var paramDecl = SqlEmitHelpers.BuildParametersDecl(whereTokens, cleanSqlRowRecord, op => CSharpUtils.ConvertOperand(op));

                                sqlMethodCode = $@"
public static partial class GlobalFunctions
{{
    /// <summary>Auto-generated SQL DELETE for ESF function “{EscapeXml(f.Name)}”.</summary>
    public static int {methodName}()
    {{
        using var conn = DataAccess.GetConnection();
        /*
{originalSqlWithComments}
        */
        {(string.IsNullOrEmpty(paramDecl) ? "" : paramDecl)}
        var sql = @""{executedSql}"";
        var rows = {(string.IsNullOrEmpty(paramDecl) ? "conn.Execute(sql)" : "conn.Execute(sql, parameters)")};
        return rows;
    }}
}}";
                                break;
                            }

                        case "INQUIRY": // single-row read — same emission as SETINQ
                        case "SETINQ":
                            {
                                var (whereClean, whereOrig, whereTokens) = SqlEmitHelpers.PrepClause(whereRaw);
                                var (selClean, selOrig, selTokens) = SqlEmitHelpers.PrepClause(selectRaw); if (string.IsNullOrWhiteSpace(selClean)) selClean = "*";
                                var (ordClean, ordOrig, ordTokens) = SqlEmitHelpers.PrepClause(orderRaw);

                                var tokens = SqlEmitHelpers.MergeTokens(selTokens, whereTokens, ordTokens);

                                var originalSqlWithComments = SqlEmitHelpers.ComposeSelect(selOrig, dbTableName, whereOrig, ordOrig);
                                var executedSql = SqlEmitHelpers.ComposeSelect(selClean, dbTableName, whereClean, ordClean);
                                executedSql = SqlEmitHelpers.StripSqlComments(executedSql);

                                var paramDecl = SqlEmitHelpers.BuildParametersDecl(tokens, cleanSqlRowRecord, op => CSharpUtils.ConvertOperand(op));

                                sqlMethodCode = $@"
public static partial class GlobalFunctions
{{
    /// <summary>Auto-generated SQL SETINQ for ESF function “{EscapeXml(f.Name)}”.</summary>
    public static IEnumerable<{cleanSqlRowRecordType}> {methodName}()
    {{
        using var conn = DataAccess.GetConnection();
        /*
{originalSqlWithComments}
        */
        {(string.IsNullOrEmpty(paramDecl) ? "" : paramDecl)}
        var sql = @""{executedSql}"";
        var rows = {(string.IsNullOrEmpty(paramDecl) ? $"conn.Query<{cleanSqlRowRecordType}>(sql)" : $"conn.Query<{cleanSqlRowRecordType}>(sql, parameters)")}.ToList();

        if (rows.Count > 0) {cleanSqlRowRecord}.Current.CopyFrom(rows[0]); else {cleanSqlRowRecord}.Current.SetEmpty();
        return rows;
    }}
}}";
                                break;
                            }

                        case "SETUPD":
                            {
                                var (whereClean, whereOrig, whereTokens) = SqlEmitHelpers.PrepClause(whereRaw);
                                var (selClean, selOrig, selTokens) = SqlEmitHelpers.PrepClause(selectRaw); if (string.IsNullOrWhiteSpace(selClean)) selClean = "*";
                                var (ordClean, ordOrig, ordTokens) = SqlEmitHelpers.PrepClause(orderRaw);
                                var forUpdRaw = f.SqlClauses?.FirstOrDefault(c => c?.ClauseType == "FORUPDATEOF")?.Text ?? "";
                                var (forUpdClean, forUpdOrig, _) = SqlEmitHelpers.PrepClause(forUpdRaw, normalizeHostVars: false);

                                var tokens = SqlEmitHelpers.MergeTokens(selTokens, whereTokens, ordTokens);

                                var originalSqlWithComments = SqlEmitHelpers.ComposeSelect(selOrig, dbTableName, whereOrig, ordOrig, forUpdOrig);
                                var executedSql = SqlEmitHelpers.ComposeSelect(selClean, dbTableName, whereClean, ordClean, forUpdClean);
                                executedSql = SqlEmitHelpers.StripSqlComments(executedSql);

                                var paramDecl = SqlEmitHelpers.BuildParametersDecl(tokens, cleanSqlRowRecord, op => CSharpUtils.ConvertOperand(op));

                                sqlMethodCode = $@"
public static partial class GlobalFunctions
{{
    /// <summary>Auto-generated SQL SETUPD for ESF function “{EscapeXml(f.Name)}”.</summary>
    public static IEnumerable<{cleanSqlRowRecordType}> {methodName}()
    {{
        using var conn = DataAccess.GetConnection();
        /*
{originalSqlWithComments}
        */
        {(string.IsNullOrEmpty(paramDecl) ? "" : paramDecl)}
        var sql = @""{executedSql}"";
        var rows = {(string.IsNullOrEmpty(paramDecl) ? $"conn.Query<{cleanSqlRowRecordType}>(sql)" : $"conn.Query<{cleanSqlRowRecordType}>(sql, parameters)")}.ToList();

        if (rows.Count > 0) {cleanSqlRowRecord}.Current.CopyFrom(rows[0]); else {cleanSqlRowRecord}.Current.SetEmpty();
        return rows;
    }}
}}";
                                break;
                            }

                        case "SCAN":
                            {
                                // SELECT ... WHERE ... ORDER BY ... ; return next row on each call
                                var (whereClean, whereOrig, whereTokens) = SqlEmitHelpers.PrepClause(whereRaw);
                                var (selClean, selOrig, selTokens) = SqlEmitHelpers.PrepClause(selectRaw); if (string.IsNullOrWhiteSpace(selClean)) selClean = "*";
                                var (ordClean, ordOrig, ordTokens) = SqlEmitHelpers.PrepClause(orderRaw);

                                // Merge tokens from ALL clauses so we bind params for SELECT/WHERE/ORDER BY as needed
                                var allTokens = SqlEmitHelpers.MergeTokens(selTokens, whereTokens, ordTokens);
                                var paramDecl = SqlEmitHelpers.BuildParametersDecl(allTokens, cleanSqlRowRecord, op => CSharpUtils.ConvertOperand(op));
                                var hasParams = !string.IsNullOrWhiteSpace(paramDecl);

                                var originalSqlWithComments = SqlEmitHelpers.ComposeSql(selOrig, dbTableName, whereOrig, ordOrig);
                                var executedSql = SqlEmitHelpers.ComposeSql(selClean, dbTableName, whereClean, ordClean);
                                executedSql = SqlEmitHelpers.StripSqlComments(executedSql);

                                var safe = SanitizeFileName(methodName);

                                sqlMethodCode = $@"
public static partial class GlobalFunctions
{{
    private static System.Collections.Generic.List<{cleanSqlRowRecordType}>? __{safe}_SCAN_ROWS;
    private static int __{safe}_SCAN_POS;
    private static string? __{safe}_SCAN_KEY;

    /// <summary>Auto-generated SQL SCAN for ESF function “{EscapeXml(f.Name)}”.</summary>
    public static bool {methodName}()
    {{
        using var conn = DataAccess.GetConnection();

        // Original SQL (with comments) — preserved for review/debug:
        /*
{originalSqlWithComments}
        */

        {(hasParams ? paramDecl + "\n        " : "")}var sql = @""{executedSql}"";

        // Build a key that changes when SQL or bound values change
        string key = sql;
        {(hasParams ? @"
        if (parameters is object p)
        {
            var props = p.GetType().GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            if (props?.Length > 0)
            {
                var parts = new System.Collections.Generic.List<string>(props.Length);
                foreach (var pr in props)
                    parts.Add(pr.Name + ""="" + (pr.GetValue(p)?.ToString() ?? """"));
                key += ""|"" + string.Join("","", parts);
            }
        }" : "")}

        // (Re)load the set if first time or the key changed
        if (__{safe}_SCAN_ROWS == null || !string.Equals(__{safe}_SCAN_KEY, key, StringComparison.Ordinal))
        {{
            var rows = {(hasParams ? $"conn.Query<{cleanSqlRowRecordType}>(sql, parameters)" : $"conn.Query<{cleanSqlRowRecordType}>(sql)")}.ToList();
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

                        case "SCANBACK":
                            {
                                // Build clauses
                                var (whereClean, whereOrig, whereTokens) = SqlEmitHelpers.PrepClause(whereRaw);
                                var (selClean, selOrig, selTokens) = SqlEmitHelpers.PrepClause(selectRaw); if (string.IsNullOrWhiteSpace(selClean)) selClean = "*";
                                var (ordClean, ordOrig, ordTokens) = SqlEmitHelpers.PrepClause(orderRaw);

                                // Params from ALL clauses (SELECT/WHERE/ORDER BY)
                                var allTokens = SqlEmitHelpers.MergeTokens(selTokens, whereTokens, ordTokens);
                                var paramDecl = SqlEmitHelpers.BuildParametersDecl(allTokens, cleanSqlRowRecord, op => CSharpUtils.ConvertOperand(op));
                                var hasParams = !string.IsNullOrWhiteSpace(paramDecl);

                                // Compose SQL
                                var originalSqlWithComments = SqlEmitHelpers.ComposeSql(selOrig, dbTableName, whereOrig, ordOrig);
                                var executedSql = SqlEmitHelpers.ComposeSql(selClean, dbTableName, whereClean, ordClean);
                                executedSql = SqlEmitHelpers.StripSqlComments(executedSql);

                                // Unique static fields per function
                                var safe = SanitizeFileName(methodName);

                                sqlMethodCode = $@"
public static partial class GlobalFunctions
{{
    private static System.Collections.Generic.List<{cleanSqlRowRecordType}>? __{safe}_SCAN_ROWS;
    private static int __{safe}_SCAN_POS;
    private static string? __{safe}_SCAN_KEY;

    /// <summary>Auto-generated SQL SCANBACK for ESF function “{EscapeXml(f.Name)}”.</summary>
    public static bool {methodName}()
    {{
        using var conn = DataAccess.GetConnection();

        // Original SQL (with comments) — preserved for review/debug:
        /*
{originalSqlWithComments}
        */

        {(hasParams ? paramDecl + "\n        " : "")}var sql = @""{executedSql}"";

        // Build key from SQL + bound values (if any)
        string key = sql;
        {(hasParams ? @"
        if (parameters is object p)
        {
            var props = p.GetType().GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            if (props?.Length > 0)
            {
                var parts = new System.Collections.Generic.List<string>(props.Length);
                foreach (var pr in props)
                    parts.Add(pr.Name + ""="" + (pr.GetValue(p)?.ToString() ?? """"));
                key += ""|"" + string.Join("","", parts);
            }
        }" : "")}

        // (Re)load if first call or the key changed
        if (__{safe}_SCAN_ROWS == null || !string.Equals(__{safe}_SCAN_KEY, key, StringComparison.Ordinal))
        {{
            var rows = {(hasParams ? $"conn.Query<{cleanSqlRowRecordType}>(sql, parameters)" : $"conn.Query<{cleanSqlRowRecordType}>(sql)")}.ToList();
            __{safe}_SCAN_ROWS = rows;
            __{safe}_SCAN_POS  = rows.Count; // start from end
            __{safe}_SCAN_KEY  = key;
        }}

        // Step backward
        if (__{safe}_SCAN_ROWS != null && __{safe}_SCAN_POS > 0)
        {{
            var row = __{safe}_SCAN_ROWS[--__{safe}_SCAN_POS];
            {cleanSqlRowRecord}.Current.CopyFrom(row);
            return true;
        }}

        {cleanSqlRowRecord}.Current.SetEmpty();
        return false;
    }}
}}";
                                break;
                            }

                        case "SQLEXEC":
                            {
                                var sqlRaw = f.SqlClauses?.FirstOrDefault(c =>
                                                string.Equals(c?.ClauseType, "SQL", StringComparison.OrdinalIgnoreCase) ||
                                                string.Equals(c?.ClauseType, "STATEMENT", StringComparison.OrdinalIgnoreCase) ||
                                                string.Equals(c?.ClauseType, "SQLTEXT", StringComparison.OrdinalIgnoreCase)
                                            )?.Text ?? string.Empty;

                                var (sqlClean, sqlOrig, sqlTokens) = SqlEmitHelpers.PrepClause(sqlRaw);
                                var originalSqlWithComments = sqlOrig;
                                var executedSql = SqlEmitHelpers.StripSqlComments(sqlClean);
                                var paramDecl = SqlEmitHelpers.BuildParametersDecl(sqlTokens, cleanSqlRowRecord, op => CSharpUtils.ConvertOperand(op));
                                bool isSelect = executedSql.TrimStart().StartsWith("SELECT", StringComparison.OrdinalIgnoreCase);

                                if (isSelect)
                                {
                                    {
                                        sqlMethodCode = $@"
public static partial class GlobalFunctions
{{
    /// <summary>Auto-generated SQL SQLEXEC (SELECT) for ESF function “{EscapeXml(f.Name)}”.</summary>
    public static IEnumerable<{cleanSqlRowRecordType}> {methodName}()
    {{
        using var conn = DataAccess.GetConnection();
        /*
{originalSqlWithComments}
        */
        {(string.IsNullOrEmpty(paramDecl) ? "" : paramDecl)}
        var sql = @""{executedSql}"";
        var rows = {(string.IsNullOrEmpty(paramDecl) ? $"conn.Query<{cleanSqlRowRecordType}>(sql)" : $"conn.Query<{cleanSqlRowRecordType}>(sql, parameters)")}.ToList();
        if (rows.Count > 0) {cleanSqlRowRecord}.Current.CopyFrom(rows[0]); else {cleanSqlRowRecord}.Current.SetEmpty();
        return rows;
    }}
}}";
                                    }
                                }
                                else
                                {
                                    {
                                        sqlMethodCode = $@"
public static partial class GlobalFunctions
{{
    /// <summary>Auto-generated SQL SQLEXEC (non-SELECT) for ESF function “{EscapeXml(f.Name)}”.</summary>
    public static int {methodName}()
    {{
        using var conn = DataAccess.GetConnection();
        /*
{originalSqlWithComments}
        */
        {(string.IsNullOrEmpty(paramDecl) ? "" : paramDecl)}
        var sql = @""{executedSql}"";
        var rows = {(string.IsNullOrEmpty(paramDecl) ? "conn.Execute(sql)" : "conn.Execute(sql, parameters)")};
        return rows;
    }}
}}";
                                    }
                                }
                                break;
                            }

                        case "CLOSE":
                            {
                                // SQL cursors aren’t held with Dapper; honor semantics by clearing Current.
                                sqlMethodCode = $@"
public static partial class GlobalFunctions
{{
    /// <summary>Auto-generated CLOSE for ESF function “{EscapeXml(f.Name)}”.</summary>
    public static void {methodName}()
    {{
        {cleanSqlRowRecord}.Current.SetEmpty();
        // NOTE: If you want to also clear SCAN/SCANBACK buffers, say the word;
        // we can centralize set caching and release it here.
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
    // Matches a "namespace <something>.Runtime" declaration regardless of the original
    // root namespace, so a stale/hardcoded namespace in a helper source can't leak through.
    private static readonly System.Text.RegularExpressions.Regex _runtimeNsRegex =
        new(@"namespace\s+[\w.]+\.Runtime", System.Text.RegularExpressions.RegexOptions.Compiled);

    private static void CopyRuntimeHelperWithNamespaceRewrite(string src, string dst, string appNs)
    {
        CopyFileIfExists(src, dst, t =>
        {
            var s = t.Replace("\r\n", "\n");
            // rewrite namespace declarations (both file-scoped and block), robust to the
            // original root namespace (e.g. EsfParser.Runtime or a stale generated one).
            s = _runtimeNsRegex.Replace(s, $"namespace {appNs}.Runtime");
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
