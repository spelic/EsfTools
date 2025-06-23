using System.Collections.Generic;
using System.Text.Json.Serialization;
using EsfCore.Esf;

namespace EsfCore.Tags
{
    public class CattrTag : IEsfTagModel
    {
        [JsonIgnore] public string TagName => "CATTR";
        public Dictionary<string, List<string>> Attributes { get; set; } = new();

        public static CattrTag Parse(TagNode node)
        {
            return new CattrTag { Attributes = node.Attributes };
        }

        public override string ToString() => $"CATTR: {Attributes.Count} attributes";
    }
}