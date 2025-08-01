using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace EsfTags.Logic
{
    public class LogicProcessor : ILogicProcessor
    {
        private readonly ITokenQualifier[] _qualifiers;
        private readonly IConditionConverter _condConv;
        private readonly List<IStatementTranslator> _translators;

        public LogicProcessor(
            IEnumerable<ITokenQualifier> qualifiers,
            IConditionConverter condConv,
            IEnumerable<IStatementTranslator> translators)
        {
            _qualifiers = qualifiers?.ToArray() ?? throw new ArgumentNullException(nameof(qualifiers));
            _condConv = condConv ?? throw new ArgumentNullException(nameof(condConv));
            _translators = translators?.ToList() ?? throw new ArgumentNullException(nameof(translators));
        }

        public string Process(IEnumerable<string> lines, int indentSpaces = 6)
        {
            var sb = new StringBuilder();
            var indent = new string(' ', indentSpaces);

            var e = lines.GetEnumerator();
            while (e.MoveNext())
            {
                var raw = e.Current;
                var trimmed = raw.Trim();
                if (string.IsNullOrWhiteSpace(trimmed))
                    continue;

                // 1) pure block‐comment? (line really starts with /*)
                if (trimmed.StartsWith("/*"))
                {
                    sb.Append(indent)
                      .Append("// ")
                      .AppendLine(trimmed);
                    continue;
                }

                // 2) pull off any inline comment (/*…*/)
                string inlineComment = null;
                int commentIdx = raw.IndexOf("/*", StringComparison.Ordinal);
                if (commentIdx >= 0)
                {
                    inlineComment = raw.Substring(commentIdx).TrimEnd();
                    raw = raw.Substring(0, commentIdx);
                    trimmed = raw.Trim();
                    if (string.IsNullOrWhiteSpace(trimmed))
                    {
                        // if code part empty, just re-emit comment
                        sb.Append(indent).AppendLine("// " + inlineComment);
                        continue;
                    }
                }

                Console.WriteLine($"{indent} Processing: {raw}");

                // --- 1) qualify all tokens ---
                var qualified = _qualifiers.Aggregate(trimmed, (txt, q) => q.Qualify(txt));
                Console.WriteLine($"{indent}     1) Qualify: {qualified}");

                // --- 2) translators for single-line statements ---
                var translator = _translators.FirstOrDefault(t => t.CanTranslate(qualified));
                if (translator != null)
                {
                    var code = translator.Translate(qualified, out var org);
                    sb.Append(indent).Append("//ORG: ").AppendLine(org);
                    foreach (var ln in code.Split(Environment.NewLine))
                    {
                        sb.Append(indent).AppendLine(ln);
                        Console.WriteLine($"{indent}     2) Translators: {ln}");
                    }
                    if (inlineComment != null)
                        sb.Append(indent).AppendLine("// " + inlineComment);
                    
                    continue;
                } else
                    Console.WriteLine($"{indent}    2) Translators: NOT APPLIED");


                // 3) IF / WHILE
                if (qualified.StartsWith("IF ", StringComparison.OrdinalIgnoreCase) ||
                    qualified.StartsWith("WHILE ", StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine($"{indent}    3) IF/WHILE: Start");

                    bool isWhile = qualified.StartsWith("WHILE", StringComparison.OrdinalIgnoreCase);
                    string keyword = isWhile ? "WHILE" : "IF";

                    // 3a) --- collect condition lines up to the first semicolon ---
                    var condLines = new List<string>();

                    // strip off the keyword
                    string rest = trimmed.Substring(keyword.Length).Trim();
                    if (rest.EndsWith(";"))
                    {
                        // single‐line condition
                        condLines.Add(rest.TrimEnd(';').Trim());
                    }
                    else
                    {
                        // first line
                        condLines.Add(rest);

                        // subsequent lines until we see the `;`
                        while (e.MoveNext())
                        {
                            var more = e.Current.Trim();
                            if (more.EndsWith(";"))
                            {
                                condLines.Add(more.TrimEnd(';').Trim());
                                break;
                            }
                            condLines.Add(more);
                        }
                    }

                    // 3b) build the full COBOL condition text
                    var condText = string.Join(" ", condLines);
                    Console.WriteLine($"{indent}    3) GEN Condition: {condText}");

                    // 3c) now parse the body until the matching END;
                    var body = new List<string>();
                    int depth = 1;
                    while (depth > 0 && e.MoveNext())
                    {
                        var line = e.Current.Trim();
                        if (line.StartsWith(keyword + " ", StringComparison.OrdinalIgnoreCase))
                        {
                            depth++;
                            body.Add(line);
                        }
                        else if (line.Equals("END;", StringComparison.OrdinalIgnoreCase))
                        {
                            depth--;
                            if (depth > 0) body.Add(line);
                        }
                        else
                        {
                            body.Add(line);
                        }
                    }

                    // 3d) convert the COBOL condition into C#
                    var csCond = _condConv.Convert(condText);
                    Console.WriteLine($"{indent}    3) C# Condition: {csCond}");

                    // 3e) emit the header
                    sb.Append(indent).Append("//ORG: ").AppendLine(trimmed);
                    sb.Append(indent)
                      .Append(isWhile ? "while" : "if")
                      .Append(" (")
                      .Append(csCond)
                      .AppendLine(") {");

                    // 3f) recurse over the body
                    sb.Append(Process(body, indentSpaces + 4));

                    sb.Append(indent).AppendLine("}");
                    Console.WriteLine($"{indent}    3) IF/WHILE END");
                    continue;
                }

                // 3) ASSIGEMENT
                if (qualified.IndexOf(" = ")>0)
                {
                    Console.WriteLine($"{indent}    3) ASSIGEMENT {qualified}");
                    continue;
                }

                // --- 4) fallback for anything else ---
                Console.WriteLine($"{indent}    4) fallback {trimmed}");
                sb.Append(indent)
                  .Append("// UNSUPPORTED: ")
                  .AppendLine(trimmed);
            }

            return sb.ToString();
        }
    }
}
