using System.Text.Json.Serialization;
using EsfCore.Esf;
using EsfCore.Tags;

namespace EsfCore.Tags
{
    public class SqlTag : IEsfTagModel
    {
        [JsonIgnore]
        public string TagName => "SQL";

        [JsonPropertyName("option")]
        public string Option { get; set; }

        public static SqlTag Parse(TagNode node)
        {
            return new SqlTag
            {
                Option = node.Attributes.TryGetValue("OPTION", out var values) && values.Count > 0
                    ? values[0]
                    : null
            };
        }

        public override string ToString() =>
            $"SqlTag: Option={Option}";
    }
}
