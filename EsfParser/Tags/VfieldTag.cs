using EsfParser.Esf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace EsfParser.Tags
{
    /// <summary>
    /// Enumeration of variable field data types.  CHA, DBCS and MIX denote
    /// single‑byte, double‑byte and mixed character data; NUM denotes a
    /// numeric field.  The type influences default justification and
    /// permissible input.
    /// </summary>
    public enum FieldType
    {
        CHA,
        DBCS,
        MIX,
        NUM
    }

    /// <summary>
    /// Represents a variable (editable) field on a map.  In addition to
    /// positional and structural attributes (ROW, COLUMN, BYTES, TYPE, NAME,
    /// DECIMALS, array INDEX etc.), this class incorporates a superset of
    /// VisualAge/ESF editing capabilities such as protection, auto‑skip,
    /// input requirements, numeric formatting, date masks, fill characters
    /// and more.  These attributes are stored on the tag so that runtime
    /// editors such as <see cref="EsfParser.Runtime.ConverseConsole"/> can
    /// enforce them.
    ///
    /// When first parsed from an ESF map, the <see cref="InitialValue"/>
    /// property stores any text defined in an EMAP block.  At runtime the
    /// <see cref="Value"/> property holds the current user input.
    /// </summary>
    public class VfieldTag : IEsfTagModel
    {
        [JsonIgnore] public string TagName => "VFIELD";

        // ---------------------------------------------------------------------
        // Fundamental map location and size
        // ---------------------------------------------------------------------
        [JsonPropertyName("row")]    public int Row { get; set; }
        [JsonPropertyName("column")] public int Column { get; set; }
        [JsonPropertyName("bytes")]  public int Bytes { get; set; }
        [JsonPropertyName("type")]   public FieldType Type { get; set; } = FieldType.CHA;
        [JsonPropertyName("name")]   public string Name { get; set; } = "";

        /// <summary>Number of decimal places for numeric values.</summary>
        [JsonPropertyName("decimals")] public int Decimals { get; set; } = 0;

        /// <summary>Optional editor order (EDITORDR) used to define the order of
        /// validation rules.  Null if not specified.</summary>
        [JsonPropertyName("editorOrder")]
        public int? EditorOrder { get; set; }

        /// <summary>Optional description (MESSAGES DESC=).</summary>
        [JsonPropertyName("description")] public string? Description { get; set; }

        /// <summary>Optional column index for MAPEDITS (COLUMN=).</summary>
        [JsonPropertyName("mapEditsColumn")] public int? MapEditsColumn { get; set; }

        /// <summary>Optional array index (VATTR INDEX=) when the same field name
        /// appears multiple times.</summary>
        [JsonPropertyName("index")] public int? Index { get; set; }

        /// <summary>Initial (default) value extracted from an EMAP block.</summary>
        [JsonPropertyName("initialValue")] public string? InitialValue { get; set; }

        /// <summary>Current value of the variable field.  This is the value
        /// presented to and edited by the user at runtime.</summary>
        public string Value { get; set; } = string.Empty;

        // ---------------------------------------------------------------------
        // Display attributes
        // ---------------------------------------------------------------------
        /// <summary>Foreground color of the field (default: White).</summary>
        public ConsoleColor Color { get; set; } = ConsoleColor.White;

        /// <summary>Intensity of the text: NORMAL (default), BRIGHT or DARK.</summary>
        public string Intensity { get; set; } = "NORMAL";

        /// <summary>Highlight attribute: NONE (default), BLINK, RVIDEO or UNDERSCORE.</summary>
        public string Highlight { get; set; } = "NONE";

        // ---------------------------------------------------------------------
        // Protection and navigation
        // ---------------------------------------------------------------------
        /// <summary>True if the field is protected (read‑only).</summary>
        public bool IsProtect { get; set; } = false;

        /// <summary>If true and the field is unprotected, automatically skip to
        /// the next field when the field is filled.  Setting AutoSkip will
        /// implicitly set <see cref="IsProtect"/> to true per VisualAge rules.</summary>
        public bool AutoSkip { get; set; } = false;

        /// <summary>True if this field is the initial cursor location on the map.
        /// Only the first such field is used if multiple fields specify this.</summary>
        public bool InitialCursor { get; set; } = false;

        // ---------------------------------------------------------------------
        // Input edits (logical rules enforced at runtime)
        // ---------------------------------------------------------------------
        public bool InputRequired { get; set; } = false;
        public bool RequireFill { get; set; } = false;
        public bool Numeric { get; set; } = false;
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

        /// <summary>Convenience property: true if Justify equals RIGHT.</summary>
        public bool RightJustify => string.Equals(Justify, "RIGHT", StringComparison.OrdinalIgnoreCase);

        /// <summary>
        /// Returns a multi‑line description of the field including both legacy and
        /// extended properties.  Useful for debugging and logging.
        /// </summary>
        public override string ToString()
        {
            return
              $"VFIELD [{Row},{Column}] Name={Name} Type={Type} Bytes={Bytes} " +
              $"Decimals={Decimals} EditorOrder={EditorOrder} MapEditsColumn={MapEditsColumn} Index={Index} " +
              $"Description='{Description}' Initial='{InitialValue}' Value='{Value}' Color={Color} Intensity={Intensity} Highlight={Highlight} " +
              $"Protect={IsProtect} AutoSkip={AutoSkip} InitialCursor={InitialCursor} InputRequired={InputRequired} RequireFill={RequireFill} " +
              $"Numeric={Numeric} Justify={Justify} Currency={Currency} NumericSeparator={NumericSeparator} Sign={Sign} ZeroEdit={ZeroEdit} " +
              $"DateMask={DateMask} HexOnly={HexOnly} FoldToUpper={FoldToUpper} MinInput={MinInput} MinValue={MinValue} MaxValue={MaxValue} " +
              $"FillChar='{FillChar}' MDT={ModifiedDataTag}";
        }

        /// <summary>
        /// Parse a TagNode into a <see cref="VfieldTag"/>.  This parser handles
        /// both legacy attributes defined in VisualAge Generator (such as
        /// EDITORDR, MESSAGES, MAPEDITS, VATTR and EMAP) and the extended
        /// attributes enumerated in Chapter 9 of the VisualAge reference.  The
        /// resulting tag can be used by runtime components to enforce
        /// protection, numeric formatting, date masks, auto‑skip and other
        /// behaviours.
        /// </summary>
        public static VfieldTag Parse(TagNode node)
        {
            // local helpers for parsing
            static int GetInt(TagNode n, params string[] keys)
            {
                foreach (var k in keys)
                    if (int.TryParse(n.GetString(k), out var v)) return v;
                return 0;
            }
            static decimal? GetDec(TagNode n, params string[] keys)
            {
                foreach (var k in keys)
                    if (decimal.TryParse(n.GetString(k), out var v)) return v;
                return null;
            }
            static string? GetStr(TagNode n, params string[] keys)
            {
                foreach (var k in keys)
                {
                    var s = n.GetString(k);
                    if (!string.IsNullOrWhiteSpace(s)) return s;
                }
                return null;
            }
            static string? FirstNonNull(params string?[] xs) => xs.FirstOrDefault(s => !string.IsNullOrWhiteSpace(s));
            static int FirstNonZero(params int[] xs) => xs.FirstOrDefault(x => x != 0);
            static bool ToBool(string? s)
            {
                if (string.IsNullOrWhiteSpace(s)) return false;
                switch (s.Trim().ToUpperInvariant())
                {
                    case "1": case "Y": case "YES": case "TRUE": case "ON": case "SET": return true;
                    case "0": case "N": case "NO": case "FALSE": case "OFF": return false;
                    default: return false;
                }
            }
            static bool TryColor(string? s, out ConsoleColor color)
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
                    case "TURQ":
                    case "TURQUOISE": color = ConsoleColor.Cyan; return true;
                    case "MONO": color = ConsoleColor.Gray; return true;
                    default: return Enum.TryParse<ConsoleColor>(s, true, out color);
                }
            }

            var tag = new VfieldTag
            {
                Row    = GetInt(node, "ROW"),
                Column = GetInt(node, "COLUMN", "COL"),
                Bytes  = GetInt(node, "BYTES", "LENGTH"),
                Name   = GetStr(node, "NAME") ?? string.Empty,
                Decimals = GetInt(node, "DECIMALS", "DEC")
            };

            // TYPE (CHA/DBCS/MIX/NUM)
            var typeText = node.GetString("TYPE");
            if (!string.IsNullOrWhiteSpace(typeText) && Enum.TryParse<FieldType>(typeText, true, out var ft))
                tag.Type = ft;

            // default Bytes from initial value length if missing
            if (tag.Bytes == 0)
            {
                string? iv = null;
                foreach (var child in node.Children)
                {
                    if (child.TagName.Equals("EMAP", StringComparison.OrdinalIgnoreCase) && !string.IsNullOrEmpty(child.Content))
                    {
                        iv = child.Content.TrimStart('.').TrimEnd('\r', '\n').Trim();
                        break;
                    }
                }
                tag.Bytes = Math.Max(1, iv?.Length ?? 1);
            }

            // EditorOrder (EDITORDR)
            if (node.Attributes.TryGetValue("EDITORDR", out var edList) && int.TryParse(edList.FirstOrDefault(), out var edVal))
                tag.EditorOrder = edVal;

            // child tags: MESSAGES, MAPEDITS, VATTR, EMAP
            foreach (var child in node.Children)
            {
                switch (child.TagName.ToUpperInvariant())
                {
                    case "MESSAGES":
                        if (child.Attributes.TryGetValue("DESC", out var dList)) tag.Description = dList.FirstOrDefault();
                        break;
                    case "MAPEDITS":
                        if (child.Attributes.TryGetValue("COLUMN", out var cList) && int.TryParse(cList.FirstOrDefault(), out var cval))
                            tag.MapEditsColumn = cval;
                        break;
                    case "VATTR":
                        if (child.Attributes.TryGetValue("INDEX", out var iList) && int.TryParse(iList.FirstOrDefault(), out var ival))
                            tag.Index = ival;
                        break;
                    case "EMAP":
                        if (!string.IsNullOrEmpty(child.Content))
                            tag.InitialValue = child.Content.TrimStart('.').TrimEnd('\r', '\n').Trim();
                        break;
                }
            }

            // display attributes
            if (TryColor(GetStr(node, "COLOR"), out var cc)) tag.Color = cc;
            var intensity = GetStr(node, "INTENSITY");
            if (!string.IsNullOrWhiteSpace(intensity)) tag.Intensity = intensity.Trim().ToUpperInvariant();
            var hilite = FirstNonNull(GetStr(node, "HILITE"), GetStr(node, "HIGHLIGHT"), GetStr(node, "EXTHILITE"));
            if (!string.IsNullOrWhiteSpace(hilite))
            {
                var h = hilite.Trim().ToUpperInvariant();
                tag.Highlight = h switch
                {
                    "BLINK" => "BLINK",
                    "RVIDEO" => "RVIDEO",
                    "UNDERSCORE" => "UNDERSCORE",
                    _ => "NONE"
                };
            }

            // protection flags
            var prot = FirstNonNull(GetStr(node, "PROTECT"), GetStr(node, "PROTECTION"));
            if (!string.IsNullOrWhiteSpace(prot))
            {
                var p = prot.Trim().ToUpperInvariant();
                if (p is "Y" or "YES" or "TRUE" or "SET" or "ON" or "PROTECTED") tag.IsProtect = true;
                if (p == "AUTOSKIP") { tag.IsProtect = true; tag.AutoSkip = true; }
            }
            var autoskip = GetStr(node, "AUTOSKIP");
            if (ToBool(autoskip)) { tag.IsProtect = true; tag.AutoSkip = true; }

            // initial cursor
            tag.InitialCursor = ToBool(FirstNonNull(GetStr(node, "INITIALCURSOR"), GetStr(node, "CURSORSTART")));

            // input edit flags
            tag.InputRequired   = ToBool(FirstNonNull(GetStr(node, "INPUTREQUIRED"), GetStr(node, "REQUIRED")));
            tag.RequireFill     = ToBool(FirstNonNull(GetStr(node, "REQUIREFILL"), GetStr(node, "FILLREQUIRED")));
            tag.Numeric         = ToBool(FirstNonNull(GetStr(node, "NUMERIC"), GetStr(node, "NUM")));
            // Decimals already set above
            tag.Justify         = (GetStr(node, "JUSTIFY") ?? (tag.Numeric ? "RIGHT" : "LEFT")).ToUpperInvariant();
            tag.Currency        = ToBool(GetStr(node, "CURRENCY"));
            tag.NumericSeparator= ToBool(FirstNonNull(GetStr(node, "NUMSEP"), GetStr(node, "NUMERICSEPARATOR"), GetStr(node, "SEPARATOR")));
            tag.Sign            = (GetStr(node, "SIGN") ?? "NONE").Trim().ToUpperInvariant();
            tag.ZeroEdit        = ToBool(FirstNonNull(GetStr(node, "ZEROEDIT"), GetStr(node, "ZERO")));
            tag.DateMask        = FirstNonNull(GetStr(node, "DATEMASK"), GetStr(node, "DATE"));
            tag.HexOnly         = ToBool(FirstNonNull(GetStr(node, "HEX"), GetStr(node, "HEXEDIT")));
            tag.FoldToUpper     = ToBool(GetStr(node, "FOLD"));
            tag.MinInput        = FirstNonZero(GetInt(node, "MININPUT"), GetInt(node, "MINLEN"));
            tag.MinValue        = GetDec(node, "MINVALUE");
            tag.MaxValue        = GetDec(node, "MAXVALUE");
            var fc              = GetStr(node, "FILLCHAR");
            if (!string.IsNullOrEmpty(fc)) tag.FillChar = fc.Trim()[0];
            tag.ModifiedDataTag = ToBool(FirstNonNull(GetStr(node, "MDT"), GetStr(node, "MODIFIEDDATATAG")));

            // Derived defaults per VisualAge spec
            if (tag.Numeric && tag.Decimals == 0 && string.Equals(tag.Justify, "LEFT", StringComparison.OrdinalIgnoreCase))
                tag.Justify = "RIGHT";
            if (tag.RequireFill && tag.MinInput == 0) tag.MinInput = tag.Bytes;

            return tag;
        }
    }
}