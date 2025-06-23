using System.Collections.Generic;
using System.Text.Json.Serialization;
using EsfCore.Esf;

namespace EsfCore.Tags
{
    public class MapEditsTag : IEsfTagModel
    {
        [JsonIgnore] public string TagName => "MAPEDITS";
        public Dictionary<string, List<string>> Attributes { get; set; } = new();

        public static MapEditsTag Parse(TagNode node)
        {
            return new MapEditsTag { Attributes = node.Attributes };
        }

        public override string ToString() => $"MAPEDITS: {Attributes.Count} attributes";
    }
}