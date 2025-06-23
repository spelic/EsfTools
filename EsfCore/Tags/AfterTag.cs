using System.Text.Json.Serialization;
using EsfCore.Esf;
using EsfCore.Tags;

namespace EsfCore.Tags
{
    public class AfterTag : IEsfTagModel
    {
        [JsonIgnore]
        public string TagName => "AFTER";

        [JsonPropertyName("model")]
        public string Model { get; set; }

        [JsonPropertyName("object")]
        public string Object { get; set; }

        public static AfterTag Parse(TagNode node)
        {
            var tag = new AfterTag();

            if (node.Attributes.TryGetValue("MODEL", out var modelList) && modelList.Count > 0)
                tag.Model = modelList[0];

            if (node.Attributes.TryGetValue("OBJECT", out var objectList) && objectList.Count > 0)
                tag.Object = objectList[0];

            return tag;
        }

        public override string ToString() =>
            $"AfterTag: Model={Model}, Object={Object}";
    }
}
