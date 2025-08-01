using System.Text.Json.Serialization;
using EsfCore.Esf;
using EsfCore.Tags;

namespace EsfTags
{
    public class QualTag : IEsfTagModel
    {
        [JsonIgnore]
        public string TagName => "QUAL";

        [JsonPropertyName("withhold")]
        public string Withhold { get; set; }

        public static QualTag Parse(TagNode node)
        {
            return new QualTag
            {
                Withhold = node.Attributes.TryGetValue("WITHHOLD", out var values) && values.Count > 0
                    ? values[0]
                    : null
            };
        }

        public override string ToString() =>
            $"QualTag: Withhold={Withhold}";
    }
}
