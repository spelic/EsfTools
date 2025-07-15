using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace EsfCore.Tags.Logic
{
    public class WorkstorQualifier : ITokenQualifier
    {
        private readonly Regex _wsRegex;
        private readonly Dictionary<string, string> _fieldToRecord;

        public WorkstorQualifier(IDictionary<string, IEnumerable<string>> workstorDefinitions)
        {
            // Flatten into (field, record) pairs
            var pairs = workstorDefinitions
     .SelectMany(kvp => kvp.Value.Select(f => (Field: f, Record: kvp.Key)))
     .OrderByDescending(t => t.Field.Length)    // longest‐first
     .GroupBy(t => t.Field, StringComparer.OrdinalIgnoreCase)
     .Select(g => g.First())
     .ToList();

            // Map field → record prefix
            _fieldToRecord = pairs
                .ToDictionary(
                    t => t.Field,
                    t => t.Record,
                    StringComparer.OrdinalIgnoreCase
                );

            // Build a single regex matching any of the fields
            // e.g. (?<=^|[^\.\w])(FIELDONE|FIELDTWO|...)(?=$|[^\.\w])
            var pattern = string.Join("|",
                pairs.Select(t => Regex.Escape(t.Field)));
            _wsRegex = new Regex(
                $@"(?<=^|[^\.\w])({pattern})(?=$|[^\.\w])",
                RegexOptions.Compiled | RegexOptions.IgnoreCase
            );
        }

        public string Qualify(string text)
        {
            // Single replacement pass
            return _wsRegex.Replace(text, m =>
            {
                var fld = m.Groups[1].Value;
                // look up the correct record with case‐insensitive key
                if (_fieldToRecord.TryGetValue(fld, out var rec))
                    return $"{rec}.{fld}";
                return fld; // fallback (shouldn't happen)
            });
        }
    }
}
