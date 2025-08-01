// MessagesTag.cs
using System;
using System.Linq;
using System.Text.Json.Serialization;
using EsfParser.Esf;

namespace EsfParser.Tags
{
    public class MessagesTag : IEsfTagModel
    {
        [JsonIgnore] public string TagName => "MESSAGES";

        [JsonPropertyName("editMsg")] public int? EditMsg { get; set; }
        [JsonPropertyName("invalMsg")] public int? InvalMsg { get; set; }
        [JsonPropertyName("minInMsg")] public int? MinInMsg { get; set; }
        [JsonPropertyName("rangeMsg")] public int? RangeMsg { get; set; }
        [JsonPropertyName("reqMsg")] public int? ReqMsg { get; set; }

        public static MessagesTag Parse(TagNode node)
        {
            int? parseInt(string key)
            {
                if (node.Attributes.TryGetValue(key, out var vs) &&
                    int.TryParse(vs.FirstOrDefault(), out var v))
                    return v;
                return null;
            }

            return new MessagesTag
            {
                EditMsg = parseInt("EDITMSG"),
                InvalMsg = parseInt("INVALMSG"),
                MinInMsg = parseInt("MININMSG"),
                RangeMsg = parseInt("RANGEMSG"),
                ReqMsg = parseInt("REQMSG")
            };
        }

        public override string ToString() =>
            $"MESSAGES: EDIT={EditMsg}, INVAL={InvalMsg}, MININ={MinInMsg}, RANGE={RangeMsg}, REQ={ReqMsg}";
    }
}
