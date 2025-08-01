using System.Text.Json.Serialization;

using EsfParser.Esf;

namespace EsfParser.Tags
{
    public class GenoptsTag : IEsfTagModel
    {
        [JsonIgnore] public string TagName => "GENOPTS";

        [JsonPropertyName("resident")] public string Resident { get; set; }
        [JsonPropertyName("typeuse")] public string TypeUse { get; set; }

        public static GenoptsTag Parse(TagNode node)
        {
            return new GenoptsTag
            {
                Resident = node.Attributes.TryGetValue("RESIDENT", out var r) ? r[0] : null,
                TypeUse = node.Attributes.TryGetValue("TYPEUSE", out var t) ? t[0] : null
            };
        }

        public override string ToString() => $"GenoptsTag: Resident={Resident}, TypeUse={TypeUse}";
    }
}