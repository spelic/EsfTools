using System.Text.Json.Serialization;

using EsfParser.Esf;

namespace EsfParser.Tags
{
    public class SsaTag : IEsfTagModel
    {
        [JsonIgnore]
        public string TagName => "SSA";

        [JsonPropertyName("updfunc")]
        public string UpdateFunction { get; set; }

        public static SsaTag Parse(TagNode node)
        {
            return new SsaTag
            {
                UpdateFunction = node.Attributes.TryGetValue("UPDFUNC", out var values) && values.Count > 0
                    ? values[0]
                    : null
            };
        }

        public override string ToString() =>
            $"SsaTag: UpdateFunction={UpdateFunction}";
    }
}
