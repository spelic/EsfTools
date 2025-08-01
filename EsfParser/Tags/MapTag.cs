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

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine($":MAP grp={GrpName} map={MapName}  date={Date}  time={Time}");
            if (Present != null) sb.AppendLine($"  PRESENT: {Present}");
            if (BypassKeys.Count > 0) sb.AppendLine($"  BYPKEY: {string.Join(',', BypassKeys)}");
            if (Devices.Count > 0) sb.AppendLine($"  DEVICES: {string.Join(',', Devices)}");
            if (HelpKey.HasValue) sb.AppendLine($"  HELPKEY: {HelpKey}");
            if (!string.IsNullOrEmpty(HelpMap)) sb.AppendLine($"  HELPMAP: {HelpMap}");
            if (MapEdits != null) sb.AppendLine($"  MAPEDITS: {MapEdits}");

            sb.AppendLine("  Cfields:");
            foreach (var cf in Cfields) sb.AppendLine($"    {cf}");

            sb.AppendLine("  Vfields:");
            foreach (var vf in Vfields) sb.AppendLine($"    {vf}");

            return sb.ToString();
        }

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

            return tag;
        }

        public string GenerateConsoleRenderer()
        {
            const int ConsoleWidth = 80;
            const int ConsoleHeight = 24;

            var sb = new StringBuilder();
            sb.AppendLine("    Console.Clear();");
            sb.AppendLine();
            sb.AppendLine("    void WriteWrapped(int col, int row, string text)");
            sb.AppendLine("    {");
            sb.AppendLine("        int c = col, r = row;");
            sb.AppendLine("        while (!string.IsNullOrEmpty(text) && r < " + ConsoleHeight + ")");
            sb.AppendLine("        {");
            sb.AppendLine("            int space = " + ConsoleWidth + " - c;");
            sb.AppendLine("            if (space <= 1) { c = 0; r++; continue; }");
            sb.AppendLine("            int take = text.Length <= space ? text.Length : space;");
            sb.AppendLine("            var part = text.Substring(0, take);");
            sb.AppendLine("            Console.SetCursorPosition(c, r);");
            sb.AppendLine("            Console.Write(part);");
            sb.AppendLine("            text = text.Substring(take);");
            sb.AppendLine("            c = 0; r++;");
            sb.AppendLine("        }");
            sb.AppendLine("    }");
            sb.AppendLine();

            // 1) Draw constant fields
            foreach (var cf in Cfields)
            {
                var txt = (cf.Value ?? "")
                    .Replace("\\", "\\\\")
                    .Replace("\"", "\\\"");
                sb.AppendLine($"    WriteWrapped({cf.Column - 1}, {cf.Row - 1}, \"{txt}\");");
            }

            sb.AppendLine();

            // 2) Draw variable fields (initially their Value or blanks)
            foreach (var vf in Vfields)
            {
                var txt = (vf.InitialValue ?? "")
                    .Replace("\\", "\\\\")
                    .Replace("\"", "\\\"");
                sb.AppendLine($"    WriteWrapped({vf.Column - 1}, {vf.Row - 1}, \"{txt}\");");
            }

            sb.AppendLine();

            // 3) Position cursor on first variable field (or top-left)
            if (Vfields.Count > 0)
            {
                var f = Vfields[0];
                sb.AppendLine($"    Console.SetCursorPosition({f.Column - 1}, {f.Row - 1});");
            }
            else
            {
                sb.AppendLine("    Console.SetCursorPosition(0, 0);");
            }

            return sb.ToString();
        }
    }
}
