// CSharpUtils.cs  ───────────────────────────────────────────────────────────
using EsfParser.Esf;
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace EsfParser.CodeGen;

/// <summary>
/// Shared helpers for all ESF-to-C# translators (MOVE, MOVEA, IF, WHILE, …).
/// </summary>
public static class CSharpUtils
{
    // ── One-time program context (set right after parsing) ─────────────────
    public static EsfProgram? Program { get; set; }

    // ── Global holder prefixes ────────────────────────────────────────────
    private const string WORK_PREFIX = "GlobalWorkstor.";
    private const string SQL_PREFIX = "GlobalSqlRow.";
    private const string MAP_PREFIX = "GlobalMaps.";
    private const string ITEM_PREFIX = "GlobalItems.";
    private const string TABLE_PREFIX = "GlobalTables.";
    private const string SYS_PREFIX = "EzFunctions.";   // EZ* special vars

    // ── Helpers ───────────────────────────────────────────────────────────
    public static string Indent(int level) => new(' ', level * 4);

    public static string MapCsType(string t, int dec) =>
        (t ?? string.Empty).ToUpperInvariant() switch
        {
            "BIN" => "int",
            "NUM" or "PACK" or "PACF" => dec > 0 ? "decimal" : "int",
            "CHA" or "DBCS" or "MIX" => "string",
            _ => "string"
        };

    public static string CleanName(string raw)
    {
        if (string.IsNullOrWhiteSpace(raw)) return raw;
        var n = raw.Replace('-', '_');
        return char.IsLetter(n[0]) || n[0] == '_' ? n : "_" + n;
    }

    // ── Pre-compiled regexes ──────────────────────────────────────────────
    private static readonly Regex _numLiteral = new(@"^-?\d+(\.\d+)?$", RegexOptions.Compiled);

    //  word(.word)*[ idx ]   — captures whole path so we can keep REC.FIELD intact
    private static readonly Regex _subscript = new(@"([\w\.]+)\[([^\]]+)\]", RegexOptions.Compiled);

    // ──────────────────────────────────────────────────────────────────────
    //  Convert a single ESF operand to fully-qualified C#
    // ──────────────────────────────────────────────────────────────────────
    public static string ConvertOperand(string operand)
    {
        operand = operand.Trim();
        if (operand.Length == 0) return operand;

        // 1️⃣  literals
        if ((operand.StartsWith('"') && operand.EndsWith('"')) ||
            (operand.StartsWith('\'') && operand.EndsWith('\'')))
        {
            var body = operand[1..^1].Replace("\\", "\\\\").Replace("\"", "\\\"");
            return $"\"{body}\"";
        }
        if (_numLiteral.IsMatch(operand)) return operand;

        var prog = EsfLogicToCs.program ?? Program;

        // 2️⃣  array subscripts
        operand = _subscript.Replace(operand, m =>
        {
            string basePath = m.Groups[1].Value;             // e.g. D133W01.APPL
            string rawIdx = m.Groups[2].Value.Trim();      // e.g. STEVAPPL or 5

            string idxExpr = _numLiteral.IsMatch(rawIdx)
                             ? (int.Parse(rawIdx) - 1).ToString()
                             : TryRecordField(basePath, rawIdx, prog)
                               ?? QualifyIdentifier(rawIdx, prog);

            return $"{basePath}[{idxExpr}]";
        });

        // 3️⃣  dotted reference  REC.FIELD or MAP.VFLD …
        int dot = operand.IndexOf('.');
        if (dot > 0)
        {
            string left = CleanName(operand[..dot]);
            string right = CleanName(operand[(dot + 1)..]);
            return $"{Prefix(left, prog)}{left}.{right}";
        }

        // 4️⃣  plain identifier
        string id = CleanName(operand);
        return $"{Prefix(id, prog)}{id}";
    }

    private static string RecordOrg(string name, EsfProgram? p)
    {
        var rec = p?.Records.Records
                     .FirstOrDefault(r => CleanName(r.Name)
                         .Equals(name, StringComparison.OrdinalIgnoreCase));
        return rec?.Org.ToUpperInvariant() ?? string.Empty;
    }

    // ── Qualify helper ────────────────────────────────────────────────────
    internal static string QualifyIdentifier(string rawId, EsfProgram? p) =>
        $"{Prefix(CleanName(rawId), p)}{CleanName(rawId)}";

    private static string Prefix(string name, EsfProgram? p) =>
           IsItem(name, p) ? ITEM_PREFIX
         : IsRecord(name, p) ? (RecordOrg(name, p) == "SQLROW"
                         ? SQL_PREFIX
                         : WORK_PREFIX)
         : IsMap(name, p) ? MAP_PREFIX
         : IsTable(name, p) ? TABLE_PREFIX
         : name.StartsWith("EZ", StringComparison.OrdinalIgnoreCase)
                              ? SYS_PREFIX
                              : string.Empty;

    // ── Record-field shortcut for array indices ───────────────────────────
    private static string? TryRecordField(string basePath, string rawIdx, EsfProgram? p)
    {
        if (p == null) return null;

        string recordName = CleanName(basePath.Split('.')[0]);   // strip field if any
        var rec = p.Records.Records
                    .FirstOrDefault(r => CleanName(r.Name)
                        .Equals(recordName, StringComparison.OrdinalIgnoreCase));
        if (rec == null) return null;

        string fieldName = CleanName(rawIdx);
        // If you have a definitive field list per record, validate here.
        string prefix = rec.Org.Equals("SQLROW", StringComparison.OrdinalIgnoreCase)
                 ? SQL_PREFIX
                 : WORK_PREFIX;
        return $"{prefix}{recordName}.{fieldName}";
    }

    // ── Null-safe look-ups ────────────────────────────────────────────────
    private static bool IsItem(string n, EsfProgram? p) =>
        p?.Items.Items.Any(i => CleanName(i.Name).Equals(n, StringComparison.OrdinalIgnoreCase)) == true;

    private static bool IsRecord(string n, EsfProgram? p) =>
        p?.Records.Records.Any(r => CleanName(r.Name).Equals(n, StringComparison.OrdinalIgnoreCase)) == true;

    private static bool IsMap(string n, EsfProgram? p) =>
        p?.Maps.Maps.Any(m => CleanName(m.MapName).Equals(n, StringComparison.OrdinalIgnoreCase)) == true;

    private static bool IsTable(string n, EsfProgram? p) =>
        p?.Tables.Tables.Any(t => CleanName(t.Name).Equals(n, StringComparison.OrdinalIgnoreCase)) == true;
}
