using EsfParser.Esf;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace EsfParser.Tags
{
    public class ItemTagCollection : IEsfTagModel
    {
        [JsonIgnore]
        public string TagName => "ITEMS";

        public List<ItemTag> Items { get; set; } = new();

        public override string ToString()
        {
            // foreach itemtag detailed description output
            if (Items.Count == 0)
                return $"{TagName}: No items defined";
            var details = new List<string>();
            foreach (var item in Items)
            {
                details.Add(item.ToString());
            }
            return $"{TagName}: {Items.Count} defined\n" + string.Join("\n", details);
                      
        }
    }
}