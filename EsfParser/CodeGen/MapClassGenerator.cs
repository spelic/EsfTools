using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Reflection;
using EsfParser.Tags;


namespace EsfParser.CodeGen
{
    /// <summary>
    /// Generates C# screen classes (CfieldTag, VfieldTag, and map screens) from ESF map definitions.
    /// Adheres to SOLID by separating concerns into smaller methods.
    /// </summary>
    public class MapClassGenerator
    {
        private readonly IEnumerable<MapTag> _maps;
        private readonly string _outputDir;
        private readonly string _namespace;

        public MapClassGenerator(IEnumerable<MapTag> maps, string outputDir, string rootNamespace)
        {
            _maps = maps;
            _outputDir = outputDir;
            _namespace = rootNamespace + ".Screens";
        }

        public void Generate()
        {
            var screensDir = Path.Combine(_outputDir, "Screens");
            Directory.CreateDirectory(screensDir);

            WriteCfieldTag(screensDir);
            WriteVfieldTag(screensDir);

            foreach (var map in _maps)
                WriteMapClass(screensDir, map);
        }

        private void WriteCfieldTag(string dir)
        {
            var path = Path.Combine(dir, "CfieldTag.cs");
            var sb = new StringBuilder();
            sb.AppendLine("using System;");
            sb.AppendLine();
            sb.AppendLine($"namespace {_namespace}");
            sb.AppendLine("{");
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
            sb.AppendLine("}");
            File.WriteAllText(path, sb.ToString());
        }

        private void WriteVfieldTag(string dir)
        {
            var path = Path.Combine(dir, "VfieldTag.cs");
            var sb = new StringBuilder();
            sb.AppendLine("using System;");
            sb.AppendLine();
            sb.AppendLine($"namespace {_namespace}");
            sb.AppendLine("{");
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
            sb.AppendLine("        public bool IsCursor() => Console.CursorTop == (Row -1) && Console.CursorLeft == (Column - 1);");
            sb.AppendLine("        public override string ToString() =>");
            sb.AppendLine("            $\"VFIELD [{Row},{Column}] {Name}/{Type}/{Bytes}B: '{Value}' (MDT={IsModified}, Intensity={Intensity})\";");
            sb.AppendLine("    }");
            sb.AppendLine("}");
            File.WriteAllText(path, sb.ToString());
        }
       
        private void WriteMapClass(string dir, MapTag map)
        {
            var className = map.MapName;
            var path = Path.Combine(dir, className + ".cs");
            var sb = new StringBuilder();

            WriteMapUsings(sb);
            sb.AppendLine($"namespace {_namespace}");
            sb.AppendLine("{");
            sb.AppendLine($"    public static class {className}");
            sb.AppendLine("    {");

            WriteVariableFieldList(sb, map);
            WriteGroupByName(sb);
            WriteAccessors(sb, map);
            WriteCursorTracking(sb, className);
            WriteRenderMethod(sb, map);
            WriteSetClear(sb);
            WriteCopyFrom(sb);

            sb.AppendLine("    }");
            sb.AppendLine("}");

            File.WriteAllText(path, sb.ToString());
        }

        private void WriteMapUsings(StringBuilder sb)
        {
            sb.AppendLine("using System;");
            sb.AppendLine("using System.Collections.Generic;");
            sb.AppendLine("using System.Linq;");
            sb.AppendLine("using System.Reflection;");
            sb.AppendLine();
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
                var csType = esfType switch
                {
                    "BIN" => "int",
                    "NUM" => "string",
                    "PACK" or "PACF" => "int",
                    "CHA" or "DBCS" or "MIX" => "string",
                    _ => "string"
                };

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
                    sb.AppendLine(csType == "string"
                      ? $"                return _vfieldsByName[\"{name}\"].Select(t=>t.Value).ToArray();"
                      : $"                return _vfieldsByName[\"{name}\"].Select(t=>int.TryParse(t.Value,out var v)?v:default).ToArray();");
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

        private void WriteCursorTracking(StringBuilder sb, string className)
        {
            sb.AppendLine($"        static {className}()\n        {{");
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
