using System.Text.Json.Serialization;
using EsfCore.Esf;

namespace EsfCore.Tags
{
    public class EzeeTag : IEsfTagModel
    {
        [JsonIgnore] public string TagName => "EZEE";
        [JsonPropertyName("version")] public string Version { get; set; }
        [JsonPropertyName("enabledProducts")] public string EnabledProducts { get; set; }
        [JsonPropertyName("creationDate")] public string CreationDate { get; set; }
        [JsonPropertyName("creationTime")] public string CreationTime { get; set; }

        public static EzeeTag Parse(TagNode node)
        {
            string raw = node.Content?.PadRight(50);
            return new EzeeTag
            {
                Version = raw.Substring(6, 3).Trim(),
                EnabledProducts = raw.Substring(10, 12).Trim(),
                CreationDate = raw.Substring(23, 8).Trim(),
                CreationTime = raw.Substring(32, 8).Trim()
            };
        }
    }
}