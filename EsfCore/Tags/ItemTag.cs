using System.Collections.Generic;
using System.Text.Json.Serialization;
using EsfCore.Esf;

namespace EsfCore.Tags
{
    public class ItemTag : IEsfTagModel
    {
        [JsonIgnore]
        public string TagName => "ITEM";

        // Core attributes
        [JsonPropertyName("name")] public string Name { get; set; }
        [JsonPropertyName("date")] public string Date { get; set; }
        [JsonPropertyName("time")] public string Time { get; set; }
        [JsonPropertyName("type")] public string Type { get; set; }
        [JsonPropertyName("bytes")] public string Bytes { get; set; }
        [JsonPropertyName("decimals")] public string Decimals { get; set; }
        [JsonPropertyName("evensql")] public string EvenSql { get; set; }
        [JsonPropertyName("desc")] public string Description { get; set; }

        // Child: MAPEDITS
        [JsonPropertyName("mapedits")]
        public TagNode MapEdits { get; set; }

        public static ItemTag Parse(TagNode node)
        {
            var tag = new ItemTag();

            string Get(string key) =>
                node.Attributes.TryGetValue(key, out var list) && list.Count > 0 ? list[0] : null;

            tag.Name = Get("NAME");
            tag.Date = Get("DATE");
            tag.Time = Get("TIME");
            tag.Type = Get("TYPE");
            tag.Bytes = Get("BYTES");
            tag.Decimals = Get("DECIMALS");
            tag.EvenSql = Get("EVENSQL");
            tag.Description = Get("DESC");

            tag.MapEdits = node.Children
                .Find(c => c.TagName.Equals("MAPEDITS", System.StringComparison.OrdinalIgnoreCase));

            return tag;
        }

        public override string ToString() =>
            $"ItemTag: {Name}, Type={Type}, Bytes={Bytes}, Decimals={Decimals}, DESC={Description}";
    }
}
