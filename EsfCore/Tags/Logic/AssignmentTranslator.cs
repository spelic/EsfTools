using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace EsfCore.Tags.Logic
{
    public class AssignmentTranslator : IStatementTranslator
    {
        private readonly IExpressionConverter _exprConv;
        private readonly ITypeResolver _typeResolver;
        private readonly HashSet<string> _recordNames;
        private readonly HashSet<string> _mapNames;

        private static readonly Regex _rx = new Regex(
            @"^\s*
              (?<lhs>               # left-hand side
                 [A-Za-z_]\w*       # identifier
                 (?:\.[A-Za-z_]\w*)*   # optional .identifiers
                 (?:\[[^\]]+\])*       # optional [subscripts]
              )
              \s*=\s*
              (?<rhs>.+?)          # everything else, non-greedy
              ;?\s*$
            ",
            RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace
        );

        public AssignmentTranslator(
            IExpressionConverter exprConv,
            ITypeResolver typeResolver,
            IEnumerable<string> recordNames,
            IEnumerable<string> mapNames)
        {
            _exprConv = exprConv ?? throw new ArgumentNullException(nameof(exprConv));
            _typeResolver = typeResolver ?? throw new ArgumentNullException(nameof(typeResolver));
            _recordNames = new HashSet<string>(recordNames, StringComparer.OrdinalIgnoreCase);
            _mapNames = new HashSet<string>(mapNames, StringComparer.OrdinalIgnoreCase);
        }

        public bool CanTranslate(string line) => _rx.IsMatch(line);

        public string Translate(string line, out string original)
        {
            original = line.TrimEnd();
            var m = _rx.Match(line);
            var rawLhs = m.Groups["lhs"].Value.Trim();
            var rawRhs = m.Groups["rhs"].Value.Trim();

            // special EZF ERR check
            if (rawRhs.Equals("ERR", StringComparison.OrdinalIgnoreCase))
            {
                var lhs = _exprConv.Convert(rawLhs);
                return $"{lhs}.IsErr;";
            }

            // WORKSTOR‐to‐WORKSTOR record copy
            if (_recordNames.Contains(rawLhs) && _recordNames.Contains(rawRhs))
            {
                var lhs = _exprConv.Convert($"Workstor.{rawLhs}");
                var rhs = _exprConv.Convert($"Workstor.{rawRhs}");
                return $"{lhs}.CopyFrom({rhs});";
            }

            // MAP‐to‐WORKSTOR copy
            if (_mapNames.Contains(rawLhs) && _recordNames.Contains(rawRhs))
            {
                var lhs = _exprConv.Convert(rawLhs);
                var rhs = _exprConv.Convert($"Workstor.{rawRhs}");
                return $"{lhs}.CopyFrom({rhs});";
            }

            // normal assignment
            var lhsCs = _exprConv.Convert(rawLhs);
            var rhsCs = _exprConv.Convert(rawRhs);

            // lookup their C# types
            var lhsType = _typeResolver.GetTypeOf(rawLhs);
            var rhsType = _typeResolver.GetTypeOf(rawRhs);

            string aaa;
            // check if aaa is not null or empty
            if (string.IsNullOrEmpty(lhsType) || string.IsNullOrEmpty(rhsType))
            {
                throw new Exception($"Unknown type for assignment: {rawLhs} = {rawRhs}");
            }


            // if assigning a non‐string into a string, add .ToString()
            if (lhsType == "string"
             && rhsType != "string"
             && !rhsCs.EndsWith(".ToString()", StringComparison.Ordinal))
            {
                rhsCs = rhsCs + ".ToString()";
            }

            // 3) if LHS is numeric but RHS is string, parse:
            if ((lhsType == "int" || lhsType == "decimal")
             && rhsType == "string")
            {
                // choose int.Parse or decimal.Parse
                var parser = lhsType == "int" ? "int.Parse" : "decimal.Parse";
                rhsCs = $"{parser}({rhsCs})";
            }


            return $"{lhsCs} = {rhsCs};";
        }
    }
}
