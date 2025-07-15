using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace EsfCore.Tags.Logic
{
    public class AssignmentTranslator : IStatementTranslator
    {
        private readonly IExpressionConverter _exprConv;
        private readonly HashSet<string> _recordNames;
        private readonly HashSet<string> _mapNames;

        private static readonly Regex _rx = new Regex(
     @"^\s*
      (?<lhs>            # left-hand side
         [A-Za-z_]\w*    # identifier
         (?:\.[A-Za-z_]\w*)*  # optional .identifiers
         (?:\[[^\]]+\])*       # optional [subscripts]
      )
      \s*=\s*
      (?<rhs>.+?)        # everything else, non-greedy
      ;?\s*$
    ",
     RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace
 );

        public AssignmentTranslator(
            IExpressionConverter exprConv,
            IEnumerable<string> recordNames,
            IEnumerable<string> mapNames)
        {
            _exprConv = exprConv;
            _recordNames = new HashSet<string>(recordNames, StringComparer.OrdinalIgnoreCase);
            _mapNames = new HashSet<string>(mapNames, StringComparer.OrdinalIgnoreCase);
        }

        public bool CanTranslate(string line) => _rx.IsMatch(line);

        public string Translate(string line, out string original)
        {
            original = line.TrimEnd();

            var m = _rx.Match(line);
            var rawLhs = m.Groups["lhs"].Value;
            var rawRhs = m.Groups["rhs"].Value;

            // if rawrhs is ERR string add .IsErr to left hand side and return
            if (rawRhs.Equals("ERR"))
            {
                var lhs = _exprConv.Convert(rawLhs);
                return $"{lhs}.IsErr";
            }

            // if *both* sides are WORKSTOR records, call CopyFrom against the
            // singleton instances in the Workstor class
            if (_recordNames.Contains(rawLhs) && _recordNames.Contains(rawRhs))
            {
                var lhs = _exprConv.Convert($"Workstor.{rawLhs}");
                var rhs = _exprConv.Convert($"Workstor.{rawRhs}");
                return $"{lhs}.CopyFrom({rhs});";
            }

            // if *both* sides are MAP records, call CopyFrom against the
            else  if (_mapNames.Contains(rawLhs) && _recordNames.Contains(rawRhs))
                {
                    var lhs = _exprConv.Convert($"{rawLhs}");
                    var rhs = _exprConv.Convert($"Workstor.{rawRhs}");
                    return $"{lhs}.CopyFrom({rhs});";
                }
            

            // otherwise fall back to a normal assignment
            var left = _exprConv.Convert(rawLhs);
            var right = _exprConv.Convert(rawRhs);
            return $"{left} = {right};";
        }
    }
}
