// MoveStatement.cs ───────────────────────────────────────────────────────
using System;
using System.Linq;
using System.Text.RegularExpressions;
using EsfParser.CodeGen;
using EsfParser.Esf;

namespace EsfParser.Parser.Logic.Statements
{
    public class MoveStatement : IStatement
    {
        public StatementType Type => StatementType.Move;

        public string OriginalCode { get; set; } = string.Empty;
        public string Source { get; set; } = string.Empty;
        public string Destination { get; set; } = string.Empty;

        public int LineNumber { get; set; }
        public int NestingLevel { get; set; } = 0;

        public override string ToString()
            => $"MoveStatement: {Source} -> {Destination} (Line: {LineNumber}, Nesting: {NestingLevel})";

        public string ToCSharp()
        {
            // Normalize both sides (handles name-name → name_name, subscripts, qualifiers…)
            string srcExpr = CSharpUtils.ConvertOperand(Source);
            // Normalize destination normally
            string dstExpr = CSharpUtils.ConvertOperand(Destination);

            // Source: if quoted literal → preserve exactly; else normalize
            string srcExpr2;
            var srcTrim = (Source ?? string.Empty).Trim();
            if (IsEsfStringLiteral(srcTrim))
                srcExpr2 = EsfStringToCSharp(srcTrim);   // keep {, spaces, and "" → \"
            else
                srcExpr2 = CSharpUtils.ConvertOperand(Source);

            var prog = EsfLogicToCs.program ?? CSharpUtils.Program;

            // ────────────────────────────────────────────────────────────
            // Whole-object copies (accept qualified *and* unqualified)
            // ────────────────────────────────────────────────────────────

            // RECORD ← RECORD
            if (TryResolveWholeRecordRef(Destination, prog, out var dstRecQ, out _, out _) &&
                TryResolveWholeRecordRef(Source, prog, out var srcRecQ, out _, out _))
            {
                return $"EsfRuntime.RecordCopier.CopyByName({AsInstance(srcRecQ)}, {AsInstance(dstRecQ)});";
            }

            // MAP ← MAP
            if (TryResolveWholeMapRef(Destination, prog, out var dstMapQ, out _) &&
                TryResolveWholeMapRef(Source, prog, out var srcMapQ, out _))
            {
                // Map classes expose a static CopyFrom(object) that expects an instance as argument.
                return $"{dstMapQ}.CopyFrom({AsInstance(srcMapQ)});";
            }

            // MAP ← RECORD
            if (TryResolveWholeMapRef(Destination, prog, out dstMapQ, out _) &&
                TryResolveWholeRecordRef(Source, prog, out srcRecQ, out _, out _))
            {
                return $"{dstMapQ}.CopyFrom({AsInstance(srcRecQ)});";
            }

            // NEW: RECORD ← MAP  (MOVE D133M01 TO D133R04;)
            if (TryResolveWholeRecordRef(Destination, prog, out dstRecQ, out _, out _) &&
                TryResolveWholeMapRef(Source, prog, out srcMapQ, out _))
            {
                // Use the same name-based copier; map instance exposes field-like properties.
                return $"EsfRuntime.RecordCopier.CopyByName({AsInstance(srcMapQ)}, {AsInstance(dstRecQ)});";
            }

            // ────────────────────────────────────────────────────────────
            // Scalar/field MOVE with light type coercion
            // ────────────────────────────────────────────────────────────
            var dstInfo = InferValueInfoSmart(Destination, dstExpr, prog);
            var srcInfo = InferValueInfoSmart(Source, srcExpr, prog);

            // decimal → int (explicit cast)
            if (dstInfo.Kind == ValueKind.Int && srcInfo.Kind == ValueKind.Decimal)
                return $"{dstExpr} = (int)({srcExpr});  // Org: {OriginalCode}";

            // int → decimal (let C# do the widening; no rounding for MOVE)
            // string ↔ numeric is left as-is (ConvertOperand already handles literals/paths)

            return $"{dstExpr} = {srcExpr};  // Org: {OriginalCode}";
        }

        // ────────────────────────────────────────────────────────────────
        // Helpers
        // ────────────────────────────────────────────────────────────────
        private static bool IsEsfStringLiteral(string s)
        {
            if (string.IsNullOrEmpty(s)) return false;
            s = s.Trim();
            return s.Length >= 2 && ((s[0] == '"' && s[^1] == '"') || (s[0] == '\'' && s[^1] == '\''));
        }

        /// <summary>
        /// Convert ESF string literal to a safe C# double-quoted literal.
        /// ESF uses doubled quotes "" inside a string to represent a single " character.
        /// We convert: inner "" → \"   and escape backslashes minimally.
        /// Examples:
        ///   "{""username"":"""  →  "{\"username\":\""
        ///   'abc'               →  "abc"
        /// </summary>
        private static string EsfStringToCSharp(string raw)
        {
            var t = (raw ?? string.Empty).Trim();
            if (t.Length < 2) return "\"\"";

            char q = t[0];
            var body = t.Substring(1, t.Length - 2); // inside quotes

            // Escape backslashes first (to keep sequences stable)
            body = body.Replace("\\", "\\\\");

            if (q == '"')
            {
                // ESF double quotes inside → doubled "" ; map to a single \" in C#
                body = body.Replace("\"\"", "\\\"");
                // Any stray single " (shouldn’t occur) also escape defensively
                body = body.Replace("\"", "\\\"");
            }
            else // q == '\''
            {
                // Single-quoted ESF → just emit as C# "..." with quotes escaped
                body = body.Replace("\"", "\\\"");
            }

            return $"\"{body}\"";
        }

        private static string AsInstance(string qualifiedTypeName) => qualifiedTypeName + ".Current";

        private static string StripIndexer(string s)
        {
            if (string.IsNullOrEmpty(s)) return s;
            int i = s.IndexOf('[');
            return i >= 0 ? s.Substring(0, i) : s;
        }

        private static bool TryGetPureNumeric(string s, out bool isDecimal)
        {
            isDecimal = false;
            if (string.IsNullOrWhiteSpace(s)) return false;
            var t = s.Trim();
            if (Regex.IsMatch(t, @"^[+-]?(\d+(\.\d*)?|\.\d+)$"))
            {
                isDecimal = t.Contains('.');
                return true;
            }
            return false;
        }

        private static ValueInfo InferValueInfoSmart(string raw, string qualified, EsfProgram? prog)
        {
            var v = InferValueInfo(raw, prog);
            if (v.Kind != ValueKind.Unknown) return v;
            return InferValueInfo(qualified, prog);
        }

        private static ValueInfo InferValueInfo(string operand, EsfProgram? prog)
        {
            if (string.IsNullOrWhiteSpace(operand)) return ValueInfo.Unknown();

            // string literal
            var t = operand.Trim();
            if (t.Length >= 2 && ((t[0] == '"' && t[^1] == '"') || (t[0] == '\'' && t[^1] == '\'')))
                return new ValueInfo(ValueKind.String, -1);

            // numeric literal
            if (TryGetPureNumeric(operand, out bool isDec))
                return new ValueInfo(isDec ? ValueKind.Decimal : ValueKind.Int, isDec ? 2 : 0);

            // map field?
            if (TryInferMapField(operand, prog, out var kind, out var scale))
                return new ValueInfo(kind, scale);

            // record item?
            if (TryInferRecordItem(operand, prog, out kind, out scale))
                return new ValueInfo(kind, scale);

            return ValueInfo.Unknown();
        }

        private static bool TryInferRecordItem(string operand, EsfProgram? prog, out ValueKind kind, out int scale)
        {
            kind = ValueKind.Unknown; scale = -1;
            if (prog?.Records?.Records == null) return false;

            // Qualified or unqualified:
            //  GlobalWorkstor.D133W04.STEVILOP
            //  GlobalSqlRow.D133R06.DELNAL
            //  D133W04.STEVILOP
            //  D133R06.DELNAL
            var m = Regex.Match(operand,
                @"^\s*(?:(GlobalWorkstor|GlobalSqlRow)\.)?(?<rec>[A-Za-z_][A-Za-z0-9_]*)\.(?<itm>[A-Za-z0-9_\-\[\]]+)\s*$");
            if (!m.Success) return false;

            string recName = m.Groups["rec"].Value;
            string itmName = StripIndexer(m.Groups["itm"].Value);

            string canonRec = CSharpUtils.CleanName(recName);
            string canonItem = CSharpUtils.CleanName(itmName);

            var rec = prog.Records.Records.FirstOrDefault(r =>
                string.Equals(CSharpUtils.CleanName(r.Name), canonRec, StringComparison.OrdinalIgnoreCase));
            if (rec == null) return false;

            var item = rec.Items?.FirstOrDefault(it =>
                string.Equals(CSharpUtils.CleanName(it.Name), canonItem, StringComparison.OrdinalIgnoreCase));
            if (item == null) return false;

            string tU = item.Type.ToString().ToUpperInvariant();
            if (tU == "PACK") { kind = ValueKind.Decimal; scale = Math.Max(0, item.Decimals); return true; }
            if (tU == "NUM") { if (item.Decimals > 0) { kind = ValueKind.Decimal; scale = item.Decimals; } else { kind = ValueKind.Int; scale = 0; } return true; }
            if (tU == "BIN") { kind = ValueKind.Int; scale = 0; return true; }

            kind = ValueKind.String; scale = -1; return true;
        }

        private static bool TryInferMapField(string operand, EsfProgram? prog, out ValueKind kind, out int scale)
        {
            kind = ValueKind.Unknown; scale = -1;
            if (prog?.Maps?.Maps == null) return false;

            // Qualified or unqualified:
            //  GlobalMaps.D133M02.IZDKOL[...]
            //  D133M02.IZDKOL[...]
            var m = Regex.Match(operand,
                @"^\s*(?:GlobalMaps\.)?(?<map>[A-Za-z_][A-Za-z0-9_]*)\.(?<fld>[A-Za-z0-9_\-\[\]]+)\s*$");
            if (!m.Success) return false;

            string mapName = m.Groups["map"].Value;
            string fldName = StripIndexer(m.Groups["fld"].Value);

            var map = prog.Maps.Maps.FirstOrDefault(x =>
                string.Equals(CSharpUtils.CleanName(x.MapName), CSharpUtils.CleanName(mapName), StringComparison.OrdinalIgnoreCase));
            if (map == null) return false;

            var vf = map.Vfields.FirstOrDefault(v =>
                string.Equals(CSharpUtils.CleanName(v.Name), CSharpUtils.CleanName(fldName), StringComparison.OrdinalIgnoreCase));
            if (vf == null) { kind = ValueKind.String; scale = -1; return false; }

            string tU = vf.Type.ToString().ToUpperInvariant();
            if (tU == "NUM")
            {
                if (vf.Decimals > 0) { kind = ValueKind.Decimal; scale = vf.Decimals; }
                else { kind = ValueKind.Int; scale = 0; }
            }
            else
            {
                kind = ValueKind.String; scale = -1;
            }
            return true;
        }

        private static bool TryResolveWholeMapRef(string operand, EsfProgram? prog, out string qualified, out string mapNameClean)
        {
            qualified = ""; mapNameClean = "";
            if (prog?.Maps?.Maps == null) return false;

            var m = Regex.Match(operand, @"^\s*(?:GlobalMaps\.)?(?<name>[A-Za-z_][A-Za-z0-9_]*)\s*$");
            if (!m.Success) return false;

            string cand = CSharpUtils.CleanName(m.Groups["name"].Value);
            bool exists = prog.Maps.Maps.Any(x =>
                string.Equals(CSharpUtils.CleanName(x.MapName), cand, StringComparison.OrdinalIgnoreCase));
            if (!exists) return false;

            mapNameClean = cand;
            qualified = $"GlobalMaps.{cand}";
            return true;
        }

        private static bool TryResolveWholeRecordRef(string operand, EsfProgram? prog, out string qualified, out string container, out string recNameClean)
        {
            qualified = ""; container = ""; recNameClean = "";
            if (prog?.Records?.Records == null) return false;

            var m = Regex.Match(operand, @"^\s*(?:(GlobalWorkstor|GlobalSqlRow)\.)?(?<name>[A-Za-z_][A-Za-z0-9_]*)\s*$");
            if (!m.Success) return false;

            string cand = CSharpUtils.CleanName(m.Groups["name"].Value);
            var rec = prog.Records.Records.FirstOrDefault(r =>
                string.Equals(CSharpUtils.CleanName(r.Name), cand, StringComparison.OrdinalIgnoreCase));
            if (rec == null) return false;

            container = rec.Org.Equals("SQLROW", StringComparison.OrdinalIgnoreCase) ? "GlobalSqlRow" : "GlobalWorkstor";
            recNameClean = cand;
            qualified = $"{container}.{cand}";
            return true;
        }

        // ────────────────────────────────────────────────────────────────
        // Typing model (minimal for MOVE)
        // ────────────────────────────────────────────────────────────────
        private enum ValueKind { Unknown, String, Int, Decimal }

        private readonly struct ValueInfo
        {
            public ValueKind Kind { get; }
            public int Scale { get; }
            public ValueInfo(ValueKind k, int scale) { Kind = k; Scale = scale; }
            public static ValueInfo Unknown() => new(ValueKind.Unknown, -1);
        }
    }
}
