using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
      // for TagNode

using EsfParser.Esf;     // for CfieldTag, VfieldTag

namespace EsfParser.Tags
{
    public class MapTag : IEsfTagModel
    {
        [JsonIgnore]
        public string TagName => "MAP";

        [JsonPropertyName("groupName")]
        public string GrpName { get; set; } = "";

        [JsonPropertyName("mapName")]
        public string MapName { get; set; } = "";

        [JsonPropertyName("date")]
        public string Date { get; set; } = "";

        [JsonPropertyName("time")]
        public string Time { get; set; } = "";

        /// <summary>
        /// BYPKEY= up to five keys (1–24), space-separated
        /// </summary>
        [JsonPropertyName("bypassKeys")]
        public List<int> BypassKeys { get; set; } = new();

        /// <summary>
        /// DEVICES= list of device names, space-separated
        /// </summary>
        [JsonPropertyName("devices")]
        public List<string> Devices { get; set; } = new();

        /// <summary>
        /// HELPKEY= single integer 1–24
        /// </summary>
        [JsonPropertyName("helpKey")]
        public int? HelpKey { get; set; }

        /// <summary>
        /// HELPMAP= name of a user-defined help map
        /// </summary>
        [JsonPropertyName("helpMap")]
        public string? HelpMap { get; set; }

        /// <summary>
        /// The optional PRESENT tag
        /// </summary>
        [JsonPropertyName("present")]
        public PresentTag? Present { get; set; }

        /// <summary>
        /// The optional MAPEDITS tag
        /// </summary>
        [JsonPropertyName("mapEdits")]
        public MapEditsTag? MapEdits { get; set; }

        /// <summary>
        /// All CFIELD children
        /// </summary>
        [JsonPropertyName("cfields")]
        public List<CfieldTag> Cfields { get; set; } = new();

        /// <summary>
        /// All VFIELD children
        /// </summary>
        [JsonPropertyName("vfields")]
        public List<VfieldTag> Vfields { get; set; } = new();

        /// <summary>
        /// Optional starting cursor row (1‑based).  When this and
        /// <see cref="StartCursorCol"/> are provided, the runtime editor
        /// positions the cursor here when the map first appears.  If these
        /// properties are null, the first unprotected variable field will be
        /// chosen automatically.
        /// </summary>
        [JsonPropertyName("startCursorRow")]
        public int? StartCursorRow { get; set; }

        /// <summary>
        /// Optional starting cursor column (1‑based).  See
        /// <see cref="StartCursorRow"/>.
        /// </summary>
        [JsonPropertyName("startCursorCol")]
        public int? StartCursorCol { get; set; }

        public static MapTag Parse(TagNode node)
        {
            static int ParseInt(string? s) => int.TryParse(s, out var v) ? v : 0;

            var tag = new MapTag
            {
                GrpName = node.GetString("GRPNAME") ?? "",
                MapName = node.GetString("MAPNAME") ?? "",
                Date = node.GetString("DATE") ?? "",
                Time = node.GetString("TIME") ?? ""
            };

            if (node.Attributes.TryGetValue("BYPKEY", out var bypass))
            {
                foreach (var tok in bypass.SelectMany(x => x.Split(' ', StringSplitOptions.RemoveEmptyEntries)))
                    if (int.TryParse(tok, out var k))
                        tag.BypassKeys.Add(k);
            }

            if (node.Attributes.TryGetValue("DEVICES", out var devs))
                tag.Devices.AddRange(devs.SelectMany(x => x.Split(' ', StringSplitOptions.RemoveEmptyEntries)));

            if (node.Attributes.TryGetValue("HELPKEY", out var hk) && int.TryParse(hk.FirstOrDefault(), out var help))
                tag.HelpKey = help;

            if (node.Attributes.TryGetValue("HELPMAP", out var hm))
                tag.HelpMap = hm.FirstOrDefault();

            foreach (var child in node.Children)
            {
                switch (child.TagName.ToUpperInvariant())
                {
                    case "PRESENT": tag.Present = PresentTag.Parse(child); break;
                    case "MAPEDITS": tag.MapEdits = MapEditsTag.Parse(child); break;
                    case "CFIELD": tag.Cfields.Add(CfieldTag.Parse(child)); break;
                    case "VFIELD": tag.Vfields.Add(VfieldTag.Parse(child)); break;
                }
            }

            // If no explicit start cursor was provided on the map itself, infer
            // it from the first VFIELD that sets InitialCursor=YES.  If none
            // have the flag, default to the first unprotected VFIELD.  This
            // behaviour mirrors the default in VisualAge where the first
            // variable field becomes the cursor location.
            if (!tag.StartCursorRow.HasValue || !tag.StartCursorCol.HasValue)
            {
                VfieldTag? init = tag.Vfields.FirstOrDefault(v => v.InitialCursor);
                if (init == null)
                {
                    // choose first unprotected field, else first field
                    init = tag.Vfields.FirstOrDefault(v => !v.IsProtect) ?? tag.Vfields.FirstOrDefault();
                }
                if (init != null)
                {
                    tag.StartCursorRow ??= init.Row;
                    tag.StartCursorCol ??= init.Column;
                }
            }

            return tag;
        }      
    }
}
