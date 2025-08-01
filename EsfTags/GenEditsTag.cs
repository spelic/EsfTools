// GenEditsTag.cs
using System.Linq;
using System.Text.Json.Serialization;
using EsfCore.Esf;

namespace EsfTags
{
    public class GenEditsTag : IEsfTagModel
    {
        [JsonIgnore] public string TagName => "GENEDITS";

        [JsonPropertyName("editFunc")] public string? EditFunc { get; set; }
        [JsonPropertyName("editTable")] public string? EditTable { get; set; }
        [JsonPropertyName("editType")] public string? EditType { get; set; }

        public static GenEditsTag Parse(TagNode node)
        {
            return new GenEditsTag
            {
                EditFunc = node.Attributes.GetValueOrDefault("EDITFUNC")?.FirstOrDefault(),
                EditTable = node.Attributes.GetValueOrDefault("EDITTBLE")?.FirstOrDefault(),
                EditType = node.Attributes.GetValueOrDefault("EDITTYPE")?.FirstOrDefault()
            };
        }

        public override string ToString() =>
            $"GENEDITS: FUNC={EditFunc}, TBLE={EditTable}, TYPE={EditType}";
    }
}
