using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace EsfCore.Tags.Logic
{
    public class ExpressionConverter : IExpressionConverter
    {
        // literal matcher: single‐ or double‐quoted
        private static readonly Regex _literal = new(@"^(['""])(.*)\1$", RegexOptions.Compiled);

        // subscript matcher: A[1]
        private static readonly Regex _subscript = new(@"(?<name>\w+)\[(?<idx>\d+)\]", RegexOptions.Compiled);

        // integer‐division matcher (COBOL “//”)
        private static readonly Regex _intDiv = new(@"//", RegexOptions.Compiled);

        private readonly ITokenQualifier[] _qualifiers;

        public ExpressionConverter(params ITokenQualifier[] qualifiers)
        {
            _qualifiers = qualifiers.ToArray();
        }

        public string Convert(string expr)
        {
            expr = expr.Trim();

            // ─────────────────────────────────────────────────────────────────
            // special-case: strip any "<MapName>.GlobalItems.<Field>" → "<MapName>.<Field>"
            // so you don't get "Foo.GlobalItems.Bar" doubled up
            // ─────────────────────────────────────────────────────────────────
            var mg = Regex.Match(expr, @"^(?<map>\w+)\.GlobalItems\.(?<fld>\w+)$", RegexOptions.IgnoreCase);
            if (mg.Success)
                return $"{mg.Groups["map"].Value}.{mg.Groups["fld"].Value}";

            // 1) string literal → always emit C# double-quoted
            var litM = _literal.Match(expr);
            if (litM.Success)
            {
                var content = litM.Groups[2].Value
                    .Replace("\\", "\\\\")
                    .Replace("\"", "\\\"");
                return $"\"{content}\"";
            }

            // 2) numeric literal → pass through
            if (Regex.IsMatch(expr, @"^\d+(\.\d+)?$"))
                return expr;

            // ─────────────────────────────────────────────────────────────────
            // special PFx / ENTER handling
            // ─────────────────────────────────────────────────────────────────
            var up = expr.ToUpperInvariant();
            if (up == "ENTER")
                return "ConsoleKey.Enter";
            var pfm = Regex.Match(up, @"^PF(?<n>\d{1,2})$", RegexOptions.IgnoreCase);
            if (pfm.Success)
                return $"ConsoleKey.F{pfm.Groups["n"].Value}";
            // (you can extend here for PAx, BYPASS, etc.)

            // 3) COBOL integer-division `//` → C# modulus `%`
            expr = _intDiv.Replace(expr, "%");

            // 4) subscripts: A[1] → A[0]
            expr = _subscript.Replace(expr, m =>
            {
                var name = m.Groups["name"].Value;
                var idx = int.Parse(m.Groups["idx"].Value) - 1;
                return $"{name}[{idx}]";
            });

            // 5) apply all token-qualifiers in turn (SpecialFunctions, GlobalItems, workstor…)
            return _qualifiers.Aggregate(expr, (current, q) => q.Qualify(current));
        }
    }
}
