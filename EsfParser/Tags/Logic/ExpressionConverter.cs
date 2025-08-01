using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace EsfParser.Tags.Logic
{
    public class ExpressionConverter : IExpressionConverter
    {
        // literal matcher: single- or double-quoted
        private static readonly Regex _literal = new(@"^(['""])(.*)\1$", RegexOptions.Compiled);
        // subscript matcher: A[1]
        private static readonly Regex _subscript = new(@"(?<name>\w+)\[(?<idx>\d+)\]", RegexOptions.Compiled);
        // integer-division matcher (COBOL “//”)
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
            // 1) Deduplicate accidental “Foo.GlobalItems.Bar” → “Foo.Bar”
            // ─────────────────────────────────────────────────────────────────
            var mg = Regex.Match(expr,
                @"^(?<map>\w+)\.GlobalItems\.(?<fld>\w+)$",
                RegexOptions.IgnoreCase
            );
            if (mg.Success)
                return $"{mg.Groups["map"].Value}.{mg.Groups["fld"].Value}";

            // ─────────────────────────────────────────────────────────────────
            // 2) string literal → always emit C# double-quoted
            // ─────────────────────────────────────────────────────────────────
            var litM = _literal.Match(expr);
            if (litM.Success)
            {
                var content = litM.Groups[2].Value
                                 .Replace("\\", "\\\\")
                                 .Replace("\"", "\\\"");
                return $"\"{content}\"";
            }

            // ─────────────────────────────────────────────────────────────────
            // 3) numeric literal → pass through
            // ─────────────────────────────────────────────────────────────────
            if (Regex.IsMatch(expr, @"^\d+(\.\d+)?$"))
                return expr;

            // ─────────────────────────────────────────────────────────────────
            // 4) PFx / ENTER handling
            // ─────────────────────────────────────────────────────────────────
            var up = expr.ToUpperInvariant();
            if (up == "ENTER")
                return "ConsoleKey.Enter";
            var pfm = Regex.Match(up, @"^PF(?<n>\d{1,2})$", RegexOptions.IgnoreCase);
            if (pfm.Success)
                return $"ConsoleKey.F{pfm.Groups["n"].Value}";
            // … you can add PAx, BYPASS etc. here …

            // ─────────────────────────────────────────────────────────────────
            // 5) COBOL integer-division “//” → C# modulus “%”
            // ─────────────────────────────────────────────────────────────────
            expr = _intDiv.Replace(expr, "%");

            // ─────────────────────────────────────────────────────────────────
            // 6) subscripts: A[1] → A[0]
            // ─────────────────────────────────────────────────────────────────
            expr = _subscript.Replace(expr, m =>
            {
                var name = m.Groups["name"].Value;
                var idx = int.Parse(m.Groups["idx"].Value) - 1;
                return $"{name}[{idx}]";
            });

            // ─────────────────────────────────────────────────────────────────
            // 7) finally, run all your qualifiers (SpecialFunctions, GlobalItems,
            //    workstor, etc.) in order
            // ─────────────────────────────────────────────────────────────────
            return _qualifiers.Aggregate(expr, (current, q) => q.Qualify(current));
        }
    }
}
