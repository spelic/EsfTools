using System.Text.Json.Serialization;
using EsfCore.Esf;
using EsfCore.Tags;

namespace EsfCore.Tags
{
    public class BeforeTag : IEsfTagModel
    {
        [JsonIgnore]
        public string TagName => "BEFORE";

        [JsonPropertyName("content")]
        public string Content { get; set; }

        public static BeforeTag Parse(TagNode node)
        {
            return new BeforeTag
            {
                Content = node.Content?.TrimStart('.').Trim()
            };
        }

        public override string ToString() =>
            $"BeforeTag: Content={(string.IsNullOrWhiteSpace(Content) ? "(empty)" : Content)}";
    }
}
