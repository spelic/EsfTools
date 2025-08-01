using System.Text.Json.Serialization;

using EsfParser.Esf;

namespace EsfParser.Tags
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