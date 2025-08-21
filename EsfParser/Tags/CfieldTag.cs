
using EsfParser.Esf;
using System;
using System.Linq;
using System.Text.Json.Serialization;

namespace EsfParser.Tags
{
    public enum CfieldType
    {
        CHA,
        DBCS,
        MIX
    }

    /// <summary>
    /// Represents a constant (read‑only) field on a map.  In addition to the
    /// original ROW, COLUMN, BYTES and TYPE properties, this extended class
    /// supports optional display attributes such as color, intensity and
    /// highlight.  These attributes are ignored on devices that do not
    /// support them.
    /// </summary>
    public class CfieldTag : IEsfTagModel
    {
        [JsonIgnore]
        public string TagName => "CFIELD";

        /// <summary>
        /// Row number of the constant field (1‑based).  Required.
        /// </summary>
        [JsonPropertyName("row")]
        public int Row { get; set; }

        /// <summary>
        /// Column number of the constant field (1‑based).  Required.
        /// </summary>
        [JsonPropertyName("column")]
        public int Column { get; set; }

        /// <summary>
        /// Length of the constant field in bytes.  If zero or omitted the
        /// length of the Value will be used.
        /// </summary>
        [JsonPropertyName("bytes")]
        public int Bytes { get; set; }

        /// <summary>
        /// One of CHA, DBCS or MIX indicating the character set.  Defaults to CHA.
        /// </summary>
        [JsonPropertyName("type")]
        public CfieldType Type { get; set; } = CfieldType.CHA;

        /// <summary>
        /// The literal text of the constant field.  Leading '.' characters on
        /// each line are trimmed.  Lines ending with 'X' will be joined with
        /// the next line as per ESF convention.
        /// </summary>
        [JsonPropertyName("value")]
        public string Value { get; set; } = string.Empty;

        /// <summary>
        /// Foreground color of the constant field.  Default: Gray (MONO).
        /// Supported values: Blue, Green, Red, Yellow, White, Pink, Turquoise,
        /// Mono.  Unknown values fall back to Gray.
        /// </summary>
        [JsonPropertyName("color")]
        public ConsoleColor Color { get; set; } = ConsoleColor.Gray;

        /// <summary>
        /// Light intensity of the constant field: NORMAL (default), BRIGHT or DARK.
        /// </summary>
        [JsonPropertyName("intensity")]
        public string Intensity { get; set; } = "NORMAL";

        /// <summary>
        /// Highlight attribute: NONE (default), BLINK, RVIDEO or UNDERSCORE.
        /// Unsupported values are treated as NONE.
        /// </summary>
        [JsonPropertyName("highlight")]
        public string Highlight { get; set; } = "NONE";

        public override string ToString()
        {
            return
              $"CFIELD [row={Row},col={Column}] type={Type} bytes={Bytes} " +
              $"value='{Value}' color={Color} intensity={Intensity} highlight={Highlight}";
        }

        /// <summary>
        /// Parses a TagNode into a CfieldTag.  In addition to the standard
        /// attributes (ROW, COLUMN, BYTES, TYPE and VALUE), this parser
        /// recognizes COLOR, INTENSITY and HIGHLIGHT (or HILITE/EXT-HILITE/EXTHILITE).
        /// </summary>
        public static CfieldTag Parse(TagNode node)
        {
            // parse numeric attributes, default to zero on failure
            _ = int.TryParse(node.GetString("ROW"), out var row);
            _ = int.TryParse(node.GetString("COLUMN"), out var column);
            _ = int.TryParse(node.GetString("BYTES"), out var bytes);

            // parse TYPE, defaulting to CHA
            var typeText = node.Attributes
                               .TryGetValue("TYPE", out var tlist)
                           ? tlist.FirstOrDefault() ?? "CHA"
                           : "CHA";

            if (!Enum.TryParse<CfieldType>(typeText, ignoreCase: true, out var type))
                type = CfieldType.CHA;

            // parse color
            var colorStr = node.GetString("COLOR");
            var color = ConsoleColor.Gray;
            if (!string.IsNullOrWhiteSpace(colorStr) && TryColor(colorStr, out var cc))
            {
                color = cc;
            }

            // parse intensity
            var intensity = node.GetString("INTENSITY")?.Trim().ToUpperInvariant();
            if (string.IsNullOrWhiteSpace(intensity)) intensity = "NORMAL";

            // parse highlight (HILITE, HIGHLIGHT, EXT-HILITE, EXTHILITE)
            string? hilite = null;
            foreach (var key in new[] { "HILITE", "HIGHLIGHT", "EXT-HILITE", "EXTHILITE" })
            {
                var s = node.GetString(key);
                if (!string.IsNullOrWhiteSpace(s)) { hilite = s.Trim(); break; }
            }
            var highlight = "NONE";
            if (!string.IsNullOrWhiteSpace(hilite))
            {
                var h = hilite.ToUpperInvariant();
                highlight = h switch
                {
                    "BLINK" => "BLINK",
                    "RVIDEO" => "RVIDEO",
                    "UNDERSCORE" => "UNDERSCORE",
                    _ => "NONE"
                };
            }

            // grab all lines of content, strip leading '.' and trim
            var raw = node.Content ?? "";
            var lines = raw
                .Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None)
                .Select(l => l.TrimStart('.'));
            // if line ends with X then join next line with this one and remove X
            lines = lines.Select(l => l.EndsWith("X") ? l.TrimEnd('X') : l)
                         .Select(l => l.TrimEnd());

            var value = string.Join("", lines).TrimEnd();

            if (bytes == 0) bytes = Math.Max(1, value?.Length ?? 1);

            return new CfieldTag
            {
                Row = row,
                Column = column,
                Bytes = bytes,
                Type = type,
                Value = value,
                Color = color,
                Intensity = intensity,
                Highlight = highlight
            };
        }

        // helper to parse color names into ConsoleColor
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
                case "TURQ":
                case "TURQUOISE": color = ConsoleColor.Cyan; return true;
                case "MONO": color = ConsoleColor.Gray; return true;
                default: return Enum.TryParse<ConsoleColor>(s, true, out color);
            }
        }
    }
}
