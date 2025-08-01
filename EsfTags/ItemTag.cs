using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.Json.Serialization;
using EsfCore.Esf;

namespace EsfTags
{
    public enum ItemType
    {
        BIN, CHA, DBCS, HEX, MIX, NUM, NUMC, PACF, PACK, UNICODE
    }

    public class ItemTag : IEsfTagModel
    {
        [JsonIgnore] public string TagName => "ITEM";

        // --- Core attributes
        [JsonPropertyName("name")]
        public string Name { get; set; } = "";

        [JsonPropertyName("date")]
        public DateTime? Date { get; set; }

        [JsonPropertyName("time")]
        public TimeSpan? Time { get; set; }

        [JsonPropertyName("type")]
        public ItemType Type { get; set; } = ItemType.CHA;

        [JsonPropertyName("bytes")]
        public int Bytes { get; set; }

        [JsonPropertyName("decimals")]
        public int Decimals { get; set; }

        [JsonPropertyName("evenSql")]
        public bool EvenSql { get; set; }

        [JsonPropertyName("desc")]
        public string Description { get; set; } = "";

        // --- Child tag blocks

        [JsonPropertyName("mapEdits")]
        public MapEditsTag? MapEdits { get; set; }

        [JsonPropertyName("messages")]
        public MessagesTag? Messages { get; set; }

        [JsonPropertyName("uiProp")]
        public UiPropTag? UiProp { get; set; }

        [JsonPropertyName("genEdits")]
        public GenEditsTag? GenEdits { get; set; }

        [JsonPropertyName("uiMsgs")]
        public UiMsgsTag? UiMsgs { get; set; }

        [JsonPropertyName("numEdits")]
        public NumEditsTag? NumEdits { get; set; }

        /// <summary>
        /// The raw help‐text lines from :FLDHELP … :EFLDHELP
        /// </summary>
        [JsonPropertyName("fieldHelp")]
        public List<string> FieldHelp { get; set; } = new();

        /// <summary>
        /// The single line of text from :LABEL … :ELABEL
        /// </summary>
        [JsonPropertyName("label")]
        public string? Label { get; set; }

        public override string ToString()
        {
            return
                $"ITEM {Name} ({Type}, {Bytes} bytes, {Decimals} dec)  " +
                $"EvenSql={(EvenSql ? "Y" : "N")}  Desc='{Description}'";
        }

        public static ItemTag Parse(TagNode node)
        {
            static DateTime? ParseDate(string? s)
            {
                if (DateTime.TryParseExact(s?.Trim('\''), "MM/dd/yyyy",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None, out var d)) return d;
                return null;
            }
            static TimeSpan? ParseTime(string? s)
            {
                if (TimeSpan.TryParseExact(s?.Trim('\''), "hh\\:mm\\:ss",
                    CultureInfo.InvariantCulture, out var t)) return t;
                return null;
            }
            static bool ParseBool(string? s) => (s?.Trim().ToUpperInvariant()) switch
            {
                "Y" => true,
                _ => false
            };

            var tag = new ItemTag();

            // 1) Attributes on the same line as :ITEM
            string getAttr(string key) =>
                node.Attributes.TryGetValue(key, out var vals) && vals.Count > 0
                ? vals[0] : null;

            tag.Name = getAttr("NAME") ?? "";
            tag.Date = ParseDate(getAttr("DATE"));
            tag.Time = ParseTime(getAttr("TIME"));
            tag.Type = Enum.TryParse(getAttr("TYPE"), true, out ItemType t) ? t : ItemType.CHA;
            tag.Bytes = int.TryParse(getAttr("BYTES"), out var b) ? b : 0;
            tag.Decimals = int.TryParse(getAttr("DECIMALS"), out var d) ? d : 0;
            tag.EvenSql = ParseBool(getAttr("EVENSQL"));
            tag.Description = getAttr("DESC")?.Trim('\'') ?? "";

            // 2) Child blocks
            foreach (var child in node.Children)
            {
                switch (child.TagName.ToUpperInvariant())
                {
                    case "MAPEDITS":
                        tag.MapEdits = MapEditsTag.Parse(child);
                        break;

                    case "MESSAGES":
                        tag.Messages = MessagesTag.Parse(child);
                        break;

                    case "UIPROP":
                        tag.UiProp = UiPropTag.Parse(child);
                        break;

                    case "GENEDITS":
                        tag.GenEdits = GenEditsTag.Parse(child);
                        break;

                    case "UIMSGS":
                        tag.UiMsgs = UiMsgsTag.Parse(child);
                        break;

                    case "NUMEDITS":
                        tag.NumEdits = NumEditsTag.Parse(child);
                        break;

                    case "FLDHELP":
                        // each line in Content is one help line (preserve exact text)
                        tag.FieldHelp.AddRange(
                            (child.Content ?? "")
                            .Split(new[] { "\r\n", "\n" }, StringSplitOptions.None)
                            .Select(l => l.TrimStart('.'))  // remove leading dot
                        );
                        break;

                    case "LABEL":
                        // single‐line label: content starts with a dot
                        tag.Label = (child.Content ?? "").Trim()
                                        .TrimStart('.');
                        break;
                }
            }

            return tag;
        }
    }
}
