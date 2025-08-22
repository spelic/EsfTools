using EsfParser.CodeGen;
using EsfParser.Esf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;

namespace EsfParser.Tags
{
    public class MapTagCollection : IEsfTagModel
    {
        [JsonIgnore]
        public string TagName => "MAPS";

        public List<MapTag> Maps { get; set; } = new();

        public override string ToString()
        {
            if (Maps.Count == 0)
                return $"{TagName}: No maps defined";

            var sb = new StringBuilder($"{TagName}: {Maps.Count} defined\n");
            foreach (var map in Maps)
            {
                sb.AppendLine($"  - {map.GrpName}/{map.MapName} ({map.Date} {map.Time})");
                if (map.Cfields.Count > 0)
                {
                    sb.AppendLine("    Cfields:");
                    foreach (var cfield in map.Cfields)
                        sb.AppendLine($"      - {cfield}");
                }
                else
                {
                    sb.AppendLine("    Cfields: None");
                }

                if (map.Vfields.Count > 0)
                {
                    sb.AppendLine("    Vfields:");
                    foreach (var vfield in map.Vfields)
                        sb.AppendLine($"      - {vfield}");
                }
                else
                {
                    sb.AppendLine("    Vfields: None");
                }
                sb.AppendLine($"    Cfields: {map.Cfields.Count}, Vfields: {map.Vfields.Count}");
            }
            return sb.ToString();
        }

        /// <summary>
        /// Generates:
        ///  1) VfieldTag/CfieldTag runtime types (in this namespace).
        ///  2) Instance models per map under EsfRuntime.Maps (IEsfRecord-like helpers).
        ///  3) GlobalMaps static container with nested static classes per map:
        ///     - Current instance
        ///     - field-level forwarders (single and multiple)
        ///     - CopyFrom(object or Type), SetClear(), Render(), Cursor tracking
        /// </summary>
        public string ToCSharp()
        {
            if (Maps == null || Maps.Count == 0)
                return "public static class GlobalMaps { }   // no MAPS tags";

            var sb = new StringBuilder();
            sb.AppendLine("namespace EsfRuntime.Maps");
            sb.AppendLine("{");

            // 1) Runtime tag types
            WriteCfieldTag(sb);
            WriteVfieldTag(sb);

            // 2) Instance models
            EmitInstanceModels(sb, Maps);

            // 3) Global container with forwarders
            EmitGlobalContainer(sb, Maps);
            sb.AppendLine("}");

            return sb.ToString();
        }

        // ──────────────────────────────────────────────────────────────────
        // Runtime tag types
        // ──────────────────────────────────────────────────────────────────
        private void WriteCfieldTag(StringBuilder sb)
        {
            var p1 = CSharpUtils.Indent(1);
            sb.AppendLine($"{p1}/// <summary>");
            sb.AppendLine($"{p1}/// A constant (read-only) field on the screen.");
            sb.AppendLine($"{p1}/// </summary>");
            sb.AppendLine($"{p1}public class CfieldTag");
            sb.AppendLine($"{p1}{{");
            sb.AppendLine($"{p1}    public int Row {{ get; set; }}");
            sb.AppendLine($"{p1}    public int Column {{ get; set; }}");
            sb.AppendLine($"{p1}    public string Type {{ get; set; }} = \"\";");
            sb.AppendLine($"{p1}    public int Bytes {{ get; set; }}");
            sb.AppendLine($"{p1}    public string Value {{ get; set; }} = \"\";");
            // Foreground color of the constant field (default Gray). Use ConsoleColor enumeration.
            sb.AppendLine($"{p1}    public ConsoleColor Color {{ get; set; }} = ConsoleColor.Gray;");
            // Intensity of the constant field: NORMAL (default), BRIGHT or DARK
            sb.AppendLine($"{p1}    public string Intensity {{ get; set; }} = \"NORMAL\";");
            sb.AppendLine($"{p1}    public override string ToString() =>");
            sb.AppendLine($"{p1}        $\"CFIELD [{{Row}},{{Column}}] {{Type}}/{{Bytes}}B: '{{Value}}'\";");
            sb.AppendLine($"{p1}}}");
            sb.AppendLine();
        }

        private void WriteVfieldTag(StringBuilder sb)
        {
            var p1 = CSharpUtils.Indent(1);
            sb.AppendLine($"{p1}/// <summary>");
            sb.AppendLine($"{p1}/// A variable (read-write) field on the screen, with runtime MDT & intensity.");
            sb.AppendLine($"{p1}/// </summary>");
            sb.AppendLine($"{p1}public class VfieldTag");
            sb.AppendLine($"{p1}{{");
            sb.AppendLine($"{p1}    public event Action<VfieldTag>? OnCursor;");
            sb.AppendLine($"{p1}    public int Row {{ get; set; }}");
            sb.AppendLine($"{p1}    public int Column {{ get; set; }}");
            sb.AppendLine($"{p1}    public string Name {{ get; set; }} = \"\";");
            sb.AppendLine($"{p1}    public string Type {{ get; set; }} = \"\";");
            sb.AppendLine($"{p1}    public ConsoleColor Color {{ get; set; }} = ConsoleColor.White;");
            sb.AppendLine($"{p1}    public int Bytes {{ get; set; }}");
            // Default Value is a single space to match VisualAge default string initialization
            sb.AppendLine($"{p1}    public string Value {{ get; set; }} = \" \";");
            sb.AppendLine($"{p1}    public string RVideo {{ get; set; }} = \"\";");
            sb.AppendLine($"{p1}    public bool IsModified {{ get; private set; }}");
            sb.AppendLine($"{p1}    public bool IsProtect {{ get; private set; }}");
            sb.AppendLine($"{p1}    public bool IsBlink {{ get; private set; }}");
            // Intensity must be publicly settable so that initial values can be assigned
            sb.AppendLine($"{p1}    public string Intensity {{ get; set; }} = \"NORMAL\";");
            // Extended field attributes for runtime conversation logic
            sb.AppendLine(p1 + "    public bool Numeric { get; set; }");
            sb.AppendLine(p1 + "    public bool HexOnly { get; set; }");
            sb.AppendLine(p1 + "    public bool FoldToUpper { get; set; }");
            sb.AppendLine(p1 + "    public int Decimals { get; set; }");
            sb.AppendLine(p1 + "    public string Justify { get; set; } = \"LEFT\";");
            sb.AppendLine(p1 + "    public bool RightJustify => string.Equals(Justify, \"RIGHT\", StringComparison.OrdinalIgnoreCase);");
            sb.AppendLine($"{p1}    public void SetModified() => IsModified = true;");
            sb.AppendLine($"{p1}    public void SetProtect() => IsProtect = true;");
            sb.AppendLine($"{p1}    public void ClearModified() => IsModified = false;");
            sb.AppendLine($"{p1}    public void SetDark() => Intensity = \"DARK\";");
            sb.AppendLine($"{p1}    public void SetBlink() => IsBlink = true;");
            sb.AppendLine($"{p1}    public void SetBright() => Intensity = \"BRIGHT\";");
            sb.AppendLine($"{p1}    public void SetNormal() => Intensity = \"NORMAL\";");
            sb.AppendLine($"{p1}    public void SetRVideo() => RVideo = \"SET\";");
            sb.AppendLine($"{p1}    public void SetCursor() => OnCursor?.Invoke(this);");
            sb.AppendLine($"{p1}    public bool IsCursor() => Console.CursorTop == (Row - 1) && Console.CursorLeft == (Column - 1);");
            sb.AppendLine($"{p1}    public void Defined()  {{ /* later logic */ }}");
            sb.AppendLine($"{p1}    public override string ToString() =>");
            sb.AppendLine($"{p1}        $\"VFIELD [{{Row}},{{Column}}] {{Name}}/{{Type}}/{{Bytes}}B: '{{Value}}' (MDT={{IsModified}}, Intensity={{Intensity}})\";");
            sb.AppendLine($"{p1}}}");
            sb.AppendLine();
        }

        // ──────────────────────────────────────────────────────────────────
        // Instance models (IEsfRecord-style) under EsfRuntime.Maps
        // ──────────────────────────────────────────────────────────────────
        private void EmitInstanceModels(StringBuilder sb, IEnumerable<MapTag> maps)
        {
            var p1 = CSharpUtils.Indent(1);
            var p2 = CSharpUtils.Indent(2);
            var p3 = CSharpUtils.Indent(3);
            var p4 = CSharpUtils.Indent(4);


            foreach (var map in maps)
            {
                string cls = CSharpUtils.CleanName(map.MapName);
                sb.AppendLine($"{p1}public sealed class {cls} : EsfRuntime.IEsfRecord<{cls}>");
                sb.AppendLine($"{p1}{{");

                // Vfields (instance list)
                sb.AppendLine($"{p2}public System.Collections.Generic.List<VfieldTag> Vfields {{ get; }} = new()");
                sb.AppendLine($"{p2}{{");
                for (int i = 0; i < map.Vfields.Count; i++)
                {
                    var vf = map.Vfields[i];
                    // Use a single space as default initial value if none supplied
                    var rawVal = string.IsNullOrEmpty(vf.InitialValue) ? " " : vf.InitialValue;
                    var val = rawVal.Replace("\\", "\\\\").Replace("\"", "\\\"");
                    var justify = (vf.Justify ?? string.Empty).Replace("\\", "\\\\").Replace("\"", "\\\"");
                    var numeric = vf.Numeric.ToString().ToLowerInvariant();
                    var hexOnly = vf.HexOnly.ToString().ToLowerInvariant();
                    var foldUpper = vf.FoldToUpper.ToString().ToLowerInvariant();
                    sb.AppendLine($"{p3}new VfieldTag {{ Row = {vf.Row}, Column = {vf.Column}, Name = \"{vf.Name}\", Type = \"{vf.Type}\", Bytes = {vf.Bytes}, Value = \"{val}\", Numeric = {numeric}, HexOnly = {hexOnly}, FoldToUpper = {foldUpper}, Decimals = {vf.Decimals}, Justify = \"{justify}\", Color = ConsoleColor.{vf.Color}, Intensity = \"{vf.Intensity}\" }},");
                }
                if (map.Vfields.Count > 0)
                    sb.Remove(sb.Length - 3, 1); // trailing comma
                sb.AppendLine($"{p2}}};");
                sb.AppendLine();

                // Cfields (instance list)
                sb.AppendLine($"{p2}public System.Collections.Generic.List<CfieldTag> Cfields {{ get; }} = new()");
                sb.AppendLine($"{p2}{{");
                for (int i = 0; i < map.Cfields.Count; i++)
                {
                    var cf = map.Cfields[i];
                    var valCf = (cf.Value ?? string.Empty).Replace("\\", "\\\\").Replace("\"", "\\\"");
                    sb.AppendLine($"{p3}new CfieldTag {{ Row = {cf.Row}, Column = {cf.Column}, Type = \"{cf.Type}\", Bytes = {cf.Bytes}, Value = \"{valCf}\", Color = ConsoleColor.{cf.Color}, Intensity = \"{cf.Intensity}\" }},");
                }
                if (map.Cfields.Count > 0)
                    sb.Remove(sb.Length - 3, 1); // trailing comma
                sb.AppendLine($"{p2}}};");
                sb.AppendLine();

                // Group by name (case-insensitive)
                sb.AppendLine($"{p2}private readonly System.Collections.Generic.Dictionary<string, System.Collections.Generic.List<VfieldTag>> _byName;");
                sb.AppendLine();

                // Cursor tracking (instance)
                sb.AppendLine($"{p2}public int CursorRow {{ get; private set; }}");
                sb.AppendLine($"{p2}public int CursorColumn {{ get; private set; }}");
                sb.AppendLine();

                // ctor: init groups + on-cursor hook
                sb.AppendLine($"{p2}public {cls}()");
                sb.AppendLine($"{p2}{{");
                sb.AppendLine($"{p3}_byName = System.Linq.Enumerable.ToDictionary(");
                sb.AppendLine($"{p4}System.Linq.Enumerable.GroupBy(Vfields, v => v.Name, System.StringComparer.OrdinalIgnoreCase),");
                sb.AppendLine($"{p4}g => g.Key,");
                sb.AppendLine($"{p4}g => System.Linq.Enumerable.ToList(g),");
                sb.AppendLine($"{p4}System.StringComparer.OrdinalIgnoreCase);");
                sb.AppendLine();
                sb.AppendLine($"{p3}foreach (var tag in Vfields)");
                sb.AppendLine($"{p3}{{");
                sb.AppendLine($"{p4}tag.OnCursor += t =>");
                sb.AppendLine($"{p4}{{");
                sb.AppendLine($"{p4}    CursorRow = t.Row;");
                sb.AppendLine($"{p4}    CursorColumn = t.Column;");
                sb.AppendLine($"{p4}    System.Console.SetCursorPosition(CursorColumn - 1, CursorRow - 1);");
                sb.AppendLine($"{p4}}};");
                sb.AppendLine($"{p3}}}");
                sb.AppendLine($"{p2}}}");
                sb.AppendLine();

                // Helper: get tags by name
                sb.AppendLine($"{p2}public System.Collections.Generic.IReadOnlyList<VfieldTag> GetTagsByName(string name)");
                sb.AppendLine($"{p2}{{");
                sb.AppendLine($"{p3}return _byName.TryGetValue(name, out var list)");
                sb.AppendLine($"{p3}    ? (System.Collections.Generic.IReadOnlyList<VfieldTag>)list");
                sb.AppendLine($"{p3}    : System.Array.Empty<VfieldTag>();");
                sb.AppendLine($"{p2}}}");
                sb.AppendLine();

                // SetClear & SetEmpty
                sb.AppendLine($"{p2}public void SetClear()");
                sb.AppendLine($"{p2}{{");
                sb.AppendLine($"{p3}foreach (var tag in Vfields)");
                sb.AppendLine($"{p3}{{");
                // Reset value to a single space rather than empty string
                sb.AppendLine($"{p4}tag.Value = \" \";");
                sb.AppendLine($"{p4}tag.ClearModified();");
                sb.AppendLine($"{p4}tag.SetNormal();");
                sb.AppendLine($"{p3}}}");
                sb.AppendLine($"{p2}}}");
                sb.AppendLine();
                sb.AppendLine($"{p2}public void SetEmpty() => SetClear();");
                sb.AppendLine();

                // CopyFrom(instance)
                sb.AppendLine($"{p2}public void CopyFrom(in {cls} src)");
                sb.AppendLine($"{p2}{{");
                sb.AppendLine($"{p3}foreach (var kv in _byName)");
                sb.AppendLine($"{p3}{{");
                sb.AppendLine($"{p4}if (!src._byName.TryGetValue(kv.Key, out var sList)) continue;");
                sb.AppendLine($"{p4}var dList = kv.Value;");
                sb.AppendLine($"{p4}int n = System.Math.Min(dList.Count, sList.Count);");
                sb.AppendLine($"{p4}for (int i = 0; i < n; i++) dList[i].Value = sList[i].Value;");
                sb.AppendLine($"{p3}}}");
                sb.AppendLine($"{p2}}}");
                sb.AppendLine();

                // CopyTo
                sb.AppendLine($"{p2}public void CopyTo(ref {cls} dst) => dst.CopyFrom(this);");
                sb.AppendLine();

                // Clone
                sb.AppendLine($"{p2}public {cls} Clone()");
                sb.AppendLine($"{p2}{{");
                sb.AppendLine($"{p3}var x = new {cls}();");
                sb.AppendLine($"{p3}x.CopyFrom(this);");
                sb.AppendLine($"{p3}return x;");
                sb.AppendLine($"{p2}}}");
                sb.AppendLine();

                // ToJson / FromJson
                sb.AppendLine($"{p2}public string ToJson()");
                sb.AppendLine($"{p2}{{");
                sb.AppendLine($"{p3}var dict = new System.Collections.Generic.Dictionary<string, string[]>(System.StringComparer.OrdinalIgnoreCase);");
                sb.AppendLine($"{p3}foreach (var kv in _byName)");
                sb.AppendLine($"{p3}{{");
                sb.AppendLine($"{p4}var arr = new string[kv.Value.Count];");
                sb.AppendLine($"{p4}for (int i = 0; i < kv.Value.Count; i++) arr[i] = kv.Value[i].Value ?? string.Empty;");
                sb.AppendLine($"{p4}dict[kv.Key] = arr;");
                sb.AppendLine($"{p3}}}");
                sb.AppendLine($"{p3}return System.Text.Json.JsonSerializer.Serialize(dict);");
                sb.AppendLine($"{p2}}}");
                sb.AppendLine();

                sb.AppendLine($"{p2}public static {cls} FromJson(string json)");
                sb.AppendLine($"{p2}{{");
                sb.AppendLine($"{p3}var obj = System.Text.Json.JsonSerializer.Deserialize<System.Collections.Generic.Dictionary<string, string[]>>(json)");
                sb.AppendLine($"{p3}          ?? new System.Collections.Generic.Dictionary<string, string[]>(System.StringComparer.OrdinalIgnoreCase);");
                sb.AppendLine($"{p3}var x = new {cls}();");
                sb.AppendLine($"{p3}foreach (var kv in obj)");
                sb.AppendLine($"{p3}{{");
                sb.AppendLine($"{p4}var tags = x.GetTagsByName(kv.Key);");
                sb.AppendLine($"{p4}int n = System.Math.Min(tags.Count, kv.Value.Length);");
                sb.AppendLine($"{p4}for (int i = 0; i < n; i++) tags[i].Value = kv.Value[i] ?? string.Empty;");
                sb.AppendLine($"{p3}}}");
                sb.AppendLine($"{p3}return x;");
                sb.AppendLine($"{p2}}}");
                sb.AppendLine();

                // CopyFrom(object or Type) – robust (static/instance, arrays), canonical name match
                sb.AppendLine($"{p2}public void CopyFromObject(object recordOrType)");
                sb.AppendLine($"{p2}{{");
                sb.AppendLine($"{p3}if (recordOrType is null) return;");
                sb.AppendLine($"{p3}System.Type srcType;");
                sb.AppendLine($"{p3}bool isStatic;");
                sb.AppendLine($"{p3}if (recordOrType is System.Type t) {{ srcType = t; isStatic = true; }} else {{ srcType = recordOrType.GetType(); isStatic = false; }}");
                sb.AppendLine($"{p3}var flags = System.Reflection.BindingFlags.Public | (isStatic ? System.Reflection.BindingFlags.Static : System.Reflection.BindingFlags.Instance);");
                sb.AppendLine($"{p3}var members = new System.Collections.Generic.Dictionary<string, System.Reflection.MemberInfo>(System.StringComparer.OrdinalIgnoreCase);");
                sb.AppendLine($"{p3}foreach (var p in srcType.GetProperties(flags)) members[Canon(p.Name)] = p;");
                sb.AppendLine($"{p3}foreach (var f in srcType.GetFields(flags))     members[Canon(f.Name)] = f;");
                sb.AppendLine();
                sb.AppendLine($"{p3}foreach (var kv in _byName)");
                sb.AppendLine($"{p3}{{");
                sb.AppendLine($"{p4}var canonName = Canon(kv.Key);");
                sb.AppendLine($"{p4}if (!members.TryGetValue(canonName, out var m)) continue;");
                sb.AppendLine($"{p4}object? value = (m is System.Reflection.PropertyInfo pi)");
                sb.AppendLine($"{p4}    ? pi.GetValue(isStatic ? null : recordOrType)");
                sb.AppendLine($"{p4}    : ((System.Reflection.FieldInfo)m).GetValue(isStatic ? null : recordOrType);");
                sb.AppendLine($"{p4}AssignToTags(kv.Value, value);");
                sb.AppendLine($"{p3}}}");
                sb.AppendLine($"{p2}}}");
                sb.AppendLine();

                // helpers inside class
                sb.AppendLine($"{p2}private static void AssignToTags(System.Collections.Generic.List<VfieldTag> tags, object? value)");
                sb.AppendLine($"{p2}{{");
                sb.AppendLine($"{p3}if (value is System.Array arr && value is not string)");
                sb.AppendLine($"{p3}{{");
                sb.AppendLine($"{p4}int n = System.Math.Min(tags.Count, arr.Length);");
                sb.AppendLine($"{p4}for (int i = 0; i < n; i++) tags[i].Value = ToStringInv(arr.GetValue(i));");
                sb.AppendLine($"{p3}}}");
                sb.AppendLine($"{p3}else");
                sb.AppendLine($"{p3}{{");
                sb.AppendLine($"{p4}string s = ToStringInv(value);");
                sb.AppendLine($"{p4}for (int i = 0; i < tags.Count; i++) tags[i].Value = s;");
                sb.AppendLine($"{p3}}}");
                sb.AppendLine($"{p2}}}");
                sb.AppendLine();

                sb.AppendLine($"{p2}private static string Canon(string s)");
                sb.AppendLine($"{p2}{{");
                sb.AppendLine($"{p3}if (string.IsNullOrEmpty(s)) return string.Empty;");
                sb.AppendLine($"{p3}var sb = new System.Text.StringBuilder(s.Length);");
                sb.AppendLine($"{p3}foreach (var ch in s) if (char.IsLetterOrDigit(ch)) sb.Append(char.ToUpperInvariant(ch));");
                sb.AppendLine($"{p3}return sb.ToString();");
                sb.AppendLine($"{p2}}}");
                sb.AppendLine();

                sb.AppendLine($"{p2}private static string ToStringInv(object? v) => v switch");
                sb.AppendLine($"{p2}{{");
                sb.AppendLine($"{p3}null => string.Empty,");
                sb.AppendLine($"{p3}System.IFormattable f => f.ToString(null, System.Globalization.CultureInfo.InvariantCulture),");
                sb.AppendLine($"{p3}_ => v.ToString() ?? string.Empty");
                sb.AppendLine($"{p2}}};");
                sb.AppendLine();

                // Render (instance) – reuse existing console renderer
                sb.AppendLine($"{p2}public void Render()");
                sb.AppendLine($"{p2}{{");
                var lines = map.GenerateConsoleRenderer().Split(Environment.NewLine);
                foreach (var line in lines)
                    sb.AppendLine($"{p3}{line}");
                sb.AppendLine($"{p2}}}");

                sb.AppendLine($"{p1}}}");
                sb.AppendLine();
            }
        }

        // ──────────────────────────────────────────────────────────────────
        // GlobalMaps container with forwarders
        // ──────────────────────────────────────────────────────────────────
        private void EmitGlobalContainer(StringBuilder sb, IEnumerable<MapTag> maps)
        {
            var p0 = CSharpUtils.Indent(0);
            var p1 = CSharpUtils.Indent(1);
            var p2 = CSharpUtils.Indent(2);
            var p3 = CSharpUtils.Indent(3);

            sb.AppendLine($"{p0}public static class GlobalMaps");
            sb.AppendLine($"{p0}{{");

            foreach (var map in maps)
            {
                string cls = CSharpUtils.CleanName(map.MapName);
                string fq = $"EsfRuntime.Maps.{cls}";

                sb.AppendLine($"{p1}public static class {cls}");
                sb.AppendLine($"{p1}{{");
                sb.AppendLine($"{p2}public static {fq} Current {{ get; }} = new {fq}();");
                sb.AppendLine();

                // Cursor forwarding
                sb.AppendLine($"{p2}public static int CursorRow => Current.CursorRow;");
                sb.AppendLine($"{p2}public static int CursorColumn => Current.CursorColumn;");
                sb.AppendLine();

                // Forward constant fields
                sb.AppendLine($"{p2}public static System.Collections.Generic.List<CfieldTag> Cfields => Current.Cfields;");
                sb.AppendLine();

                // Accessors per VFIELD group
                var groups = map.Vfields
                                .GroupBy(v => v.Name, StringComparer.OrdinalIgnoreCase)
                                .ToDictionary(g => g.Key, g => g.ToList(), StringComparer.OrdinalIgnoreCase);

                foreach (var kv in groups)
                {
                    var rawName = kv.Key;
                    var list = kv.Value;
                    var safe = CSharpUtils.CleanName(rawName);

                    // decide csType (FIX: no null-conditional on enum)
                    var first = list[0];
                    string typeStr = first.Type.ToString();
                    int decimals = first.Decimals; // assuming Decimals property exists on map Vfield model
                    string csType = string.Equals(typeStr, "NUM", StringComparison.OrdinalIgnoreCase)
                        ? (decimals > 0 ? "decimal" : "int")
                        : "string";

                    // Single occurrence
                    if (list.Count == 1)
                    {
                        sb.AppendLine($"{p2}/// <summary>Variable field '{rawName}'</summary>");
                        sb.AppendLine($"{p2}public static VfieldTag {safe}Tag => Current.GetTagsByName(\"{rawName}\")[0];");
                        sb.AppendLine($"{p2}public static {csType} {safe}");
                        sb.AppendLine($"{p2}{{");
                        if (csType == "string")
                        {
                            sb.AppendLine($"{p3}get => {safe}Tag.Value;");
                            sb.AppendLine($"{p3}set => {safe}Tag.Value = value;");
                        }
                        else if (csType == "decimal")
                        {
                            sb.AppendLine($"{p3}get => decimal.TryParse({safe}Tag.Value, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var v) ? v : default;");
                            sb.AppendLine($"{p3}set => {safe}Tag.Value = value.ToString(System.Globalization.CultureInfo.InvariantCulture);");
                        }
                        else // int
                        {
                            sb.AppendLine($"{p3}get => int.TryParse({safe}Tag.Value, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var v) ? v : default;");
                            sb.AppendLine($"{p3}set => {safe}Tag.Value = value.ToString(System.Globalization.CultureInfo.InvariantCulture);");
                        }
                        sb.AppendLine($"{p2}}}");
                        sb.AppendLine();
                    }
                    else
                    {
                        // Multiple occurrences (arrays)
                        sb.AppendLine($"{p2}/// <summary>Variable field '{rawName}' (multiple)</summary>");
                        sb.AppendLine($"{p2}public static VfieldTag[] {safe}Tag");
                        sb.AppendLine($"{p2}{{");
                        sb.AppendLine($"{p3}get");
                        sb.AppendLine($"{p3}{{");
                        sb.AppendLine($"{p3}    var list2 = Current.GetTagsByName(\"{rawName}\");");
                        sb.AppendLine($"{p3}    return System.Linq.Enumerable.ToArray(list2);");
                        sb.AppendLine($"{p3}}}");
                        sb.AppendLine($"{p2}}}");
                        sb.AppendLine();

                        sb.AppendLine($"{p2}public static {csType}[] {safe}");
                        sb.AppendLine($"{p2}{{");
                        sb.AppendLine($"{p3}get");
                        sb.AppendLine($"{p3}{{");
                        sb.AppendLine($"{p3}    var list2 = Current.GetTagsByName(\"{rawName}\");");
                        sb.AppendLine($"{p3}    var arr = new {csType}[list2.Count];");
                        if (csType == "string")
                        {
                            sb.AppendLine($"{p3}    for (int i = 0; i < list2.Count; i++) arr[i] = list2[i].Value;");
                        }
                        else if (csType == "decimal")
                        {
                            sb.AppendLine($"{p3}    for (int i = 0; i < list2.Count; i++)");
                            sb.AppendLine($"{p3}        arr[i] = decimal.TryParse(list2[i].Value, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var v) ? v : default;");
                        }
                        else // int
                        {
                            sb.AppendLine($"{p3}    for (int i = 0; i < list2.Count; i++)");
                            sb.AppendLine($"{p3}        arr[i] = int.TryParse(list2[i].Value, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var v) ? v : default;");
                        }
                        sb.AppendLine($"{p3}    return arr;");
                        sb.AppendLine($"{p3}}}");
                        sb.AppendLine($"{p3}set");
                        sb.AppendLine($"{p3}{{");
                        sb.AppendLine($"{p3}    var list2 = Current.GetTagsByName(\"{rawName}\");");
                        sb.AppendLine($"{p3}    for (int i = 0; i < list2.Count && i < value.Length; i++)");
                        if (csType == "string")
                            sb.AppendLine($"{p3}        list2[i].Value = value[i];");
                        else
                            sb.AppendLine($"{p3}        list2[i].Value = value[i].ToString(System.Globalization.CultureInfo.InvariantCulture);");
                        sb.AppendLine($"{p3}}}");
                        sb.AppendLine($"{p2}}}");
                        sb.AppendLine();
                    }
                }

                // Forwarding helpers
                sb.AppendLine($"{p2}public static void SetClear() => Current.SetClear();");
                sb.AppendLine($"{p2}public static void Render() => Current.Render();");
                sb.AppendLine($"{p2}public static void CopyFrom(object recordOrType) => Current.CopyFromObject(recordOrType);");
                sb.AppendLine($"{p2}public static void CopyFrom(System.Type type) => Current.CopyFromObject(type);");
                sb.AppendLine($"{p2}public static string ToJson() => Current.ToJson();");
                sb.AppendLine($"{p2}public static {fq} FromJson(string json) => {fq}.FromJson(json); // produces a new instance; does not mutate Current");
                sb.AppendLine();

                sb.AppendLine($"{p1}}}");
                sb.AppendLine();
            }

            sb.AppendLine($"{p0}}}");
            sb.AppendLine();
        }
    }
}
