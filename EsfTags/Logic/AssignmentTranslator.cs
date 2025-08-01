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

            // Helper to qualify LHS/RHS if needed
            string Qualify(string expr)
            {
                // Already qualified
                if (expr.StartsWith("Workstor.", StringComparison.OrdinalIgnoreCase) ||
                    expr.StartsWith("GlobalItems.", StringComparison.OrdinalIgnoreCase) ||
                    _mapNames.Any(map => expr.StartsWith(map + ".", StringComparison.OrdinalIgnoreCase)))
                {
                    return expr;
                }

                // If it's a record name, qualify as Workstor
                if (_recordNames.Contains(expr))
                    return $"Workstor.{expr}";

                // If it's a map name, qualify as map
                if (_mapNames.Contains(expr))
                    return expr;

                return expr;
            }

            // Qualify LHS and RHS if needed
            var qualifiedLhs = Qualify(rawLhs);
            var qualifiedRhs = Qualify(rawRhs);

            // MAP‐to‐WORKSTOR copy
            if (_mapNames.Contains(rawLhs) && _recordNames.Contains(rawRhs))
            {
                var lhs = _exprConv.Convert(rawLhs);
                var rhs = _exprConv.Convert($"Workstor.{rawRhs}");
                return $"{lhs}.CopyFrom({rhs});";
            }

            // normal assignment
            var lhsCs = _exprConv.Convert(qualifiedLhs);
            var rhsCs = _exprConv.Convert(qualifiedRhs);

            // lookup their C# types
            var lhsType = _typeResolver.GetTypeOf(qualifiedLhs);
            var rhsType = _typeResolver.GetTypeOf(qualifiedRhs);

            if (string.IsNullOrEmpty(lhsType) || string.IsNullOrEmpty(rhsType))
            {
                throw new Exception($"Unknown type for assignment: {qualifiedLhs} = {qualifiedRhs}");
            }

            // if assigning a non‐string into a string, add .ToString()
            if (lhsType == "string"
             && rhsType != "string"
             && !rhsCs.EndsWith(".ToString()", StringComparison.Ordinal))
            {
                rhsCs = rhsCs + ".ToString()";
            }

            // if LHS is numeric but RHS is string, parse:
            if ((lhsType == "int" || lhsType == "decimal")
             && rhsType == "string")
            {
                var parser = lhsType == "int" ? "int.Parse" : "decimal.Parse";
                rhsCs = $"{parser}({rhsCs})";
            }

            return $"{lhsCs} = {rhsCs};";
        }
    }
}
