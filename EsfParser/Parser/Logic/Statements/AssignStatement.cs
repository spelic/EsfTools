// AssignStatement.cs ───────────────────────────────────────────────────────
// Robust ESF “Assignment” ⇒ C# generator
//
// Supports:
//  • Pure movement (item ← item/literal/function), incl. whole-record copy
//  • Arithmetic (+, -, *, /, //) with precedence, unary ±, parentheses
//  • Rounding marker “(R” (only for arithmetic, applied at the *end*)
//  • Safe, *inline* type coercions (int/decimal/string) w/o over-conversion
//  • name-name → name_name (via CSharpUtils.ConvertOperand) everywhere
//  • Smart int increments: X = X + 1 → X++;  X = 1 + X → X++;  X = X - 1 → X--
//  • int ← decimal movement emits simple cast: (int)rhs
//  • Whole-record copy: GlobalSqlRow.D133R06 = GlobalSqlRow.D133R01
//      → EsfRuntime.RecordCopier.CopyByName(rhs, lhs)
//
// Notes:
//  • We only use Convert.ToDecimal for decimal targets (or unknown→numeric).
//  • No Convert.ToDecimal in simple int math or int movement where not needed.
//
using EsfParser.CodeGen;
using EsfParser.Esf;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace EsfParser.Parser.Logic.Statements
{
    public class AssignStatement : IStatement
    {
        public StatementType Type => StatementType.Assign;

        public string OriginalCode { get; set; } = string.Empty;
        public string Left { get; set; } = string.Empty;
        public string Right { get; set; } = string.Empty;
        public int LineNumber { get; set; }
        public int NestingLevel { get; set; }

        public string ToCSharp()
        {
            // 1) Guard
            if (string.IsNullOrWhiteSpace(Left) || string.IsNullOrWhiteSpace(Right))
                return $"// ⚠ malformed assignment at line {LineNumber}: {OriginalCode}";

            var prog = CSharpUtils.Program;

            var leftRaw = Left.Trim();
            var rightRaw = Right.Trim();

            // Normalize/qualify operands using compiler rules (handles name-name → name_name)
            var leftQualified = NormalizeOperand(CSharpUtils.ConvertOperand(leftRaw));
            var rightQualified = NormalizeOperand(CSharpUtils.ConvertOperand(rightRaw));

            // ── Special-case: EZESCNCT(target, source) on RHS ─────────────────────────
            // RESULT = EZESCNCT(PRINT_LINE, SALARY);
            if (TryParseEzescnctCall(rightRaw, out var ez_tgtRaw, out var ez_srcRaw))
            {
                var tgtQ = CSharpUtils.ConvertOperand(ez_tgtRaw.Trim());
                var srcQ = CSharpUtils.ConvertOperand(ez_srcRaw.Trim());

                // LHS typing for proper movement of int result
                var lhsInfo2 = InferTargetInfo(leftQualified, prog, ValueInfo.NumericUnknown(), isArithmetic: false);

                // Unique temps per line
                var id = LineNumber <= 0 ? Environment.TickCount : LineNumber;
                var tVar = $"__ezc_t_{id}";
                var rVar = $"__ezc_rc_{id}";
                var nVar = $"__ezc_new_{id}";

                // Move the int result into LHS with proper coercion
                var movedResult = CoerceMovement(rVar, new ValueInfo(ValueKind.Int, 0), lhsInfo2, rounding: false);

                return
            $@"{{
    var {tVar} = {tgtQ};
    var {rVar} = EzFunctions.EZESCNCT({tVar}, {srcQ}, out var {nVar});
    {tgtQ} = {nVar};
    {leftQualified} = {movedResult};  // ORG: {OriginalCode}
}}";
            }


            // 2) Handle rounding marker "(R" (arithmetic-only). Remove from RHS but remember it.
            bool rounding = ContainsRoundMarker(rightRaw);
            var rhsNoRound = RemoveRoundMarker(rightRaw);

            // 3) Tokenize RHS (for arithmetic detection/parsing)
            bool rhsIsPureNumber = TryGetPureNumeric(rhsNoRound, out bool rhsLiteralIsDecimal);
            List<Token> tokens = rhsIsPureNumber
                ? new List<Token> { new Token(TokKind.Number, rhsNoRound.Trim(), 0), new Token(TokKind.End, "", 0) }
                : new Tokenizer(rhsNoRound).Tokenize();

            bool isArithmetic = !rhsIsPureNumber && tokens.Any(t =>
                t.Kind is TokKind.Plus or TokKind.Minus or TokKind.Star or TokKind.Slash or TokKind.Rem
                       or TokKind.LParen or TokKind.RParen);

            // Single string literal (e.g. "text with  spaces")? Preserve exactly.
            bool rhsIsSingleStringLiteral =
                !rhsIsPureNumber &&
                tokens.Count >= 2 &&
                tokens[0].Kind == TokKind.String &&
                tokens[1].Kind == TokKind.End;


            if (isArithmetic && TryRewriteSimpleIncDec(leftQualified, rhsNoRound, out var incDecEarly))
                return incDecEarly + ";";

            // 4) Infer RHS/LHS value kinds (movement path prefers metadata)
            var rhsInfo = rhsIsPureNumber
                ? new ValueInfo(rhsLiteralIsDecimal ? ValueKind.Decimal : ValueKind.Int,
                                rhsLiteralIsDecimal ? 2 : 0)
                : (isArithmetic ? ValueInfo.NumericUnknown()
                                : InferValueInfoSmart(rightRaw, rightQualified, prog));

            // Use qualified LHS for metadata lookup (fixes name-name → name_name)
            var lhsInfo = InferTargetInfo(leftQualified, prog, rhsInfo, isArithmetic);


            // If parser saw arithmetic and LHS looks like string, but there are *no* string tokens,
            // treat this statement as numeric to avoid ToString(...) conversions.
            if (isArithmetic && lhsInfo.Kind == ValueKind.String && IsPureNumericArithmetic(tokens))
            {
                // Downgrade the target "kind" for this assignment only, so we won't stringify the result
                lhsInfo = ValueInfo.NumericUnknown();
            }

            // 5) Whole-record copy (handles qualified and unqualified)
            //    D133R06 = D133R01;
            //    GlobalSqlRow.D133R06 = GlobalSqlRow.D133R01;
            if (!isArithmetic &&
                TryResolveWholeRecordRef(leftRaw, prog, out var lhsRecQualified, out _, out _) &&
                TryResolveWholeRecordRef(rightRaw, prog, out var rhsRecQualified, out _, out _))
            {
                return $"RecordCopier.CopyByName({leftQualified}.Current, {rightQualified}.Current);  // ORG: {OriginalCode} ";
            }

            // 6) Whole-map copy (handles qualified and unqualified)
            //    D133M04 = D133M02;
            //    GlobalMaps.D133M04 = GlobalMaps.D133M02;
            if (!isArithmetic &&
                TryResolveWholeMapRef(leftRaw, prog, out var lhsMapQualified, out _) &&
                TryResolveWholeMapRef(rightRaw, prog, out var rhsMapQualified, out _))
            {
                return $"{leftQualified}.CopyFrom({rightQualified}.Current); // ORG: {OriginalCode} ";
            }

            // 6b) Whole-record → map copy (handles qualified and unqualified)
            //     D133M04 = D133R01;
            //     GlobalMaps.D133M04 = GlobalWorkstor.D133R01;
            //     GlobalMaps.D133M04 = GlobalSqlRow.D133R01;
            if (!isArithmetic &&
                TryResolveWholeMapRef(leftRaw, prog, out var lhsMapQualifiedRM, out _) &&
                TryResolveWholeRecordRef(rightRaw, prog, out var rhsRecQualifiedRM, out _, out _))
            {
                // Map knows how to copy from either a map instance or a record instance
                return $"{leftQualified}.CopyFrom({rightQualified}.Current); // ORG: {OriginalCode} ";
            }

            // 7) Arithmetic RHS
            if (isArithmetic)
            {
                // X = X ± N  or  X = N + X  (int targets) → ++ / -- / += N
                if (lhsInfo.Kind == ValueKind.Int && TryRewriteSimpleIncDec(leftQualified, rhsNoRound, out var incDec))
                    return incDec + $";      // ORG: {OriginalCode}";

                // Build arithmetic expression in C#
                var parser = new Parser(tokens, prog);
                string arith = parser.ParseExpression();

                // Apply rounding for *arithmetic* (final result only)
                string coerced = CoerceForTarget(arith, lhsInfo, rounding);
                return $"{leftQualified} = {coerced};      // ORG: {OriginalCode}";
            }

            // 8) Movement RHS (no operators): coerce inline
            string rhsExpr;
            if (rhsIsPureNumber)
            {
                rhsExpr = rhsNoRound.Trim();
            }
            else if (rhsIsSingleStringLiteral)
            {
                // Preserve original string exactly (incl. spaces) as a C# double-quoted literal
                rhsExpr = ExtractOriginalStringLiteral(rightRaw);
            }
            else
            {
                // Identifiers / functions / anything else
                rhsExpr = rightQualified;
            }


            // Final movement coercion (e.g., int ← decimal → (int)rhs)
            string moved = CoerceMovement(rhsExpr, rhsInfo, lhsInfo, rounding: false);
            return $"{leftQualified} = {moved};      // ORG: {OriginalCode}";
        }

        private static bool TryParseEzescnctCall(string s, out string targetArg, out string sourceArg)
        {
            targetArg = sourceArg = string.Empty;
            if (string.IsNullOrWhiteSpace(s)) return false;

            var span = s.AsSpan().Trim();
            // case-insensitive match of name prefix
            var name = "EZESCNCT";
            if (span.Length < name.Length + 2) return false;
            if (!span.Slice(0, name.Length).ToString().Equals(name, StringComparison.OrdinalIgnoreCase)) return false;

            int i = name.Length;
            while (i < span.Length && char.IsWhiteSpace(span[i])) i++;
            if (i >= span.Length || span[i] != '(') return false;
            i++; // after '('

            // scan up to matching ')', split on first top-level comma
            int depth = 1; char? q = null;
            int comma = -1, start = i;
            for (; i < span.Length; i++)
            {
                char c = span[i];
                if (q != null)
                {
                    if (c == q) q = null;
                    else if (c == '\\' && i + 1 < span.Length) i++; // skip escaped char
                    continue;
                }
                if (c == '\'' || c == '"') { q = c; continue; }
                if (c == '(') { depth++; continue; }
                if (c == ')') { depth--; if (depth == 0) break; continue; }
                if (c == ',' && depth == 1 && comma < 0) { comma = i; }
            }
            if (depth != 0) return false;
            int end = i;

            ReadOnlySpan<char> inside = span.Slice(start, end - start);
            if (comma < 0) return false;

            // split args
            var a1 = inside.Slice(0, comma - start).ToString().Trim();
            var a2 = inside.Slice(comma - start + 1).ToString().Trim();
            if (a1.Length == 0 || a2.Length == 0) return false;

            targetArg = a1;
            sourceArg = a2;
            return true;
        }

        private static string ExtractOriginalStringLiteral(string s)
        {
            var t = (s ?? string.Empty).Trim();
            if (t.Length >= 2 && (t[0] == '"' || t[0] == '\''))
            {
                char q = t[0];
                var body = t.Substring(1, t.Length - 2); // keep spaces exactly as written
                if (q == '\'')
                {
                    // Convert to C# double-quoted literal; escape internal quotes minimally
                    return $"\"{body.Replace("\"", "\\\"")}\"";
                }
                // Already double-quoted; keep as-is (escape internal quotes minimally)
                return $"\"{body.Replace("\"", "\\\"")}\"";
            }
            // Fallback (shouldn't happen): return as-is
            return t;
        }


        private static bool IsPureNumericArithmetic(List<Token> toks)
        {
            foreach (var t in toks)
            {
                switch (t.Kind)
                {
                    case TokKind.End:
                    case TokKind.Number:
                    case TokKind.Ident:
                    case TokKind.Plus:
                    case TokKind.Minus:
                    case TokKind.Star:
                    case TokKind.Slash:
                    case TokKind.Rem:
                    case TokKind.LParen:
                    case TokKind.RParen:
                        continue;
                    case TokKind.String:
                        return false; // string literal present → not purely numeric
                    default:
                        return false;
                }
            }
            return true;
        }


        // ──────────────────────────────────────────────────────────────────
        // Movement/Arithmetic coercion
        // ──────────────────────────────────────────────────────────────────

        private static string CoerceForTarget(string expr, ValueInfo lhs, bool rounding)
        {
            // Decimal targets: ensure decimal and round to scale
            if (lhs.Kind == ValueKind.Decimal)
            {
                string dec = ToDec(expr);
                if (lhs.Scale >= 0)
                {
                    return rounding
                        ? $"System.Math.Round({dec}, {lhs.Scale}, System.MidpointRounding.AwayFromZero)"
                        : $"System.Math.Round({dec}, {lhs.Scale}, System.MidpointRounding.ToZero)";
                }
                return dec;
            }

            // Int targets: either simple cast or rounded cast if explicitly requested
            if (lhs.Kind == ValueKind.Int)
            {
                if (rounding)
                    return $"(int)System.Math.Round({ToDec(expr)}, 0, System.MidpointRounding.AwayFromZero)";

                // Default: keep int math clean — just cast final result
                return $"(int)({expr})";
            }

            // String targets: stringify
            if (lhs.Kind == ValueKind.String)
                return $"({ToDec(expr)}).ToString(System.Globalization.CultureInfo.InvariantCulture)";

            // Unknown targets: pass-through
            return expr;

            static string ToDec(string e) =>
                $"System.Convert.ToDecimal((object)({e}), System.Globalization.CultureInfo.InvariantCulture)";
        }
        private static string AsInstance(string qualifiedTypeName) => qualifiedTypeName + ".Current";

        private static string CoerceMovement(string rhsExpr, ValueInfo rhs, ValueInfo lhs, bool rounding)
        {
            static string ToDec(string e) =>
                $"System.Convert.ToDecimal((object)({e}), System.Globalization.CultureInfo.InvariantCulture)";

            static bool IsIntLiteral(string e) =>
                Regex.IsMatch(e, @"^\s*[+-]?\d+\s*$");

            // Treat assigning blank or whitespace string literals to numeric targets as zero.
            static bool IsBlankStringLiteralExpr(string expr)
            {
                if (string.IsNullOrWhiteSpace(expr)) return false;
                var t = expr.Trim();
                // Check for double-quoted string literal
                if (t.Length >= 2 && t[0] == '"' && t[^1] == '"')
                {
                    var body = t.Substring(1, t.Length - 2);
                    // Unescape common escape sequences minimal (only \"), treat others as-is
                    // We only care about whitespace, so unescaping is unnecessary here.
                    return string.IsNullOrWhiteSpace(body);
                }
                return false;
            }

            // In CoerceMovement → INT target → rhs.Kind == Int:
            if (IsIntLiteral(rhsExpr))
                return Regex.Match(rhsExpr, @"[+-]?\d+").Value; // emits "0" etc. verbatim


            // DECIMAL target
            if (lhs.Kind == ValueKind.Decimal)
            {
                // If assigning a blank string literal to a decimal target, return zero
                if (rhs.Kind == ValueKind.String && IsBlankStringLiteralExpr(rhsExpr))
                {
                    // use 0m to explicitly denote decimal zero
                    return "0m";
                }
                string dec = (rhs.Kind == ValueKind.Decimal) ? rhsExpr : ToDec(rhsExpr);
                //if (lhs.Scale >= 0)
                //{
                //    return rounding
                //        ? $"System.Math.Round({dec}, {lhs.Scale}, System.MidpointRounding.AwayFromZero)"
                //        : $"System.Math.Round({dec}, {lhs.Scale}, System.MidpointRounding.ToZero)";
                //}
                return dec;
            }

            // INT target
            if (lhs.Kind == ValueKind.Int)
            {
                // If assigning a blank string literal to an int target, return zero
                if (rhs.Kind == ValueKind.String && IsBlankStringLiteralExpr(rhsExpr))
                {
                    return "0";
                }
                // int ← int
                if (rhs.Kind == ValueKind.Int)
                {
                    // plain integer literal → emit as-is
                    if (IsIntLiteral(rhsExpr))
                        return Regex.Match(rhsExpr, @"[+-]?\d+").Value;

                    return rhsExpr;
                }

                // int ← decimal
                if (rhs.Kind == ValueKind.Decimal)
                {
                    return rounding
                        ? $"(int)System.Math.Round({ToDec(rhsExpr)}, 0, System.MidpointRounding.AwayFromZero)"
                        : $"(int)({rhsExpr})";
                }

                // int ← unknown:
                // If RHS *looks like* a record field path, prefer a simple cast.
                if (RecordFieldPathRegex.IsMatch(NormalizeOperand(rhsExpr)))
                    return $"(int)({rhsExpr})";

                // Otherwise, go via decimal then truncate/round-to-zero
                return rounding
                    ? $"(int)System.Math.Round({ToDec(rhsExpr)}, 0, System.MidpointRounding.AwayFromZero)"
                    : $"(int)System.Math.Round({ToDec(rhsExpr)}, 0, System.MidpointRounding.ToZero)";
            }

            // STRING target
            if (lhs.Kind == ValueKind.String)
            {
                // if RHS is already a string literal → leave as-is
                var t = rhsExpr.Trim();
                if (t.Length >= 2 && (t[0] == '"' && t[^1] == '"'))
                    return rhsExpr;

                if (rhs.Kind == ValueKind.String)
                    return $"System.Convert.ToString({rhsExpr}, System.Globalization.CultureInfo.InvariantCulture) ?? string.Empty";

                // numeric/unknown → ToString(invariant)
                return $"({ToDec(rhsExpr)}).ToString(System.Globalization.CultureInfo.InvariantCulture)";
            }

            return rhsExpr;
        }

        // ──────────────────────────────────────────────────────────────────
        // Heuristics / metadata inference
        // ──────────────────────────────────────────────────────────────────

        private static ValueInfo InferTargetInfo(string lhsQualified, EsfProgram? prog, ValueInfo rhsInfo, bool isArithmetic)
        {
            // Try map field
            if (TryInferMapField(lhsQualified, prog, out var kind, out var scale))
                return new ValueInfo(kind, scale);

            // Try record item
            if (TryInferRecordItem(lhsQualified, prog, out kind, out scale))
                return new ValueInfo(kind, scale);

            // If arithmetic and RHS is numeric, return numeric-unknown (fall back later in coercion)
            if (isArithmetic && (rhsInfo.Kind == ValueKind.Int || rhsInfo.Kind == ValueKind.Decimal || rhsInfo.Kind == ValueKind.NumericUnknown))
                return ValueInfo.NumericUnknown();

            // Default to string
            return new ValueInfo(ValueKind.String, -1);
        }

        private static ValueInfo InferValueInfoSmart(string raw, string qualified, EsfProgram? prog)
        {
            var v = InferValueInfo(raw, prog);
            if (v.Kind != ValueKind.Unknown) return v;
            return InferValueInfo(qualified, prog);
        }

        private static ValueInfo InferValueInfo(string operand, EsfProgram? prog)
        {
            if (string.IsNullOrWhiteSpace(operand))
                return ValueInfo.Unknown();

            // String literal?
            var t = operand.Trim();
            if (t.Length >= 2 && (t[0] == '"' && t[^1] == '"'))
                return new ValueInfo(ValueKind.String, -1);

            // Pure numeric literal?
            if (TryGetPureNumeric(operand, out bool isDec))
                return new ValueInfo(isDec ? ValueKind.Decimal : ValueKind.Int, isDec ? 2 : 0);

            // Map field?
            if (TryInferMapField(operand, prog, out var kind, out var scale))
                return new ValueInfo(kind, scale);

            // Record item?
            if (TryInferRecordItem(operand, prog, out kind, out scale))
                return new ValueInfo(kind, scale);

            // Unknown
            return ValueInfo.Unknown();
        }

        private static bool TryResolveWholeMapRef(string operand, EsfProgram? prog, out string qualified, out string mapNameClean)
        {
            qualified = ""; mapNameClean = "";
            if (string.IsNullOrWhiteSpace(operand) || prog?.Maps?.Maps == null) return false;

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
            if (string.IsNullOrWhiteSpace(operand) || prog?.Records?.Records == null) return false;

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


        // PACKED is always decimal; NUM → decimal when Decimals>0 else int; BINARY → int; else string.
        private static bool TryInferRecordItem(string operand, EsfProgram? prog, out ValueKind kind, out int scale)
        {
            kind = ValueKind.Unknown; scale = -1;
            if (string.IsNullOrWhiteSpace(operand)) return false;

            var m = Regex.Match(operand,
                @"\b(GlobalWorkstor|GlobalSqlRow)\.(?<rec>[A-Za-z_0-9]+)\.(?<itm>[A-Za-z0-9_\-\[\]]+)");
            if (!m.Success) return false;

            string recName = m.Groups["rec"].Value;
            string itmName = StripIndexer(m.Groups["itm"].Value);

            var recs = prog?.Records?.Records;
            if (recs == null) return false;

            string canonRec = CSharpUtils.CleanName(recName);
            string canonItem = Canon(itmName);

            var rec = recs.FirstOrDefault(r =>
                string.Equals(CSharpUtils.CleanName(r.Name), canonRec, StringComparison.OrdinalIgnoreCase));
            if (rec == null) return false;

            var item = rec.Items?.FirstOrDefault(it =>
                string.Equals(Canon(it.Name), canonItem, StringComparison.OrdinalIgnoreCase));
            if (item == null) return false;

            string tU = item.Type.ToString().ToUpperInvariant();

            if (tU == "PACKED")
            {
                kind = ValueKind.Decimal;
                scale = Math.Max(0, item.Decimals);
                return true;
            }

            if (tU == "PACK")
            {
                kind = ValueKind.Decimal;
                scale = Math.Max(0, item.Decimals);
                return true;
            }


            if (tU == "NUM")
            {
                if (item.Decimals > 0) { kind = ValueKind.Decimal; scale = item.Decimals; }
                else { kind = ValueKind.Int; scale = 0; }
                return true;
            }

            if (tU == "BINARY")
            {
                kind = ValueKind.Int; scale = 0; return true;
            }

            if (tU == "BIN")
            {
                kind = ValueKind.Int; scale = 0; return true;
            }

            kind = ValueKind.String; scale = -1; return true;
        }

        private static bool TryInferMapField(string operand, EsfProgram? prog, out ValueKind kind, out int scale)
        {
            kind = ValueKind.Unknown; scale = -1;
            if (prog?.Maps?.Maps == null) return false;

            var m = Regex.Match(operand, @"GlobalMaps\.(?<map>[A-Za-z_0-9]+)\.(?<fld>[A-Za-z0-9_\-\[\]]+)");
            if (!m.Success) return false;

            string mapName = m.Groups["map"].Value;
            string fldName = StripIndexer(m.Groups["fld"].Value);

            var map = prog.Maps.Maps.FirstOrDefault(x =>
                string.Equals(CSharpUtils.CleanName(x.MapName), CSharpUtils.CleanName(mapName), StringComparison.OrdinalIgnoreCase));
            if (map == null) return false;

            string canonFld = Canon(fldName);
            var vf = map.Vfields.FirstOrDefault(v =>
                string.Equals(Canon(v.Name), canonFld, StringComparison.OrdinalIgnoreCase));
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

        // ──────────────────────────────────────────────────────────────────
        // Helpers (lexing/parsing/normalization)
        // ──────────────────────────────────────────────────────────────────

        private static readonly Regex RecordFieldPathRegex = new(
            @"^\s*(GlobalWorkstor|GlobalSqlRow)\s*\.\s*[A-Za-z_][A-Za-z0-9_]*\s*\.\s*[A-Za-z0-9_\-\[\]]+\s*$",
            RegexOptions.Compiled);

        private static bool TryGetContainerRecordRef(string operand, out string instanceExpr)
        {
            // Whole-record reference: GlobalWorkstor.XYZ or GlobalSqlRow.XYZ
            instanceExpr = operand?.Trim() ?? "";
            var m = Regex.Match(instanceExpr, @"^\s*(GlobalWorkstor|GlobalSqlRow)\s*\.\s*([A-Za-z_][A-Za-z0-9_]*)\s*$");
            if (!m.Success) return false;
            // Use the exact text; ConvertOperand upstream will qualify as needed
            return true;
        }

        private static bool IsWholeGlobalMap(string operand)
        {
            return Regex.IsMatch(operand ?? "",
                @"^\s*GlobalMaps\s*\.\s*[A-Za-z_][A-Za-z0-9_]*\s*$",
                RegexOptions.Compiled);
        }

        private static string StripIndexer(string s)
        {
            if (string.IsNullOrEmpty(s)) return s;
            int i = s.IndexOf('[');
            return i >= 0 ? s.Substring(0, i) : s;
        }

        // Normalize minimal formatting for comparisons (safe on non-literals)
        private static string NormalizeOperand(string s)
            => Regex.Replace(s ?? "", @"\s+", "");

        private static string Canon(string s)
            => Regex.Replace((s ?? "").Replace("-", "_"), @"\s+", "").Trim();

        private static bool TryGetPureNumeric(string s, out bool isDecimal)
        {
            isDecimal = false;
            if (string.IsNullOrWhiteSpace(s)) return false;
            var t = s.Trim();
            // simple forms: -12, +34, 12.34, .5, 5.
            if (Regex.IsMatch(t, @"^[+-]?(\d+(\.\d*)?|\.\d+)$"))
            {
                isDecimal = t.Contains('.');
                return true;
            }
            return false;
        }

        private static bool ContainsRoundMarker(string s)
            => Regex.IsMatch(s ?? "", @"\(\s*R\b", RegexOptions.IgnoreCase);

        private static string RemoveRoundMarker(string s)
            => Regex.Replace(s ?? "", @"\(\s*R\b", "", RegexOptions.IgnoreCase);

        // X = X ± N  OR  X = N + X  (N is integer literal)
        private static bool TryRewriteSimpleIncDec(string leftQualified, string rhsRaw, out string rewritten)
        {
            rewritten = "";
            string lhsCanon = Canon(leftQualified);

            // lhs + N
            var m1 = Regex.Match(rhsRaw, @"^\s*(?<a>.+?)\s*\+\s*(?<n>[+-]?\d+)\s*$");
            if (m1.Success)
            {
                var a = Canon(NormalizeOperand(CSharpUtils.ConvertOperand(m1.Groups["a"].Value)));
                if (a == lhsCanon)
                {
                    int n = int.Parse(m1.Groups["n"].Value, CultureInfo.InvariantCulture);
                    rewritten = (n == 1) ? $"{leftQualified}++" : $"{leftQualified} += {n}";
                    return true;
                }
            }

            // N + lhs
            var m2 = Regex.Match(rhsRaw, @"^\s*(?<n>[+-]?\d+)\s*\+\s*(?<a>.+?)\s*$");
            if (m2.Success)
            {
                var a = Canon(NormalizeOperand(CSharpUtils.ConvertOperand(m2.Groups["a"].Value)));
                if (a == lhsCanon)
                {
                    int n = int.Parse(m2.Groups["n"].Value, CultureInfo.InvariantCulture);
                    rewritten = (n == 1) ? $"{leftQualified}++" : $"{leftQualified} += {n}";
                    return true;
                }
            }

            // lhs - N
            var m3 = Regex.Match(rhsRaw, @"^\s*(?<a>.+?)\s*-\s*(?<n>\d+)\s*$");
            if (m3.Success)
            {
                var a = Canon(NormalizeOperand(CSharpUtils.ConvertOperand(m3.Groups["a"].Value)));
                if (a == lhsCanon)
                {
                    int n = int.Parse(m3.Groups["n"].Value, CultureInfo.InvariantCulture);
                    rewritten = (n == 1) ? $"{leftQualified}--" : $"{leftQualified} -= {n}";
                    return true;
                }
            }

            return false;
        }

        // ──────────────────────────────────────────────────────────────────
        // Expression parser (Pratt-style, minimal)
        // ──────────────────────────────────────────────────────────────────

        private enum TokKind { End, Number, String, Ident, Plus, Minus, Star, Slash, Rem, LParen, RParen, Comma }

        private readonly struct Token
        {
            public TokKind Kind { get; }
            public string Text { get; }
            public int Pos { get; }
            public Token(TokKind k, string t, int p) { Kind = k; Text = t; Pos = p; }
            public override string ToString() => $"{Kind}:{Text}";
        }

        private sealed class Tokenizer
        {
            private readonly string _s;
            private int _i;

            public Tokenizer(string s) { _s = s ?? ""; _i = 0; }

            public List<Token> Tokenize()
            {
                var list = new List<Token>();
                Token t;
                do { t = Next(); list.Add(t); } while (t.Kind != TokKind.End);
                return list;
            }

            private Token Next()
            {
                SkipWs();
                if (_i >= _s.Length) return new Token(TokKind.End, "", _i);
                char c = _s[_i];

                // number
                // number (no sign here; + / - are always separate tokens)
                if (char.IsDigit(c) || (c == '.' && _i + 1 < _s.Length && char.IsDigit(_s[_i + 1])))
                    return ReadNumber();

                // string literal
                if (c == '"' || c == '\'') return ReadString();

                // ident (allow ., _, digits, [subscript], hyphen stays here but ConvertOperand will normalize)
                if (char.IsLetter(c) || c == '_')
                {
                    int start = _i;
                    while (_i < _s.Length)
                    {
                        char ch = _s[_i];
                        if (char.IsLetterOrDigit(ch) || ch == '_' || ch == '.' || ch == '-')
                        { _i++; continue; }
                        if (ch == '[')
                        {
                            int depth = 1; _i++;
                            while (_i < _s.Length && depth > 0)
                            {
                                if (_s[_i] == '[') depth++;
                                else if (_s[_i] == ']') depth--;
                                _i++;
                            }
                            continue;
                        }
                        break;
                    }
                    return new Token(TokKind.Ident, _s.Substring(start, _i - start), start);
                }

                // operators
                if (c == '+') { _i++; return new Token(TokKind.Plus, "+", _i - 1); }
                if (c == '-') { _i++; return new Token(TokKind.Minus, "-", _i - 1); }
                if (c == '*') { _i++; return new Token(TokKind.Star, "*", _i - 1); }
                if (c == '/')
                {
                    if (_i + 1 < _s.Length && _s[_i + 1] == '/')
                    {
                        _i += 2; return new Token(TokKind.Rem, "//", _i - 2);
                    }
                    _i++; return new Token(TokKind.Slash, "/", _i - 1);
                }
                if (c == '(') { _i++; return new Token(TokKind.LParen, "(", _i - 1); }
                if (c == ')') { _i++; return new Token(TokKind.RParen, ")", _i - 1); }
                if (c == ',') { _i++; return new Token(TokKind.Comma, ",", _i - 1); }

                // fallback: skip unknown char
                _i++; return Next();
            }

            private void SkipWs()
            {
                while (_i < _s.Length && char.IsWhiteSpace(_s[_i])) _i++;
            }

            private bool LookaheadIsNumber()
            {
                int j = _i + 1;
                while (j < _s.Length && char.IsWhiteSpace(_s[j])) j++;
                return j < _s.Length && (char.IsDigit(_s[j]) || (_s[j] == '.' && j + 1 < _s.Length && char.IsDigit(_s[j + 1])));
            }

            private Token ReadNumber()
            {
                int start = _i;
                bool sawDot = false;

                while (_i < _s.Length)
                {
                    char ch = _s[_i];
                    if (char.IsDigit(ch)) { _i++; continue; }
                    if (ch == '.' && !sawDot) { sawDot = true; _i++; continue; }
                    break;
                }
                return new Token(TokKind.Number, _s.Substring(start, _i - start), start);
            }

            private Token ReadString()
            {
                int start = _i;
                char q = _s[_i++]; // quote
                while (_i < _s.Length)
                {
                    char d = _s[_i++];
                    if (d == q) break;
                    if (d == '\\' && _i < _s.Length) _i++; // skip escaped next
                }
                return new Token(TokKind.String, _s.Substring(start, _i - start), start);
            }
        }

        private sealed class Parser
        {
            private readonly List<Token> _toks;
            private int _p;
            private readonly EsfProgram? _prog;

            public Parser(List<Token> toks, EsfProgram? prog) { _toks = toks; _prog = prog; _p = 0; }

            public string ParseExpression() => ParseAddSub();

            private string ParseAddSub()
            {
                string left = ParseMulDivRem();
                while (PeekKind() is TokKind.Plus or TokKind.Minus)
                {
                    var op = Next().Kind;
                    string right = ParseMulDivRem();
                    left = $"({left} {(op == TokKind.Plus ? "+" : "-")} {right})";
                }
                return left;
            }

            private string ParseMulDivRem()
            {
                string left = ParseUnary();
                while (PeekKind() is TokKind.Star or TokKind.Slash or TokKind.Rem)
                {
                    var op = Next().Kind;
                    string right = ParseUnary();
                    if (op == TokKind.Star) left = $"({left} * {right})";
                    else if (op == TokKind.Slash) left = $"({left} / {right})";
                    else
                    {
                        // remainder: works for ints and decimals
                        // (a) - (b) * Truncate((a)/(b))
                        left = $"(({left}) - ({right}) * System.Math.Truncate(({left})/({right})))";
                    }
                }
                return left;
            }

            private string ParseUnary()
            {
                if (PeekKind() == TokKind.Plus) { Next(); return ParseUnary(); }
                if (PeekKind() == TokKind.Minus) { Next(); var inner = ParseUnary(); return $"(-({inner}))"; }
                return ParsePrimary();
            }

            private string ParsePrimary()
            {
                var k = PeekKind();

                if (k == TokKind.Number) return Next().Text.Trim();

                if (k == TokKind.String)
                {
                    var t = Next().Text;
                    // Normalize to C# double-quoted string
                    if (t.Length >= 2)
                    {
                        char q = t[0];
                        string body = t.Substring(1, t.Length - 2);
                        if (q == '\'') body = body.Replace("\"", "\\\"");
                        return $"\"{body}\"";
                    }
                    return "\"\"";
                }

                if (k == TokKind.LParen)
                {
                    Next(); // '('
                    var expr = ParseExpression();
                    Expect(TokKind.RParen);
                    return $"({expr})";
                }

                if (k == TokKind.Ident)
                {
                    var id = Next().Text;
                    // Function call?
                    if (PeekKind() == TokKind.LParen)
                    {
                        Next(); // '('
                        var args = new List<string>();
                        if (PeekKind() != TokKind.RParen)
                        {
                            do
                            {
                                args.Add(ParseExpression());
                            } while (TryConsume(TokKind.Comma));
                        }
                        Expect(TokKind.RParen);
                        var f = CSharpUtils.ConvertOperand(id); // Qualify EZ* if needed
                        return $"{f}({string.Join(", ", args)})";
                    }

                    // plain identifier (operand)
                    return CSharpUtils.ConvertOperand(id);
                }

                // End/unknown
                return "0";
            }

            private TokKind PeekKind() => _toks[_p].Kind;
            private Token Next() => _toks[_p++];
            private bool TryConsume(TokKind k) { if (PeekKind() == k) { _p++; return true; } return false; }
            private void Expect(TokKind k) { if (PeekKind() == k) { _p++; return; } /* tolerant */ }
        }

        // ──────────────────────────────────────────────────────────────────
        // Value typing model
        // ──────────────────────────────────────────────────────────────────

        private enum ValueKind
        {
            Unknown,
            String,
            Int,
            Decimal,
            NumericUnknown // used for arithmetic where exact numeric kind is not known
        }

        private readonly struct ValueInfo
        {
            public ValueKind Kind { get; }
            public int Scale { get; } // decimal places for Decimal; -1 otherwise

            public ValueInfo(ValueKind k, int scale) { Kind = k; Scale = scale; }

            public static ValueInfo Unknown() => new(ValueKind.Unknown, -1);
            public static ValueInfo NumericUnknown() => new(ValueKind.NumericUnknown, -1);
        }
    }
}
