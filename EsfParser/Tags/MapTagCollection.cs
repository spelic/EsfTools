using EsfParser.CodeGen;
using EsfParser.Esf;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using System.Xml.Linq;

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
            var result = $"{TagName}: {Maps.Count} defined\n";
            foreach (var map in Maps)
            {
                result += $"  - {map.GrpName}/{map.MapName} ({map.Date} {map.Time})\n";
                // add detailed information for each cfield and vfield
                if (map.Cfields.Count > 0)
                {
                    result += "    Cfields:\n";
                    foreach (var cfield in map.Cfields)
                    {
                        result += $"      - {cfield}\n";
                    }
                }
                else
                {
                    result += "    Cfields: None\n";
                }
                if (map.Vfields.Count > 0)
                {
                    result += "    Vfields:\n";
                    foreach (var vfield in map.Vfields)
                    {
                        result += $"      - {vfield}\n";
                    }
                }
                else
                {
                    result += "    Vfields: None\n";
                }
                result += $"    Cfields: {map.Cfields.Count}, Vfields: {map.Vfields.Count}\n";
            }

            return result;
        }

        /// <summary>
        /// Generates a C# code snippet that defines a static class containing fields for each map.
        /// For each <see cref="MapTag"/> in <see cref="Maps"/>, a static field is declared
        /// whose type is determined via <see cref="CSharpUtils.MapCsType"/> and whose name
        /// is sanitized via <see cref="CSharpUtils.CleanUnderscore"/>.  If a map includes a
        /// description, it is emitted as an XML summary comment above the corresponding field.
        /// </summary>
        /// <returns>The generated C# class definition.</returns>
        public string ToCSharp()
        {

            if (Maps == null || Maps.Count == 0)
            {
                return "public static class GlobalMaps {}";
            }

            var sb = new StringBuilder();

            WriteCfieldTag(sb);
            WriteVfieldTag(sb);
            sb.AppendLine(CSharpUtils.Indent(0) + "public static class GlobalMaps");
            sb.AppendLine(CSharpUtils.Indent(0) + "{");
            foreach (var map in Maps)
            {
                sb.AppendLine(CSharpUtils.Indent(1) + $"public static class {map.MapName}");
                sb.AppendLine(CSharpUtils.Indent(1) + "{");
                WriteVariableFieldList(sb, map);
                WriteGroupByName(sb);
                WriteAccessors(sb, map);
                WriteCursorTracking(sb, map.MapName);
                WriteRenderMethod(sb, map);
                WriteSetClear(sb);
                WriteCopyFrom(sb);
                sb.AppendLine(CSharpUtils.Indent(1) + "}");
            }
            sb.AppendLine(CSharpUtils.Indent(0) + "}");
            var r = sb.ToString();
            return r;
        }

        private void WriteCfieldTag(StringBuilder sb)
        {
            sb.AppendLine("    /// <summary>");
            sb.AppendLine("    /// A constant (read-only) field on the screen.");
            sb.AppendLine("    /// </summary>");
            sb.AppendLine("    public class CfieldTag");
            sb.AppendLine("    {");
            sb.AppendLine("        public int Row { get; set; }");
            sb.AppendLine("        public int Column { get; set; }");
            sb.AppendLine("        public string Type { get; set; } = \"\";");
            sb.AppendLine("        public int Bytes { get; set; }");
            sb.AppendLine("        public string Value { get; set; } = \"\";");
            sb.AppendLine("        public override string ToString() =>");
            sb.AppendLine("            $\"CFIELD [{Row},{Column}] {Type}/{Bytes}B: '{Value}'\";");
            sb.AppendLine("    }");
        }

        private void WriteVfieldTag(StringBuilder sb)
        {
            sb.AppendLine("    /// <summary>");
            sb.AppendLine("    /// A variable (read-write) field on the screen, with runtime MDT & intensity.");
            sb.AppendLine("    /// </summary>");
            sb.AppendLine("    public class VfieldTag");
            sb.AppendLine("    {");
            sb.AppendLine("        public event Action<VfieldTag>? OnCursor;");
            sb.AppendLine("        public int Row { get; set; }");
            sb.AppendLine("        public int Column { get; set; }");
            sb.AppendLine("        public string Name { get; set; } = \"\";");
            sb.AppendLine("        public string Type { get; set; } = \"\";");
            sb.AppendLine("        public int Bytes { get; set; }");
            sb.AppendLine("        public string Value { get; set; } = \"\";");
            sb.AppendLine("        public bool IsModified { get; private set; }");
            sb.AppendLine("        public string Intensity { get; private set; } = \"NORMAL\";");
            sb.AppendLine("        public void SetModified() => IsModified = true;");
            sb.AppendLine("        public void ClearModified() => IsModified = false;");
            sb.AppendLine("        public void SetDark() => Intensity = \"DARK\";");
            sb.AppendLine("        public void SetBright() => Intensity = \"BRIGHT\";");
            sb.AppendLine("        public void SetNormal() => Intensity = \"NORMAL\";");
            sb.AppendLine("        public void SetCursor() => OnCursor?.Invoke(this);");
            sb.AppendLine("        public void Defined()  { /* later logic */ }");
            sb.AppendLine("        public override string ToString() =>");
            sb.AppendLine("            $\"VFIELD [{Row},{Column}] {Name}/{Type}/{Bytes}B: '{Value}' (MDT={IsModified}, Intensity={Intensity})\";");
            sb.AppendLine("    }");
        }

        private void WriteVariableFieldList(StringBuilder sb, MapTag map)
        {
            sb.AppendLine("        /// <summary>All variable fields on this map</summary>");
            sb.AppendLine("        public static readonly IReadOnlyList<VfieldTag> Vfields = new List<VfieldTag>");
            sb.AppendLine("        {");
            foreach (var vf in map.Vfields)
            {
                var val = vf.InitialValue?.Replace("\\", "\\\\").Replace("\"", "\\\"") ?? "";
                sb.AppendLine(
                    $"            new VfieldTag {{ Row = {vf.Row}, Column = {vf.Column}, Name = \"{vf.Name}\", Type = \"{vf.Type}\", Bytes = {vf.Bytes}, Value = \"{val}\" }},");
            }
            sb.AppendLine("        };\n");
        }

        private void WriteGroupByName(StringBuilder sb)
        {
            sb.AppendLine("        // group variable fields by Name");
            sb.AppendLine("        private static readonly Dictionary<string, IReadOnlyList<VfieldTag>> _vfieldsByName =");
            sb.AppendLine("            Vfields");
            sb.AppendLine("             .GroupBy(v => v.Name, StringComparer.OrdinalIgnoreCase)");
            sb.AppendLine("             .ToDictionary(g => g.Key, g => (IReadOnlyList<VfieldTag>)g.ToList());");
            sb.AppendLine();
        }

        private void WriteAccessors(StringBuilder sb, MapTag map)
        {
            var groups = map.Vfields
                            .GroupBy(v => v.Name, StringComparer.OrdinalIgnoreCase)
                            .ToDictionary(g => g.Key, g => g.ToList(), StringComparer.OrdinalIgnoreCase);

            foreach (var kv in groups)
            {
                var name = kv.Key;
                var list = kv.Value;
                var count = list.Count;
                var safe = name.Replace('-', '_');
                var esfType = list[0].Type.ToString().ToUpperInvariant();
                var csType = "string";

                if (list[0].Type.ToString() == "NUM")
                {
                    if (list[0].Decimals > 0)
                        csType = "decimal";
                    else
                        csType = "int";
                }

                sb.AppendLine($"        /// <summary>Variable field '{name}'{(count > 1 ? " (multiple)" : "")}</summary>");
                if (count == 1)
                {
                    sb.AppendLine($"        public static VfieldTag {safe}Tag => _vfieldsByName[\"{name}\"][0];");
                    sb.AppendLine();
                    sb.AppendLine($"        public static {csType} {safe}");
                    sb.AppendLine("        {");
                    if (csType == "string")
                    {
                        sb.AppendLine($"            get => {safe}Tag.Value;");
                        sb.AppendLine($"            set => {safe}Tag.Value = value;");
                    }
                    else
                    {
                        sb.AppendLine($"            get => int.TryParse({safe}Tag.Value,out var v)?v:default;");
                        sb.AppendLine($"            set => {safe}Tag.Value = value.ToString();");
                    }
                    sb.AppendLine("        }\n");
                }
                else
                {
                    sb.AppendLine($"        public static {csType}[] {safe}");
                    sb.AppendLine("        {");

                    sb.AppendLine("            get");
                    sb.AppendLine("            {");
                    if (csType == "string")
                    {

                        sb.AppendLine("                return _vfieldsByName[\"{name}\"].Select(t=>t.Value).ToArray();");
                    } else if (csType == "decimal")
                    {
                        sb.AppendLine("                return _vfieldsByName[\"{name}\"].Select(t=>decimal.TryParse(t.Value,out var v)?v:default).ToArray();");
                    } else if (csType == "int")
                    {
                        sb.AppendLine($"                return _vfieldsByName[\"{name}\"].Select(t=>int.TryParse(t.Value,out var v)?v:default).ToArray();");
                    }
                      
                    sb.AppendLine("            }");
                    sb.AppendLine("            set");
                    sb.AppendLine("            {");
                    sb.AppendLine("                var list = _vfieldsByName[\"{name}\"];");
                    sb.AppendLine("                for(int i=0;i<list.Count && i<value.Length;i++)");
                    sb.AppendLine(csType == "string"
                      ? "                    list[i].Value = value[i];"
                      : "                    list[i].Value = value[i].ToString();");
                    sb.AppendLine("            }");
                    sb.AppendLine("        }\n");
                }
            }
        }

        private void WriteCursorTracking(StringBuilder sb, string mapName)
        {
            sb.AppendLine($"        static {mapName}()\n        {{");
            sb.AppendLine("            foreach(var tag in Vfields)");
            sb.AppendLine("                tag.OnCursor += t =>");
            sb.AppendLine("                {\n                    CursorRow = t.Row;\n                    CursorColumn = t.Column;\n                    Console.SetCursorPosition(CursorColumn - 1, CursorRow - 1);\n                };\n        }");
            foreach (var line in new[] {
                "        public static int CursorRow { get; private set; }",
                "        public static int CursorColumn { get; private set; }\n"
            })
            {
                sb.AppendLine(line);
            }
        }

        private void WriteRenderMethod(StringBuilder sb, MapTag map)
        {
            sb.AppendLine("        public static void Render()\n        {");
            var lines = map.GenerateConsoleRenderer().Split(Environment.NewLine);
            foreach (var line in lines)
                sb.AppendLine("            " + line);
            sb.AppendLine("        }\n");
        }

        private void WriteSetClear(StringBuilder sb)
        {
            foreach (var line in new[] {
                "        public static void SetClear()",
                "        {",
                "            foreach(var tag in Vfields)",
                "            {",
                "                tag.Value = string.Empty;",
                "                tag.ClearModified();",
                "                tag.SetNormal();",
                "            }",
                "            Render();",
                "        }\n"
            })
            {
                sb.AppendLine(line);
            }
        }


        private void WriteCopyFrom(StringBuilder sb)
        {
            foreach (var line in new[] {
                "        public static void CopyFrom(object record)",
                "        {",
                "            var props = record.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);",
                "            foreach(var tag in Vfields)",
                "            {",
                "                var p = props.FirstOrDefault(x => string.Equals(x.Name, tag.Name, StringComparison.OrdinalIgnoreCase));",
                "                if(p==null) continue;",
                "                tag.Value = p.GetValue(record)?.ToString() ?? string.Empty;",
                "            }",
                "            Render();",
                "        }"
            })
            {
                sb.AppendLine(line);
            }
        }
    }
}