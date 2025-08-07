using System.Collections.Generic;
using System.Text.Json.Serialization;
using EsfParser.Esf;
using EsfParser.CodeGen;

namespace EsfParser.Tags
{
    /// <summary>
    /// Represents a collection of <c>TableTag</c> items within an ESF tag hierarchy.
    /// Provides utility methods for summarizing the collection and generating C# code.
    /// </summary>
    public class TableTagCollection : IEsfTagModel
    {
        [JsonIgnore]
        public string TagName => "TABLES";

        /// <summary>
        /// The list of table definitions contained within this collection.
        /// </summary>
        public List<TableTag> Tables { get; set; } = new();

        public override string ToString()
        {
            if (Tables.Count == 0)
                return $"{TagName}: No tables defined";
            var details = new List<string>();
            foreach (var table in Tables)
            {
                details.Add(table.ToString());
            }
            return $"{TagName}: {Tables.Count} defined\n" + string.Join("\n", details);
        }

        /// <summary>
        /// Generates a C# code snippet that defines a static class containing nested classes for each table.
        /// Each table is represented by a nested static class whose name is derived from the table's <see cref="TableTag.Name"/>.
        /// Within each table class, constants are generated for selected attributes (such as TabType, Fold, and Usage) when present,
        /// and a readonly dictionary of rows is emitted to represent the table data.
        /// </summary>
        /// <returns>The generated C# class definition.</returns>
        public string ToCSharp()
        {
            if (Tables == null || Tables.Count == 0)
            {
                return "public static class GlobalTables {}";
            }

            var sb = new System.Text.StringBuilder();
            sb.AppendLine("#region GLOBAL TABLES");
            sb.AppendLine(CSharpUtils.Indent(1) + "public static class GlobalTables");
            sb.AppendLine(CSharpUtils.Indent(1) + "{");
            foreach (var table in Tables)
            {
                // Derive a valid C# class name from the table name
                var tableClassName = CSharpUtils.CleanUnderscore(table.Name);
                sb.AppendLine(CSharpUtils.Indent(2) + $"public static class {tableClassName}");
                sb.AppendLine(CSharpUtils.Indent(2) + "{");

                // Emit constants for table-level attributes when they are defined
                if (!string.IsNullOrWhiteSpace(table.TabType))
                {
                    sb.AppendLine(CSharpUtils.Indent(3) + $"public const string TabType = \"{EscapeForString(table.TabType)}\";");
                }
                if (!string.IsNullOrWhiteSpace(table.Fold))
                {
                    sb.AppendLine(CSharpUtils.Indent(3) + $"public const string Fold = \"{EscapeForString(table.Fold)}\";");
                }
                if (!string.IsNullOrWhiteSpace(table.Usage))
                {
                    sb.AppendLine(CSharpUtils.Indent(3) + $"public const string Usage = \"{EscapeForString(table.Usage)}\";");
                }

                // Emit a dictionary for rows if any rows are present
                if (table.Rows != null && table.Rows.Count > 0)
                {
                    sb.AppendLine(CSharpUtils.Indent(3) + "public static readonly System.Collections.Generic.Dictionary<string, string> Rows = new()");
                    sb.AppendLine(CSharpUtils.Indent(3) + "{");
                    foreach (var row in table.Rows)
                    {
                        var rowKey = EscapeForString(row.Key);
                        var rowText = EscapeForString(row.Text);
                        sb.AppendLine(CSharpUtils.Indent(4) + $"{{ \"{rowKey}\", \"{rowText}\" }},");
                    }
                    sb.AppendLine(CSharpUtils.Indent(3) + "};");
                }

                sb.AppendLine(CSharpUtils.Indent(2) + "}");
                sb.AppendLine();
            }
            sb.AppendLine(CSharpUtils.Indent(1) + "}");
            sb.AppendLine("#endregion");
            return sb.ToString();
        }

        // Local helper to escape special characters for inclusion in C# string literals
        private static string EscapeForString(string input)
        {
            return input
                .Replace("\\", "\\\\")
                .Replace("\"", "\\\"");
        }
    }
}