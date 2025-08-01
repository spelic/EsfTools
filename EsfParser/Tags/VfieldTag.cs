
using EsfParser.Esf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace EsfParser.Tags
{
    public enum FieldType
    {
        CHA,
        DBCS,
        MIX,
        NUM
    }

    public class VfieldTag : IEsfTagModel
    {
        [JsonIgnore] public string TagName => "VFIELD";

        [JsonPropertyName("row")]
        public int Row { get; set; }

        [JsonPropertyName("column")]
        public int Column { get; set; }

        [JsonPropertyName("bytes")]
        public int Bytes { get; set; }

        [JsonPropertyName("type")]
        public FieldType Type { get; set; } = FieldType.CHA;

        [JsonPropertyName("name")]
        public string Name { get; set; } = "";

        /// <summary>
        /// DECIMALS=…  (default 0)
        /// </summary>
        [JsonPropertyName("decimals")]
        public int Decimals { get; set; } = 0;

        /// <summary>
        /// EDITORDR=…  (optional)
        /// </summary>
        [JsonPropertyName("editorOrder")]
        public int? EditorOrder { get; set; }

        /// <summary>
        /// MESSAGES DESC=…  (optional)
        /// </summary>
        [JsonPropertyName("description")]
        public string? Description { get; set; }

        /// <summary>
        /// MAPEDITS COLUMN=…  (optional)
        /// </summary>
        [JsonPropertyName("mapEditsColumn")]
        public int? MapEditsColumn { get; set; }

        /// <summary>
        /// VATTR INDEX=…  (optional)
        /// </summary>
        [JsonPropertyName("index")]
        public int? Index { get; set; }

        /// <summary>
        /// initial value from the EMAP block (optional)
        /// </summary>
        [JsonPropertyName("initialValue")]
        public string? InitialValue { get; set; }

        public override string ToString()
        {
            return
              $"VFIELD [{Row},{Column}]  Name={Name}  Type={Type}  Bytes={Bytes}  " +
              $"Decimals={Decimals}  EditorOrder={EditorOrder}  " +
              $"MapEditsColumn={MapEditsColumn}  Index={Index}  " +
              $"Description='{Description}'  Initial='{InitialValue}'";
        }

        public static VfieldTag Parse(TagNode node)
        {
            int ParseInt(string? s, int def = 0)
                => int.TryParse(s, out var v) ? v : def;

            TEnum ParseEnum<TEnum>(string? s, TEnum def) where TEnum : struct, Enum
                => Enum.TryParse<TEnum>(s, ignoreCase: true, out var e) ? e : def;

            var tag = new VfieldTag
            {
                Row = ParseInt(node.GetString("ROW")),
                Column = ParseInt(node.GetString("COLUMN")),
                Bytes = ParseInt(node.GetString("BYTES")),
                Type = ParseEnum(node.GetString("TYPE"), FieldType.CHA),
                Name = node.GetString("NAME") ?? "",
                Decimals = ParseInt(node.GetString("DECIMALS"), 0),
            };

            // EDITORDR on the VFIELD itself
            if (node.Attributes.TryGetValue("EDITORDR", out var edList)
                && int.TryParse(edList.FirstOrDefault(), out var edVal))
            {
                tag.EditorOrder = edVal;
            }

            // Now pick off the four child‐tags if present:
            foreach (var child in node.Children)
            {
                switch (child.TagName.ToUpperInvariant())
                {
                    case "MESSAGES":
                        // MESSAGES has DESC=
                        if (child.Attributes.TryGetValue("DESC", out var dList))
                            tag.Description = dList.FirstOrDefault();
                        break;

                    case "MAPEDITS":
                        // MAPEDITS has COLUMN=
                        if (child.Attributes.TryGetValue("COLUMN", out var cList)
                            && int.TryParse(cList.FirstOrDefault(), out var cval))
                        {
                            tag.MapEditsColumn = cval;
                        }
                        break;

                    case "VATTR":
                        // VATTR has INDEX=
                        if (child.Attributes.TryGetValue("INDEX", out var iList)
                            && int.TryParse(iList.FirstOrDefault(), out var ival))
                        {
                            tag.Index = ival;
                        }
                        break;

                    case "EMAP":
                        // EMAP content block begins with a dot
                        if (!string.IsNullOrEmpty(child.Content))
                        {
                            tag.InitialValue = child.Content
                                .TrimStart('.')
                                .TrimEnd('\r', '\n')
                                .Trim();
                        }
                        break;
                }
            }

            return tag;
        }
    }
}
