using System;
using System.Collections.Generic;
using System.Text;
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
            var rowRegex = new Regex(@"^:ROW\.\s*(?<key>\d+)\s+(?<text>.+)$", RegexOptions.Compiled);

            int i = start;
            while (i < end)
            {
                var line = lines[i].TrimEnd();
                if (string.IsNullOrWhiteSpace(line)) { i++; continue; }

                // ROW tag
                var rowMatch = rowRegex.Match(line);
                if (rowMatch.Success)
                {
                    var rowNode = new TagNode("ROW", i + 1)
                    {
                        Content = $"{rowMatch.Groups["key"].Value} {rowMatch.Groups["text"].Value}",
                        EndLine = i + 1
                    };
                    result.Add(rowNode);
                    i++;
                    continue;
                }

                // Start-tag?
                var startMatch = tagStartRegex.Match(line);
                if (startMatch.Success)
                {
                    string tagName = startMatch.Groups["name"].Value.ToUpperInvariant();
                    int blockStart = i;
                    int blockEnd = FindTagEnd(lines, blockStart + 1, tagName);
                    if (blockEnd <= blockStart) blockEnd = blockStart + 1;

                    // Zberi VEÈ-vrstiène atribute, prekinemo pri vrstici, ki zaène z ':' ali '.'
                    string attrs = startMatch.Groups["attrs"].Value.Trim();
                    int attrEndLine = blockStart + 1;
                    if (!string.IsNullOrEmpty(attrs) || !line.EndsWith("."))
                    {
                        var attrBuilder = new StringBuilder(attrs);
                        int j = blockStart + 1;
                        while (j < blockEnd)
                        {
                            var trimmed = lines[j].TrimStart();
                            if (trimmed.StartsWith(":") || trimmed.StartsWith("."))
                                break;
                            attrBuilder.Append(' ').Append(lines[j].TrimEnd());
                            j++;
                        }
                        attrs = attrBuilder.ToString();
                        attrEndLine = j;
                    }

                    // Ustvari vozlišèe
                    var node = new TagNode(tagName, blockStart + 1)
                    {
                        EndLine = blockEnd
                    };

                    // Parsaj atribute
                    var attrRegex = new Regex(
                        @"(?<key>\w+)\s*=\s*(?:(?<sq>'[^']*')|(?<dq>""[^""]*"")|(?<raw>\S+))",
                        RegexOptions.Compiled);
                    foreach (Match m in attrRegex.Matches(attrs))
                    {
                        var key = m.Groups["key"].Value.ToUpperInvariant();
                        var val = m.Groups["sq"].Success
                            ? m.Groups["sq"].Value.Trim('\'')
                            : m.Groups["dq"].Success
                                ? m.Groups["dq"].Value.Trim('"')
                                : m.Groups["raw"].Value;
                        if (!node.Attributes.ContainsKey(key))
                            node.Attributes[key] = new List<string>();
                        node.Attributes[key].Add(val);
                    }

                    // Zberi vsebino (vrstice, ki zaènejo s '.' ali katerakoli vrsta "gole" vsebine)
                    var contentLines = new List<string>();
                    for (int k = attrEndLine; k < blockEnd; k++)
                    {
                        var l = lines[k];
                        var t = l.TrimStart();
                        if (t.StartsWith(":")) break;   // podtag
                        // vsebino sprejmemo tudi, èe se zaène s piko
                        contentLines.Add(l.TrimEnd());
                    }
                    node.Content = string.Join(Environment.NewLine, contentLines).Trim();

                    // Rekurzivno parsaj otroke
                    if (blockEnd - attrEndLine > 1)
                    {
                        var children = new List<TagNode>();
                        ParseBlock(lines, attrEndLine, blockEnd, children);
                        node.Children.AddRange(children);
                    }

                    result.Add(node);
                    i = blockEnd;
                    continue;
                }

                // Èe ni tag, preskoèi
                i++;
            }
        }

        private static int FindTagEnd(string[] lines, int from, string tagName)
        {
            var endRegex = new Regex($":e{tagName}\\.", RegexOptions.IgnoreCase);
            for (int i = from; i < lines.Length; i++)
                if (endRegex.IsMatch(lines[i]))
                    return i + 1;
            for (int i = from; i < lines.Length; i++)
                if (lines[i].TrimStart().StartsWith(":"))
                    return i;
            return lines.Length;
        }
    }
}
