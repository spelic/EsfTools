using EsfParser.Tags;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;


namespace EsfParser.Parser.Logic
{
    public static class BlockParser
    {
        // tags whose bodies we treat as opaque text (no nested children)
        private static readonly HashSet<string> _noChildTags =
            new(StringComparer.OrdinalIgnoreCase)
            { "SQL", "BEFORE", "AFTER", "QUAL", "RETURN" };

        private static readonly Regex _tagStartRegex = new Regex(
            @"^:(?<name>\w+)(?<attrs>[^.]*)\.?$",
            RegexOptions.Compiled);

        private static readonly Regex _tagEndRegex = new Regex(
            @"^:e(?<name>\w+)\.?$",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private static readonly Regex _rowRegex = new Regex(
            @"^:ROW\.\s*(?<key>\d+)\s+(?<text>.+)$",
            RegexOptions.Compiled);

        private static readonly Regex _attrRegex = new Regex(
            @"(?<key>\w+)\s*=\s*(?:'(?<sq>[^']*)'|""(?<dq>[^""]*)""|(?<raw>\S+))",
            RegexOptions.Compiled);

        public static List<TagNode> Parse(string[] lines)
        {
            var result = new List<TagNode>();
            ParseBlock(lines, 0, lines.Length, result);
            return result;
        }

        private static void ParseBlock(string[] lines, int start, int end, List<TagNode> result)
        {
            for (int i = start; i < end;)
            {
                var line = lines[i].TrimEnd();
                if (string.IsNullOrWhiteSpace(line))
                {
                    i++;
                    continue;
                }

                // --- special‐case ROW
                var rowM = _rowRegex.Match(line);
                if (rowM.Success)
                {
                    result.Add(new TagNode("ROW", i + 1)
                    {
                        Content = $"{rowM.Groups["key"].Value} {rowM.Groups["text"].Value}",
                        EndLine = i + 1
                    });
                    i++;
                    continue;
                }

                // --- tag start?
                var m = _tagStartRegex.Match(line);
                if (!m.Success)
                {
                    // not a tag at all → append as content to last node
                    if (result.Count > 0)
                        result[^1].Content += line + Environment.NewLine;
                    i++;
                    continue;
                }

                string tagName = m.Groups["name"].Value.ToUpperInvariant();

                // 1) look for explicit :eTAG
                int blockEnd = -1;
                for (int k = i + 1; k < end; k++)
                {
                    var t = lines[k].TrimStart();
                    var em = _tagEndRegex.Match(t);
                    if (em.Success &&
                        string.Equals(em.Groups["name"].Value, tagName, StringComparison.OrdinalIgnoreCase))
                    {
                        blockEnd = k + 1;
                        break;
                    }
                }
                // 2) fallback: next line that starts with ':'
                if (blockEnd < 0)
                {
                    for (int k = i + 1; k < end; k++)
                    {
                        if (lines[k].TrimStart().StartsWith(":"))
                        {
                            blockEnd = k;
                            break;
                        }
                    }
                }
                if (blockEnd < 0) blockEnd = end;

                // --- build node
                var node = new TagNode(tagName, i + 1)
                {
                    EndLine = blockEnd
                };

                // --- attribute parsing: accumulate from the first line + any non‐colon lines up to blockEnd
                var rawAttrSb = new StringBuilder(m.Groups["attrs"].Value);
                for (int a = i + 1; a < blockEnd; a++)
                {
                    if (lines[a].TrimStart().StartsWith(":")) break;
                    rawAttrSb.Append(' ')
                             .Append(lines[a].Trim());
                }
                var rawAttrs = rawAttrSb.ToString();
                foreach (Match a in _attrRegex.Matches(rawAttrs))
                {
                    var key = a.Groups["key"].Value.ToUpperInvariant();
                    var val = a.Groups["sq"].Success ? a.Groups["sq"].Value
                            : a.Groups["dq"].Success ? a.Groups["dq"].Value
                            : a.Groups["raw"].Value;
                    if (!node.Attributes.ContainsKey(key))
                        node.Attributes[key] = new List<string>();
                    node.Attributes[key].Add(val);
                }

                // --- content: inline after the period on the start line...
                var sb = new StringBuilder();
                var dot = line.IndexOf('.');
                if (dot >= 0 && dot < line.Length - 1)
                {
                    var after = line.Substring(dot + 1).Trim();
                    if (after.Length > 0)
                        sb.AppendLine(after);
                }
                // ...and every line in between start+1 and blockEnd-1
                for (int k = i + 1; k < blockEnd - 1; k++)
                    sb.AppendLine(lines[k]);
                node.Content = sb.ToString().TrimEnd('\r', '\n');

                result.Add(node);

                // --- recurse unless it's in the no‐child set
                if (!_noChildTags.Contains(tagName))
                {
                    var children = new List<TagNode>();
                    ParseBlock(lines, i + 1, blockEnd - 1, children);
                    node.Children.AddRange(children);
                }

                // advance past the entire block (which consumes any :eTAG too)
                i = blockEnd;
            }
        }
    }
}
