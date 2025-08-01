using System;
using System.Collections.Generic;

namespace EsfParser.Tags
{
    public class TagNode
    {
        public string TagName { get; }
        public Dictionary<string, List<string>> Attributes { get; } = new(StringComparer.OrdinalIgnoreCase);
        public string Content { get; set; } = string.Empty;
        public List<TagNode> Children { get; } = new();
        public int StartLine { get; }
        public int EndLine { get; set; }

        public TagNode(string tagName, int startLine)
        {
            TagName = tagName;
            StartLine = startLine;
        }

        public override string ToString()
        {
            return ToString(0);
        }

        private string ToString(int indent)
        {
            var pad = new string(' ', indent * 2);
            var lines = new List<string>
                {
                    $"{pad}<{TagName}> (lines {StartLine}-{(EndLine > 0 ? EndLine : StartLine)})"
                };

            if (Attributes.Count > 0)
            {
                lines.Add($"{pad}  Attributes:");
                foreach (var kvp in Attributes)
                    lines.Add($"{pad}    {kvp.Key} = {string.Join(", ", kvp.Value)}");
            }

            if (!string.IsNullOrWhiteSpace(Content))
            {
                lines.Add($"{pad}  Content:");
                foreach (var line in Content.Trim().Split('\n'))
                    lines.Add($"{pad}    {line.Trim()}");
            }

            if (Children.Any())
            {
                lines.Add($"{pad}  Children:");
                foreach (var child in Children)
                    lines.Add(child.ToString(indent + 2));
            }

            return string.Join("\n", lines);
        }

    }
}