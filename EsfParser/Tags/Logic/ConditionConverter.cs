using System;

namespace EsfParser.Tags.Logic
{
    public class FastConditionConverter : IConditionConverter
    {
        private readonly IExpressionConverter _exprConv;

        // map COBOL words → C# symbols
        private static readonly Dictionary<string, string> _wordOps = new(StringComparer.OrdinalIgnoreCase)
        {
            {"EQ","=="}, {"NE","!="}, {"GT",">"}, {"LT","<"},
            {"GE",">="}, {"LE","<="}, {"IS","=="}, {"AND","&&"}, {"OR","||"}, {"NOT","!="}
        };

        public FastConditionConverter(IExpressionConverter exprConv)
        {
            _exprConv = exprConv;
        }

        public string Convert(string input)
        {
            var sb = new System.Text.StringBuilder(input.Length * 2);
            int i = 0, n = input.Length;

            while (i < n)
            {
                char c = input[i];

                // 1) String literal
                if (c == '\'' || c == '"')
                {
                    char quote = c;
                    i++;
                    int start = i;
                    while (i < n && input[i] != quote) i++;
                    var content = input.Substring(start, i - start)
                        .Replace("\\", "\\\\")
                        .Replace("\"", "\\\"");
                    sb.Append('"').Append(content).Append('"');
                    i++; // skip closing quote
                }
                // 2) COBOL integer‐division `//`
                else if (c == '/' && i + 1 < n && input[i + 1] == '/')
                {
                    sb.Append('%');
                    i += 2;
                }
                // 3) Two‐char operators (`>=`, `<=`, `<>`, `=>`)
                else if (i + 1 < n)
                {
                    var two = input.Substring(i, 2);
                    if (two == ">=" || two == "<=" || two == "<>" || two == "=>")
                    {
                        sb.Append(two == "<>" ? "!=" : two == "=>" ? ">=" : two);
                        i += 2;
                    }
                    else
                    {
                        goto single;
                    }
                }
                else
                {
                    goto single;
                }
                continue;

                single:
                // 4) Single‐char operators & punctuation
                if (c == '=')
                {
                    // we treat `=` as `==` (after WORD checks below handles word-based IS/EQ)
                    sb.Append("==");
                    i++;
                }
                else if (c is '>' or '<' or '(' or ')')
                {
                    sb.Append(c);
                    i++;
                }
                else if (char.IsWhiteSpace(c))
                {
                    sb.Append(c);
                    i++;
                }
                else if (char.IsLetter(c))
                {
                    // read full word
                    int start = i;
                    while (i < n && (char.IsLetterOrDigit(input[i]) || input[i] == '_' || input[i] == '.')) i++;
                    var word = input.Substring(start, i - start);

                    // 5) Keyword operators?
                    if (_wordOps.TryGetValue(word, out var op))
                    {
                        sb.Append(op);
                    }
                    else
                    {
                        // 6) Special case for "IS ERR"
                        if (word.Equals("ERR", StringComparison.OrdinalIgnoreCase) && sb.ToString().EndsWith("== "))
                        {
                            sb.Length -= 3; // Remove "== "
                            sb.Append(".IsErr");
                        }
                        else
                        {
                            // 7) Bare identifier: qualify via IExpressionConverter
                            //    but leave alone if already contains dot or is a number
                            if (word.Contains(".") || int.TryParse(word, out _))
                                sb.Append(word);
                            else
                                sb.Append(_exprConv.Convert(word));
                        }
                    }
                }
                else
                {
                    // anything else (digits, punctuation)
                    sb.Append(c);
                    i++;
                }
            }
            return sb.ToString();
        }
    }
}
