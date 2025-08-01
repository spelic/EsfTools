using System.Text.Json.Serialization;
using EsfCore.Esf;
using EsfCore.Tags;

namespace EsfCore.Tags
{
    public class DlicallTag : IEsfTagModel
    {
        [JsonIgnore]
        public string TagName => "DLICALL";

        [JsonPropertyName("singrow")]
        public string SingRow { get; set; }

        public static DlicallTag Parse(TagNode node)
        {
            return new DlicallTag
            {
                SingRow = node.Attributes.TryGetValue("SINGROW", out var list) ? list[0] : null
            };
        }

        public override string ToString() =>
            $"DlicallTag: SingRow={SingRow}";
    }
}
