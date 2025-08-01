using System.Text.Json.Serialization;
using EsfCore.Esf;
using EsfCore.Tags;

namespace EsfTags
{
    public class TabrecTag : IEsfTagModel
    {
        [JsonIgnore] public string TagName => "TABREC";

        [JsonPropertyName("name")] public string Name { get; set; }
        [JsonPropertyName("type")] public string Type { get; set; }

        public static TabrecTag Parse(TagNode node)
        {
            return new TabrecTag
            {
                Name = node.Attributes.TryGetValue("NAME", out var nameList) ? nameList[0] : null,
                Type = node.Attributes.TryGetValue("TYPE", out var typeList) ? typeList[0] : null
            };
        }

        public override string ToString() => $"TabrecTag: Name={Name}, Type={Type}";
    }
}