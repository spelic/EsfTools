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

                    // relational word operators
                    if (word.Equals("EQ", StringComparison.OrdinalIgnoreCase)) { sb.Append("=="); continue; }
                    if (word.Equals("NE", StringComparison.OrdinalIgnoreCase)) { sb.Append("!="); continue; }
                    if (word.Equals("GT", StringComparison.OrdinalIgnoreCase)) { sb.Append(">"); continue; }
                    if (word.Equals("GE", StringComparison.OrdinalIgnoreCase)) { sb.Append(">="); continue; }
                    if (word.Equals("LT", StringComparison.OrdinalIgnoreCase)) { sb.Append("<"); continue; }
                    if (word.Equals("LE", StringComparison.OrdinalIgnoreCase)) { sb.Append("<="); continue; }




                    // ---------- lookahead: <operand> IS [NOT] {CURSOR|PFx|<identifier>} ----------
                    {
                        int look = i;
                        // skip spaces
                        while (look < esfExpr.Length && char.IsWhiteSpace(esfExpr[look])) look++;

                        // read next word (should be IS)
                        int wStart = look;
                        while (look < esfExpr.Length && (char.IsLetter(esfExpr[look]) || esfExpr[look] == '_')) look++;
                        string maybeIs = look > wStart ? esfExpr.Substring(wStart, look - wStart) : "";

                        if (maybeIs.Equals("IS", StringComparison.OrdinalIgnoreCase))
                        {
                            // parse optional NOT
                            while (look < esfExpr.Length && char.IsWhiteSpace(esfExpr[look])) look++;
                            int nStart = look;
                            while (look < esfExpr.Length && (char.IsLetter(esfExpr[look]) || esfExpr[look] == '_')) look++;
                            string maybeNot = look > nStart ? esfExpr.Substring(nStart, look - nStart) : "";
                            bool hasNot = maybeNot.Equals("NOT", StringComparison.OrdinalIgnoreCase);

                            if (!hasNot) look = nStart; // rewind if NOT wasn't there

                            // parse RHS token
                            while (look < esfExpr.Length && char.IsWhiteSpace(esfExpr[look])) look++;
                            int rStart = look;
                            while (look < esfExpr.Length && (char.IsLetterOrDigit(esfExpr[look]) || esfExpr[look] == '_')) look++;
                            string rhs = look > rStart ? esfExpr.Substring(rStart, look - rStart) : "";

                            // Build left operand text from the token we just scanned (word already includes subscripts/dots)
                            string leftOp = ConvertOperand(word);

                            // CASE 1: CURSOR check -> <op>Tag[...].IsCursor()
                            if (rhs.Equals("CURSOR", StringComparison.OrdinalIgnoreCase))
                            {
                                string tagged = InsertTagBeforeTrailingIndexer(leftOp);
                                if (hasNot)
                                {
                                    sb.Append("!(").Append(tagged).Append(".IsCursor())");
                                }
                                else
                                {
                                    sb.Append(tagged).Append(".IsCursor()");
                                }
                                i = look; // consume "IS [NOT] CURSOR"
                                continue;
                            }

                            // CASE 2: PFx mapping, only when LHS is EZEAID (qualified or not)
                            if (rhs.StartsWith("PF", StringComparison.OrdinalIgnoreCase) &&
                                IsEzEaidIdentifier(word)) // use original word to test identity
                            {
                                string pfNum = rhs.Substring(2);
                                string op = hasNot ? " != " : " == ";
                                sb.Append(leftOp).Append(op).Append("ConsoleKey.F").Append(pfNum);
                                i = look; // consume "IS [NOT] PFx"
                                continue;
                            }

                            // CASE 3: generic IS / IS NOT -> equality/inequality
                            {
                                string op = hasNot ? " != " : " == ";
                                sb.Append(leftOp).Append(op).Append(ConvertOperand(rhs));
                                i = look; // consume "IS [NOT] rhs"
                                continue;
                            }
                        }

                        // If not an IS-sequence, fall through to normal handling below.
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

        private static string InsertTagBeforeTrailingIndexer(string token)
        {
            // Insert ".Tag" before the last segment's '[' if present, else append ".Tag".
            // Examples:
            //  "A.B[1]"   -> "A.BTag[1]"
            //  "A.B.C"    -> "A.B.CTag"
            //  "A[B].C[1]"-> "A[B].CTag[1]"

            int bracket = token.IndexOf('[');

            if (bracket >= 0)
            {
                return token.Insert(bracket, "Tag");
            }
            else
            {
                if (token.EndsWith(".Tag", StringComparison.Ordinal) || token.EndsWith("Tag", StringComparison.Ordinal))
                    return token;

                return token + "Tag";
            }
        }

        private static bool IsEzEaidIdentifier(string originalWord)
        {
            // Check the raw token (with dots/indexers) to see if the last segment is EZEAID
            // Accepts: "EZEAID", "EzFunctions.EZEAID", "X.Y[0].EZEAID"
            if (string.IsNullOrEmpty(originalWord)) return false;

            // Strip trailing indexer(s) from the last segment only
            int lastDot = originalWord.LastIndexOf('.');
            int segStart = lastDot + 1;
            string tail = originalWord.Substring(segStart);

            // Remove trailing [..] groups
            int idx = tail.IndexOf('[');
            if (idx >= 0) tail = tail.Substring(0, idx);

            return tail.Equals("EZEAID", StringComparison.OrdinalIgnoreCase);
        }

    }
}
