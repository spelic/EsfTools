// RecordTagCollection.cs ────────────────────────────────────────────────
using EsfParser.CodeGen;
using EsfParser.Esf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;

namespace EsfParser.Tags
{
    /// <summary>
    /// Holds all <c>:RECORD</c> tags and converts them to two runtime
    /// containers:
    /// <list type="bullet">
    ///   <item><c>GlobalWorkstor</c> – WORKSTOR records</item>
    ///   <item><c>GlobalSqlRow</c>   – SQLROW   records</item>
    /// </list>
    /// Each record becomes a nested static class containing all its items.
    /// </summary>
    public class RecordTagCollection : IEsfTagModel
    {
        [JsonIgnore]
        public string TagName => "RECORDS";

        public List<RecordTag> Records { get; set; } = new();

        public override string ToString()
        {
            if (Records.Count == 0)
                return $"{TagName}: No records defined";

            var sb = new StringBuilder($"{TagName}: {Records.Count} defined\n");
            sb.Append(string.Join("\n", Records.Select(r => r.ToString())));
            return sb.ToString();
        }

        // ────────────────────────────────────────────────────────────────
        //  Code generation
        // ────────────────────────────────────────────────────────────────
        public string ToCSharp()
        {
            if (Records == null || Records.Count == 0)
                return "public static class GlobalWorkstor { }   // no RECORD tags";

            var sb = new StringBuilder();

            // local helper --------------------------------------------------
            void EmitContainer(string className, IEnumerable<RecordTag> recs)
            {
                string pad1 = CSharpUtils.Indent(1);
                string pad2 = CSharpUtils.Indent(2);
                string pad3 = CSharpUtils.Indent(3);
                string pad4 = CSharpUtils.Indent(4);

                sb.AppendLine($"{pad1}public static class {className}");
                sb.AppendLine($"{pad1}{{");

                foreach (var rec in recs)
                {
                    string recCls = CSharpUtils.CleanName(rec.Name);
                    sb.AppendLine($"{pad2}public static class {recCls}");
                    sb.AppendLine($"{pad2}{{");

                    // ── field declarations ──────────────────────────────
                    foreach (var itm in rec.Items)
                    {
                        if (itm.Name == "*")               // virtual filler
                        {
                            sb.AppendLine($"{pad3}// * virtual filler, ignored");
                            continue;
                        }

                        string csType = CSharpUtils.MapCsType(itm.Type.ToString(), itm.Decimals);
                        string fieldName = CSharpUtils.CleanName(itm.Name);
                        bool isArray = int.TryParse(itm.Occurs, out var occ) && occ > 1;

                        // XML doc
                        if (!string.IsNullOrWhiteSpace(itm.Description))
                        {
                            sb.AppendLine($"{pad3}/// <summary>");
                            sb.AppendLine($"{pad3}/// {itm.Description}");
                            sb.AppendLine($"{pad3}/// </summary>");
                        }

                        if (isArray)
                        {
                            sb.AppendLine($"{pad3}public static {csType}[] {fieldName} = new {csType}[{occ}];");
                        }
                        else
                        {
                            sb.AppendLine($"{pad3}public static {csType} {fieldName};");
                        }
                    }

                    // ── SetEmpty() helper ───────────────────────────────
                    sb.AppendLine();
                    sb.AppendLine($"{pad3}/// <summary>Reset all fields to default values.</summary>");
                    sb.AppendLine($"{pad3}public static void SetEmpty()");
                    sb.AppendLine($"{pad3}{{");

                    foreach (var itm in rec.Items)
                    {
                        if (itm.Name == "*") continue;   // skip fillers

                        string baseType = CSharpUtils.MapCsType(itm.Type.ToString(), itm.Decimals);
                        string fld = CSharpUtils.CleanName(itm.Name);
                        bool isArray = int.TryParse(itm.Occurs, out var occ) && occ > 1;

                        if (isArray)
                            sb.AppendLine($"{pad4}{fld} = new {baseType}[{itm.Occurs}];");
                        else
                            sb.AppendLine($"{pad4}{fld} = default({baseType});");
                    }
                    sb.AppendLine($"{pad3}}}");

                    // ── ToJson() helper ───────────────────────────────
                    sb.AppendLine();
                    sb.AppendLine($"{pad3}/// <summary>public static method to json</summary>");
                    sb.AppendLine($"{pad3}public static string ToJson()");
                    sb.AppendLine($"{pad3}{{");

                    sb.AppendLine($"{pad4}return System.Text.Json.JsonSerializer.Serialize(new");
                    sb.AppendLine($"{pad4}{{");
                    foreach (var itm in rec.Items)
                    {
                        if (itm.Name == "*") continue;   // skip fillers

                        string fld = CSharpUtils.CleanName(itm.Name);
                        sb.AppendLine($"{pad4}    {fld},");
                    }
                    sb.Remove(sb.Length - 3, 1); // remove last comma
                    sb.AppendLine($"{pad4}}});");



                    sb.AppendLine($"{pad3}}}");
                    sb.AppendLine($"{pad2}}}");
                    sb.AppendLine();
                }

                sb.AppendLine($"{pad1}}}");
                sb.AppendLine();
            }

            // split by organisation ---------------------------------------
            var workstorRecs = Records
                .Where(r => r.Org.Equals("WORKSTOR", StringComparison.OrdinalIgnoreCase));
            var sqlrowRecs = Records
                .Where(r => r.Org.Equals("SQLROW", StringComparison.OrdinalIgnoreCase));

            if (workstorRecs.Any())
                EmitContainer("GlobalWorkstor", workstorRecs);

            if (sqlrowRecs.Any())
                EmitContainer("GlobalSqlRow", sqlrowRecs);


            var r = sb.ToString();
            return r;
        }
    }
}
