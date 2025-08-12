// ConditionBuilder.cs
using System;
using System.Text;
using EsfParser.Esf;
using static EsfParser.CodeGen.CSharpUtils;

namespace EsfParser.CodeGen
{
    internal static class ConditionBuilder
    {
        public static string ToCSharp(string esfExpr, EsfProgram? p = null)
        {
            var prog = EsfLogicToCs.program ?? Program ?? p;
            var sb = new StringBuilder();
            int i = 0;

            while (i < esfExpr.Length)
            {
                char c = esfExpr[i];

                // whitespace passthrough
                if (char.IsWhiteSpace(c)) { sb.Append(c); i++; continue; }

                // ---------- string literals ----------
                // ESF allows both 'single' and "double" quoted; C# requires ".
                if (c == '"' || c == '\'')
                {
                    char q = c; i++;           // consume opening quote
                    sb.Append('"');            // always open with double quote in C#

                    while (i < esfExpr.Length)
                    {
                        char d = esfExpr[i++];

                        // end of this literal?
                        if (d == q) break;

                        // backslash escapes: copy the escaped char verbatim
                        if (d == '\\' && i < esfExpr.Length)
                        {
                            sb.Append('\\');
                            sb.Append(esfExpr[i++]);
                            continue;
                        }

                        // if ESF used single quotes, we may need to escape embedded "
                        if (q == '\'' && d == '"') sb.Append('\\');

                        sb.Append(d);
                    }

                    sb.Append('"');            // close as C# double-quoted string
                    continue;
                }

                // ---------- multi-char symbolic operators ----------
                // Handle them before single '=' so we don't corrupt <= and >= etc.
                if (c == '<')
                {
                    if (i + 1 < esfExpr.Length && esfExpr[i + 1] == '>') { sb.Append("!="); i += 2; continue; }
                    if (i + 1 < esfExpr.Length && esfExpr[i + 1] == '=') { sb.Append("<="); i += 2; continue; }
                }
                if (c == '>')
                {
                    if (i + 1 < esfExpr.Length && esfExpr[i + 1] == '=') { sb.Append(">="); i += 2; continue; }
                }
                if (c == '=')
                {
                    // If ESF ever emits '==', keep it as-is; otherwise translate '=' -> '=='
                    if (i + 1 < esfExpr.Length && esfExpr[i + 1] == '=') { sb.Append("=="); i += 2; continue; }
                    sb.Append("=="); i++; continue;
                }

                // ---------- number literal (simple: digits + optional '.') ----------
                if (char.IsDigit(c))
                {
                    int start = i;
                    while (i < esfExpr.Length && (char.IsDigit(esfExpr[i]) || esfExpr[i] == '.')) i++;
                    sb.Append(esfExpr.AsSpan(start, i - start));
                    continue;
                }

                // ---------- identifier / dotted / subscript / word-ops / functions ----------
                if (char.IsLetter(c) || c == '_')
                {
                    int start = i;

                    // token can include letters/digits/_/., plus bracketed subscripts: NAME[...]
                    while (i < esfExpr.Length)
                    {
                        char ch = esfExpr[i];
                        if (char.IsLetterOrDigit(ch) || ch == '_' || ch == '.')
                        { i++; continue; }
                        if (ch == '[')
                        {
                            int depth = 1; i++;
                            while (i < esfExpr.Length && depth > 0)
                            {
                                if (esfExpr[i] == '[') depth++;
                                else if (esfExpr[i] == ']') depth--;
                                i++;
                            }
                            continue;
                        }
                        break;
                    }

                    string word = esfExpr.Substring(start, i - start);

                    // logical word-operators (token-level)
                    if (word.Equals("AND", StringComparison.OrdinalIgnoreCase)) { sb.Append("&&"); continue; }
                    if (word.Equals("OR", StringComparison.OrdinalIgnoreCase)) { sb.Append("||"); continue; }
                    if (word.Equals("NOT", StringComparison.OrdinalIgnoreCase)) { sb.Append('!'); continue; }

                    // ---------- special case: <operand> IS [NOT] CURSOR ----------
                    // Example:
                    //    GlobalMaps.D133M01.STNAROC1 IS CURSOR
                    // -> GlobalMaps.D133M01.STNAROC1Tag.IsCursor()
                    //
                    // and:
                    //    <op> IS NOT CURSOR  ->  !<op>Tag.IsCursor()
                    int k = i;                 // scan ahead after <operand>
                    // skip spaces
                    while (k < esfExpr.Length && char.IsWhiteSpace(esfExpr[k])) k++;

                    // special "IS" handling
                    if (word.Equals("IS", StringComparison.OrdinalIgnoreCase))
                    {
                        // skip spaces
                        while (i < esfExpr.Length && char.IsWhiteSpace(esfExpr[i])) i++;

                        // capture next token
                        int nextStart = i;
                        while (i < esfExpr.Length && (char.IsLetterOrDigit(esfExpr[i]) || esfExpr[i] == '_'))
                            i++;
                        string nextWord = esfExpr.Substring(nextStart, i - nextStart);

                        if (nextWord.Equals("CURSOR", StringComparison.OrdinalIgnoreCase))
                        {
                            sb.Append("Tag.IsCursor()");
                        }
                        else if (nextWord.StartsWith("PF", StringComparison.OrdinalIgnoreCase))
                        {
                            // Example: PF3 -> ConsoleKey.F3
                            string pfNum = nextWord.Substring(2);
                            sb.Append("== ConsoleKey.F" + pfNum);
                        }
                        else
                        {
                            sb.Append("== ").Append(nextWord);
                        }
                        continue;
                    }

                    // function call? (peek next non-space char)
                    int j = i;
                    while (j < esfExpr.Length && char.IsWhiteSpace(j < esfExpr.Length ? esfExpr[j] : '\0')) j++;
                    if (j < esfExpr.Length && esfExpr[j] == '(')
                    {
                        // qualify only EZ* functions; other names are left as-is
                        if (word.StartsWith("EZ", StringComparison.OrdinalIgnoreCase))
                            sb.Append(QualifyIdentifier(word, prog));
                        else
                            sb.Append(word);
                        continue;
                    }

                    // plain operand
                    sb.Append(ConvertOperand(word));
                    continue;
                }

                // ---------- default passthrough (parens, + - * / , etc.) ----------
                sb.Append(c);
                i++;
            }

            return sb.ToString();
        }
    }
}
