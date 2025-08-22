using System;
using System.Collections.Generic;
using System.Text;
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
        /// per field.  Coordinates outside the console bounds are clamped.
        /// </summary>
        public static void Render(int rows, int cols,
            IReadOnlyList<CfieldTag> cfields,
            IReadOnlyList<VfieldTag> vfields)
        {
            rows = Math.Max(1, rows);
            cols = Math.Max(1, cols);

            Console.Clear();

            // Create a canvas buffer of characters (spaces by default)
            var canvas = new char[rows, cols];
            for (int r = 0; r < rows; r++)
                for (int c = 0; c < cols; c++)
                    canvas[r, c] = ' ';

            // Draw constants directly into the canvas
            foreach (var cf in cfields)
            {
                int r = Math.Clamp(cf.Row - 1, 0, rows - 1);
                int c = Math.Clamp(cf.Column - 1, 0, cols - 1);
                // default constant field value to single space when null or empty
                string text = string.IsNullOrEmpty(cf.Value) ? " " : cf.Value;
                int maxLen = cf.Bytes > 0 ? cf.Bytes : text.Length;
                int len = Math.Min(text.Length, maxLen);
                for (int i = 0; i < len && c + i < cols; i++)
                    canvas[r, c + i] = text[i];
            }

            // Reserve blank areas for variable fields
            foreach (var vf in vfields)
            {
                int r = Math.Clamp(vf.Row - 1, 0, rows - 1);
                int c = Math.Clamp(vf.Column - 1, 0, cols - 1);
                for (int i = 0; i < vf.Bytes && c + i < cols; i++)
                    canvas[r, c + i] = ' ';
            }

            // Blit the canvas to the console
            var sb = new StringBuilder(cols + 2);
            for (int r = 0; r < rows; r++)
            {
                sb.Clear();
                for (int c = 0; c < cols; c++) sb.Append(canvas[r, c]);
                Console.SetCursorPosition(0, r);
                Console.Write(sb.ToString());
            }

            // Overlay constant field values with colour and intensity
            foreach (var cf in cfields)
            {
                int r = Math.Clamp(cf.Row - 1, 0, rows - 1);
                int c = Math.Clamp(cf.Column - 1, 0, cols - 1);
                // default constant value to single space when null or empty
                string raw = string.IsNullOrEmpty(cf.Value) ? " " : cf.Value;
                // Format to field width (truncate or pad right)
                string display;
                int maxLen = cf.Bytes > 0 ? cf.Bytes : raw.Length;
                if (raw.Length > maxLen) display = raw.Substring(0, maxLen);
                else display = raw.PadRight(maxLen);
                // Choose console colour based on intensity for constants
                ConsoleColor prev = Console.ForegroundColor;
                if (cf.Intensity == "DARK") Console.ForegroundColor = ConsoleColor.DarkGray;
                else if (cf.Intensity == "BRIGHT") Console.ForegroundColor = ConsoleColor.White;
                else Console.ForegroundColor = cf.Color;
                try
                {
                    Console.SetCursorPosition(c, r);
                    Console.Write(display);
                }
                catch
                {
                    // ignore positioning errors on small consoles
                }
                Console.ForegroundColor = prev;
            }

            // Overlay variable field values with colour and justification
            foreach (var vf in vfields)
            {
                int r = Math.Clamp(vf.Row - 1, 0, rows - 1);
                int c = Math.Clamp(vf.Column - 1, 0, cols - 1);
                // default variable value to single space when null or empty
                string val = string.IsNullOrEmpty(vf.Value) ? " " : vf.Value;
                // Format value to fit field width
                string display = FitToBytes(val, vf.Bytes, vf.RightJustify);
                // Choose console colour based on intensity
                ConsoleColor prev = Console.ForegroundColor;
                if (vf.Intensity == "DARK") Console.ForegroundColor = ConsoleColor.DarkGray;
                else if (vf.Intensity == "BRIGHT") Console.ForegroundColor = ConsoleColor.White;
                else Console.ForegroundColor = vf.Color;
                try
                {
                    Console.SetCursorPosition(c, r);
                    Console.Write(display);
                }
                catch
                {
                    // ignore positioning errors on small consoles
                }
                Console.ForegroundColor = prev;
            }
        }

        private static string FitToBytes(string s, int len, bool right)
        {
            if (s.Length > len) return s[..len];
            return right ? s.PadLeft(len) : s.PadRight(len);
        }
    }
}