using System.Collections.Generic;
using System.Text.Json.Serialization;
using EsfParser.Esf;

namespace EsfParser.Tags
{
    public class TbleTag : IEsfTagModel
    {
        [JsonIgnore] public string TagName => "TBLE";

        [JsonPropertyName("name")] public string Name { get; set; }
        [JsonPropertyName("date")] public string Date { get; set; }
        [JsonPropertyName("time")] public string Time { get; set; }
        [JsonPropertyName("tabtype")] public string TabType { get; set; }
        [JsonPropertyName("fold")] public string Fold { get; set; }
        [JsonPropertyName("usage")] public string Usage { get; set; }

        [JsonPropertyName("genopts")] public TagNode Genopts { get; set; }
        [JsonPropertyName("prols")] public List<TagNode> Prols { get; set; } = new();
        [JsonPropertyName("defitems")] public List<TagNode> DefItems { get; set; } = new();
        [JsonPropertyName("contitems")] public List<TagNode> ContItems { get; set; } = new();
        [JsonPropertyName("rows")] public List<RowEntry> Rows { get; set; } = new();

        public static TbleTag Parse(TagNode node)
        {
            var tag = new TbleTag();
            node.Attributes.TryGetValue("NAME", out var name); tag.Name = name?.Count > 0 ? name[0] : null;
            node.Attributes.TryGetValue("DATE", out var date); tag.Date = date?.Count > 0 ? date[0] : null;
            node.Attributes.TryGetValue("TIME", out var time); tag.Time = time?.Count > 0 ? time[0] : null;
            node.Attributes.TryGetValue("TABTYPE", out var type); tag.TabType = type?.Count > 0 ? type[0] : null;
            node.Attributes.TryGetValue("FOLD", out var fold); tag.Fold = fold?.Count > 0 ? fold[0] : null;
            node.Attributes.TryGetValue("USAGE", out var usage); tag.Usage = usage?.Count > 0 ? usage[0] : null;

            foreach (var child in node.Children)
            {
                switch (child.TagName.ToUpperInvariant())
                {
                    case "GENOPTS": tag.Genopts = child; break;
                    case "PROL": tag.Prols.Add(child); break;
                    case "DEFITEM": tag.DefItems.Add(child); break;
                    case "CONTITEM": tag.ContItems.Add(child); break;
                    case "ROW":
                        var parts = child.Content?.Trim().Split(' ', 2);
                        if (parts?.Length == 2)
                            tag.Rows.Add(new RowEntry { Key = parts[0], Text = parts[1].Trim() });
                        break;
                }
            }

            return tag;
        }

        public class RowEntry
        {
            public string Key { get; set; }
            public string Text { get; set; }
            public override string ToString() => $"{Key}: {Text}";
        }

        public override string ToString()
        {
            return $"TbleTag: {Name}, Rows={Rows.Count}, DefItems={DefItems.Count}, ContItems={ContItems.Count}";
        }
    }
}