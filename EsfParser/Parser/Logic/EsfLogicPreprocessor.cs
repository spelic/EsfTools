public static class EsfLogicPreprocessor
{
    public static List<PreprocessedLine> Preprocess(List<string> rawLines)
    {
        var result = new List<PreprocessedLine>();
        var buffer = new List<string>();
        var inlineComments = new List<(int lineNo, string text)>();
        int? startLine = null;

        for (int i = 0; i < rawLines.Count; i++)
        {
            string raw = rawLines[i];
            string trimmed = raw.Trim();

            // === Skip or extract full-line comments (starts with ;, //, /*)
            string fullComment = ExtractFullLineComment(trimmed);
            if (fullComment != null)
            {
                result.Add(new PreprocessedLine
                {
                    StartLineNumber = i + 1,
                    EndLineNumber = i + 1,
                    FullLineComment = fullComment,
                    OriginalBlock = raw,
                    CleanLine = ""
                });
                continue;
            }

            // as long as raw start with ; remove ; and trim in loop
            while (trimmed.StartsWith(";"))
            {
                trimmed = trimmed[1..].Trim();
            }

            // if raw empty or just whitespace, skip
            if (string.IsNullOrWhiteSpace(trimmed))
            {
                continue;
            }

            // === Start logic block
            if (buffer.Count == 0)
                startLine = i + 1;

            buffer.Add(raw);

            // === Check for end of logic (first semicolon outside quotes)
            int semiIndex = IndexOfFirstSemicolonOutsideQuotes(trimmed);
            if (semiIndex >= 0)
            {
                string logic = trimmed.Substring(0, semiIndex).Trim();
                string after = trimmed.Substring(semiIndex + 1).Trim();

                if (!string.IsNullOrWhiteSpace(after))
                    inlineComments.Add((i + 1, after));

                string cleanLogic = StripAfterFirstSemicolonOutsideQuotes(string.Join(" ", buffer.Select(b => b.Trim()))).Trim();

                result.Add(new PreprocessedLine
                {
                    StartLineNumber = startLine ?? i + 1,
                    EndLineNumber = i + 1,
                    OriginalBlock = string.Join("\n", buffer),
                    CleanLine = $"{cleanLogic};", // explicitly ends with ;
                    InlineComment = inlineComments.Count > 0
                        ? string.Join("; ", inlineComments.Select(c => $"{c.lineNo}: {c.text}"))
                        : null
                });

                // reset state
                buffer.Clear();
                inlineComments.Clear();
            }
        }

        // === Final flush
        if (buffer.Count > 0)
        {
            string full = string.Join("\n", buffer);
            string clean = StripAfterFirstSemicolonOutsideQuotes(full).Trim();

            result.Add(new PreprocessedLine
            {
                StartLineNumber = startLine ?? rawLines.Count,
                EndLineNumber = rawLines.Count,
                OriginalBlock = full,
                CleanLine = $"{clean};",
                InlineComment = inlineComments.Count > 0
                    ? string.Join("; ", inlineComments.Select(c => $"{c.lineNo}: {c.text}"))
                    : null
            });
        }

        return result;
    }

    private static string? ExtractFullLineComment(string line)
    {
        if (string.IsNullOrWhiteSpace(line)) return null;

        string trimmed = line.TrimStart();
        if (trimmed.StartsWith(";") || trimmed.StartsWith("//") || trimmed.StartsWith("/*"))
        {
            return trimmed.TrimStart(';', '/', '*').Trim();
        }

        return null;
    }

    private static int IndexOfFirstSemicolonOutsideQuotes(string line)
    {
        bool inDouble = false;
        bool inSingle = false;

        for (int i = 0; i < line.Length; i++)
        {
            char c = line[i];
            if (c == '"' && !inSingle) inDouble = !inDouble;
            else if (c == '\'' && !inDouble) inSingle = !inSingle;

            if (c == ';' && !inDouble && !inSingle)
                return i;
        }
        return -1;
    }

    private static string StripAfterFirstSemicolonOutsideQuotes(string line)
    {
        int idx = IndexOfFirstSemicolonOutsideQuotes(line);
        return idx >= 0 ? line.Substring(0, idx).Trim() : line;
    }
}
