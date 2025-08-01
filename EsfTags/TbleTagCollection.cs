using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace EsfTags
{
    public class TbleTagCollection : Esf.IEsfTagModel
    {
        [JsonIgnore]
        public string TagName => "TABLES";

        public List<TbleTag> Tables { get; set; } = new();

        public override string ToString()
        {
            return $"{TagName}: {Tables.Count} defined";
        }
    }
}