using System.Text.Json.Serialization;
using EsfCore.Esf;
using EsfCore.Tags;

namespace EsfCore.Tags
{
    public class CallparmTag : IEsfTagModel
    {
        [JsonIgnore] public string TagName => "CALLPARM";

        [JsonPropertyName("name")] public string Name { get; set; }
        [JsonPropertyName("type")] public string Type { get; set; }

        public static CallparmTag Parse(TagNode node)
        {
            return new CallparmTag
            {
                Name = node.Attributes.TryGetValue("NAME", out var nameList) ? nameList[0] : null,
                Type = node.Attributes.TryGetValue("TYPE", out var typeList) ? typeList[0] : null
            };
        }

        public override string ToString() => $"CallparmTag: Name={Name}, Type={Type}";
    }
}