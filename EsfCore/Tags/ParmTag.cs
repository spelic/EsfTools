using System.Text.Json.Serialization;
using EsfCore.Esf;
using EsfCore.Tags;

namespace EsfCore.Tags
{
    public class ParmTag : IEsfTagModel
    {
        [JsonIgnore]
        public string TagName => "PARM";

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        public static ParmTag Parse(TagNode node)
        {
            var tag = new ParmTag();

            if (node.Attributes.TryGetValue("NAME", out var nameList) && nameList.Count > 0)
                tag.Name = nameList[0];

            if (node.Attributes.TryGetValue("TYPE", out var typeList) && typeList.Count > 0)
                tag.Type = typeList[0];

            return tag;
        }

        public override string ToString() =>
            $"ParmTag: Name={Name}, Type={Type}";
    }
}
