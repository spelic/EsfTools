// UiMsgsTag.cs
using System.Linq;
using System.Text.Json.Serialization;
using EsfParser.Esf;

namespace EsfParser.Tags
{
    public class UiMsgsTag : IEsfTagModel
    {
        [JsonIgnore] public string TagName => "UIMSGS";

        [JsonPropertyName("funcKey")] public string? FuncKey { get; set; }
        [JsonPropertyName("minInKey")] public string? MinInKey { get; set; }
        [JsonPropertyName("rangeKey")] public string? RangeKey { get; set; }
        [JsonPropertyName("reqKey")] public string? ReqKey { get; set; }
        [JsonPropertyName("tbleKey")] public string? TbleKey { get; set; }
        [JsonPropertyName("typeKey")] public string? TypeKey { get; set; }

        public static UiMsgsTag Parse(TagNode node)
        {
            string strip(string? s) => s?.Trim('\'') ?? "";

            return new UiMsgsTag
            {
                FuncKey = strip(node.Attributes.GetValueOrDefault("FUNCKEY")?.FirstOrDefault()),
                MinInKey = strip(node.Attributes.GetValueOrDefault("MININKEY")?.FirstOrDefault()),
                RangeKey = strip(node.Attributes.GetValueOrDefault("RANGEKEY")?.FirstOrDefault()),
                ReqKey = strip(node.Attributes.GetValueOrDefault("REQKEY")?.FirstOrDefault()),
                TbleKey = strip(node.Attributes.GetValueOrDefault("TBLEKEY")?.FirstOrDefault()),
                TypeKey = strip(node.Attributes.GetValueOrDefault("TYPEKEY")?.FirstOrDefault())
            };
        }

        public override string ToString() =>
            $"UIMSGS: FUNC={FuncKey}, MININ={MinInKey}, RANGE={RangeKey}, REQ={ReqKey}, TBLE={TbleKey}, TYPE={TypeKey}";
    }
}
