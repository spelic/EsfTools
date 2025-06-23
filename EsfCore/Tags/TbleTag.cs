using System.Collections.Generic;
using System.Text.Json.Serialization;
using EsfCore.Esf;

namespace EsfCore.Tags
{
    public class TbleTag : IEsfTagModel
    {
        [JsonIgnore]
        public string TagName => "TBLE";

        [JsonPropertyName("name")] public string Name { get; set; }
        [JsonPropertyName("date")] public string Date { get; set; }
        [JsonPropertyName("time")] public string Time { get; set; }
        [JsonPropertyName("tabtype")] public string TabType { get; set; }
        [JsonPropertyName("usage")] public string Usage { get; set; }
        [JsonPropertyName("fold")] public string Fold { get; set; }

        [JsonPropertyName("genopts")] public TagNode Genopts { get; set; }
        [JsonPropertyName("defitems")] public List<TagNode> DefItems { get; set; } = new();
        [JsonPropertyName("contitems")] public List<TagNode> ContItems { get; set; } = new();
        [JsonPropertyName("rows")] public List<RowEntry> Rows { get; set; } = new();

        public static TbleTag Parse(TagNode node)
        {
            var tag = new TbleTag();

            string Get(string key) =>
                node.Attributes.TryGetValue(key, out var list) && list.Count > 0 ? list[0] : null;

            tag.Name = Get("NAME");
            tag.Date = Get("DATE");
            tag.Time = Get("TIME");
            tag.TabType = Get("TABTYPE");
            tag.Usage = Get("USAGE");
            tag.Fold = Get("FOLD");

            tag.Genopts = node.Children.Find(n => n.TagName == "GENOPTS");
            tag.DefItems = node.Children.FindAll(n => n.TagName == "DEFITEM");
            tag.ContItems = node.Children.FindAll(n => n.TagName == "CONTITEM");

            foreach (var child in node.Children)
            {
                if (child.TagName == "ROW")
                {
                    var parts = child.Content?.Trim().Split(' ', 2);
                    if (parts != null && parts.Length == 2)
                        tag.Rows.Add(new RowEntry { Key = parts[0], Text = parts[1].Trim() });
                }
            }

            return tag;
        }

        public override string ToString()
        {
            return $"TbleTag: {Name}, Rows={Rows.Count}";
        }

        public class RowEntry
        {
            public string Key { get; set; }
            public string Text { get; set; }

            public override string ToString() => $"{Key}: {Text}";
        }
    }
}