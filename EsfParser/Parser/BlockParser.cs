using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using EsfCore.Tags;

namespace EsfParser.Parser
{
    public static class BlockParser
    {
        public static List<TagNode> Parse(string[] lines)
        {
            var result = new List<TagNode>();
            ParseBlock(lines, 0, lines.Length, result);
            return result;
        }

        public static void PrintTagTree(List<TagNode> nodes, int indent = 0)
        {
            foreach (var node in nodes)
            {
                string pad = new string(' ', indent * 2);
                Console.WriteLine($"{pad}- {node.TagName} (line {node.StartLine}–{node.EndLine})");

                if (node.Children?.Count > 0)
                    PrintTagTree(node.Children, indent + 1);
            }
        }
        private static void ParseBlock(string[] lines, int start, int end, List<TagNode> result)
        {
            var tagStartRegex = new Regex(@"^:(?<name>\w+)(?<attrs>[^.]*)\.?$", RegexOptions.Compiled);
            var tagEndRegex = new Regex(@"^:e(?<name>\w+)\.?$", RegexOptions.Compiled);
            var rowRegex = new Regex(@"^:ROW\.\s*(?<key>\d+)\s+(?<text>.+)$", RegexOptions.Compiled);

            for (int i = start; i < end;)
            {
                var line = lines[i].TrimEnd();

                if (string.IsNullOrWhiteSpace(line)) { i++; continue; }

                if (rowRegex.IsMatch(line))
                {
                    var rowMatch = rowRegex.Match(line);
                    var rowNode = new TagNode("ROW", i + 1)
                    {
                        Content = $"{rowMatch.Groups["key"].Value} {rowMatch.Groups["text"].Value}",
                        EndLine = i + 1
                    };
                    result.Add(rowNode);
                    i++;
                    continue;
                }

                var startMatch = tagStartRegex.Match(line);
                if (startMatch.Success)
                {
                    string tagName = startMatch.Groups["name"].Value.ToUpperInvariant();
                    int blockStart = i;
                    int blockEnd = FindTagEnd(lines, i + 1, tagName);

                    var node = new TagNode(tagName, blockStart + 1);
                    node.EndLine = blockEnd;

                    // parse attributes
                    string fullAttr = startMatch.Groups["attrs"].Value;
                    int j = i + 1;
                    while (j < blockEnd && !lines[i].TrimEnd().EndsWith(".") && !lines[j].StartsWith(":"))
                    {
                        fullAttr += " " + lines[j].TrimEnd();
                        j++;
                    }

                    var attrRegex = new Regex(@"(?<key>\w+)\s*=\s*(?:(?<sq>'[^']*')|(?<dq>""[^""]*"")|(?<raw>\S+))", RegexOptions.Compiled);
                    foreach (Match m in attrRegex.Matches(fullAttr))
                    {
                        var key = m.Groups["key"].Value.ToUpperInvariant();
                        var val = m.Groups["sq"].Success ? m.Groups["sq"].Value.Trim('\'') :
                                  m.Groups["dq"].Success ? m.Groups["dq"].Value.Trim('"') :
                                  m.Groups["raw"].Value;
                        if (!node.Attributes.ContainsKey(key))
                            node.Attributes[key] = new List<string>();
                        node.Attributes[key].Add(val);
                    }

                    // parse body and children recursively
                    var children = new List<TagNode>();
                    ParseBlock(lines, i + 1, blockEnd - 1, children);
                    node.Children.AddRange(children);

                    result.Add(node);
                    i = blockEnd;
                    continue;
                }

                // non-tag lines are content
                if (result.Count > 0)
                    result[^1].Content += line + Environment.NewLine;

                i++;
            }
        }

        private static int FindTagEnd(string[] lines, int from, string tagName)
        {
            var endRegex = new Regex($":e{tagName}\\.", RegexOptions.IgnoreCase);
            for (int i = from; i < lines.Length; i++)
            {
                if (endRegex.IsMatch(lines[i]))
                    return i + 1;
            }

            for (int i = from; i < lines.Length; i++)
            {
                if (lines[i].TrimStart().StartsWith(":")) return i;
            }

            return lines.Length;
        }
    }
}