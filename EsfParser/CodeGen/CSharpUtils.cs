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
    // NOTE: this remains static for now to avoid threading the program through every
    // translator call site. Assigning it rebuilds the O(1) symbol-lookup caches below,
    // which replaced the previous per-operand linear scans over every record/item/map/table.
    private static EsfProgram? _program;
    public static EsfProgram? Program
    {
        get => _program;
        set { _program = value; RebuildSymbolCaches(value); }
    }

    // ── Symbol lookup caches (clean-name keyed, case-insensitive) ──────────
    // Rebuilt once whenever Program is assigned.
    private static Dictionary<string, Tags.ItemTag> _itemsByName = new(StringComparer.OrdinalIgnoreCase);
    private static Dictionary<string, Tags.RecordTag> _recordsByName = new(StringComparer.OrdinalIgnoreCase);
    private static Dictionary<string, Tags.MapTag> _mapsByName = new(StringComparer.OrdinalIgnoreCase);
    private static Dictionary<string, Tags.TableTag> _tablesByName = new(StringComparer.OrdinalIgnoreCase);
    private static Dictionary<string, Tags.RecordTag> _recordByFieldName = new(StringComparer.OrdinalIgnoreCase);

    private static void RebuildSymbolCaches(EsfProgram? p)
    {
        var cmp = StringComparer.OrdinalIgnoreCase;
        _itemsByName = new(cmp);
        _recordsByName = new(cmp);
        _mapsByName = new(cmp);
        _tablesByName = new(cmp);
        _recordByFieldName = new(cmp);
        if (p == null) return;

        foreach (var i in p.Items.Items)
            _itemsByName.TryAdd(CleanName(i.Name), i);          // first wins, mirrors FirstOrDefault

        foreach (var r in p.Records.Records)
        {
            _recordsByName.TryAdd(CleanName(r.Name), r);
            if (r.Items == null) continue;
            foreach (var it in r.Items)
                _recordByFieldName.TryAdd(CleanName(it.Name), r);
        }

        foreach (var m in p.Maps.Maps)
            _mapsByName.TryAdd(CleanName(m.MapName), m);

        foreach (var t in p.Tables.Tables)
            _tablesByName.TryAdd(CleanName(t.Name), t);
    }

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

        var prog = Program;

        // 2️⃣  array subscripts (qualify the index expression)
        operand = _subscript.Replace(operand, m =>
        {
            string basePath = m.Groups[1].Value.Trim();  // e.g. D133W01.APPL or REC
            string rawIdx = m.Groups[2].Value.Trim();  // e.g. STEVAPPL or 5

            string idxExpr = _numLiteral.IsMatch(rawIdx)
                             ? (int.Parse(rawIdx) - 1).ToString() // 1-based -> 0-based
                             : TryRecordField(basePath, rawIdx, prog)
                               ?? QualifyIdentifier(rawIdx, prog);

            // Leave basePath as-is; we’ll qualify the left side later if needed
            return $"{basePath}[{idxExpr}]";
        });

        // 3️⃣  dotted reference: LEFT.(RIGHT[...]) — handle arrays on LEFT too
        //     We must qualify by the *base name* of LEFT (without any [..]).
        int dot = operand.IndexOf('.');
        if (dot > 0)
        {
            string leftRaw = operand[..dot];               // could be "REC" or "REC[...]" or "MAP"
            string rightRaw = operand[(dot + 1)..];        // e.g. "FIELD" or "FIELD[expr]"

            // Split leftRaw into base + optional [..] index suffix
            string leftBase = leftRaw;
            string leftIndexSuffix = string.Empty;
            int leftBracket = leftRaw.IndexOf('[');
            if (leftBracket >= 0)
            {
                leftBase = leftRaw[..leftBracket];
                leftIndexSuffix = leftRaw[leftBracket..];  // includes '['
            }

            string leftId = CleanName(leftBase);
            string qualifiedLeft = $"{Prefix(leftId, prog)}{leftId}{leftIndexSuffix}";

            // Clean only the identifier head of the right side; preserve any trailing [..]
            string rightHead = rightRaw;
            string rightSuffix = string.Empty;
            int rightBracket = rightRaw.IndexOf('[');
            if (rightBracket >= 0)
            {
                rightHead = rightRaw[..rightBracket];
                rightSuffix = rightRaw[rightBracket..];
            }

            string rightId = CleanName(rightHead);

            // check if rightSuffix needs to be converted to operand
            if (rightSuffix.Length > 0)
            {
                if (rightSuffix.StartsWith("[") && rightSuffix.EndsWith("]"))
                    rightSuffix = "[" + ConvertOperand(rightSuffix.Substring(1, rightSuffix.Length - 2)) + "]";
                else
                    rightSuffix = ConvertOperand(rightSuffix.Substring(1, rightSuffix.Length - 2));
            }

            return $"{qualifiedLeft}.{rightId}{rightSuffix}";
        }

        // 4️⃣  plain identifier
        string id = CleanName(operand);
        return $"{Prefix(id, prog)}{id}";
    }

    private static string RecordOrg(string name, EsfProgram? p)
    {
        // null-safe: a record may exist with a null Org (previously threw NRE).
        return _recordsByName.TryGetValue(name, out var rec)
                   ? (rec.Org?.ToUpperInvariant() ?? string.Empty)
                   : string.Empty;
    }

    // ── Qualify helper ────────────────────────────────────────────────────
    internal static string QualifyIdentifier(string rawId, EsfProgram? p)
    {
        if (string.IsNullOrWhiteSpace(rawId)) return rawId;
        var trimmed = rawId.Trim();

        // Already qualified? leave as-is
        if (trimmed.StartsWith(WORK_PREFIX, StringComparison.Ordinal) ||
            trimmed.StartsWith(SQL_PREFIX, StringComparison.Ordinal) ||
            trimmed.StartsWith(MAP_PREFIX, StringComparison.Ordinal) ||
            trimmed.StartsWith(ITEM_PREFIX, StringComparison.Ordinal) ||
            trimmed.StartsWith(TABLE_PREFIX, StringComparison.Ordinal) ||
            trimmed.StartsWith(SYS_PREFIX, StringComparison.Ordinal))
        {
            return trimmed;
        }

        var id = CleanName(trimmed);
        return $"{Prefix(id, p)}{id}";
    }

    private static string Prefix(string name, EsfProgram? p)
    {

        return
        IsItem(name, p) ? ITEM_PREFIX
        : IsRecord(name, p) ? (RecordOrg(name, p) == "SQLROW" ? SQL_PREFIX : WORK_PREFIX)
        : IsMap(name, p) ? MAP_PREFIX
        : IsTable(name, p) ? TABLE_PREFIX
        : name.StartsWith("EZ", StringComparison.OrdinalIgnoreCase) ? SYS_PREFIX
        : (IsRecordProperty(name, p) is (true, var recName)) ? (RecordOrg(name, p) == "SQLROW" ? SQL_PREFIX : WORK_PREFIX) + recName + "."
        : string.Empty;  // no prefix
    }

    // ── Record-field shortcut for array indices ───────────────────────────
    private static string? TryRecordField(string basePath, string rawIdx, EsfProgram? p)
    {
        // If basePath contains a field (REC.FIELD), the 'recordName' is still the leftmost token.
        string recordName = CleanName(basePath.Split('.')[0]);  // e.g. "REC" from "REC.FIELD"
        if (!_recordsByName.TryGetValue(recordName, out var rec)) return null;

        string fieldName = CleanName(rawIdx);

        // check if field is in basePath record
        if (fieldName.IndexOf(".") < 0 && rec.Items.FirstOrDefault(t => t.Name == fieldName) == null)
        {
            // find the right basePath
            var (found, recName) = IsRecordProperty(fieldName, p);
            if (found)
            {
                recordName = recName;
                _recordsByName.TryGetValue(CleanName(recName), out rec);
            }
            else
            {
                throw new EsfTranslationException(
                    $"Field '{fieldName}' not found in record '{recordName}' or any other record.");
            }
        }

        string prefix = string.Equals(rec?.Org, "SQLROW", StringComparison.OrdinalIgnoreCase)
                            ? SQL_PREFIX
                            : WORK_PREFIX;
        if (fieldName.IndexOf(".") > 0)
        {
            string fieldbase = CleanName(basePath.Split('.')[0]);
            if (fieldbase == recordName)
                return $"{prefix}{fieldName}";
        }
        // NOTE: If you have a definitive field list per record, validate fieldName here.
        return $"{prefix}{recordName}.{fieldName}";
    }

    // ── Cache-backed look-ups (O(1), case-insensitive on clean names) ──────
    private static bool IsItem(string n, EsfProgram? p) => _itemsByName.ContainsKey(n);

    private static bool IsRecord(string n, EsfProgram? p) => _recordsByName.ContainsKey(n);

    private static (bool, string) IsRecordProperty(string n, EsfProgram? p) =>
        _recordByFieldName.TryGetValue(n, out var rec) ? (true, rec.Name) : (false, string.Empty);

    private static bool IsMap(string n, EsfProgram? p) => _mapsByName.ContainsKey(n);

    private static bool IsTable(string n, EsfProgram? p) => _tablesByName.ContainsKey(n);
}
