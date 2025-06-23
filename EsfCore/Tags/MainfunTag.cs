using System.Text.Json.Serialization;
using EsfCore.Esf;
using EsfCore.Tags;

namespace EsfCore.Tags
{
    public class MainfunTag : IEsfTagModel
    {
        [JsonIgnore] public string TagName => "MAINFUN";

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("flow")]
        public string FlowStatements { get; set; }

        public static MainfunTag Parse(TagNode node)
        {
            return new MainfunTag
            {
                Name = node.Attributes.TryGetValue("NAME", out var nameList) ? nameList[0] : null,
                FlowStatements = node.Content?.Trim()
            };
        }

        public override string ToString() => $"MainfunTag: Name={Name}";
    }
}