// File: SqlTableTag.cs
using System.Linq;


namespace EsfParser.Tags
{
    /// <summary>
    /// Represents one :SQLTABLE child of a RECORD.
    /// </summary>
    public class SqlTableTag
    {
        public string? Label { get; }
        public string TableId { get; }
        public bool IsTableNameHostVar { get; }

        private SqlTableTag(string? label, string tableId, bool isHostVar)
        {
            Label = label;
            TableId = tableId;
            IsTableNameHostVar = isHostVar;
        }

        /// <summary>
        /// Parse a :SQLTABLE TagNode into a SqlTableTag.
        /// </summary>
        public static SqlTableTag Parse(TagNode node)
        {
            // LABEL is optional
            var label = node.GetString("LABEL");

            // TABLEID is required (we'll throw if missing)
            var tableId = node.Attributes.TryGetValue("TABLEID", out var ids)
                       ? ids.FirstOrDefault() ?? throw new FormatException("SQLTABLE missing TABLEID")
                       : throw new FormatException("SQLTABLE missing TABLEID");

            // TBLNHVAR defaults to N
            var tbln = node.Attributes.TryGetValue("TBLNHVAR", out var ts)
                        ? ts.FirstOrDefault()?.ToUpperInvariant()
                        : null;
            var isHostVar = tbln == "Y";

            return new SqlTableTag(label, tableId, isHostVar);
        }

        public override string ToString()
            => $"SQLTABLE TableId={TableId}"
             + (Label is not null ? $" Label={Label}" : "")
             + (IsTableNameHostVar ? " (host-var)" : "");
    }
}
