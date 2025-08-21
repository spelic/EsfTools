using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EsfParser.Tags;

namespace EsfParser.Runtime
{
    /// <summary>
    /// Provides a simple CONVERSE editor for ESF maps running in the console.
    /// This class uses the <see cref="ConsoleMapRenderer"/> to draw the map
    /// and then allows the user to edit each unprotected variable field
    /// sequentially.  When the map is complete (Enter) or a function key
    /// (F1–F10 or Esc) is pressed, the method returns an appropriate
    /// <see cref="AidKey"/>.  Values entered by the user are stored on
    /// each <see cref="VfieldTag.Value"/>.
    /// </summary>
    public static class ConverseConsole
    {
        /// <summary>
        /// Displays the supplied map on the console and processes user input.
        /// </summary>
        /// <param name="rows">Maximum console rows (default 24).</param>
        /// <param name="cols">Maximum console columns (default 80).</param>
        /// <param name="cfields">Constant fields to render.</param>
        /// <param name="vfields">Variable fields to render and edit.</param>
        /// <param name="startCursorRow">Optional initial cursor row.</param>
        /// <param name="startCursorCol">Optional initial cursor column.</param>
        /// <returns>An AID key representing how the edit session terminated.</returns>
        public static AidKey RenderAndEdit(int rows, int cols,
            IReadOnlyList<CfieldTag> cfields,
            IReadOnlyList<VfieldTag> vfields,
            int? startCursorRow = null,
            int? startCursorCol = null)
        {
            // Draw initial map
            ConsoleMapRenderer.Render(rows, cols, cfields, vfields);

            // Determine tab order: unprotected fields sorted by row then column
            var order = vfields
                .Select((vf, i) => (vf, i))
                .Where(x => !x.vf.IsProtect)
                .OrderBy(x => x.vf.Row)
                .ThenBy(x => x.vf.Column)
                .Select(x => x.i)
                .ToArray();
            if (order.Length == 0) return AidKey.Enter;

            // Determine starting index
            int curIndex = 0;
            if (startCursorRow.HasValue && startCursorCol.HasValue)
            {
                for (int k = 0; k < order.Length; k++)
                {
                    var vf = vfields[order[k]];
                    if (vf.Row == startCursorRow.Value && vf.Column == startCursorCol.Value)
                    {
                        curIndex = k;
                        break;
                    }
                }
            }

            // Edit loop across fields
            while (true)
            {
                var vf = vfields[order[curIndex]];
                var aid = EditOneField(vf, rows, cols, out bool completedOrEnter);
                if (aid.HasValue)
                {
                    return aid.Value;
                }
                if (completedOrEnter)
                {
                    // move to next field; if none, return Enter
                    if (!Next(order, ref curIndex))
                        return AidKey.Enter;
                }
            }
        }

        // Edit a single field.  Returns null if editing continues to another
        // field; returns an AidKey if the user pressed a function key or Esc.
        private static AidKey? EditOneField(VfieldTag vf, int rows, int cols, out bool completedOrEnter)
        {
            completedOrEnter = false;

            // Ensure Value is not null
            vf.Value ??= string.Empty;

            // Truncate value to the field length
            if (vf.Value.Length > vf.Bytes)
                vf.Value = vf.Value.Substring(0, vf.Bytes);

            // Buffer holds the editable text
            var buffer = new StringBuilder(vf.Value);
            buffer.Length = Math.Min(buffer.Length, vf.Bytes);
            // caret position begins at end of buffer
            int caret = Math.Min(buffer.Length, vf.Bytes);

            WriteField(vf);
            SafeSetPos((vf.Column - 1) + caret, vf.Row - 1);

            while (true)
            {
                var keyInfo = Console.ReadKey(intercept: true);
                var key = keyInfo.Key;
                char ch = keyInfo.KeyChar;

                // AID keys
                if (key == ConsoleKey.Escape)
                    return AidKey.Esc;
                if (key >= ConsoleKey.F1 && key <= ConsoleKey.F10)
                {
                    return (AidKey)((int)AidKey.F1 + (key - ConsoleKey.F1));
                }
                // Enter accepts field and triggers move to next
                if (key == ConsoleKey.Enter)
                {
                    vf.Value = buffer.ToString();
                    WriteField(vf);
                    completedOrEnter = true;
                    return null;
                }
                // Tab key also moves to next field
                if (key == ConsoleKey.Tab)
                {
                    vf.Value = buffer.ToString();
                    WriteField(vf);
                    completedOrEnter = true;
                    return null;
                }
                // navigation keys
                switch (key)
                {
                    case ConsoleKey.LeftArrow:
                        if (caret > 0) caret--;
                        SafeSetPos((vf.Column - 1) + caret, vf.Row - 1);
                        continue;
                    case ConsoleKey.RightArrow:
                        if (caret < buffer.Length) caret++;
                        SafeSetPos((vf.Column - 1) + caret, vf.Row - 1);
                        continue;
                    case ConsoleKey.Home:
                        caret = 0;
                        SafeSetPos(vf.Column - 1, vf.Row - 1);
                        continue;
                    case ConsoleKey.End:
                        caret = buffer.Length;
                        SafeSetPos((vf.Column - 1) + caret, vf.Row - 1);
                        continue;
                    case ConsoleKey.Backspace:
                        if (caret > 0)
                        {
                            buffer.Remove(caret - 1, 1);
                            caret--;
                            vf.Value = buffer.ToString();
                            WriteField(vf);
                            SafeSetPos((vf.Column - 1) + caret, vf.Row - 1);
                        }
                        continue;
                    case ConsoleKey.Delete:
                        if (caret < buffer.Length)
                        {
                            buffer.Remove(caret, 1);
                            vf.Value = buffer.ToString();
                            WriteField(vf);
                            SafeSetPos((vf.Column - 1) + caret, vf.Row - 1);
                        }
                        continue;
                }
                // Character input
                if (!char.IsControl(ch))
                {
                    // Validate numeric/hex/fold to upper rules
                    if (vf.Numeric && !IsAllowedNumericChar(ch, buffer, caret, vf))
                        continue;
                    if (vf.HexOnly && !IsAllowedHexChar(ch))
                        continue;
                    if (vf.FoldToUpper) ch = char.ToUpperInvariant(ch);
                    // Insert or overtype character
                    if (buffer.Length < vf.Bytes)
                    {
                        buffer.Insert(caret, ch);
                        caret++;
                    }
                    else
                    {
                        // field is at maximum length; overtype at caret
                        if (caret < vf.Bytes)
                        {
                            buffer[caret] = ch;
                            caret++;
                        }
                    }
                    // update field value and display
                    vf.Value = buffer.ToString();
                    WriteField(vf);
                    SafeSetPos((vf.Column - 1) + caret, vf.Row - 1);
                    // auto‑advance when full
                    if (buffer.Length >= vf.Bytes)
                    {
                        completedOrEnter = true;
                        return null;
                    }
                }
            }
        }

        // Advances current index to the next item in order.  Returns false
        // if there is no next item.
        private static bool Next(int[] order, ref int curIndex)
        {
            if (curIndex + 1 < order.Length)
            {
                curIndex++;
                return true;
            }
            return false;
        }

        // Writes the current value of a variable field to the console using
        // the field's colour, intensity and justification.  The write
        // operation is limited to the field width and respects the current
        // Console encoding.
        private static void WriteField(VfieldTag vf)
        {
            string display = FitToBytes(vf.Value ?? string.Empty, vf.Bytes, vf.RightJustify);
            var prev = Console.ForegroundColor;
            if (vf.Intensity == "DARK") Console.ForegroundColor = ConsoleColor.DarkGray;
            else if (vf.Intensity == "BRIGHT") Console.ForegroundColor = ConsoleColor.White;
            else Console.ForegroundColor = vf.Color;
            try
            {
                Console.SetCursorPosition(vf.Column - 1, vf.Row - 1);
                Console.Write(display);
            }
            catch
            {
                // ignore if cursor position is off screen
            }
            Console.ForegroundColor = prev;
        }

        // Helper: set cursor position safely
        private static void SafeSetPos(int col, int row)
        {
            try
            {
                Console.SetCursorPosition(Math.Max(0, col), Math.Max(0, row));
            }
            catch
            {
                // ignore if console is too small
            }
        }

        // Determines if a character is allowed in a numeric field.  Accepts
        // digits, a minus sign at the beginning, and one decimal separator
        // (period or comma) if Decimals > 0.
        private static bool IsAllowedNumericChar(char ch, StringBuilder buffer, int caret, VfieldTag vf)
        {
            if (char.IsDigit(ch)) return true;
            if (ch == '-' && caret == 0 && !buffer.ToString().Contains('-')) return true;
            if ((ch == '.' || ch == ',') && vf.Decimals > 0 && !buffer.ToString().Contains('.') && !buffer.ToString().Contains(',')) return true;
            return false;
        }

        // Determines if a character is a valid hexadecimal digit.
        private static bool IsAllowedHexChar(char ch)
        {
            return char.IsDigit(ch) || (ch >= 'a' && ch <= 'f') || (ch >= 'A' && ch <= 'F');
        }

        private static string FitToBytes(string s, int len, bool right)
        {
            if (s.Length > len) return s.Substring(0, len);
            return right ? s.PadLeft(len) : s.PadRight(len);
        }
    }
}