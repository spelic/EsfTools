using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace EsfParser.Tags.Logic
{
    public class SetTranslator : IStatementTranslator
    {
        private readonly IExpressionConverter _exprConv;
        private static readonly Regex _rx = new(@"^SET\s+(?<tgt>\S+)\s+(?<attrs>.+);$", RegexOptions.IgnoreCase);

        public SetTranslator(IExpressionConverter exprConv) => _exprConv = exprConv;
        public bool CanTranslate(string line) => _rx.IsMatch(line);

        public string Translate(string line, out string? original)
        {
            original = line;
            var m = _rx.Match(line);
            if (!m.Success)
                throw new ArgumentException("Invalid SET statement", nameof(line));

            // first, convert the raw target (e.g. "IS00M01.ZASIFRA")
            var rawTgt = m.Groups["tgt"].Value;
            var converted = _exprConv.Convert(rawTgt);

            // if it's a "qualified" reference (contains a dot), append "Tag" suffix
            // so "IS00M01.ZASIFRA" => "IS00M01.ZASIFRATag"
            var tgt = converted.Contains('.') && !converted.EndsWith("Tag")
                ? converted + "Tag"
                : converted;

            var attrs = m.Groups["attrs"].Value
                         .Split(',', StringSplitOptions.RemoveEmptyEntries)
                         .Select(a => a.Trim().ToUpperInvariant());

            var calls = new List<string>();
            foreach (var a in attrs)
            {
                calls.Add(a switch
                {
                    "MODIFIED" => $"{tgt}.SetModified();",
                    "DARK" => $"{tgt}.SetDark();",
                    "EMPTY" => $"Workstor.{tgt}.SetEmpty();",
                    "CLEAR" => $"{tgt}.SetClear();",
                    "CURSOR" => $"{tgt}.SetCursor();",
                    "BRIGHT" => $"{tgt}.SetBright();",
                    "YELLOW" => $"{tgt}.SetYellow();",
                    "PROTECT" => $"{tgt}.Protect();", // TODO: this is just a temp
                    _ => throw new NotSupportedException($"SET attribute '{a}' not supported")
                });
            }

            return string.Join(Environment.NewLine, calls);
        }
    }
}
