// SqlClause.cs
using System;
using System.Collections.Generic;
using System.Linq;
using EsfCore.Esf;
using System.Text.Json.Serialization;
using EsfCore.Tags;

namespace EsfCore.Tags
{
    public class SqlClause : IEsfTagModel
    {
        [JsonIgnore] public string TagName => "SQL";

        public string Clause { get; set; }
        public bool Refine { get; set; }
        public List<string> HostVars { get; set; } = new();
        public List<string> Lines { get; set; } = new();

        public override string ToString()
        {
            var hv = HostVars.Any() ? $" HOSTVAR=[{string.Join(", ", HostVars)}]" : "";
            return $"SQL CLAUSE={Clause}{hv}, {Lines.Count} lines";
        }

        public static SqlClause Parse(TagNode node)
        {
            var s = new SqlClause();

            // attributes
            if (node.Attributes.TryGetValue("CLAUSE", out var c))
                s.Clause = c.First();
            if (node.Attributes.TryGetValue("REFINE", out var r) && bool.TryParse(r.First(), out var rb))
                s.Refine = rb;
            if (node.Attributes.TryGetValue("HOSTVAR", out var hv))
                s.HostVars = hv.ToList();

            // raw lines
            if (!string.IsNullOrWhiteSpace(node.Content))
            {
                s.Lines = node.Content
                              .Split('\n', StringSplitOptions.RemoveEmptyEntries)
                              .Select(l => l.Trim())
                              .ToList();
            }

            return s;
        }
    }
}