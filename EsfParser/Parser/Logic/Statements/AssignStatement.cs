// AssignStatement.cs ───────────────────────────────────────────────────────
using EsfParser.CodeGen;
using EsfParser.Esf;
using System;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;

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

        private static readonly Regex ContainerRefRegex = new(
            @"^\s*(?<container>GlobalWorkstor|GlobalSqlRow)\s*\.\s*(?<rec>[A-Za-z_][A-Za-z0-9_]*)\s*(?:\.\s*Current\s*)?$",
            RegexOptions.Compiled);

        private static readonly Regex GlobalMapRootRegex = new(
            @"^\s*GlobalMaps\.(?<map>[A-Za-z_][A-Za-z0-9_]*)\s*$",
            RegexOptions.Compiled);

        private static readonly Regex TrailingRoundRegex = new(
            @"\(\s*R\s*\)?\s*$",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);

        // NEW: detect pure numeric literals like: 0, -5, (+12), ( -3.14 )
        private static readonly Regex PureNumericRegex = new(
            @"^\s*\(*\s*(?<sign>[+-])?\s*(?<num>\d+(?:\.\d+)?)\s*\)*\s*$",
            RegexOptions.Compiled);

        private static readonly Regex RecordFieldPathRegex = new(
    @"^\s*(GlobalWorkstor|GlobalSqlRow)\s*\.\s*[A-Za-z_][A-Za-z0-9_]*\s*\.\s*[A-Za-z0-9_\-\[\]]+\s*$",
    RegexOptions.Compiled);


        public string ToCSharp()
        {
            if (string.IsNullOrWhiteSpace(Left) || string.IsNullOrWhiteSpace(Right))
                return $"// ⚠ malformed assignment at line {LineNumber}: {OriginalCode}";

            var prog = CSharpUtils.Program;
            var leftRaw = Left.Trim();
            var rightRaw = Right.Trim();

            // Map root on the left → CopyFrom(...)
            if (TryGetMapRoot(leftRaw, prog, out var leftMapRoot, out _))
            {
                if (TryGetMapRoot(rightRaw, prog, out var rightMapRoot, out _))
                    return $"{leftMapRoot}.CopyFrom(typeof({rightMapRoot}));";

                var rightQualified2 = NormalizeOperand(CSharpUtils.ConvertOperand(rightRaw));
                if (TryGetContainerInstance(rightQualified2, out var rightInst, out _))
                    return $"{leftMapRoot}.CopyFrom({rightInst});";

                return $"{leftMapRoot}.CopyFrom({rightQualified2});";
            }

            // Whole-record copy (instance model)
            var leftQualified = NormalizeOperand(CSharpUtils.ConvertOperand(leftRaw));
            var rightQualified = NormalizeOperand(CSharpUtils.ConvertOperand(rightRaw));
            if (TryGetContainerInstance(leftQualified, out var leftInst2, out _) &&
                TryGetContainerInstance(rightQualified, out var rightInst2, out _))
            {
                return $"EsfRuntime.RecordCopier.CopyByName({rightInst2}, {leftInst2});";
            }

            // Rounding marker
            bool rounding = false;
            string rhsNoRound = rightRaw;
            if (TrailingRoundRegex.IsMatch(rhsNoRound))
            {
                rounding = true;
                rhsNoRound = TrailingRoundRegex.Replace(rhsNoRound, string.Empty);
            }

            // Detect pure numeric literal (so we don't treat '0' as arithmetic)
            bool rhsIsPureNumber = TryGetPureNumeric(rhsNoRound, out bool rhsLiteralIsDecimal);

            // Arithmetic detection (unchanged)
            List<Token> tokens = rhsIsPureNumber ? new List<Token> { new Token(TokKind.Number, "0", 0), new Token(TokKind.End, "", 0) }
                                                 : new Tokenizer(rhsNoRound).Tokenize();
            bool isArithmetic = !rhsIsPureNumber && tokens.Any(t =>
                                   t.Kind is TokKind.Plus or TokKind.Minus or TokKind.Star or TokKind.Slash or TokKind.Rem
                                    or TokKind.LParen or TokKind.RParen);

            // RHS info (movement): try RAW first, then QUALIFIED (handles name-name → name_name)
            var rhsInfo = rhsIsPureNumber
                ? new ValueInfo(rhsLiteralIsDecimal ? ValueKind.Decimal : ValueKind.Int, rhsLiteralIsDecimal ? 2 : 0)
                : (isArithmetic ? ValueInfo.NumericUnknown()
                                : InferValueInfoSmart(rightRaw, rightQualified, prog));

            // LHS info: **use the qualified/converted** form (your fix)
            var lhsInfo = InferTargetInfo(leftQualified, prog, rhsInfo, isArithmetic);


            string lhs = CSharpUtils.ConvertOperand(leftRaw);

            if (isArithmetic)
            {
                var parser = new Parser(tokens, prog);
                var decimalExpr = parser.ParseNumericExpression(out _);
                string coerced = CoerceForTarget(decimalExpr, lhsInfo, rounding);
                return $"{lhs} = {coerced};   // ORG: {OriginalCode}";
            }

            // Movement
            string rhs = CSharpUtils.ConvertOperand(rightRaw);
            string moved = CoerceMovement(rhs, rhsInfo, lhsInfo, rounding);
            return $"{lhs} = {moved};   // ORG: {OriginalCode}";
        }

        private static ValueInfo InferValueInfoSmart(string raw, string qualified, EsfProgram? prog)
        {
            var v = InferValueInfo(raw, prog);
            if (v.Kind != ValueKind.Unknown) return v;
            return InferValueInfo(qualified, prog);
        }
        // ────────────────────────────────────────────────────────────────
        // Map-root detection
        // ────────────────────────────────────────────────────────────────
        private static bool TryGetMapRoot(string operand, EsfProgram? prog, out string qualified, out string cleanMapName)
        {
            qualified = NormalizeOperand(CSharpUtils.ConvertOperand(operand));
            cleanMapName = string.Empty;

            var m = GlobalMapRootRegex.Match(qualified);
            if (m.Success)
            {
                cleanMapName = CSharpUtils.CleanName(m.Groups["map"].Value);
                return true;
            }

            var parts = qualified.Split('.');
            string? candidate =
                parts.Length == 1 ? parts[0]
              : (parts.Length == 2 && string.Equals(parts[0], "GlobalMaps", StringComparison.Ordinal)) ? parts[1]
              : null;

            if (string.IsNullOrEmpty(candidate))
                return false;

            var maps = prog?.Maps?.Maps;
            if (maps == null) return false;

            string candClean = CSharpUtils.CleanName(candidate);
            var exists = maps.Any(x =>
                string.Equals(CSharpUtils.CleanName(x.MapName), candClean, StringComparison.OrdinalIgnoreCase));

            if (!exists) return false;

            cleanMapName = candClean;
            qualified = $"GlobalMaps.{candClean}";
            return true;
        }

        // ────────────────────────────────────────────────────────────────
        // Movement conversions (inline)
        // ────────────────────────────────────────────────────────────────
        private static string CoerceMovement(string rhsExpr, ValueInfo rhs, ValueInfo lhs, bool rounding)
        {
            static string ToDec(string e) =>
                $"System.Convert.ToDecimal((object)({e}), System.Globalization.CultureInfo.InvariantCulture)";

            static bool IsIntLiteral(string e) =>
                Regex.IsMatch(e, @"^\s*[+-]?\d+\s*$");

            // DECIMAL target
            if (lhs.Kind == ValueKind.Decimal)
            {
                string dec = (rhs.Kind == ValueKind.Decimal) ? rhsExpr : ToDec(rhsExpr);
                if (lhs.Scale >= 0)
                {
                    return rounding
                        ? $"System.Math.Round({dec}, {lhs.Scale}, System.MidpointRounding.AwayFromZero)"
                        : $"System.Math.Round({dec}, {lhs.Scale}, System.MidpointRounding.ToZero)";
                }
                return dec;
            }

            // INT target
            if (lhs.Kind == ValueKind.Int)
            {
                // int ← int
                if (rhs.Kind == ValueKind.Int)
                {
                    // plain integer literal → emit as-is
                    if (IsIntLiteral(rhsExpr))
                        return Regex.Match(rhsExpr, @"[+-]?\d+").Value;

                    // already int-typed expression → pass through
                    return rhsExpr;
                }

                // int ← decimal
                if (rhs.Kind == ValueKind.Decimal)
                {
                    // If user specified (R), honor rounding; otherwise do a simple cast
                    return rounding
                        ? $"(int)System.Math.Round({ToDec(rhsExpr)}, 0, System.MidpointRounding.AwayFromZero)"
                        : $"(int)({rhsExpr})";
                }

                // int ← unknown
                // If the RHS *looks like* a record field path (GlobalWorkstor/GlobalSqlRow),
                // prefer a simple cast to int (covers decimal record fields we couldn't infer).
                var rhsNorm = NormalizeOperand(rhsExpr);
                if (RecordFieldPathRegex.IsMatch(rhsNorm))
                    return $"(int)({rhsExpr})";

                // Otherwise, go through decimal conversion + truncate/round
                string dec2 = ToDec(rhsExpr);
                return rounding
                    ? $"(int)System.Math.Round({dec2}, 0, System.MidpointRounding.AwayFromZero)"
                    : $"(int)System.Math.Round({dec2}, 0, System.MidpointRounding.ToZero)";
            }

            // STRING target
            if (lhs.Kind == ValueKind.String)
            {
                var t = rhsExpr.Trim();
                if (t.Length >= 2 && t[0] == '"' && t[^1] == '"')
                    return rhsExpr;

                if (rhs.Kind == ValueKind.String)
                    return $"System.Convert.ToString({rhsExpr}, System.Globalization.CultureInfo.InvariantCulture) ?? string.Empty";

                // numeric/unknown → stringify decimal representation
                return $"({ToDec(rhsExpr)}).ToString(System.Globalization.CultureInfo.InvariantCulture)";
            }

            // Unknown target → pass-through
            return rhsExpr;
        }

        private static string CoerceForTarget(string decimalExpr, ValueInfo target, bool rounding)
        {
            return target.Kind switch
            {
                ValueKind.Int => rounding
                    ? $"(int)System.Math.Round({decimalExpr}, 0, System.MidpointRounding.AwayFromZero)"
                    : $"(int)System.Math.Round({decimalExpr}, 0, System.MidpointRounding.ToZero)",

                ValueKind.Decimal => target.Scale >= 0
                    ? (rounding
                        ? $"System.Math.Round({decimalExpr}, {target.Scale}, System.MidpointRounding.AwayFromZero)"
                        : $"System.Math.Round({decimalExpr}, {target.Scale}, System.MidpointRounding.ToZero)")
                    : decimalExpr,

                ValueKind.String => $"({decimalExpr}).ToString(System.Globalization.CultureInfo.InvariantCulture)",

                _ => decimalExpr
            };
        }

        // ────────────────────────────────────────────────────────────────
        // Type inference (metadata + safe fallback)
        // ────────────────────────────────────────────────────────────────
        private static ValueInfo InferTargetInfo(string operand, EsfProgram? prog, ValueInfo rhsHint, bool isArithmetic)
        {
            var v = InferValueInfo(operand, prog);
            if (v.Kind != ValueKind.Unknown) return v;

            if (!isArithmetic)
            {
                // Safe RHS-aware fallbacks for movement
                if (rhsHint.Kind == ValueKind.String) return new ValueInfo(ValueKind.String, -1);
                if (rhsHint.Kind == ValueKind.Int) return new ValueInfo(ValueKind.Int, 0);
                if (rhsHint.Kind == ValueKind.Decimal) return new ValueInfo(ValueKind.Decimal, rhsHint.Scale < 0 ? 2 : rhsHint.Scale);
            }
            else
            {
                // Arithmetic with unknown LHS: if it *looks* like a field path, default to INT
                string conv = NormalizeOperand(CSharpUtils.ConvertOperand(operand));
                if (Regex.IsMatch(conv, @"\b(GlobalWorkstor|GlobalSqlRow|GlobalMaps)\.[A-Za-z_0-9]+\.[A-Za-z0-9_]+"))
                    return new ValueInfo(ValueKind.Int, 0);
            }

            return v;
        }

        private static ValueInfo InferValueInfo(string operand, EsfProgram? prog)
        {
            if (TryInferMapField(operand, prog, out var kind, out var scale))
                return new ValueInfo(kind, scale);

            if (TryInferRecordItem(operand, prog, out kind, out scale))
                return new ValueInfo(kind, scale);

            var s = operand.Trim();

            if (s.Length > 0 && (s[0] == '"' || s[0] == '\''))
                return new ValueInfo(ValueKind.String, -1);

            // numeric literal?
            var m = PureNumericRegex.Match(s);
            if (m.Success)
            {
                bool hasDot = m.Groups["num"].Value.Contains('.');
                return new ValueInfo(hasDot ? ValueKind.Decimal : ValueKind.Int, hasDot ? 2 : 0);
            }

            return ValueInfo.Unknown();
        }

        // ────────────────────────────────────────────────────────────────
        // Map/Record type inference (UPDATED: PACKED is always decimal)
        // ────────────────────────────────────────────────────────────────
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

            // NEW: PACKED is always represented as decimal in generated code, even when Decimals == 0
            if (tU == "PACKED")
            {
                kind = ValueKind.Decimal;
                scale = Math.Max(0, item.Decimals);
                return true;
            }

            // NUM: decimal only when Decimals > 0, otherwise int
            if (tU == "NUM")
            {
                if (item.Decimals > 0) { kind = ValueKind.Decimal; scale = item.Decimals; }
                else { kind = ValueKind.Int; scale = 0; }
                return true;
            }

            // BINARY → int (typical)
            if (tU == "BINARY")
            {
                kind = ValueKind.Int;
                scale = 0;
                return true;
            }

            // Otherwise treat as string
            kind = ValueKind.String; scale = -1;
            return true;
        }

        private static bool TryInferMapField(string operand, EsfProgram? prog, out ValueKind kind, out int scale)
        {
            kind = ValueKind.Unknown;
            scale = -1;
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
            if (vf == null)
            {
                kind = ValueKind.String; scale = -1;
                return false;
            }

            string tU = vf.Type.ToString().ToUpperInvariant();

            // Vfields are usually "NUM"/"CHA". Treat NUM with Decimals>0 as decimal; else int. CHA → string.
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

        // ────────────────────────────────────────────────────────────────
        // Arithmetic tokenizer & parser
        // ────────────────────────────────────────────────────────────────
        private enum TokKind { Identifier, Number, String, LParen, RParen, Plus, Minus, Star, Slash, Rem, Comma, End }
        private sealed record Token(TokKind Kind, string Text, int Pos);

        private sealed class Tokenizer
        {
            private readonly string _s;
            private int _i;

            public Tokenizer(string s) { _s = s ?? string.Empty; }

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
                int start = _i;

                if (c == '"' || c == '\'')
                {
                    char q = c; _i++;
                    var sb = new StringBuilder().Append(q);
                    while (_i < _s.Length)
                    {
                        char d = _s[_i++]; sb.Append(d);
                        if (d == q) break;
                        if (d == '\\' && _i < _s.Length) sb.Append(_s[_i++]);
                    }
                    return new Token(TokKind.String, sb.ToString(), start);
                }

                if (char.IsDigit(c))
                {
                    _i++;
                    while (_i < _s.Length && (char.IsDigit(_s[_i]) || _s[_i] == '.')) _i++;
                    var num = _s.Substring(start, _i - start);
                    return new Token(TokKind.Number, num, start);
                }

                if (c == '/')
                {
                    if (_i + 1 < _s.Length && _s[_i + 1] == '/')
                    { _i += 2; return new Token(TokKind.Rem, "//", start); }
                    _i++; return new Token(TokKind.Slash, "/", start);
                }
                if (c == '+') { _i++; return new Token(TokKind.Plus, "+", start); }
                if (c == '-') { _i++; return new Token(TokKind.Minus, "-", start); }
                if (c == '*') { _i++; return new Token(TokKind.Star, "*", start); }
                if (c == '(') { _i++; return new Token(TokKind.LParen, "(", start); }
                if (c == ')') { _i++; return new Token(TokKind.RParen, ")", start); }
                if (c == ',') { _i++; return new Token(TokKind.Comma, ",", start); }

                if (char.IsLetter(c) || c == '_')
                {
                    _i++;
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
                    string id = _s.Substring(start, _i - start);
                    return new Token(TokKind.Identifier, id, start);
                }

                _i++;
                return new Token(TokKind.Identifier, c.ToString(), start);
            }

            private void SkipWs()
            {
                while (_i < _s.Length && char.IsWhiteSpace(_s[_i])) _i++;
            }
        }

        private sealed class Parser
        {
            private readonly List<Token> _t;
            private int _p;
            private readonly EsfProgram? _prog;

            public Parser(List<Token> tokens, EsfProgram? prog)
            {
                _t = tokens ?? new List<Token> { new Token(TokKind.End, "", 0) };
                _p = 0; _prog = prog;
            }

            public string ParseNumericExpression(out bool usedRemainder)
            {
                usedRemainder = false;
                return ParseAddSub(ref usedRemainder);
            }

            private static string ToDec(string e) =>
                $"System.Convert.ToDecimal((object)({e}), System.Globalization.CultureInfo.InvariantCulture)";

            private string ParseAddSub(ref bool usedRem)
            {
                string left = ParseMulDivRem(ref usedRem);
                while (true)
                {
                    var k = Peek().Kind;
                    if (k == TokKind.Plus || k == TokKind.Minus)
                    {
                        var op = Next().Text;
                        string right = ParseMulDivRem(ref usedRem);
                        left = $"({left} {op} {right})";
                        continue;
                    }
                    break;
                }
                return left;
            }

            private string ParseMulDivRem(ref bool usedRem)
            {
                string left = ParseUnary(ref usedRem);
                while (true)
                {
                    var k = Peek().Kind;
                    if (k == TokKind.Star || k == TokKind.Slash || k == TokKind.Rem)
                    {
                        var opTok = Next();
                        string right = ParseUnary(ref usedRem);

                        if (opTok.Kind == TokKind.Rem)
                        {
                            usedRem = true;
                            left = $"({left} - ({right} * decimal.Truncate({left} / {right})))";
                        }
                        else
                        {
                            left = $"({left} {opTok.Text} {right})";
                        }
                        continue;
                    }
                    break;
                }
                return left;
            }

            private string ParseUnary(ref bool usedRem)
            {
                var k = Peek().Kind;
                if (k == TokKind.Plus)
                {
                    Next();
                    string e = ParseUnary(ref usedRem);
                    return $"(+{e})";
                }
                if (k == TokKind.Minus)
                {
                    Next();
                    string e = ParseUnary(ref usedRem);
                    return $"(-{e})";
                }
                return ParsePrimary(ref usedRem);
            }

            private string ParsePrimary(ref bool usedRem)
            {
                var t = Peek();
                switch (t.Kind)
                {
                    case TokKind.Number:
                        Next();
                        return t.Text.Contains('.') ? $"{t.Text}m" : $"(decimal){t.Text}";

                    case TokKind.String:
                        Next();
                        return $"decimal.Parse({t.Text}, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture)";

                    case TokKind.Identifier:
                        {
                            string id = Next().Text;
                            if (Peek().Kind == TokKind.LParen)
                            {
                                string call = id + CaptureCallText();
                                string conv = CSharpUtils.ConvertOperand(call);
                                return ToDec(conv);
                            }
                            else
                            {
                                string conv = CSharpUtils.ConvertOperand(id);
                                return ToDec(conv);
                            }
                        }

                    case TokKind.LParen:
                        {
                            Next();
                            string e = ParseAddSub(ref usedRem);
                            Expect(TokKind.RParen);
                            return $"({e})";
                        }

                    default:
                        Next();
                        return "(decimal)0";
                }
            }

            private string CaptureCallText()
            {
                var sb = new StringBuilder();
                int depth = 0;

                var tok = Next(); // '('
                sb.Append(tok.Text);
                depth++;

                while (_p < _t.Count && depth > 0)
                {
                    var t = Next();
                    sb.Append(t.Text);
                    if (t.Kind == TokKind.LParen) depth++;
                    else if (t.Kind == TokKind.RParen) depth--;
                }
                return sb.ToString();
            }

            private Token Peek() => _p < _t.Count ? _t[_p] : new Token(TokKind.End, "", _t.Count > 0 ? _t[^1].Pos : 0);
            private Token Next() => _p < _t.Count ? _t[_p++] : new Token(TokKind.End, "", _t.Count > 0 ? _t[^1].Pos : 0);
            private void Expect(TokKind k) { var t = Next(); /* lenient */ }
        }

        // ────────────────────────────────────────────────────────────────
        // Utils & typing
        // ────────────────────────────────────────────────────────────────
        private static bool TryGetContainerInstance(string operand, out string instanceExpr, out string recordName)
        {
            instanceExpr = string.Empty;
            recordName = string.Empty;
            if (string.IsNullOrWhiteSpace(operand)) return false;

            var m = ContainerRefRegex.Match(operand);
            if (!m.Success) return false;

            recordName = m.Groups["rec"].Value;
            instanceExpr = operand.EndsWith(".Current", StringComparison.Ordinal) ? operand : operand + ".Current";
            return true;
        }

        private static string NormalizeOperand(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return string.Empty;
            var trimmed = s.Trim();

            while (trimmed.Length > 0 && (trimmed[^1] == ';' || char.IsWhiteSpace(trimmed[^1])))
                trimmed = trimmed[..^1].TrimEnd();

            int cmt = trimmed.IndexOf("//", StringComparison.Ordinal);
            if (cmt >= 0) trimmed = trimmed[..cmt].TrimEnd();

            trimmed = Regex.Replace(trimmed, @"\s*\.\s*", ".");
            return trimmed;
        }

        private static string StripIndexer(string s)
        {
            if (string.IsNullOrEmpty(s)) return s;
            int i = s.IndexOf('[');
            return i >= 0 ? s.Substring(0, i) : s;
        }

        private static string Canon(string s)
        {
            if (string.IsNullOrEmpty(s)) return string.Empty;
            var sb = new StringBuilder(s.Length);
            foreach (var ch in s)
                if (char.IsLetterOrDigit(ch)) sb.Append(char.ToUpperInvariant(ch));
            return sb.ToString();
        }

        private static bool TryGetPureNumeric(string s, out bool isDecimal)
        {
            var m = PureNumericRegex.Match(s);
            if (!m.Success) { isDecimal = false; return false; }
            isDecimal = m.Groups["num"].Value.Contains('.');
            return true;
        }

        private enum ValueKind { Unknown, Int, Decimal, String }

        private readonly struct ValueInfo
        {
            public ValueKind Kind { get; }
            public int Scale { get; }

            public ValueInfo(ValueKind kind, int scale) { Kind = kind; Scale = scale; }

            public static ValueInfo Unknown() => new(ValueKind.Unknown, -1);
            public static ValueInfo NumericUnknown() => new(ValueKind.Decimal, -1);
        }
    }
}
