using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace EsfTags.Logic
{
    public class WorkstorQualifier : ITokenQualifier
    {
        private readonly HashSet<string> _records;
        private readonly Dictionary<string, string> _fieldToRecord;
        // you pass in the list of global-item names at construction
        private readonly HashSet<string> _names;
        private readonly Regex _rx;
        /// <summary>
        /// workstorDefinitions: map of recordName → field‑names
        /// </summary>
        public WorkstorQualifier(IDictionary<string, IEnumerable<string>> workstorDefinitions, IEnumerable<string> globalItemNames)
        {
            // record names
            _records = new HashSet<string>(
                workstorDefinitions.Keys,
                StringComparer.OrdinalIgnoreCase
            );

            _names = new HashSet<string>(globalItemNames, StringComparer.OrdinalIgnoreCase);

            _rx = new Regex(
                $@"(?<!GlobalItems\.)\b({string.Join("|", _names.Select(Regex.Escape))})\b",
                RegexOptions.Compiled | RegexOptions.IgnoreCase
            );

            // flatten & filter out any "*" pseudo‑fields
            _fieldToRecord = workstorDefinitions
                .SelectMany(kvp => kvp.Value
                    .Where(name => name != "*")
                    .Select(name => (Field: name, Record: kvp.Key)))
                // longest‑first to avoid substring collisions
                .OrderByDescending(t => t.Field.Length)
                .GroupBy(t => t.Field, StringComparer.OrdinalIgnoreCase)
                .Select(g => g.First())
                .ToDictionary(
                    t => t.Field,
                    t => t.Record,
                    StringComparer.OrdinalIgnoreCase
                );
        }

        public string Qualify(string text)
        {
            //split on whitespace so that we preserve all original spacing
            var parts = Regex.Split(text, @"(\s+)");


            for (int i = 0; i < parts.Length; i++)
            {
                var tok = parts[i];
                if (string.IsNullOrWhiteSpace(tok)) continue;

                // if tok starts with ' or " then it's a string literal continue;
                if (tok.StartsWith('\'') || tok.StartsWith('"'))
                {
                    // do not qualify string literals
                    continue;
                }

                // chack if tok is not started with character
                // that is not a word character (e.g. letter, digit, or underscore)
                var beforeString = "";
                if (Regex.IsMatch(tok, @"^\W"))
                {
                    // remove all non-word characters from beggining of tok and store them in tok2 variable
                    beforeString = Regex.Replace(tok, @"^\W+", "");
                }
                
                // remove all non-word characters from beggining of tok
                tok = Regex.Replace(tok, @"^\W+", "");
                if (string.IsNullOrWhiteSpace(tok)) continue;

                // already fully qualified?
                if (tok.StartsWith("Workstor.", StringComparison.OrdinalIgnoreCase))
                    continue;

                // if tok has . inside use only what is before .
                if (tok.Contains('.'))
                {
                    // e.g. "IS00R10.ZAPOREDJE" → "Workstor.IS00R10.ZAPOREDJE"
                    var parts2 = tok.Split('.');
                    if (_records.Contains(parts2[0]))
                    {
                        parts[i] = $"Workstor.{parts[i]}";
                        continue;
                    }
                }

                if (_records.Contains(tok))
                {
                    // it's a record name
                    parts[i] = $"Workstor.{beforeString}{tok}";
                    continue;
                }
                else if (_fieldToRecord.TryGetValue(tok, out var rec))
                {
                    // it's a field of some record
                    parts[i] = $"Workstor.{rec}.{beforeString}{tok}";
                } else
                {                     
                    parts[i] = _rx.Replace(beforeString + tok, "GlobalItems.$1");
                }


            }

            return string.Concat(parts);
        }
    }
}
