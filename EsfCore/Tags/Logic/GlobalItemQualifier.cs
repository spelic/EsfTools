using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace EsfCore.Tags.Logic
{
    /// <summary>
    /// Prefixes any bare ESF‐ITEM name with GlobalItems.<Name>, but
    /// never touches names already qualified (i.e. those preceded by a dot).
    /// </summary>
    public class GlobalItemQualifier : ITokenQualifier
    {
        // you pass in the list of global-item names at construction
        private readonly HashSet<string> _names;
        private readonly Regex _rx;

        public GlobalItemQualifier(IEnumerable<string> globalItemNames)
        {
            _names = new HashSet<string>(globalItemNames, StringComparer.OrdinalIgnoreCase);
            _rx = new Regex(
                $@"(?<!GlobalItems\.)\b({string.Join("|", _names.Select(Regex.Escape))})\b",
                RegexOptions.Compiled | RegexOptions.IgnoreCase
            );
        }

        public string Qualify(string text)
            => _rx.Replace(text, "GlobalItems.$1");
    }
}
