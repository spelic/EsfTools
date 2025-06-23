using System.Collections.Generic;
using System.Text.Json.Serialization;
using EsfCore.Esf;

namespace EsfCore.Tags
{
    public class FuncTag : IEsfTagModel
    {
        [JsonIgnore] public string TagName => "FUNC";

        [JsonPropertyName("name")] public string Name { get; set; }
        [JsonPropertyName("lines")] public List<EsfCodeLine> Lines { get; set; } = new();

        public static FuncTag Parse(TagNode node)
        {
            var tag = new FuncTag
            {
                Name = node.Attributes.TryGetValue("NAME", out var nameList) ? nameList[0] : null
            };

            var esfLines = (node.Content ?? "").Split('\n');
            var converter = new EsfCodeGen.EsfToCSharpConverter();

            foreach (var line in esfLines)
            {
                var trimmed = line.TrimEnd();
                if (string.IsNullOrWhiteSpace(trimmed)) continue;

                tag.Lines.Add(new EsfCodeLine
                {
                    Original = trimmed,
                    Translated = converter.ConvertSingleLine(trimmed)
                });
            }

            return tag;
        }

        public override string ToString() => $"FuncTag: {Name}, {Lines.Count} lines";
    }
}