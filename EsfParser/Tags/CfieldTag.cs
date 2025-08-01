
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

    public class CfieldTag : IEsfTagModel
    {
        [JsonIgnore]
        public string TagName => "CFIELD";

        /// <summary>
        /// Row number of the constant field.
        /// </summary>
        [JsonPropertyName("row")]
        public int Row { get; set; }

        /// <summary>
        /// Column number of the constant field.
        /// </summary>
        [JsonPropertyName("column")]
        public int Column { get; set; }

        /// <summary>
        /// Length of the constant field in bytes.
        /// </summary>
        [JsonPropertyName("bytes")]
        public int Bytes { get; set; }

        /// <summary>
        /// One of CHA, DBCS or MIX.
        /// </summary>
        [JsonPropertyName("type")]
        public CfieldType Type { get; set; } = CfieldType.CHA;

        /// <summary>
        /// The literal text of the constant field, 
        /// with leading '.' trimmed from each line.
        /// May be multi-line.
        /// </summary>
        [JsonPropertyName("value")]
        public string Value { get; set; } = string.Empty;

        public override string ToString()
        {
            return $"CFIELD [row={Row},col={Column}] type={Type} bytes={Bytes} value='{Value}'";
        }

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

            // grab all lines of content, strip leading '.' and trim
            var raw = node.Content ?? "";
            var lines = raw
                .Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None)
                .Select(l => l.TrimStart('.'));
            // if line ends with X than join next line with this one and remove X
            lines = lines.Select(l => l.EndsWith("X") ? l.TrimEnd('X') : l)
                         .Select(l => l.TrimEnd());

            var value = string.Join("", lines).TrimEnd();

            return new CfieldTag
            {
                Row = row,
                Column = column,
                Bytes = bytes,
                Type = type,
                Value = value
            };
        }
    }
}
