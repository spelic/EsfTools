using EsfParser.Tags;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EsfParser.Parser
{
    public static class MyEsfParser
    {

        // private function to parse tag name from line
        private static string ParseTagName(string line)
        {
            int endIndex = line.IndexOf(' ', 1);
            if (endIndex < 0) endIndex = line.Length;
            if (line.EndsWith('.') && line.IndexOf(' ')<0) endIndex--;

            return line.Substring(1, endIndex - 1).ToUpperInvariant();
        }

        // private function to search for tag end
        private static int FindTagEnd(TagNode tagNode, string[] lines, int startRow)
        {
            tagNode.EndLine = -1;
            for (int nextRowIdx = startRow + 1; nextRowIdx < lines.Length; nextRowIdx++)
            {
                string nextLine = lines[nextRowIdx].Trim();
                if (nextLine.StartsWith(":", StringComparison.OrdinalIgnoreCase))
                {
                    if (tagNode.EndLine < 0)
                    {
                        tagNode.EndLine = nextRowIdx - 1; // found first line starting with ':'
                    }

                    string nextTagName = ParseTagName(nextLine);
                    if (nextTagName.StartsWith("E" + tagNode.TagName, StringComparison.OrdinalIgnoreCase))
                    {
                        // found end tag
                        tagNode.EndLine = nextRowIdx;
                        break;
                    }
                }
            }
            return tagNode.EndLine;
        }

        public static List<TagNode> Parse(string[] lines)
        {
            var result = new List<TagNode>();

            for (int curRow = 0; curRow < lines.Length; curRow++)
            {
                string line = lines[curRow].Trim();

                if (string.IsNullOrWhiteSpace(line)) continue;

                if (line.StartsWith(":", StringComparison.OrdinalIgnoreCase))
                {
                    TagNode newTag = new TagNode(ParseTagName(line), curRow);
                    line = line.Substring(newTag.TagName.Length +1).Trim();

                    if (newTag.TagName == "EZEE")
                    {
                        // special case for EZEE tag
                        newTag.Content = line;
                        newTag.EndLine = curRow;
                        result.Add(newTag);
                        continue;
                    }

                    // check for TAG end

                    FindTagEnd(newTag, lines, curRow);                   

                    newTag = ParseTag(newTag, lines);
                    result.Add(newTag);
                    curRow = newTag.EndLine;
                }

            }

            return result;
        }

        // private function to parse tag from start line to end line, parse attributes and content
        private static TagNode ParseTag(TagNode tagNode, string[] lines)
        {
            
            for (int i = tagNode.StartLine; i <= tagNode.EndLine; i++)
            {
                string line = lines[i].Trim();

                // if line contains  :ETAG stop
                if (line.StartsWith(":E" + tagNode.TagName, StringComparison.OrdinalIgnoreCase))
                {
                    // this is an end tag, stop parsing
                    tagNode.EndLine = i;
                    return tagNode;
                }

                // if line contains current start :TAG then remove :TAG and continue par possible attributes
                if (line.StartsWith(":" + tagNode.TagName, StringComparison.OrdinalIgnoreCase))
                {
                    if (line.EndsWith(".") || line.IndexOf('=')>0)
                    {
                        line = line.Substring(tagNode.TagName.Length+1).Trim();
                    } else
                        line = line.Substring(tagNode.TagName.Length).Trim();
                }

 

                // if line strts with : parse child
                if (line.StartsWith(":", StringComparison.OrdinalIgnoreCase))
                {
                    // this is a child tag, parse it

                    TagNode childNode = new TagNode(ParseTagName(line), i);
                    FindTagEnd(childNode, lines, i);
                    childNode = ParseTag(childNode, lines);
                    tagNode.Children.Add(childNode);
                    i = childNode.EndLine; // skip to end of child tag
                    continue;
                }


                // parse tag attributes as key value
                if (line.Contains('=') && !line.StartsWith('.'))
                {

                    bool isContentNext = false;
                    string afterContent = "";

                    if (line.EndsWith("."))
                    {
                        afterContent = line.Remove(0, line.IndexOf('.')+1);
                        line = line.Remove(line.Length - 1).Trim(); // remove trailing dot if exists
                        isContentNext = true;
                    }
                    // check how many equals signs are in the line and parse accordingly each key=value pair
                    if (line.Count(c => c == '=') > 1)
                    {
                        // multiple attributes in one line, split by spaces
                        var parts = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        if (parts.Length % 3 == 0)
                        {
                            for (int ii = 0; ii < parts.Length; ii=ii+3)
                            {
                                var key = parts[ii].Trim().ToUpperInvariant();
                                var value = parts[ii+2].Trim();
                                if (!tagNode.Attributes.ContainsKey(key))
                                    tagNode.Attributes[key] = new List<string>();
                                tagNode.Attributes[key].Add(value);
                            }
                        }
                        
                    }
                    else
                    // single attribute in line, split by first equals sign
                    {
                        var parts = line.Split(new[] { '=' }, 2);
                        var key = parts[0].Trim().ToUpperInvariant();
                        var value = parts.Length > 1 ? parts[1].Trim() : string.Empty;
                        if (!tagNode.Attributes.ContainsKey(key))
                            tagNode.Attributes[key] = new List<string>();
                        tagNode.Attributes[key].Add(value);
                        
                    }
                    if (!isContentNext) continue; // if not content next, continue to next line

                    // line in string after dot .
                    line = afterContent;
                }

                // if lineindex is tag start index and line ends with dot . that till end of tag there is content
                if (i == tagNode.StartLine
                    || (tagNode.TagName == "PROL.")
                    || (tagNode.TagName == "SQL")
                    || (tagNode.TagName == "BEFORE.")
                    || (tagNode.TagName == "AFTER.")
                    )
                {
                    // this is the content of the tag
                    if (line == ".")
                        line = "";
                    tagNode.Content = line;
                    
                    // add the rest of the content lines until next tag or end of tag
                    for (int j = i + 1; j <= tagNode.EndLine; j++)
                    {
                        string nextLine = lines[j].Trim();
                        if (//nextLine.StartsWith(":") ||
                            nextLine.StartsWith(":E" + tagNode.TagName, StringComparison.OrdinalIgnoreCase))
                        {
                            // found next tag or end tag, stop adding content
                            tagNode.EndLine = j;
                            break;
                        }
                        tagNode.Content += Environment.NewLine + nextLine;
                    }

                    return tagNode;
                }     

                if (line.StartsWith('.'))
                {
                    int endIndex = -1;
                    for (int j = i + 1; j <= tagNode.EndLine; j++)
                    {
                        if (lines[j-1].EndsWith('X') && lines[j - 1].Length>=72)
                        {
                            // this line is part of the content, continue
                            continue;
                        }

                        if (lines[j].StartsWith(":") || lines[j].StartsWith("E" + tagNode.TagName, StringComparison.OrdinalIgnoreCase))
                        {
                            // found next tag or end tag, stop adding content
                            endIndex = j;
                            break;
                        }
                    }

                    if (endIndex<0)
                    {
                        endIndex = tagNode.EndLine;
                    }

                    tagNode.Content = string.Join("", lines
                                            .Skip(i)
                                            .Take(endIndex - i)
                                            .Select(l => l.StartsWith(".") ? l[1..] : l)        // remove leading '.'
                                            .Select(l => l.EndsWith("X") ? l[..^1] : l + "\n")  // strip trailing 'X', else add newline
                                        );
                    i = endIndex;
                    continue;
                }

                throw new InvalidOperationException(
                    $"Unexpected line format at line {i + 1}: '{line}'. Expected attributes or content for tag '{tagNode.TagName}'.");
            }
            return tagNode;
        }

    }
}
