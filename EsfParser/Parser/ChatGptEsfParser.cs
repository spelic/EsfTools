using EsfParser.Tags;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace EsfParser.Parser
{
    /// <summary>Single-pass stack-based ESF parser (IBM VAGen “ESF Reference”).</summary>
    public static class ChatGptEsfParser
    {
        // helpers
        private static readonly HashSet<string> BodyOnlyTags =
            new(StringComparer.OrdinalIgnoreCase) { "PROL", "SQL", "BEFORE", "AFTER" };

        private static bool IsEndTagFor(string tagName, string line)
        {
            var s = line.TrimStart();
            if (!s.StartsWith(":E", StringComparison.OrdinalIgnoreCase)) return false;

            int expected = 2 + tagName.Length; // ":E" + name
            if (s.Length < expected) return false;

            // match ":E" + name (exact name, ignoring case), optionally followed by whitespace or '.'
            if (!s.AsSpan(0, expected).Equals($":E{tagName}".AsSpan(), StringComparison.OrdinalIgnoreCase))
                return false;

            return s.Length == expected || char.IsWhiteSpace(s[expected]) || s[expected] == '.';
        }

        // ParseTagName (robust)
        private static string ParseTagName(string line)
        {
            var s = line.AsSpan().TrimStart();
            if (s.Length == 0 || s[0] != ':') return string.Empty;
            s = s[1..]; // skip ':'
            int i = 0;
            while (i < s.Length && !char.IsWhiteSpace(s[i])) i++;
            var name = s.Slice(0, i);
            if (name.Length > 0 && name[^1] == '.') name = name[..^1];
            return name.ToString().ToUpperInvariant();
        }

        // Find a '.' that isn't within single or double quotes
        private static int FindUnquotedDot(ReadOnlySpan<char> s)
        {
            bool inSingle = false, inDouble = false;
            for (int i = 0; i < s.Length; i++)
            {
                char c = s[i];
                if (c == '\'' && !inDouble) inSingle = !inSingle;
                else if (c == '"' && !inSingle) inDouble = !inDouble;
                else if (c == '.' && !inSingle && !inDouble) return i;
            }
            return -1;
        }

        private static bool EndsWithContinuationX(string line)
        {
            if (string.IsNullOrEmpty(line)) return false;

            // Find last non-space character
            int last = line.Length - 1;
            while (last >= 0 && char.IsWhiteSpace(line[last])) last--;

            if (last < 0) return false;
            if (line[last] != 'X') return false;

            // Traditional continuation is an 'X' sitting in col 72. Use >= 72 to stay tolerant.
            return (last + 1) >= 72;
        }

        private static int FindTagEnd(TagNode tagNode, string[] lines, int startRow)
        {
            // Default: if no explicit end, cap at the line before the next ':' (sibling tag)
            int firstColonAfterStart = -1;

            // Support nested tags with the same name.
            int depth = 0;

            for (int i = startRow + 1; i < lines.Length; i++)
            {
                var t = lines[i].TrimStart();
                if (!t.StartsWith(":", StringComparison.Ordinal))
                    continue;

                if (firstColonAfterStart < 0)
                    firstColonAfterStart = i;

                if (IsEndTagFor(tagNode.TagName, t))
                {
                    if (depth == 0)
                    {
                        tagNode.EndLine = i;
                        return tagNode.EndLine;
                    }
                    depth--;
                    continue;
                }

                // Another opening of the same tag name -> nested
                var nm = ParseTagName(t);
                if (!string.IsNullOrEmpty(nm) &&
                    nm.Equals(tagNode.TagName, StringComparison.OrdinalIgnoreCase))
                {
                    depth++;
                }
            }

            // No explicit end found; choose policy: end before the next ':' line, else EOF.
            tagNode.EndLine = (firstColonAfterStart >= 0) ? (firstColonAfterStart - 1) : (lines.Length - 1);
            return tagNode.EndLine;
        }


        public static List<TagNode> Parse(string[] lines)
        {
            var result = new List<TagNode>();
            for (int curRow = 0; curRow < lines.Length; curRow++)
            {
                string line = lines[curRow].Trim();
                if (string.IsNullOrWhiteSpace(line)) continue;
                if (!line.StartsWith(":", StringComparison.OrdinalIgnoreCase)) continue;

                // Skip end tags at top level
                if (line.StartsWith(":E", StringComparison.OrdinalIgnoreCase)) continue;

                TagNode newTag = new TagNode(ParseTagName(line), curRow);

                // Special case: EZEE keeps rest of header as content
                if (newTag.TagName.Equals("EZEE", StringComparison.OrdinalIgnoreCase))
                {
                    newTag.Content = line.Substring(1 + newTag.TagName.Length).Trim();
                    newTag.EndLine = curRow;
                    result.Add(newTag);
                    continue;
                }

                FindTagEnd(newTag, lines, curRow);
                newTag = ParseTag(newTag, lines);
                result.Add(newTag);
                curRow = newTag.EndLine;
            }
            return result;
        }

        private static void ParseAttributesInto(TagNode tagNode, ReadOnlySpan<char> span)
        {
            int i = 0;
            while (i < span.Length)
            {
                // skip spaces
                while (i < span.Length && char.IsWhiteSpace(span[i])) i++;

                // read key
                int keyStart = i;
                while (i < span.Length && !char.IsWhiteSpace(span[i]) && span[i] != '=') i++;
                if (i == keyStart) break; // nothing more
                var key = span.Slice(keyStart, i - keyStart).ToString().ToUpperInvariant();

                // skip spaces
                while (i < span.Length && char.IsWhiteSpace(span[i])) i++;
                if (i >= span.Length || span[i] != '=') break; // malformed; bail
                i++; // skip '='

                // skip spaces
                while (i < span.Length && char.IsWhiteSpace(span[i])) i++;

                // read value (quoted or unquoted)
                string value;
                if (i < span.Length && (span[i] == '"' || span[i] == '\''))
                {
                    char quote = span[i++];
                    int valStart = i;
                    while (i < span.Length && span[i] != quote) i++;
                    value = span.Slice(valStart, i - valStart).ToString();
                    if (i < span.Length && span[i] == quote) i++; // skip closing quote
                }
                else
                {
                    int valStart = i;
                    while (i < span.Length && !char.IsWhiteSpace(span[i])) i++;
                    value = span.Slice(valStart, i - valStart).ToString();
                }

                if (!tagNode.Attributes.TryGetValue(key, out var list))
                    tagNode.Attributes[key] = list = new List<string>();
                list.Add(value);
            }
        }


        private static TagNode ParseTag(TagNode tagNode, string[] lines)
        {
            for (int i = tagNode.StartLine; i <= tagNode.EndLine; i++)
            {
                string raw = lines[i];
                string line = raw.Trim();

                if (IsEndTagFor(tagNode.TagName, line))
                {
                    tagNode.EndLine = i;
                    return tagNode;
                }

                // Header line: remove ":" + name and parse remainder
                if (line.StartsWith(":" + tagNode.TagName, StringComparison.OrdinalIgnoreCase))
                {
                    line = line.Substring(tagNode.TagName.Length + 1).Trim();

                    // attributes + optional inline '.' + content
                    int dotAt = FindUnquotedDot(line.AsSpan());
                    if (dotAt >= 0)
                    {
                        var attrPart = line.Substring(0, dotAt).TrimEnd();
                        var inlineContent = line[(dotAt + 1)..].TrimStart();

                        if (!string.IsNullOrEmpty(attrPart))
                            ParseAttributesInto(tagNode, attrPart.AsSpan());

                        tagNode.Content = inlineContent;
                        return ParseBodyContent(tagNode, lines, i + 1);
                    }
                    else if (line.Contains('='))
                    {
                        ParseAttributesInto(tagNode, line.AsSpan());
                        // Continue to next line to look for body / children.
                        continue; // *** FIX: do not fall through and treat header as a child tag
                    }
                    else
                    {
                        // Header has no attributes and no inline content; continue to body.
                        continue; // *** FIX: same reason as above
                    }
                }

                // Child tag
                if (line.StartsWith(":", StringComparison.Ordinal))
                {
                    var child = new TagNode(ParseTagName(line), i);
                    FindTagEnd(child, lines, i);
                    child = ParseTag(child, lines);
                    tagNode.Children.Add(child);
                    i = child.EndLine;
                    continue;
                }

                // Content‑only tags or first body line
                if (i == tagNode.StartLine || BodyOnlyTags.Contains(tagNode.TagName))
                {
                    tagNode.Content = (line == ".") ? "" : raw.TrimEnd();
                    return ParseBodyContent(tagNode, lines, i + 1);
                }

                // Dot‑prefixed content block
                if (line.StartsWith(".", StringComparison.Ordinal))
                {
                    // Collect lines up to the next tag or end tag
                    var sb = new StringBuilder();
                    int j = i;
                    for (; j <= tagNode.EndLine; j++)
                    {
                        var currentRaw = lines[j];

                        // Un-dot and append
                        string contentLine = currentRaw.StartsWith(".") ? currentRaw[1..] : currentRaw;

                        if (EndsWithContinuationX(currentRaw))
                        {
                            // remove the trailing 'X' (last non-space) and do not append a newline
                            int last = contentLine.Length - 1;
                            while (last >= 0 && char.IsWhiteSpace(contentLine[last])) last--;
                            if (last >= 0 && contentLine[last] == 'X')
                                contentLine = contentLine[..last]; // drop 'X'
                            sb.Append(contentLine);
                        }
                        else
                        {
                            sb.AppendLine(contentLine);
                        }

                        // Check stop condition for the next iteration
                        if (j + 1 <= tagNode.EndLine)
                        {
                            var peek = lines[j + 1].TrimStart();
                            if (peek.StartsWith(":", StringComparison.Ordinal) || IsEndTagFor(tagNode.TagName, peek))
                            {
                                j++; // advance to the first non-content line
                                break;
                            }
                        }
                    }

                    tagNode.Content = sb.ToString();
                    i = j - 1; // -1 because for-loop will i++ next
                    continue;
                }

                // Any other non-empty line is part of the body
                // (preserve indentation but trim trailing whitespace)
                if (!string.IsNullOrWhiteSpace(raw))
                {
                    if (!string.IsNullOrEmpty(tagNode.Content))
                        tagNode.Content += Environment.NewLine;
                    tagNode.Content += raw.TrimEnd();
                    continue;
                }

                // If we get here with an unexpected format, throw.
                throw new InvalidOperationException(
                    $"Unexpected line format at line {i + 1}: '{line}'. Expected attributes, child, or content for '{tagNode.TagName}'.");
            }
            return tagNode;
        }

        // Parse the body lines until end tag or next tag (used after header/inline content)
        private static TagNode ParseBodyContent(TagNode tagNode, string[] lines, int fromExclusive)
        {
            var sb = new StringBuilder(tagNode.Content ?? string.Empty);

            for (int j = fromExclusive; j <= tagNode.EndLine; j++)
            {
                var raw = lines[j];
                var next = raw.TrimStart();

                if (IsEndTagFor(tagNode.TagName, next))
                {
                    tagNode.EndLine = j;
                    break;
                }

                // stop if we hit a new tag at this nesting level
                if (next.StartsWith(":", StringComparison.Ordinal))
                {
                    tagNode.EndLine = j - 1;
                    break;
                }

                if (sb.Length > 0)
                    sb.AppendLine();
                sb.Append(raw.TrimEnd());
            }

            tagNode.Content = sb.ToString();
            return tagNode;
        }

    }
}
