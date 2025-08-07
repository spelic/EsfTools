using System.Collections.Generic;
using System.Text.Json.Serialization;
using EsfParser.Esf;
using EsfParser.CodeGen;

namespace EsfParser.Tags
{
    public class RecordTagCollection : IEsfTagModel
    {
        [JsonIgnore]
        public string TagName => "RECORDS";

        public List<RecordTag> Records { get; set; } = new();

        public override string ToString()
        {
            if (Records.Count == 0)
                return $"{TagName}: No records defined";

            var details = new List<string>();
            foreach (var record in Records)
            {
                details.Add(record.ToString());
            }
            return $"{TagName}: {Records.Count} defined\n" + string.Join("\n", details);
        }

        /// <summary>
        /// Generates a C# code snippet representing this collection's <see cref="Records"/> list.
        /// Each <c>RecordTag</c> in the list is translated into a static field within
        /// a <c>GlobalRecords</c> class.  The type of each field is determined via
        /// <see cref="CSharpUtils.MapCsType"/>, the field name is cleaned via
        /// <see cref="CSharpUtils.CleanUnderscore"/>, and descriptions are emitted
        /// as XML summary comments when present.
        /// </summary>
        /// <returns>A string containing the generated C# code for the records.</returns>
        public string ToCSharp()
        {
            // When there are no records, return an empty GlobalRecords class definition
            if (Records == null || Records.Count == 0)
            {
                return "public static class GlobalRecords {}";
            }

            var sb = new System.Text.StringBuilder();
            sb.AppendLine("#region GLOBAL RECORDS");
            // Define the GlobalRecords container class
            sb.AppendLine(CSharpUtils.Indent(1) + "public static class GlobalRecords");
            sb.AppendLine(CSharpUtils.Indent(1) + "{");
            foreach (var record in Records)
            {
                // Each record becomes its own nested static class
                var recordClassName = CSharpUtils.CleanUnderscore(record.Name);
                sb.AppendLine(CSharpUtils.Indent(2) + $"public static class {recordClassName}");
                sb.AppendLine(CSharpUtils.Indent(2) + "{");
                // Emit fields for all items defined in the record
                foreach (var item in record.Items)
                {
                    var csType = CSharpUtils.MapCsType(item.Type.ToString(), item.Decimals);
                    var fieldName = CSharpUtils.CleanUnderscore(item.Name);
                    if (!string.IsNullOrWhiteSpace(item.Description))
                    {
                        sb.AppendLine(CSharpUtils.Indent(3) + "/// <summary>");
                        sb.AppendLine(CSharpUtils.Indent(3) + $"/// {item.Description}");
                        sb.AppendLine(CSharpUtils.Indent(3) + "/// </summary>");
                    }
                    if (fieldName == "_*")
                        sb.AppendLine(CSharpUtils.Indent(3) + $"// public static {csType} {fieldName}; // VIRTUAL FILLER");
                    else
                        sb.AppendLine(CSharpUtils.Indent(3) + $"public static {csType} {fieldName};");
                }

                // TODO: ADD SQLTABLE NAME AS Property

                sb.AppendLine(CSharpUtils.Indent(2) + "}");
                sb.AppendLine();
            }
            sb.AppendLine(CSharpUtils.Indent(1) + "}");
            sb.AppendLine("#endregion");
            return sb.ToString();
        }
    }
}