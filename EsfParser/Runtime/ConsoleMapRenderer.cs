using EsfParser.Tags;

namespace EsfParser.Runtime
{
    /// <summary>
    /// Provides a simple console renderer for ESF maps.  The renderer draws
    /// constant fields and reserves space for variable fields, then overlays
    /// each variable field's current value with appropriate justification
    /// and colour.  This class is used by <see cref="ConverseConsole"/> to
    /// draw the map prior to editing.
    /// </summary>
    public static class ConsoleMapRenderer
    {
        /// <summary>
        /// Renders a map on the console.  Constant fields are drawn first,
        /// variable fields are cleared to blanks, then the current values of
        /// variable fields are overlaid.  Colours and intensities are applied
        /// per field.  Supports wrapping across columns: overflow continues on next row column 1.
        /// </summary>
        public static void Render(int rows, int cols,
     IReadOnlyList<CfieldTag> cfields,
     IReadOnlyList<VfieldTag> vfields)
        {
            rows = Math.Max(1, rows);
            cols = Math.Max(1, cols);

            Console.Clear();

            // 1) Draw constants (direct, with wrapping + color/intensity)
            foreach (var cf in cfields)
            {
                int r0 = cf.Row - 1;
                int c0 = cf.Column;

                string raw = string.IsNullOrEmpty(cf.Value) ? " " : cf.Value;
                int maxLen = cf.Bytes > 0 ? cf.Bytes : raw.Length;
                string display = raw.Length > maxLen ? raw.Substring(0, maxLen) : raw.PadRight(maxLen);

                ConsoleColor prev = Console.ForegroundColor;
                if (cf.Intensity == "DARK") Console.ForegroundColor = ConsoleColor.DarkGray;
                else if (cf.Intensity == "BRIGHT") Console.ForegroundColor = ConsoleColor.White;
                else Console.ForegroundColor = cf.Color;

                try
                {
                    WriteWrappedToConsole(rows, cols, r0, c0, display);
                }
                catch
                {
                    // ignore small-console / positioning issues
                }
                finally
                {
                    Console.ForegroundColor = prev;
                }
            }

            // 2) Reserve blank areas for variable fields (direct, with wrapping)
            // This intentionally clears any constants under VFIELDs, matching "reserve space" semantics.
            foreach (var vf in vfields)
            {
                int r0 = vf.Row - 1;
                int c0 = vf.Column - 1;

                int n = Math.Max(0, vf.Bytes);
                if (n == 0) continue;

                try
                {
                    WriteWrappedToConsole(rows, cols, r0, c0, new string(' ', n));
                }
                catch
                {
                    // ignore
                }
            }

            // 3) Overlay variable field values (direct, with wrapping + color/intensity)
            foreach (var vf in vfields)
            {
                int r0 = vf.Row - 1;
                int c0 = vf.Column - 1;

                string val = string.IsNullOrEmpty(vf.Value) ? " " : vf.Value;
                string display = FitToBytes(val, vf.Bytes, vf.RightJustify);

                ConsoleColor prev = Console.ForegroundColor;
                if (vf.Intensity == "DARK") Console.ForegroundColor = ConsoleColor.DarkGray;
                else if (vf.Intensity == "BRIGHT") Console.ForegroundColor = ConsoleColor.White;
                else Console.ForegroundColor = vf.Color;

                try
                {
                    WriteWrappedToConsole(rows, cols, r0, c0, display);
                }
                catch
                {
                    // ignore
                }
                finally
                {
                    Console.ForegroundColor = prev;
                }
            }
        }

        private static string FitToBytes(string s, int len, bool right)
        {
            len = Math.Max(0, len);
            if (len == 0) return string.Empty;
            if (s.Length > len) return s[..len];
            return right ? s.PadLeft(len) : s.PadRight(len);
        }

        /// <summary>
        /// Writes text directly to console with wrapping across columns:
        /// if col exceeds cols-1, it continues at next row, col=0.
        /// Characters outside rows/cols are ignored.
        /// </summary>
        private static void WriteWrappedToConsole(int rows, int cols, int startRow, int startCol, string text)
        {
            if (string.IsNullOrEmpty(text)) return;

            int idx = 0;
            int r = startRow;
            int c = startCol;

            // Normalize start col into row wraps
            if (cols > 0 && c >= cols)
            {
                int addRows = c / cols;
                r += addRows;
                c = c % cols;
            }

            // Negative column => skip chars
            if (c < 0)
            {
                int skip = -c;
                idx += skip;
                c = 0;
            }

            // Negative row => skip full rows worth of chars
            if (r < 0 && cols > 0)
            {
                int skipChars = (-r) * cols;
                idx += skipChars;
                r = 0;
            }

            while (idx < text.Length && r < rows)
            {
                if (c >= cols)
                {
                    r++;
                    c = 0;
                    continue;
                }

                if (r >= 0 && r < rows && c >= 0 && c < cols)
                {
                    Console.SetCursorPosition(c, r);
                    Console.Write(text[idx]);
                }

                idx++;
                c++;
            }
        }

    }
}
