using EsfParser.Tags;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace EsfParser.Esf
{
    public class CfieldTag2 : IEsfTagModel
    {
        public int Row { get; set; }
        public int Column { get; set; }
        public int Bytes { get; set; }
        public string Value { get; set; } = "";
        public ConsoleColor Color { get; set; } = ConsoleColor.Gray;
        public string Intensity { get; set; } = "NORMAL";
        public string Highlight { get; set; } = "NONE";

        public string TagName => "CFIELD";
        public static CfieldTag2 Parse(TagNode node)
        {
            var t = new CfieldTag2
            {
                Row = GetInt(node, "ROW"),
                Column = GetInt(node, "COL", "COLUMN"),
                Bytes = FirstNonZero(GetInt(node, "BYTES"), GetInt(node, "LENGTH")),
                Value = GetStr(node, "VALUE") ?? ""
            };

            var color = GetStr(node, "COLOR");
            if (TryColor(color, out var cc)) t.Color = cc;

            var intensity = GetStr(node, "INTENSITY");
            if (!string.IsNullOrWhiteSpace(intensity))
                t.Intensity = intensity.Trim().ToUpperInvariant();

            var hilite = FirstNonNull(
                GetStr(node, "HILITE"), GetStr(node, "HIGHLIGHT"), GetStr(node, "EXT-HILITE"), GetStr(node, "EXTHILITE"));
            if (!string.IsNullOrWhiteSpace(hilite))
            {
                var h = hilite.Trim().ToUpperInvariant();
                t.Highlight = h switch { "BLINK" or "RVIDEO" or "UNDERSCORE" => h, _ => "NONE" };
            }

            if (t.Bytes == 0) t.Bytes = Math.Max(1, t.Value?.Length ?? 1);
            return t;
        }

        private static string? GetStr(TagNode n, params string[] keys)
        {
            foreach (var k in keys)
            {
                var s = n.GetString(k);
                if (!string.IsNullOrWhiteSpace(s)) return s;
            }
            return null;
        }
        private static int GetInt(TagNode n, params string[] keys)
        {
            foreach (var k in keys)
                if (int.TryParse(n.GetString(k), out var v)) return v;
            return 0;
        }
        private static int FirstNonZero(params int[] xs) => xs.FirstOrDefault(x => x != 0);
        private static string? FirstNonNull(params string?[] xs) => xs.FirstOrDefault(s => !string.IsNullOrWhiteSpace(s));

        private static bool TryColor(string? s, out ConsoleColor color)
        {
            color = ConsoleColor.Gray;
            if (string.IsNullOrWhiteSpace(s)) return false;
            switch (s.Trim().ToUpperInvariant())
            {
                case "BLUE": color = ConsoleColor.Blue; return true;
                case "GREEN": color = ConsoleColor.Green; return true;
                case "RED": color = ConsoleColor.Red; return true;
                case "YELLOW": color = ConsoleColor.Yellow; return true;
                case "WHITE": color = ConsoleColor.White; return true;
                case "PINK": color = ConsoleColor.Magenta; return true;
                case "TURQ": case "TURQUOISE": color = ConsoleColor.Cyan; return true;
                case "MONO": color = ConsoleColor.Gray; return true;
                default: return Enum.TryParse<ConsoleColor>(s, true, out color);
            }
        }
    }

    public class VfieldTag2 : IEsfTagModel
    {
        public int Row { get; set; }
        public int Column { get; set; }
        public int Bytes { get; set; }
        public string Name { get; set; } = "";
        public string Value { get; set; } = "";

        public ConsoleColor Color { get; set; } = ConsoleColor.White;
        public string Intensity { get; set; } = "NORMAL";
        public string Highlight { get; set; } = "NONE";

        public bool IsProtect { get; set; } = false;
        public bool AutoSkip { get; set; } = false;
        public bool InitialCursor { get; set; } = false;

        public bool InputRequired { get; set; } = false;
        public bool RequireFill { get; set; } = false;
        public bool Numeric { get; set; } = false;
        public int Decimals { get; set; } = 0;
        public string Justify { get; set; } = "LEFT";
        public bool Currency { get; set; } = false;
        public bool NumericSeparator { get; set; } = false;
        public string Sign { get; set; } = "NONE";
        public bool ZeroEdit { get; set; } = false;
        public string? DateMask { get; set; } = null;
        public bool HexOnly { get; set; } = false;
        public bool FoldToUpper { get; set; } = false;
        public int MinInput { get; set; } = 0;
        public decimal? MinValue { get; set; }
        public decimal? MaxValue { get; set; }
        public char FillChar { get; set; } = ' ';
        public bool ModifiedDataTag { get; set; } = false;

        public bool RightJustify => string.Equals(Justify, "RIGHT", StringComparison.OrdinalIgnoreCase);

        public string TagName => "VFIELD";

        public static VfieldTag2 Parse(TagNode node)
        {
            var t = new VfieldTag2
            {
                Row = GetInt(node, "ROW"),
                Column = GetInt(node, "COL", "COLUMN"),
                Bytes = FirstNonZero(GetInt(node, "BYTES"), GetInt(node, "LENGTH")),
                Name = GetStr(node, "NAME") ?? "",
                Value = FirstNonNull(GetStr(node, "VALUE"), GetStr(node, "INITIALVALUE")) ?? ""
            };

            if (t.Bytes == 0) t.Bytes = Math.Max(1, t.Value?.Length ?? 1);

            if (TryColor(GetStr(node, "COLOR"), out var cc)) t.Color = cc;
            var intensity = GetStr(node, "INTENSITY");
            if (!string.IsNullOrWhiteSpace(intensity)) t.Intensity = intensity.Trim().ToUpperInvariant();
            var hilite = FirstNonNull(GetStr(node, "HILITE"), GetStr(node, "HIGHLIGHT"), GetStr(node, "EXTHILITE"));
            if (!string.IsNullOrWhiteSpace(hilite))
            {
                var h = hilite.Trim().ToUpperInvariant();
                t.Highlight = h switch { "BLINK" or "RVIDEO" or "UNDERSCORE" => h, _ => "NONE" };
            }

            var prot = FirstNonNull(GetStr(node, "PROTECT"), GetStr(node, "PROTECTION"));
            if (!string.IsNullOrWhiteSpace(prot))
            {
                var p = prot.Trim().ToUpperInvariant();
                if (p is "Y" or "YES" or "TRUE" or "SET" or "ON" or "PROTECTED") t.IsProtect = true;
                if (p == "AUTOSKIP") { t.IsProtect = true; t.AutoSkip = true; }
            }

            var autoskip = GetStr(node, "AUTOSKIP");
            if (ToBool(autoskip)) { t.IsProtect = true; t.AutoSkip = true; }

            t.InitialCursor = ToBool(FirstNonNull(GetStr(node, "INITIALCURSOR"), GetStr(node, "CURSORSTART")));

            t.InputRequired = ToBool(FirstNonNull(GetStr(node, "INPUTREQUIRED"), GetStr(node, "REQUIRED")));
            t.RequireFill = ToBool(FirstNonNull(GetStr(node, "REQUIREFILL")));
            t.Numeric = ToBool(FirstNonNull(GetStr(node, "NUMERIC"), GetStr(node, "NUM")));
            t.Decimals = GetInt(node, "DECIMALS", "DEC");
            t.Justify = (GetStr(node, "JUSTIFY") ?? (t.Numeric ? "RIGHT" : "LEFT")).ToUpperInvariant();
            t.Currency = ToBool(GetStr(node, "CURRENCY"));
            t.NumericSeparator = ToBool(FirstNonNull(GetStr(node, "NUMSEP"), GetStr(node, "NUMERICSEPARATOR")));
            t.Sign = (GetStr(node, "SIGN") ?? "NONE").Trim().ToUpperInvariant();
            t.ZeroEdit = ToBool(FirstNonNull(GetStr(node, "ZEROEDIT"), GetStr(node, "ZERO")));
            t.DateMask = FirstNonNull(GetStr(node, "DATEMASK"), GetStr(node, "DATE"));
            t.HexOnly = ToBool(FirstNonNull(GetStr(node, "HEX"), GetStr(node, "HEXEDIT")));
            t.FoldToUpper = ToBool(GetStr(node, "FOLD"));
            t.MinInput = FirstNonZero(GetInt(node, "MININPUT"), GetInt(node, "MINLEN"));
            t.MinValue = GetDec(node, "MINVALUE");
            t.MaxValue = GetDec(node, "MAXVALUE");
            var fc = GetStr(node, "FILLCHAR");
            if (!string.IsNullOrEmpty(fc)) t.FillChar = fc.Trim()[0];

            t.ModifiedDataTag = ToBool(FirstNonNull(GetStr(node, "MDT"), GetStr(node, "MODIFIEDDATATAG")));

            if (t.Numeric && t.Decimals == 0 && string.Equals(t.Justify, "LEFT", StringComparison.OrdinalIgnoreCase))
                t.Justify = "RIGHT";
            if (t.RequireFill && t.MinInput == 0) t.MinInput = t.Bytes;

            return t;
        }

        private static string? GetStr(TagNode n, params string[] keys)
        {
            foreach (var k in keys)
            {
                var s = n.GetString(k);
                if (!string.IsNullOrWhiteSpace(s)) return s;
            }
            return null;
        }
        private static int GetInt(TagNode n, params string[] keys)
        {
            foreach (var k in keys)
                if (int.TryParse(n.GetString(k), out var v)) return v;
            return 0;
        }
        private static decimal? GetDec(TagNode n, params string[] keys)
        {
            foreach (var k in keys)
                if (decimal.TryParse(n.GetString(k), out var v)) return v;
            return null;
        }
        private static int FirstNonZero(params int[] xs) => xs.FirstOrDefault(x => x != 0);
        private static string? FirstNonNull(params string?[] xs) => xs.FirstOrDefault(s => !string.IsNullOrWhiteSpace(s));

        private static bool ToBool(string? s)
        {
            if (string.IsNullOrWhiteSpace(s)) return false;
            switch (s.Trim().ToUpperInvariant())
            {
                case "1": case "Y": case "YES": case "TRUE": case "ON": case "SET": return true;
                case "0": case "N": case "NO": case "FALSE": case "OFF": return false;
                default: return false;
            }
        }

        private static bool TryColor(string? s, out ConsoleColor color)
        {
            color = ConsoleColor.White;
            if (string.IsNullOrWhiteSpace(s)) return false;
            switch (s.Trim().ToUpperInvariant())
            {
                case "BLUE": color = ConsoleColor.Blue; return true;
                case "GREEN": color = ConsoleColor.Green; return true;
                case "RED": color = ConsoleColor.Red; return true;
                case "YELLOW": color = ConsoleColor.Yellow; return true;
                case "WHITE": color = ConsoleColor.White; return true;
                case "PINK": color = ConsoleColor.Magenta; return true;
                case "TURQ": case "TURQUOISE": color = ConsoleColor.Cyan; return true;
                case "MONO": color = ConsoleColor.Gray; return true;
                default: return Enum.TryParse<ConsoleColor>(s, true, out color);
            }
        }
    }
}
