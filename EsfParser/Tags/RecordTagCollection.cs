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
    /// Holds all <c>:RECORD</c> tags and converts them to runtime classes:
    /// - Instance models per record under namespace <c>EsfRuntime.Records</c> (implementing IEsfRecord&lt;TSelf&gt;).
    /// - Global containers <c>GlobalWorkstor</c> and <c>GlobalSqlRow</c> that expose a nested static class per record,
    ///   each with a singleton <c>Current</c> instance and field-level forwarders for backwards-compatible usage.
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

            // Pads
            string pad0 = CSharpUtils.Indent(0);
            string pad1 = CSharpUtils.Indent(1);
            string pad2 = CSharpUtils.Indent(2);
            string pad3 = CSharpUtils.Indent(3);
            string pad4 = CSharpUtils.Indent(4);
            string pad5 = CSharpUtils.Indent(5);

            // ── runtime support (once) ─────────────────────────────────
            void EmitRuntimeSupport()
            {
                sb.AppendLine("namespace EsfRuntime");
                sb.AppendLine("{");
                sb.AppendLine($"{pad1}public interface IEsfRecord<TSelf>");
                sb.AppendLine($"{pad1}{{");
                sb.AppendLine($"{pad2}void SetEmpty();");
                sb.AppendLine($"{pad2}void CopyFrom(in TSelf src);");
                sb.AppendLine($"{pad2}void CopyTo(ref TSelf dst);");
                sb.AppendLine($"{pad2}TSelf Clone();");
                sb.AppendLine($"{pad2}string ToJson();");
                sb.AppendLine($"{pad2}static abstract TSelf FromJson(string json);");
                sb.AppendLine($"{pad1}}}");
                sb.AppendLine();

                sb.AppendLine($"{pad1}internal static class RecordCopier");
                sb.AppendLine($"{pad1}{{");
                sb.AppendLine($"{pad2}public static void CopyByName(object? src, object? dst)");
                sb.AppendLine($"{pad2}{{");
                sb.AppendLine($"{pad3}if (src is null || dst is null) return;");
                sb.AppendLine($"{pad3}var sType = src.GetType();");
                sb.AppendLine($"{pad3}var dType = dst.GetType();");
                sb.AppendLine($"{pad3}var sProps = sType.GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);");
                sb.AppendLine($"{pad3}var dProps = dType.GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance)");
                sb.AppendLine($"{pad4}.ToDictionary(p => p.Name, StringComparer.OrdinalIgnoreCase);");
                sb.AppendLine($"{pad3}foreach (var sp in sProps)");
                sb.AppendLine($"{pad3}{{");
                sb.AppendLine($"{pad4}if (!sp.CanRead) continue;");
                sb.AppendLine($"{pad4}if (!dProps.TryGetValue(sp.Name, out var dp) || !dp.CanWrite) continue;");
                sb.AppendLine($"{pad4}var val = sp.GetValue(src);");
                sb.AppendLine($"{pad4}object? conv = ConvertValue(val, dp.PropertyType);");
                sb.AppendLine($"{pad4}dp.SetValue(dst, conv);");
                sb.AppendLine($"{pad3}}}");
                sb.AppendLine();

                sb.AppendLine($"{pad3}var sFields = sType.GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);");
                sb.AppendLine($"{pad3}var dFields = dType.GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance)");
                sb.AppendLine($"{pad4}.ToDictionary(f => f.Name, StringComparer.OrdinalIgnoreCase);");
                sb.AppendLine($"{pad3}foreach (var sf in sFields)");
                sb.AppendLine($"{pad3}{{");
                sb.AppendLine($"{pad4}if (!dFields.TryGetValue(sf.Name, out var df)) continue;");
                sb.AppendLine($"{pad4}var val = sf.GetValue(src);");
                sb.AppendLine($"{pad4}object? conv = ConvertValue(val, df.FieldType);");
                sb.AppendLine($"{pad4}df.SetValue(dst, conv);");
                sb.AppendLine($"{pad3}}}");
                sb.AppendLine($"{pad2}}}");
                sb.AppendLine();

                sb.AppendLine($"{pad2}private static object? ConvertValue(object? value, Type targetType)");
                sb.AppendLine($"{pad2}{{");
                sb.AppendLine($"{pad3}if (value == null)");
                sb.AppendLine($"{pad3}{{");
                sb.AppendLine($"{pad4}return targetType.IsValueType ? Activator.CreateInstance(targetType) : null;");
                sb.AppendLine($"{pad3}}}");
                sb.AppendLine($"{pad3}var srcType = value.GetType();");
                sb.AppendLine($"{pad3}if (targetType.IsAssignableFrom(srcType)) return value;");
                sb.AppendLine($"{pad3}try");
                sb.AppendLine($"{pad3}{{");
                sb.AppendLine($"{pad4}if (targetType == typeof(string)) return System.Convert.ToString(value, System.Globalization.CultureInfo.InvariantCulture);");
                sb.AppendLine($"{pad4}if (value is string s)");
                sb.AppendLine($"{pad4}{{");
                sb.AppendLine($"{pad5}if (targetType == typeof(int)) return int.TryParse(s, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var i) ? i : default(int);");
                sb.AppendLine($"{pad5}if (targetType == typeof(long)) return long.TryParse(s, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var l) ? l : default(long);");
                sb.AppendLine($"{pad5}if (targetType == typeof(decimal)) return decimal.TryParse(s, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var m) ? m : default(decimal);");
                sb.AppendLine($"{pad5}if (targetType == typeof(double)) return double.TryParse(s, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var d) ? d : default(double);");
                sb.AppendLine($"{pad5}if (targetType == typeof(float)) return float.TryParse(s, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var f) ? f : default(float);");
                sb.AppendLine($"{pad5}if (targetType == typeof(DateTime)) return DateTime.TryParse(s, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out var dt) ? dt : default(DateTime);");
                sb.AppendLine($"{pad4}}}");
                sb.AppendLine($"{pad4}if (value is IConvertible) return System.Convert.ChangeType(value, targetType, System.Globalization.CultureInfo.InvariantCulture);");
                sb.AppendLine($"{pad3}}} catch {{ /* default on failure */ }}");
                sb.AppendLine($"{pad3}return targetType.IsValueType ? Activator.CreateInstance(targetType) : null;");
                sb.AppendLine($"{pad2}}}");
                sb.AppendLine($"{pad1}}}");
                sb.AppendLine("}");
                sb.AppendLine();
            }

            // ── instance models (per record) ──────────────────────────
            void EmitInstanceModels(IEnumerable<RecordTag> recs)
            {
                sb.AppendLine("namespace EsfRuntime.Records");
                sb.AppendLine("{");

                foreach (var rec in recs)
                {
                    string recCls = CSharpUtils.CleanName(rec.Name);

                    sb.AppendLine($"{pad1}public sealed class {recCls} : EsfRuntime.IEsfRecord<{recCls}>");
                    sb.AppendLine($"{pad1}{{");

                    // fields/properties
                    foreach (var itm in rec.Items)
                    {
                        if (itm.Name == "*") { sb.AppendLine($"{pad2}// * virtual filler, ignored"); continue; }

                        string csType = CSharpUtils.MapCsType(itm.Type.ToString(), itm.Decimals);
                        string prop = CSharpUtils.CleanName(itm.Name);
                        bool isArray = int.TryParse(itm.Occurs, out var occ) && occ > 1;

                        if (!string.IsNullOrWhiteSpace(itm.Description))
                        {
                            sb.AppendLine($"{pad2}/// <summary>{itm.Description}</summary>");
                        }

                        if (isArray)
                            sb.AppendLine($"{pad2}public {csType}[] {prop} {{ get; set; }}");
                        else
                            sb.AppendLine($"{pad2}public {csType} {prop} {{ get; set; }}");
                    }

                    // ctor
                    sb.AppendLine();
                    sb.AppendLine($"{pad2}public {recCls}() {{ SetEmpty(); }}");
                    sb.AppendLine();

                    // SetEmpty
                    sb.AppendLine($"{pad2}public void SetEmpty()");
                    sb.AppendLine($"{pad2}{{");
                    foreach (var itm in rec.Items)
                    {
                        if (itm.Name == "*") continue;
                        string csType = CSharpUtils.MapCsType(itm.Type.ToString(), itm.Decimals);
                        string prop = CSharpUtils.CleanName(itm.Name);
                        bool isArray = int.TryParse(itm.Occurs, out var occ) && occ > 1;

                        if (isArray)
                            sb.AppendLine($"{pad3}{prop} = new {csType}[{itm.Occurs}];");
                        else
                            sb.AppendLine($"{pad3}{prop} = default({csType});");
                    }
                    sb.AppendLine($"{pad2}}}");
                    sb.AppendLine();

                    // CopyFrom / CopyTo
                    sb.AppendLine($"{pad2}public void CopyFrom(in {recCls} src)");
                    sb.AppendLine($"{pad2}{{");
                    foreach (var itm in rec.Items)
                    {
                        if (itm.Name == "*") continue;
                        string csType = CSharpUtils.MapCsType(itm.Type.ToString(), itm.Decimals);
                        string prop = CSharpUtils.CleanName(itm.Name);
                        bool isArray = int.TryParse(itm.Occurs, out var occ) && occ > 1;

                        if (isArray)
                            sb.AppendLine($"{pad3}{prop} = (src.{prop} != null) ? ({csType}[])src.{prop}.Clone() : null;");
                        else
                            sb.AppendLine($"{pad3}{prop} = src.{prop};");
                    }
                    sb.AppendLine($"{pad2}}}");
                    sb.AppendLine();

                    sb.AppendLine($"{pad2}public void CopyTo(ref {recCls} dst) => dst.CopyFrom(this);");
                    sb.AppendLine();

                    // Clone
                    sb.AppendLine($"{pad2}public {recCls} Clone()");
                    sb.AppendLine($"{pad2}{{");
                    sb.AppendLine($"{pad3}var x = new {recCls}();");
                    sb.AppendLine($"{pad3}x.CopyFrom(this);");
                    sb.AppendLine($"{pad3}return x;");
                    sb.AppendLine($"{pad2}}}");
                    sb.AppendLine();

                    // JSON
                    sb.AppendLine($"{pad2}public string ToJson() => System.Text.Json.JsonSerializer.Serialize(this);");
                    sb.AppendLine($"{pad2}public static {recCls} FromJson(string json) => System.Text.Json.JsonSerializer.Deserialize<{recCls}>(json) ?? new {recCls}();");

                    sb.AppendLine($"{pad1}}}");
                    sb.AppendLine();
                }

                sb.AppendLine("}");
                sb.AppendLine();
            }

            // ── global containers with forwarders ─────────────────────
            void EmitContainer(string className, IEnumerable<RecordTag> recs)
            {
                sb.AppendLine($"public static class {className}");
                sb.AppendLine("{");

                foreach (var rec in recs)
                {
                    string recCls = CSharpUtils.CleanName(rec.Name);
                    string fqRec = $"EsfRuntime.Records.{recCls}";

                    sb.AppendLine($"{pad1}public static class {recCls}");
                    sb.AppendLine($"{pad1}{{");
                    sb.AppendLine($"{pad2}public static {fqRec} Current {{ get; }} = new {fqRec}();");
                    sb.AppendLine();

                    // Forwarding properties (field-level)
                    foreach (var itm in rec.Items)
                    {
                        if (itm.Name == "*") continue;
                        string csType = CSharpUtils.MapCsType(itm.Type.ToString(), itm.Decimals);
                        string prop = CSharpUtils.CleanName(itm.Name);
                        bool isArray = int.TryParse(itm.Occurs, out var occ) && occ > 1;

                        if (!string.IsNullOrWhiteSpace(itm.Description))
                            sb.AppendLine($"{pad2}/// <summary>Forwards to Current.{prop} – {itm.Description}</summary>");
                        else
                            sb.AppendLine($"{pad2}/// <summary>Forwards to Current.{prop}</summary>");

                        if (isArray)
                        {
                            sb.AppendLine($"{pad2}public static {csType}[] {prop}");
                            sb.AppendLine($"{pad2}{{");
                            sb.AppendLine($"{pad3}get => Current.{prop};");
                            sb.AppendLine($"{pad3}set => Current.{prop} = value;");
                            sb.AppendLine($"{pad2}}}");
                        }
                        else
                        {
                            sb.AppendLine($"{pad2}public static {csType} {prop}");
                            sb.AppendLine($"{pad2}{{");
                            sb.AppendLine($"{pad3}get => Current.{prop};");
                            sb.AppendLine($"{pad3}set => Current.{prop} = value;");
                            sb.AppendLine($"{pad2}}}");
                        }
                        sb.AppendLine();
                    }

                    // Forwarding helpers
                    sb.AppendLine($"{pad2}public static void SetEmpty() => Current.SetEmpty();");
                    sb.AppendLine($"{pad2}public static void CopyFrom(in {fqRec} src) => Current.CopyFrom(src);");
                    sb.AppendLine($"{pad2}public static void CopyTo(ref {fqRec} dst) => Current.CopyTo(ref dst);");
                    sb.AppendLine($"{pad2}public static {fqRec} Clone() => Current.Clone();");
                    sb.AppendLine($"{pad2}public static string ToJson() => Current.ToJson();");
                    sb.AppendLine($"{pad2}public static void FromJson(string json)");
                    sb.AppendLine($"{pad2}{{");
                    sb.AppendLine($"{pad3}var tmp = {fqRec}.FromJson(json);");
                    sb.AppendLine($"{pad3}Current.CopyFrom(tmp);");
                    sb.AppendLine($"{pad2}}}");

                    sb.AppendLine($"{pad1}}}");
                    sb.AppendLine();
                }

                sb.AppendLine("}");
                sb.AppendLine();
            }

            // split by organisation
            var workstorRecs = Records.Where(r => r.Org.Equals("WORKSTOR", StringComparison.OrdinalIgnoreCase)).ToList();
            var sqlrowRecs = Records.Where(r => r.Org.Equals("SQLROW", StringComparison.OrdinalIgnoreCase)).ToList();
            var allRecs = Records;

            EmitRuntimeSupport();
            EmitInstanceModels(allRecs);
            if (workstorRecs.Any()) EmitContainer("GlobalWorkstor", workstorRecs);
            if (sqlrowRecs.Any()) EmitContainer("GlobalSqlRow", sqlrowRecs);

            return sb.ToString();
        }
    }
}
