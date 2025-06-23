using System.Collections.Generic;
using System.Linq;
using System.Text;
using EsfCore.Esf;

namespace EsfCore.Tags
{
    public class MapTag : IEsfTagModel
    {
        public string GrpName { get; set; } = string.Empty;
        public string MapName { get; set; } = string.Empty;
        public string Date { get; set; } = string.Empty;
        public string Time { get; set; } = string.Empty;

        public List<CfieldTag> Cfields { get; set; } = new();
        public List<VfieldTag> Vfields { get; set; } = new();

        public string TagName => "MAP";

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine($":MAP {GrpName}/{MapName} ({Date} {Time})");
            sb.AppendLine("  Cfields:");
            foreach (var f in Cfields) sb.AppendLine($"    - {f}");
            sb.AppendLine("  Vfields:");
            foreach (var f in Vfields) sb.AppendLine($"    - {f}");
            return sb.ToString();
        }

        public static MapTag Parse(TagNode node)
        {
            var tag = new MapTag
            {
                GrpName = node.GetString("GRPNAME"),
                MapName = node.GetString("MAPNAME"),
                Date = node.GetString("DATE"),
                Time = node.GetString("TIME")
            };

            foreach (var child in node.Children)
            {
                switch (child.TagName.ToUpperInvariant())
                {
                    case "CFIELD":
                        tag.Cfields.Add(CfieldTag.Parse(child));
                        break;
                    case "VFIELD":
                        tag.Vfields.Add(VfieldTag.Parse(child));
                        break;
                }
            }

            return tag;
        }
    }
}