using System.Text.Json.Serialization;
using EsfCore.Esf;
using EsfCore.Tags;

namespace EsfCore.Tags
{
    public class TracbagTag : IEsfTagModel
    {
        [JsonIgnore] public string TagName => "TRACBAG";

        [JsonPropertyName("time")]
        public string Time { get; set; }

        public static TracbagTag Parse(TagNode node)
        {
            return new TracbagTag
            {
                Time = node.Attributes.TryGetValue("TIME", out var t) ? t[0] : null
            };
        }

        public override string ToString() => $"TracbagTag: Time={Time}";
    }
}