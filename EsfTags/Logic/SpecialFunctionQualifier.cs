using System;
using System.Linq;
using System.Text.RegularExpressions;
using EsfCore.Tags;    // for SpecialFunctions

namespace EsfCore.Tags.Logic
{
    public class SpecialFunctionQualifier : ITokenQualifier
    {
        // grab all the static properties off SpecialFunctions
        private static readonly string[] _sfNames =
            typeof(SpecialFunctions)
                .GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static)
                .Select(p => p.Name)
                .ToArray();

        // build a single regex to catch any of them as a whole word,
        // but only if NOT already preceded by a dot or letter/underscore.
        private static readonly Regex _sfRx = new Regex(
            @"(?<![.\w])\b(" + string.Join("|", _sfNames.Select(Regex.Escape)) + @")\b",
            RegexOptions.Compiled | RegexOptions.IgnoreCase
        );

        // PF keys: PF1–PF24
        private static readonly Regex _pfRx = new Regex(
            @"(?<![.\w])PF(?<n>[1-9]|1\d|2[0-4])\b",
            RegexOptions.IgnoreCase | RegexOptions.Compiled
        );
        // PA keys: PA1–PA3
        private static readonly Regex _paRx = new Regex(
            @"(?<![.\w])PA(?<n>[1-3])\b",
            RegexOptions.IgnoreCase | RegexOptions.Compiled
        );

        public string Qualify(string text)
        {
            // if there's already a dot anywhere, skip PF/PA → ConsoleKey mapping,
            // but we can still map unqualified special functions afterwards.
            if (!text.Contains("."))
            {
                // 1) any PFn → ConsoleKey.Fn
                text = _pfRx.Replace(text, m => $"ConsoleKey.F{m.Groups["n"].Value}");
                // 2) any PAn → ConsoleKey.Fn
                text = _paRx.Replace(text, m => $"ConsoleKey.F{m.Groups["n"].Value}");
            }

            // 3) Now qualify all SpecialFunctions.<name>
            //    But don't re-qualify if already something.SpecialFunctions
            text = _sfRx.Replace(text, "SpecialFunctions.$1");

            return text;
        }
    }
}
