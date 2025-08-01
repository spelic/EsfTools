// NumEditsTag.cs
using System;
using System.Linq;
using System.Text.Json.Serialization;
using EsfParser.Esf;

namespace EsfParser.Tags
{
    public class NumEditsTag : IEsfTagModel
    {
        [JsonIgnore] public string TagName => "NUMEDITS";

        [JsonPropertyName("currency")] public bool? Currency { get; set; }
        [JsonPropertyName("currSymb")] public string? CurrSymb { get; set; }
        [JsonPropertyName("numSep")] public bool? NumSep { get; set; }
        [JsonPropertyName("range")] public string? Range { get; set; }
        [JsonPropertyName("sign")] public string? Sign { get; set; }
        [JsonPropertyName("zeroEdit")] public bool? ZeroEdit { get; set; }

        public static NumEditsTag Parse(TagNode node)
        {
            bool? parseFlag(string key)
            {
                var v = node.Attributes.GetValueOrDefault(key)?.FirstOrDefault()?.ToUpperInvariant();
                return v switch { "Y" => true, "N" => false, _ => (bool?)null };
            }

            return new NumEditsTag
            {
                Currency = parseFlag("CURRENCY"),
                CurrSymb = node.Attributes.GetValueOrDefault("CURRSYMB")?.FirstOrDefault()?.Trim('\''),
                NumSep = parseFlag("NUMSEP"),
                Range = node.Attributes.GetValueOrDefault("RANGE")?.FirstOrDefault(),
                Sign = node.Attributes.GetValueOrDefault("SIGN")?.FirstOrDefault(),
                ZeroEdit = parseFlag("ZEROEDIT")
            };
        }

        public override string ToString() =>
            $"NUMEDITS: CURRENCY={Currency}, CURRSYMB={CurrSymb}, NUMSEP={NumSep}, RANGE={Range}, SIGN={Sign}, ZEROEDIT={ZeroEdit}";
    }
}
