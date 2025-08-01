public static class PrettyOutputWriter
{
    public static void PrintPreprocessedLines(List<PreprocessedLine> lines)
    {
        const int linePad = 6;
        const int cleanPad = 50;
        const int inlinePad = 40;
        const int fullPad = 40;

        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("==== PREPROCESSED LOGIC ====");
        Console.ResetColor();

        Console.WriteLine($"{"Start",linePad} {"End",linePad}  {"CleanLine",-cleanPad} {"InlineComment",-inlinePad} {"FullComment",-fullPad}");
        Console.WriteLine(new string('-', linePad * 2 + cleanPad + inlinePad + fullPad + 6));

        foreach (var line in lines)
        {
            Console.ForegroundColor = string.IsNullOrWhiteSpace(line.CleanLine) ? ConsoleColor.DarkGray : ConsoleColor.Green;

            Console.WriteLine($"{line.StartLineNumber,linePad} {line.EndLineNumber,linePad}  " +
                $"{Truncate(line.CleanLine, cleanPad),-cleanPad} " +
                $"{Truncate(line.InlineComment, inlinePad),-inlinePad} " +
                $"{Truncate(line.FullLineComment, fullPad),-fullPad}");

            Console.ResetColor();

            // Print OriginalBlock in italic/dimmed style
            Console.ForegroundColor = ConsoleColor.DarkGray;
            var originalLines = line.OriginalBlock.Split('\n');

            foreach (var orig in originalLines)
            {
                Console.WriteLine($"{new string(' ', linePad * 2 + 2)}  >> {orig.Trim()}");
            }

            Console.ResetColor();
            Console.WriteLine();
        }
    }

    private static string Truncate(string? value, int max)
    {
        if (string.IsNullOrWhiteSpace(value)) return "";
        return value.Length > max ? value.Substring(0, max - 3) + "..." : value;
    }
}
