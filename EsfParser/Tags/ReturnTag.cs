using System.Text.Json.Serialization;

using EsfParser.Esf;

namespace EsfParser.Tags
{
    public class ReturnTag : IEsfTagModel
    {
        [JsonIgnore]
        public string TagName => "RETURN";

        [JsonPropertyName("desc")]
        public string Description { get; set; }

        public static ReturnTag Parse(TagNode node)
        {
            return new ReturnTag
            {
                Description = node.Attributes.TryGetValue("DESC", out var values) && values.Count > 0
                    ? values[0]
                    : null
            };
        }

        public override string ToString() =>
            $"ReturnTag: Desc={Description}";
    }
}
