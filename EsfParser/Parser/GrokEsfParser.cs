using EsfParser.Tags;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace EsfParser.Parser
{
    public static class GrokEsfParser
    {
        private static readonly Regex AttributeRegex = new Regex(@"(?<key>\w+)\s*=\s*(?:""(?<value>[^""]*)""|(?<value>\S+))", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private static string ParseTagName(string line)
        {
            int endIndex = line.IndexOf(' ', 1);
            if (endIndex < 0) endIndex = line.Length;
            if (line.EndsWith('.') && line.IndexOf(' ') < 0) endIndex--;
            return line.Substring(1, endIndex - 1).ToUpperInvariant();
        }

        public static List<TagNode> Parse(string[] lines)
        {
            var result = new List<TagNode>();
            var stack = new Stack<TagNode>();
            TagNode current = null;
            int curRow = 0;

            while (curRow < lines.Length)
            {
                string line = lines[curRow].Trim();
                if (string.IsNullOrWhiteSpace(line))
                {
                    curRow++;
                    continue;
                }

                if (!line.StartsWith(":", StringComparison.OrdinalIgnoreCase))
                {
                    if (current != null)
                    {
                        string contentLine = line;
                        if (contentLine.StartsWith('.')) contentLine = contentLine.Substring(1).TrimStart();
                        if (contentLine.EndsWith('X') && contentLine.Length >= 72) contentLine = contentLine[0..^1];
                        current.Content += (string.IsNullOrEmpty(current.Content) ? "" : Environment.NewLine) + contentLine;
                    }
                    curRow++;
                    continue;
                }

                string tagName = ParseTagName(line);

                // Special case for EZEE (self-closing start tag, not end)
                if (tagName == "EZEE")
                {
                    TagNode ezeeTag = new TagNode(tagName, curRow);
                    ezeeTag.EndLine = curRow;
                    // Parse content after '.' if present
                    int dotIndex = line.IndexOf('.');
                    if (dotIndex >= 0)
                    {
                        ezeeTag.Content = line.Substring(dotIndex + 1).Trim();
                    }
                    // Parse attributes if any
                    string attrPart2 = line.Substring(tagName.Length + 1, dotIndex > 0 ? dotIndex - tagName.Length - 1 : line.Length - tagName.Length - 1).Trim();
                    var matches2 = AttributeRegex.Matches(attrPart2);
                    foreach (Match match in matches2)
                    {
                        string key = match.Groups["key"].Value.Trim().ToUpperInvariant();
                        string value = match.Groups["value"].Value.Trim();
                        if (!ezeeTag.Attributes.ContainsKey(key))
                            ezeeTag.Attributes[key] = new List<string>();
                        ezeeTag.Attributes[key].Add(value);
                    }
                    result.Add(ezeeTag);
                    curRow++;
                    continue;
                }

                // End tag check (skip if it's EZEE or similar special)
                if (tagName.StartsWith("E") && tagName != "EZEE")
                {
                    string baseName = tagName.Substring(1);
                    if (current != null && current.TagName == baseName)
                    {
                        current.EndLine = curRow;
                        if (stack.Count > 0)
                            current = stack.Pop();
                    }
                    else
                    {
                        (current ?? new TagNode("ROOT", 0)).Errors.Add($"Mismatched end tag :{tagName} at line {curRow + 1}");
                    }
                    curRow++;
                    continue;
                }

                // Start tag
                TagNode newTag = new TagNode(tagName, curRow);

                // Parse attributes
                int attrEnd = line.IndexOf('.');
                string attrPart = (attrEnd >= 0 ? line.Substring(tagName.Length + 1, attrEnd - tagName.Length - 1) : line.Substring(tagName.Length + 1)).Trim();
                var matches = AttributeRegex.Matches(attrPart);
                foreach (Match match in matches)
                {
                    string key = match.Groups["key"].Value.Trim().ToUpperInvariant();
                    string value = match.Groups["value"].Value.Trim();
                    if (!newTag.Attributes.ContainsKey(key))
                        newTag.Attributes[key] = new List<string>();
                    newTag.Attributes[key].Add(value);
                }

                if (current != null)
                {
                    current.Children.Add(newTag);
                    stack.Push(current);
                    current = newTag;
                }
                else
                {
                    result.Add(newTag);
                    current = newTag;
                }

                // If self-closing (ends with '.' and no body expected)
                if (line.EndsWith('.') && tagName != "PROL" && tagName != "SQL") // Exclude multi-line content tags
                {
                    newTag.EndLine = curRow;
                    if (stack.Count > 0) current = stack.Pop();
                }

                curRow++;
            }

            // Close any open tags at EOF
            while (stack.Count > 0)
            {
                var unclosed = stack.Pop();
                unclosed.Errors.Add($"Unclosed tag {unclosed.TagName} at EOF");
                unclosed.EndLine = lines.Length - 1;
            }

            return result;
        }
    }
}