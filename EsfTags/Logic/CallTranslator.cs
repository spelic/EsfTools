using System;
using System.Text.RegularExpressions;

namespace EsfTags.Logic
{
    public class CallTranslator : IStatementTranslator
    {
        // Matches:   Foo();                Foo() /* comment */   Foo(); /* comment */
        // Captures the bare function name into the group "name"
        private static readonly Regex _rx = new Regex(
            @"^(?<name>[A-Za-z_]\w*)\s*\(\)\s*;?\s*(?:/\*.*)?$",
            RegexOptions.Compiled | RegexOptions.IgnoreCase
        );

        public bool CanTranslate(string line) => _rx.IsMatch(line);

        public string Translate(string line, out string org)
        {
            // preserve exactly what the user wrote (minus trailing line breaks)
            
            org = line.TrimEnd();

            var m = _rx.Match(line);
            var funcName = m.Groups["name"].Value;            // e.g. "IS00P01" or "EZECOMIT"
            var key = funcName.ToUpperInvariant();            // for switch

            return key switch
            {
                // system‐supplied “functions” that actually map back to SpecialFunctions.X.Execute()
                "EZECOMIT" => "SpecialFunctions.EZECOMIT.Execute();",
                "EZEROLLB" => "SpecialFunctions.EZEROLLB.Execute();",
                "EZERTN" => "SpecialFunctions.EZERTN.Execute();",

                // default: your own generated functions
                _ => $"{funcName}.Execute();"
            };
        }
    }
}
