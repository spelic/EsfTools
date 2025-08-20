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
                if (c == '"' || c == '\'')
                {
                    char q = c; i++;
                    sb.Append('"');
                    while (i < esfExpr.Length)
                    {
                        char d = esfExpr[i++];
                        if (d == q) break;
                        if (d == '\\' && i < esfExpr.Length)
                        {
                            sb.Append('\\');
                            sb.Append(esfExpr[i++]);
                            continue;
                        }
                        if (q == '\'' && d == '"') sb.Append('\\');
                        sb.Append(d);
                    }
                    sb.Append('"');
                    continue;
                }

                // ---------- multi-char symbolic operators ----------
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
                    if (i + 1 < esfExpr.Length && esfExpr[i + 1] == '=') { sb.Append("=="); i += 2; continue; }
                    sb.Append("=="); i++; continue;
                }

                // ---------- number literal ----------
                if (char.IsDigit(c))
                {
                    int start = i;
                    while (i < esfExpr.Length && (char.IsDigit(esfExpr[i]) || esfExpr[i] == '.')) i++;
                    sb.Append(esfExpr.AsSpan(start, i - start));
                    continue;
                }

                // ---------- identifiers ----------
                if (char.IsLetter(c) || c == '_')
                {
                    int start = i;
                    while (i < esfExpr.Length)
                    {
                        char ch = esfExpr[i];
                        if (char.IsLetterOrDigit(ch) || ch == '_' || ch == '.' || ch == '-')
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

                    // logical word-operators
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

                    // ESF NULL literal → C# null
                    if (word.Equals("NULL", StringComparison.OrdinalIgnoreCase))
                    {
                        sb.Append("null");
                        continue;
                    }

                    // ---------- QUICK form: <SQLRecord> [NOT] (ERR|NRF) ----------
                    {
                        int look = i;
                        while (look < esfExpr.Length && char.IsWhiteSpace(esfExpr[look])) look++;

                        // optional NOT
                        int maybeNotStart = look;
                        while (look < esfExpr.Length && char.IsLetter(esfExpr[look])) look++;
                        string maybeNot = look > maybeNotStart ? esfExpr.Substring(maybeNotStart, look - maybeNotStart) : "";
                        bool hasNot = maybeNot.Equals("NOT", StringComparison.OrdinalIgnoreCase);
                        if (!hasNot) look = maybeNotStart; // rewind if NOT wasn't there

                        while (look < esfExpr.Length && char.IsWhiteSpace(esfExpr[look])) look++;
                        int statusStart = look;
                        while (look < esfExpr.Length && char.IsLetter(esfExpr[look])) look++;
                        string statusTok = look > statusStart ? esfExpr.Substring(statusStart, look - statusStart) : "";

                        if (TryMapStatusToken(statusTok, out var statusProp) &&
                            IsSqlRecordReference(word, prog))
                        {
                            string leftOp = ConvertOperand(word);
                            if (hasNot) sb.Append('!');
                            sb.Append(leftOp).Append('.').Append(statusProp);
                            i = look; // consume "[NOT] ERR/NRF"
                            continue;
                        }
                    }

                    // ---------- <SQLRecord> op (ERR|NRF)  (==, <>, EQ, NE) ----------
                    {
                        int look = i;
                        while (look < esfExpr.Length && char.IsWhiteSpace(esfExpr[look])) look++;

                        bool isEq = false, isNe = false;
                        int save = look;

                        if (look < esfExpr.Length)
                        {
                            if (esfExpr[look] == '=')
                            {
                                isEq = true; look++;
                                if (look < esfExpr.Length && esfExpr[look] == '=') look++; // '=='
                            }
                            else if (look + 1 < esfExpr.Length && esfExpr[look] == '<' && esfExpr[look + 1] == '>')
                            {
                                isNe = true; look += 2; // '<>'
                            }
                            else
                            {
                                int opStart = look;
                                while (look < esfExpr.Length && char.IsLetter(esfExpr[look])) look++;
                                string opWord = look > opStart ? esfExpr.Substring(opStart, look - opStart) : "";
                                if (opWord.Equals("EQ", StringComparison.OrdinalIgnoreCase)) { isEq = true; }
                                else if (opWord.Equals("NE", StringComparison.OrdinalIgnoreCase)) { isNe = true; }
                                else { look = save; }
                            }
                        }

                        if (isEq || isNe)
                        {
                            while (look < esfExpr.Length && char.IsWhiteSpace(esfExpr[look])) look++;
                            int rStart = look;
                            while (look < esfExpr.Length && (char.IsLetterOrDigit(esfExpr[look]) || esfExpr[look] == '_')) look++;
                            string rhsTok = look > rStart ? esfExpr.Substring(rStart, look - rStart) : "";

                            if (TryMapStatusToken(rhsTok, out var statusProp) &&
                                IsSqlRecordReference(word, prog))
                            {
                                string leftOp = ConvertOperand(word);
                                if (isNe) sb.Append('!');
                                sb.Append(leftOp).Append('.').Append(statusProp);
                                i = look; // consume op+status
                                continue;
                            }
                        }
                    }

                    // ---------- EZEAID op PF/PA ----------
                    if (IsEzEaidIdentifier(word))
                    {
                        int look = i;
                        while (look < esfExpr.Length && char.IsWhiteSpace(esfExpr[look])) look++;

                        bool isEq = false, isNe = false;
                        int save = look;

                        if (look < esfExpr.Length)
                        {
                            if (esfExpr[look] == '=')
                            {
                                isEq = true; look++;
                                if (look < esfExpr.Length && esfExpr[look] == '=') look++;
                            }
                            else if (look + 1 < esfExpr.Length && esfExpr[look] == '<' && esfExpr[look + 1] == '>')
                            {
                                isNe = true; look += 2;
                            }
                            else
                            {
                                int opStart = look;
                                while (look < esfExpr.Length && char.IsLetter(esfExpr[look])) look++;
                                string opWord = look > opStart ? esfExpr.Substring(opStart, look - opStart) : "";
                                if (opWord.Equals("EQ", StringComparison.OrdinalIgnoreCase)) { isEq = true; }
                                else if (opWord.Equals("NE", StringComparison.OrdinalIgnoreCase)) { isNe = true; }
                                else if (opWord.Equals("NOT", StringComparison.OrdinalIgnoreCase)) { isNe = true; }
                                else { look = save; }
                            }
                        }

                        if (isEq || isNe)
                        {
                            while (look < esfExpr.Length && char.IsWhiteSpace(esfExpr[look])) look++;
                            int rStart = look;
                            while (look < esfExpr.Length && (char.IsLetterOrDigit(esfExpr[look]) || esfExpr[look] == '_')) look++;
                            string rhsTok = look > rStart ? esfExpr.Substring(rStart, look - rStart) : "";

                            if (TryParseAid(rhsTok, out var aidKind, out var aidNum))
                            {
                                string leftOp = ConvertOperand(word);

                                if (aidKind == "PF" && aidNum >= 1 && aidNum <= 12)
                                {
                                    string op = isNe ? " != " : " == ";
                                    sb.Append(leftOp).Append(op).Append("ConsoleKey.F").Append(aidNum);
                                }
                                else
                                {
                                    if (isNe) sb.Append('!');
                                    sb.Append("EzFunctions.IsAid(\"").Append(aidKind).Append(aidNum).Append("\")");
                                }

                                i = look;
                                continue;
                            }
                        }
                    }

                    // ---------- <operand> IS [NOT] {CURSOR|PF/PA|ERR|NRF|NULL|<id>} ----------
                    {
                        int look = i;
                        while (look < esfExpr.Length && char.IsWhiteSpace(esfExpr[look])) look++;

                        int wStart = look;
                        while (look < esfExpr.Length && (char.IsLetter(esfExpr[look]) || esfExpr[look] == '_')) look++;
                        string maybeIs = look > wStart ? esfExpr.Substring(wStart, look - wStart) : "";

                        if (maybeIs.Equals("IS", StringComparison.OrdinalIgnoreCase))
                        {
                            while (look < esfExpr.Length && char.IsWhiteSpace(esfExpr[look])) look++;
                            int nStart = look;
                            while (look < esfExpr.Length && (char.IsLetter(esfExpr[look]) || esfExpr[look] == '_')) look++;
                            string maybeNot = look > nStart ? esfExpr.Substring(nStart, look - nStart) : "";
                            bool hasNot = maybeNot.Equals("NOT", StringComparison.OrdinalIgnoreCase);
                            if (!hasNot) look = nStart;

                            while (look < esfExpr.Length && char.IsWhiteSpace(esfExpr[look])) look++;
                            int rStart = look;
                            while (look < esfExpr.Length && (char.IsLetterOrDigit(esfExpr[look]) || esfExpr[look] == '_')) look++;
                            string rhs = look > rStart ? esfExpr.Substring(rStart, look - rStart) : "";

                            string leftOp = ConvertOperand(word);

                            // ERR / NRF on SQL records
                            if (TryMapStatusToken(rhs, out var statusProp) &&
                                IsSqlRecordReference(word, prog))
                            {
                                if (hasNot) sb.Append('!');
                                sb.Append(leftOp).Append('.').Append(statusProp);
                                i = look;
                                continue;
                            }

                            // CURSOR on map tags
                            if (rhs.Equals("CURSOR", StringComparison.OrdinalIgnoreCase))
                            {
                                string tagged = InsertTagBeforeTrailingIndexer(leftOp);
                                if (hasNot) sb.Append("!(").Append(tagged).Append(".IsCursor())");
                                else sb.Append(tagged).Append(".IsCursor()");
                                i = look;
                                continue;
                            }

                            // PF/PA on EZEAID
                            if (IsEzEaidIdentifier(word) && TryParseAid(rhs, out var aidKind2, out var aidNum2))
                            {
                                string op = hasNot ? " != " : " == ";
                                if (aidKind2 == "PF" && aidNum2 >= 1 && aidNum2 <= 12)
                                {
                                    sb.Append(leftOp).Append(op).Append("ConsoleKey.F").Append(aidNum2);
                                }
                                else if (aidKind2 == "ENTER")
                                {
                                    sb.Append(leftOp).Append(op).Append("ConsoleKey.Enter");
                                }
                                else
                                {
                                    if (hasNot) sb.Append('!');
                                    sb.Append("EzFunctions.IsAid(\"").Append(aidKind2).Append(aidNum2).Append("\")");
                                }
                                i = look;
                                continue;
                            }

                            // IS [NOT] NULL
                            if (rhs.Equals("NULL", StringComparison.OrdinalIgnoreCase))
                            {
                                string op = hasNot ? " != " : " == ";
                                sb.Append(leftOp).Append(op).Append("null");
                                i = look;
                                continue;
                            }

                            // generic IS
                            {
                                string op = hasNot ? " != " : " == ";
                                sb.Append(leftOp).Append(op).Append(ConvertOperand(rhs));
                                i = look;
                                continue;
                            }
                        }
                    }

                    // function call? (peek next non-space)
                    int j = i;
                    while (j < esfExpr.Length && char.IsWhiteSpace(j < esfExpr.Length ? esfExpr[j] : '\0')) j++;
                    if (j < esfExpr.Length && esfExpr[j] == '(')
                    {
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

                // default passthrough
                sb.Append(c);
                i++;
            }

            return sb.ToString();
        }

        private static bool TryMapStatusToken(string tok, out string prop)
        {
            prop = "";
            if (string.IsNullOrWhiteSpace(tok)) return false;
            var up = tok.Trim().ToUpperInvariant();
            if (up == "ERR") { prop = "Current.Err"; return true; }
            if (up == "NRF") { prop = "Current.Nrf"; return true; }
            return false;
        }

        private static string InsertTagBeforeTrailingIndexer(string token)
        {
            int bracket = token.IndexOf('[');
            if (bracket >= 0) return token.Insert(bracket, "Tag");
            if (token.EndsWith(".Tag", StringComparison.Ordinal) || token.EndsWith("Tag", StringComparison.Ordinal))
                return token;
            return token + "Tag";
        }

        private static bool IsEzEaidIdentifier(string originalWord)
        {
            if (string.IsNullOrEmpty(originalWord)) return false;
            int lastDot = originalWord.LastIndexOf('.');
            int segStart = lastDot + 1;
            string tail = originalWord.Substring(segStart);
            int idx = tail.IndexOf('[');
            if (idx >= 0) tail = tail.Substring(0, idx);
            return tail.Equals("EZEAID", StringComparison.OrdinalIgnoreCase);
        }

        private static bool IsSqlRecordReference(string token, EsfProgram? prog)
        {
            if (string.IsNullOrWhiteSpace(token) || prog?.Records?.Records == null) return false;
            if (token.IndexOf('[') >= 0) return false;

            var parts = token.Split('.');
            string recName;
            if (parts.Length == 1) recName = parts[0];
            else if (parts.Length == 2 && parts[0].Equals("GlobalSqlRow", StringComparison.OrdinalIgnoreCase)) recName = parts[1];
            else return false;

            string clean = CleanName(recName);
            foreach (var r in prog.Records.Records)
            {
                if (!r.Org.Equals("SQLROW", StringComparison.OrdinalIgnoreCase)) continue;
                if (CleanName(r.Name).Equals(clean, StringComparison.OrdinalIgnoreCase))
                    return true;
            }
            return false;
        }

        private static bool TryParseAid(string rhs, out string kind, out int num)
        {
            kind = ""; num = 0;
            if (string.IsNullOrWhiteSpace(rhs)) return false;
            var up = rhs.Trim().ToUpperInvariant();

            if (up.StartsWith("PF"))
            {
                if (int.TryParse(up.Substring(2), out num) && num >= 1 && num <= 24)
                { kind = "PF"; return true; }
                return false;
            }
            if (up.StartsWith("PA"))
            {
                if (int.TryParse(up.Substring(2), out num) && num >= 1 && num <= 3)
                { kind = "PA"; return true; }
                return false;
            }
            if (up.StartsWith("ENTER"))
            {
                { kind = "ENTER"; return true; }
            }
            return false;
        }
    }
}
