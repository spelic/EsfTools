using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using EsfCore.Tags;

namespace EsfParser.Parser
{
    public static class EsfParser
    {
        public static List<TagNode> Parse(string[] lines)
        {
            var result = new List<TagNode>();
            var stack = new Stack<TagNode>();

            var startTagRegex = new Regex(@"^:(?<name>\w+)(?<attrs>.*)?\.?$", RegexOptions.Compiled);
            var endTagRegex = new Regex(@"^:e(?<name>\w+)\.?$", RegexOptions.Compiled);
            var rowTagRegex = new Regex(@"^:ROW\.\s*(?<key>\d+)\s+(?<text>.+)$", RegexOptions.Compiled);

            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i].TrimEnd();
                if (string.IsNullOrWhiteSpace(line)) continue;

                if (line.StartsWith(":EZEE", StringComparison.OrdinalIgnoreCase))
                {
                    var node = new TagNode("EZEE", i + 1)
                    {
                        Content = line,
                        EndLine = i + 1
                    };
                    result.Add(node);
                    continue;
                }

                // Handle :ROW. lines (special case)
                var rowMatch = rowTagRegex.Match(line);
                if (rowMatch.Success)
                {
                    var node = new TagNode("ROW", i + 1)
                    {
                        Content = $"{rowMatch.Groups["key"].Value} {rowMatch.Groups["text"].Value}",
                        EndLine = i + 1
                    };

                    if (stack.Count > 0)
                        stack.Peek().Children.Add(node);
                    else
                        result.Add(node);
                    continue;
                }

                // END tag
                var endMatch = endTagRegex.Match(line);
                if (endMatch.Success)
                {
                    string endName = endMatch.Groups["name"].Value.ToUpperInvariant();
                    while (stack.Count > 0)
                    {
                        var popped = stack.Pop();
                        popped.EndLine = i + 1;
                        if (popped.TagName.Equals(endName, StringComparison.OrdinalIgnoreCase))
                            break;
                    }
                    continue;
                }

                // START tag
                var startMatch = startTagRegex.Match(line);
                if (startMatch.Success && line.StartsWith(":"))
                {
                    string name = startMatch.Groups["name"].Value.ToUpperInvariant();
                    string attrLine = startMatch.Groups["attrs"].Value;

                    var node = new TagNode(name, i + 1);
                    string fullAttr = attrLine;
                    bool isLineScoped = RequiresImplicitEndTag(name);

                    while (i + 1 < lines.Length && !lines[i].TrimEnd().EndsWith("."))
                    {
                        var peek = lines[i + 1].TrimStart();
                        if (peek.StartsWith(":")) break;
                        i++;
                        fullAttr += " " + lines[i].TrimEnd();
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

                    if (stack.Count > 0)
                        stack.Peek().Children.Add(node);
                    else
                        result.Add(node);

                    if (RequiresEndTag(name))
                        stack.Push(node);
                    else if (!isLineScoped)
                        node.EndLine = node.StartLine;

                    continue;
                }

                // Handle .content lines inside CFIELD, etc.
                if (line.StartsWith(".") && stack.Count > 0)
                {
                    stack.Peek().Content += line + Environment.NewLine;
                    continue;
                }

                if (stack.Count > 0)
                    stack.Peek().Content += line + Environment.NewLine;
            }

            return result;
        }

        private static bool RequiresEndTag(string tagName)
        {
            var blockTags = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "PROGRAM", "FUNC", "RECORD", "MAINFUN", "PROL", "SQL", "ITEM",
                "BEFORE", "AFTER", "QUAL", "RETURN", "SSA", "TRACBAG", "CFIELD",
                "MAP", "VFIELD", "RECDITEM", "CATTR", "VATTR", "MAPEDITS",
                "TBLE", "DEFITEM", "CONTITEM", "MAPG", "AREA"
            };
            return blockTags.Contains(tagName);
        }

        private static bool RequiresImplicitEndTag(string tagName)
        {
            return tagName switch
            {
                "GENOPTS" => true,
                "TARGSYS" => true,
                "SQLTABLE" => true,
                _ => false
            };
        }
    }
}