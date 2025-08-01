using System.Text.Json.Serialization;
using EsfCore.Esf;
using EsfCore.Tags;

namespace EsfCore.Tags
{
    public class ProlTag : IEsfTagModel
    {
        [JsonIgnore] public string TagName => "PROL";

        [JsonPropertyName("text")]
        public string Text { get; set; }

        public static ProlTag Parse(TagNode node)
        {
            return new ProlTag
            {
                Text = node.Content?.Trim()
            };
        }

        public override string ToString() => $"ProlTag: {Text}";
    }
}