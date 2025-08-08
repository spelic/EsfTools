// TestStatement.cs
using System;
using System.Text;
using System.Text.RegularExpressions;
using EsfParser.CodeGen;
using EsfParser.Parser.Logic.Statements;

namespace EsfParser.Parser.Logic.Statements
{
    public class TestStatement : IStatement
    {
        public StatementType Type => StatementType.Test;
        public string OriginalCode { get; set; } = string.Empty;

        /// <summary>
        /// Everything after the keyword TEST and before ';'.
        /// Examples:
        ///   "ITEM BLANKS TRUEFN,FALSEFN"
        ///   "MAP1.FLD1 NUMERIC ISNUM,NOTNUM"
        ///   "EZEAID PF1 PFK1ONLY"
        ///   "TEMPNUM +2 OVER2,NOTOVER"
        ///   "TEMPNUM -3 ,LT3"
        /// </summary>
        public string Expression { get; set; } = string.Empty;

        public int LineNumber { get; set; }
        public int NestingLevel { get; set; } = 0;

        public string ToCSharp()
        {
            var ind = CSharpUtils.Indent(NestingLevel);
            var sb = new StringBuilder();

            // Parse once
            var parsed = Parser.Parse(Expression);

            // Subject and (maybe) argument become C# expressions
            string subject = CSharpUtils.ConvertOperand(parsed.Subject);

            // Build condition code
            string condExpr = BuildCondition(subject, parsed);

            // Targets (optional)
            string trueCall = string.IsNullOrWhiteSpace(parsed.TrueTarget)
                ? null
                : $"GlobalFunctions.{CSharpUtils.CleanName(parsed.TrueTarget)}();";
            string falseCall = string.IsNullOrWhiteSpace(parsed.FalseTarget)
                ? null
                : $"GlobalFunctions.{CSharpUtils.CleanName(parsed.FalseTarget)}();";

            sb.AppendLine($"{ind}{{  // TEST  {OriginalCode}".TrimEnd());
            sb.AppendLine($"{ind}    bool __cond = {condExpr};");

            if (trueCall != null || falseCall != null)
            {
                // Emit transfer(s)
                if (trueCall != null && falseCall != null)
                {
                    sb.AppendLine($"{ind}    if (__cond) {{ {trueCall} }} else {{ {falseCall} }}");
                }
                else if (trueCall != null)
                {
                    sb.AppendLine($"{ind}    if (__cond) {{ {trueCall} }}");
                }
                else // only false
                {
                    sb.AppendLine($"{ind}    if (!__cond) {{ {falseCall} }}");
                }
            }
            else
            {
                sb.AppendLine($"{ind}    // (no branch labels specified)");
            }

            sb.AppendLine($"{ind}}}");
            return sb.ToString();
        }

        public override string ToString()
            => $"TestStatement: {Expression} (Line: {LineNumber}, Nesting: {NestingLevel})";

        // ──────────────────────────────────────────────────────────────────
        //  Condition builders
        // ──────────────────────────────────────────────────────────────────

        private static string BuildCondition(string subjectExpr, ParsedTest p)
        {
            switch (p.Kind)
            {
                case TestKind.Blanks:
                    // Map semantics may be richer; for plain items treat as blanks/whitespace empty
                    return $"string.IsNullOrWhiteSpace(({subjectExpr}) ?? string.Empty)";

                case TestKind.Nulls:
                    // Treat like blanks for non-map items (per Test Facility note)
                    return $"string.IsNullOrWhiteSpace(({subjectExpr}) ?? string.Empty)";

                case TestKind.Numeric:
                    return $"System.Text.RegularExpressions.Regex.IsMatch((({subjectExpr}) ?? string.Empty).Trim(), \"^\\\\d+$\")";

                case TestKind.LengthEqual:
                    return $"((({subjectExpr}) ?? string.Empty).Trim()).Length == {p.LengthValue}";

                case TestKind.LengthGreater:
                    return $"((({subjectExpr}) ?? string.Empty).Trim()).Length > {p.LengthValue}";

                case TestKind.LengthLess:
                    return $"((({subjectExpr}) ?? string.Empty).Trim()).Length < {p.LengthValue}";

                case TestKind.EzeaId:
                    // Accept both "PF1" / "PA1" etc. Runtime must expose EzFunctions.EZEAID as comparable (string/enum)
                    return $"object.Equals(EzFunctions.EZEAID, \"{p.AidValue}\")";

                // Placeholders for advanced variants we can wire up later
                case TestKind.Modified:
                    // TODO: require runtime hook; placeholder always false to be safe
                    return "false /* TODO: MODIFIED requires map/record runtime hook */";

                case TestKind.Cursor:
                    return "false /* TODO: CURSOR requires map runtime hook */";

                case TestKind.Data:
                    // DATA means “non-blank/non-null present”
                    return $"!string.IsNullOrWhiteSpace(({subjectExpr}) ?? string.Empty)";

                case TestKind.Trunc:
                    return "false /* TODO: TRUNC requires SQL row runtime hook */";

                case TestKind.Sys:
                    // EZESYS SYS value — p.SysValue holds e.g. 'OS/400', compare against EzFunctions.EZESYS
                    return $"string.Equals(EzFunctions.EZESYS, \"{p.SysValue}\", StringComparison.OrdinalIgnoreCase)";

                default:
                    return "false /* unsupported TEST variant */";
            }
        }

        // ──────────────────────────────────────────────────────────────────
        //  Minimal parser for TEST expressions (string → structured)
        //  Handles:
        //    SUBJECT BLANK(S)|NULL(S)|NUMERIC|(+n|-n|n) [true[,false]]
        //    EZEAID KEY [true[,false]]
        //    SUBJECT DATA [true[,false]]
        // ──────────────────────────────────────────────────────────────────
        private static class Parser
        {
            private static readonly Regex _lenPat = new(@"^(?<sgn>[+-]?)(?<num>\d+)$", RegexOptions.Compiled | RegexOptions.CultureInvariant);

            public static ParsedTest Parse(string expr)
            {
                var result = new ParsedTest();

                if (string.IsNullOrWhiteSpace(expr))
                    return result;

                // Pull false target if there is a comma. We do this first to keep spaces simple.
                string left = expr;
                var comma = expr.IndexOf(',');
                if (comma >= 0)
                {
                    result.FalseTarget = expr[(comma + 1)..].Trim();
                    left = expr[..comma].Trim();
                }

                // Now left could be "SUBJ COND [TRUE]"
                var parts = left.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length == 0) return result;

                result.Subject = parts[0];

                // Only subject provided -> nothing else to do
                if (parts.Length == 1) return result;

                // Special: EZEAID <key> [true]
                if (parts[0].Equals("EZEAID", StringComparison.OrdinalIgnoreCase))
                {
                    result.Kind = TestKind.EzeaId;
                    result.AidValue = parts.Length > 1 ? parts[1] : string.Empty;
                    if (parts.Length > 2)
                        result.TrueTarget = parts[2];
                    return result;
                }

                // Special: EZESYS SYS-VALUE [true]
                if (parts[0].Equals("EZESYS", StringComparison.OrdinalIgnoreCase))
                {
                    result.Kind = TestKind.Sys;
                    result.SysValue = parts.Length > 1 ? parts[1] : string.Empty;
                    if (parts.Length > 2)
                        result.TrueTarget = parts[2];
                    return result;
                }

                // Generic: SUBJECT <cond> [true]
                string cond = parts[1].ToUpperInvariant();

                if (cond is "BLANK" or "BLANKS")
                {
                    result.Kind = TestKind.Blanks;
                    if (parts.Length > 2) result.TrueTarget = parts[2];
                    return result;
                }

                if (cond is "NULL" or "NULLS")
                {
                    result.Kind = TestKind.Nulls;
                    if (parts.Length > 2) result.TrueTarget = parts[2];
                    return result;
                }

                if (cond is "NUMERIC")
                {
                    result.Kind = TestKind.Numeric;
                    if (parts.Length > 2) result.TrueTarget = parts[2];
                    return result;
                }

                if (cond is "DATA")
                {
                    result.Kind = TestKind.Data;
                    if (parts.Length > 2) result.TrueTarget = parts[2];
                    return result;
                }

                if (cond is "MODIFIED")
                {
                    result.Kind = TestKind.Modified;
                    if (parts.Length > 2) result.TrueTarget = parts[2];
                    return result;
                }

                if (cond is "CURSOR")
                {
                    result.Kind = TestKind.Cursor;
                    if (parts.Length > 2) result.TrueTarget = parts[2];
                    return result;
                }

                if (cond is "TRUNC")
                {
                    result.Kind = TestKind.Trunc;
                    if (parts.Length > 2) result.TrueTarget = parts[2];
                    return result;
                }

                // Length form: SUBJECT n / +n / -n
                var m = _lenPat.Match(cond);
                if (m.Success)
                {
                    int n = int.Parse(m.Groups["num"].Value);
                    string s = m.Groups["sgn"].Value;
                    result.LengthValue = n;
                    result.Kind = s switch
                    {
                        "+" => TestKind.LengthGreater,
                        "-" => TestKind.LengthLess,
                        _ => TestKind.LengthEqual
                    };
                    if (parts.Length > 2) result.TrueTarget = parts[2];
                    return result;
                }

                // Fallback: unsupported
                result.Kind = TestKind.Unknown;
                if (parts.Length > 2) result.TrueTarget = parts[2];
                return result;
            }
        }

        private enum TestKind
        {
            Unknown = 0,
            Blanks,
            Nulls,
            Numeric,
            Data,
            Modified,
            Cursor,
            Trunc,
            LengthEqual,
            LengthGreater,
            LengthLess,
            EzeaId,
            Sys
        }

        private sealed class ParsedTest
        {
            public string Subject { get; set; } = string.Empty;

            public TestKind Kind { get; set; } = TestKind.Unknown;

            // For length tests
            public int LengthValue { get; set; }

            // For EZEAID
            public string AidValue { get; set; } = string.Empty;

            // For EZESYS
            public string SysValue { get; set; } = string.Empty;

            // Optional branch targets
            public string? TrueTarget { get; set; }
            public string? FalseTarget { get; set; }
        }
    }
}
