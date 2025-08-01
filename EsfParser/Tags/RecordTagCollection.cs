using System.Collections.Generic;
using System.Text.Json.Serialization;
using EsfParser.Esf;

namespace EsfParser.Tags
{
    public class RecordTagCollection : IEsfTagModel
    {
        [JsonIgnore]
        public string TagName => "RECORDS";

        public List<RecordTag> Records { get; set; } = new();

        public override string ToString()
        {
     
            if (Records.Count == 0)
                return $"{TagName}: No records defined";

         
            var details = new List<string>();
            foreach (var record in Records)
            {
                details.Add(record.ToString());
            }
            return $"{TagName}: {Records.Count} defined\n" + string.Join("\n", details);
        }
    }
}