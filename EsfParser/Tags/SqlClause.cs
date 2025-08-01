// SqlClause.cs
using System;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
   // for TagNode

namespace EsfParser.Tags
{
    public class SqlClause
    {
        /// <summary>
        /// One of SELECT, INTO, SET, WHERE, ORDERBY, SQLEXEC, FORUPDATEOF, VALUES, INSERTCOLNAME
        /// </summary>
        [JsonPropertyName("clause")]
        public string ClauseType { get; set; }

        /// <summary>
        /// Either “?” or “@”
        /// </summary>
        [JsonPropertyName("hostVar")]
        public string HostVar { get; set; }

        /// <summary>
        /// The full text of the clause (everything after the dot on the :SQL line,
        /// plus any following lines, up to—but not including—the next tag like :ESQL).
        /// </summary>
        [JsonPropertyName("text")]
        public string Text { get; set; }

        public override string ToString()
            => $"{ClauseType} (hostVar={HostVar}): {Text.Replace("\n", "\\n")}";

        public static SqlClause Parse(TagNode node)
        {
            // 1) required CLAUSE=
            if (!node.Attributes.TryGetValue("CLAUSE", out var cl) || cl.Count == 0)
                throw new InvalidOperationException(
                    $":SQL missing CLAUSE at line {node.StartLine}");
            var clauseType = cl[0].ToUpperInvariant();

            // 2) optional HOSTVAR= (default '?')
            var hostVar = node.Attributes.TryGetValue("HOSTVAR", out var hvList) && hvList.Count > 0
               ? hvList[0].Trim('\'', '"')
               : "?";

            // 3) split the captured content into lines
            var raw = node.Content ?? "";
            var lines = raw
                .Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(l => l.TrimEnd())
                .ToList();

            var sb = new StringBuilder();
            if (lines.Count > 0)
            {
                // handle the first line specially:
                //  - if it contains a dot, take everything after the first dot
                //  - otherwise take the entire line
                var first = lines[0];
                var idx = first.IndexOf('.');
                if (idx >= 0)
                {
                    // skip the dot itself
                    if (idx < first.Length - 1)
                        sb.AppendLine(first.Substring(idx + 1).Trim());
                    // else nothing after the dot => empty
                }
                else
                {
                    sb.AppendLine(first);
                }

                // then any subsequent lines until we hit another ":" start
                foreach (var line in lines.Skip(1))
                {
                    if (line.StartsWith(":", StringComparison.Ordinal))
                        break;
                    sb.AppendLine(line);
                }
            }

            return new SqlClause
            {
                ClauseType = clauseType,
                HostVar = hostVar,
                Text = sb.ToString().TrimEnd('\r', '\n')
            };
        }
    }

    /// <summary>
    /// Tiny helper to pull single attributes out of a TagNode.
    /// </summary>
    internal static class AttrHelpers
    {
        public static string GetSingle(this TagNode node, string key)
        {
            if (node.Attributes.TryGetValue(key, out var vals) && vals.Count > 0)
                return vals[0];
            return null;
        }
    }
}
