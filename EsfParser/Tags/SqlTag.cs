// SqlTag.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

using EsfParser.Esf;

namespace EsfParser.Tags
{
    public enum SqlClauseType
    {
        SELECT,
        INTO,
        SET,
        WHERE,
        ORDERBY,
        SQLEXEC,
        FORUPDATEOF,
        VALUES,
        INSERTCOLNAME
    }

    public class SqlTag : IEsfTagModel
    {
        [JsonIgnore] public string TagName => "SQL";

        /// <summary>
        /// Which clause this is (SELECT, WHERE, ORDERBY, etc.)
        /// </summary>
        public SqlClauseType Clause { get; set; }

        /// <summary>
        /// The character used to prefix host-variables ('?' or '@').
        /// </summary>
        public char HostVar { get; set; } = '?';

        /// <summary>
        /// The raw lines of SQL text contained in this tag.
        /// </summary>
        public List<string> Lines { get; set; } = new();

        /// <summary>
        /// The full SQL for this clause (all lines joined by newline).
        /// </summary>
        public string Sql => string.Join(Environment.NewLine, Lines).Trim();

        public override string ToString()
        {
            return $"{Clause} (hostvar='{HostVar}'): {Lines.Count} line(s) : SQL: {Sql}";
        }

        public static SqlTag Parse(TagNode node)
        {
            var t = new SqlTag();

            // 1) parse CLAUSE=
            if (node.Attributes.TryGetValue("CLAUSE", out var cls) &&
                Enum.TryParse<SqlClauseType>(cls.First(), true, out var ct))
            {
                t.Clause = ct;
            }
            else
            {
                throw new InvalidOperationException(
                    $"SQL tag missing or invalid CLAUSE at line {node.StartLine}");
            }

            // 2) parse HOSTVAR= (optional, default '?')
            if (node.Attributes.TryGetValue("HOSTVAR", out var hv) && !string.IsNullOrWhiteSpace(hv.First()))
            {
                var c = hv.First().Trim('\'', '"');
                if (c.Length == 1)
                    t.HostVar = c[0];
            }

            // 3) split out all content lines
            if (!string.IsNullOrWhiteSpace(node.Content))
            {
                // each line in node.Content corresponds to one line of ESF SQL
                var all = node.Content
                              .Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                              .Select(l => l.TrimEnd());

                t.Lines.AddRange(all);
            }

            return t;
        }
    }
}
