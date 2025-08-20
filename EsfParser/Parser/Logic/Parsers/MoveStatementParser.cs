using EsfParser.Parser.Logic.Parsers;
using EsfParser.Parser.Logic.Statements;
using System;
using System.Collections.Generic;

public class MoveStatementParser : IStatementParser
{
    public bool CanParse(string line)
        => line.TrimStart().StartsWith("MOVE ", StringComparison.OrdinalIgnoreCase);

    public IStatement Parse(List<PreprocessedLine> lines, ref int index, int currentLevel = 0)
    {
        var line = lines[index];
        var original = line.OriginalBlock;
        // strip trailing ';' if present, then remove leading "MOVE "
        var raw = line.CleanLine.TrimEnd().TrimEnd(';');
        if (raw.Length < 5)
            return Unknown(line);

        var clean = raw.Substring(5).Trim(); // after "MOVE "

        // 1) Try robust split by the TOKEN 'TO' outside quotes/brackets/parentheses
        if (TrySplitByTopLevelTo(clean, out var src, out var dst))
        {
            return new MoveStatement
            {
                OriginalCode = original,
                LineNumber = line.StartLineNumber,
                Source = src.Trim(),
                Destination = dst.Trim()
            };
        }

        // 2) Fallback: split by first top-level whitespace (outside quotes/brackets)
        if (TrySplitByTopLevelWhitespace(clean, out src, out dst))
        {
            return new MoveStatement
            {
                OriginalCode = original,
                LineNumber = line.StartLineNumber,
                Source = src.Trim(),
                Destination = dst.Trim()
            };
        }

        // 3) Give up – let higher layers handle / report
        return Unknown(line);
    }

    private static UnknownStatement Unknown(PreprocessedLine line) =>
        new UnknownStatement
        {
            OriginalCode = line.OriginalBlock,
            LineNumber = line.StartLineNumber,
        };

    /// <summary>
    /// Find the token "TO" (case-insensitive) outside quotes, (), and [].
    /// Returns true and the split (left,right) if found.
    /// </summary>
    private static bool TrySplitByTopLevelTo(string s, out string left, out string right)
    {
        left = right = string.Empty;
        if (string.IsNullOrWhiteSpace(s)) return false;

        int paren = 0, bracket = 0;
        char? q = null;
        for (int i = 0; i < s.Length; i++)
        {
            char c = s[i];

            // handle doubled quotes inside same-quote string: "" => a single quote char
            if (q == '"' && c == '"' && i + 1 < s.Length && s[i + 1] == '"') { i++; continue; }
            if (q == '\'' && c == '\'' && i + 1 < s.Length && s[i + 1] == '\'') { i++; continue; }

            if (q != null)
            {
                // optional backslash escape (tolerant)
                if (c == '\\' && i + 1 < s.Length) { i++; continue; }
                if (c == q) q = null;
                continue;
            }

            if (c == '"' || c == '\'') { q = c; continue; }
            if (c == '(') { paren++; continue; }
            if (c == ')') { if (paren > 0) paren--; continue; }
            if (c == '[') { bracket++; continue; }
            if (c == ']') { if (bracket > 0) bracket--; continue; }

            if (paren == 0 && bracket == 0)
            {
                // check for word 'TO' with token boundaries
                if ((c == 'T' || c == 't') && i + 1 < s.Length && (s[i + 1] == 'O' || s[i + 1] == 'o'))
                {
                    int start = i;
                    int before = i - 1;
                    int after = i + 2;

                    bool leftBoundary = before < 0 || char.IsWhiteSpace(s[before]);
                    bool rightBoundary = after >= s.Length || char.IsWhiteSpace(s[after]);

                    if (leftBoundary && rightBoundary)
                    {
                        left = s.Substring(0, start);
                        right = (after < s.Length) ? s.Substring(after) : string.Empty;
                        return true;
                    }
                }
            }
        }
        return false;
    }

    /// <summary>
    /// Split into two parts by the first top-level whitespace outside quotes/() /[].
    /// Useful for legacy "MOVE A B" forms when 'TO' is omitted.
    /// </summary>
    private static bool TrySplitByTopLevelWhitespace(string s, out string left, out string right)
    {
        left = right = string.Empty;
        if (string.IsNullOrWhiteSpace(s)) return false;

        int paren = 0, bracket = 0;
        char? q = null;
        int i = 0;

        // scan to first top-level whitespace
        for (; i < s.Length; i++)
        {
            char c = s[i];

            if (q == '"' && c == '"' && i + 1 < s.Length && s[i + 1] == '"') { i++; continue; }
            if (q == '\'' && c == '\'' && i + 1 < s.Length && s[i + 1] == '\'') { i++; continue; }

            if (q != null)
            {
                if (c == '\\' && i + 1 < s.Length) { i++; continue; }
                if (c == q) q = null;
                continue;
            }

            if (c == '"' || c == '\'') { q = c; continue; }
            if (c == '(') { paren++; continue; }
            if (c == ')') { if (paren > 0) paren--; continue; }
            if (c == '[') { bracket++; continue; }
            if (c == ']') { if (bracket > 0) bracket--; continue; }

            if (paren == 0 && bracket == 0 && char.IsWhiteSpace(c))
                break;
        }

        if (i == 0 || i >= s.Length) return false;

        // left up to i; right after skipping any further whitespace
        left = s.Substring(0, i).TrimEnd();
        int j = i;
        while (j < s.Length && char.IsWhiteSpace(s[j])) j++;
        if (j >= s.Length) return false;
        right = s.Substring(j).TrimStart();
        return true;
    }
}
