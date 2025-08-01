using EsfCore.Esf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using EsfCode.CodeGen;

namespace EsfCore.Tags
{
    public class EsfProgram
    {
        [JsonIgnore]
        public List<TagNode> Nodes { get; set; } = new();

        public EzeeTag? Ezee { get; set; }
        public ProgramTag? Program { get; set; }
        public FuncTagCollection Funcs { get; set; } = new();
        public MapTagCollection Maps { get; set; } = new();
        public RecordTagCollection Records { get; set; } = new();
        public ItemTagCollection Items { get; set; } = new();
        public TbleTagCollection Tables { get; set; } = new();

        /// <summary>
        /// The actual RecordTag corresponding to PROGRAM.WORKSTOR
        /// (e.g. if WORKSTOR="IS00W01", this will return the IS00W01 record).
        /// </summary>
        [JsonIgnore]
        public RecordTag? WorkstorRecord =>
                                            Program == null
                                                         ? null
                                                         : Records.Records
                                                             .FirstOrDefault(r =>
                                                                string.Equals(r.Name, Program.Workstor, StringComparison.OrdinalIgnoreCase)
                                                                                 );

        /// <summary>
        /// All MapTag instances in the program’s MAPGROUP.
        /// </summary>
        [JsonIgnore]
        public IEnumerable<MapTag> ProgramMaps =>
                                                     Program == null
                                                     ? Enumerable.Empty<MapTag>()
                                                     : Maps.Maps
                                                                .Where(m =>
                                                                string.Equals(m.GrpName, Program.MapGroup, StringComparison.OrdinalIgnoreCase)
                                                         );



        public List<IEsfTagModel> Tags => new IEsfTagModel[] {
            Ezee, Program, Funcs, Maps, Records, Items, Tables
        }.Where(t => t != null).ToList();

        public T? GetTag<T>() where T : class, IEsfTagModel
            => Tags.OfType<T>().FirstOrDefault();

        public override string ToString()
        {
            var sb = new StringBuilder();
            foreach (var tag in Tags)
                sb.AppendLine(tag?.ToString());
            return sb.ToString();
        }

        public void AddTag(IEsfTagModel tag)
        {
            if (tag is EzeeTag e) Ezee = e;
            else if (tag is ProgramTag p) Program = p;
            else if (tag is FuncTagCollection f) Funcs.Functions.AddRange(f.Functions);
            else if (tag is MapTagCollection m) Maps.Maps.AddRange(m.Maps);
            else if (tag is RecordTagCollection r) Records.Records.AddRange(r.Records);
            else if (tag is ItemTagCollection i) Items.Items.AddRange(i.Items);
            else if (tag is TbleTagCollection t) Tables.Tables.AddRange(t.Tables);
        }

        public void ExportToConsoleProject(string outputDir, string projectNameSpace = "EsfConsoleApp")
        {
            Directory.CreateDirectory(outputDir);

            // 1) Project file
            GenerateCsProj(outputDir, projectNameSpace);

            // 2) Screens
            var generatorMap = new MapClassGenerator(Maps.Maps, outputDir, projectNameSpace);
            generatorMap.Generate();

            // 3) SpecialFunctions helper class
            GenerateSpecialFunctionsClass(outputDir, projectNameSpace);

            // 4) Functions
            GenerateFunctionClasses(outputDir, projectNameSpace);

            // 5) Program + DataAccess
            GenerateProgramCs(outputDir, projectNameSpace);

            // 6) Records
            // GenerateRecordClasses(outputDir);

            // 6) Records
            var generator = new RecordClassGenerator(records: Records.Records, outputDirectory: outputDir, rootNamespace: projectNameSpace);
            generator.Generate();


            // 7) Items → a static Globals class
            GenerateItemGlobals(outputDir, projectNameSpace);

            Console.WriteLine($"Scaffolded console project in '{outputDir}'.");
        }
        private void GenerateSpecialFunctionsClass(string outputDir, string projectNameSpace)
        {
            var itemsDir = Path.Combine(outputDir, "Items");
            Directory.CreateDirectory(itemsDir);

            var itemsNs = $"{projectNameSpace}.Items";

            var dir = new DirectoryInfo(Directory.GetCurrentDirectory());
            while (dir != null && !Directory.Exists(Path.Combine(dir.FullName, "EsfCore", "Tags")))
                dir = dir.Parent;
            if (dir == null)
                throw new FileNotFoundException("Could not locate EsfCore\\Tags\\SpecialFunctions.cs");

            var sourcePath = Path.Combine(dir.FullName, "EsfCore", "Tags", "SpecialFunctions.cs");
            if (!File.Exists(sourcePath))
                throw new FileNotFoundException($"No SpecialFunctions.cs at {sourcePath}");

            var lines = File.ReadAllLines(sourcePath);
            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i].TrimStart().StartsWith("namespace "))
                {
                    lines[i] = $"namespace {itemsNs}";
                    break;
                }
            }

            File.WriteAllLines(Path.Combine(itemsDir, "SpecialFunctions.cs"), lines);
        }
        private void GenerateItemGlobals(string outputDir, string ProjectNameSpace)
        {
            var itemsDir = Path.Combine(outputDir, "Items");
            Directory.CreateDirectory(itemsDir);

            var itemsNs = $"{ProjectNameSpace}.Items";
            var sb = new StringBuilder();

            sb.AppendLine("using System;");
            sb.AppendLine();
            sb.AppendLine($"namespace {itemsNs}");
            sb.AppendLine("{");
            sb.AppendLine("    /// <summary>");
            sb.AppendLine("    /// Global variables for all ESF ITEM definitions");
            sb.AppendLine("    /// </summary>");
            sb.AppendLine("    public static class GlobalItems");
            sb.AppendLine("    {");

            foreach (var fld in Items.Items)
            {
                string csType = fld.Type.ToString().ToUpper() switch
                {
                    "BIN" => "int",
                    "NUM" => fld.Decimals > 0 ? "decimal" : "int",
                    "PACK" or "PACF" => fld.Decimals > 0 ? "decimal" : "int",
                    "CHA" or "DBCS" or "MIX" => "string",
                    _ => "string"
                };

                if (!string.IsNullOrWhiteSpace(fld.Description))
                {
                    sb.AppendLine("        /// <summary>");
                    sb.AppendLine($"        /// {fld.Description}");
                    sb.AppendLine("        /// </summary>");
                }

                var name = fld.Name;
                if (!char.IsLetter(name[0]) && name[0] != '_')
                    name = "_" + name;
                name = name.Replace('-', '_');

                sb.AppendLine($"        public static {csType} {name};");
                sb.AppendLine();
            }

            sb.AppendLine("    }");
            sb.AppendLine("}");

            File.WriteAllText(Path.Combine(itemsDir, "GlobalItems.cs"), sb.ToString());
        }
        private void GenerateCsProj(string outputDir, string ProjectNameSpace)
        {
            var csproj = Path.Combine(outputDir, $"{ProjectNameSpace}.csproj");
            File.WriteAllText(csproj, $@"
<Project Sdk=""Microsoft.NET.Sdk"">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net7.0</TargetFramework>
    <RootNamespace>{ProjectNameSpace}</RootNamespace>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include=""Dapper"" Version=""2.0.123"" />
    <PackageReference Include=""IBM.Data.DB2.Core"" Version=""3.1.0"" />
  </ItemGroup>
</Project>".Trim());
        }
        private void GenerateFunctionClasses(string outputDir, string ProjectNameSpace)
        {
            var functions = Funcs.Functions;
            var fnDir = Path.Combine(outputDir, "Functions");
            Directory.CreateDirectory(fnDir);

            EsfLogicToCs.program = this;

            foreach (var func in functions)
            {
                var className = func.Name;
                var filePath = Path.Combine(fnDir, className + ".cs");
                var sb = new StringBuilder();

                sb.AppendLine("using System;");
                sb.AppendLine($"using {ProjectNameSpace}.Records;");
                sb.AppendLine($"using {ProjectNameSpace}.Items;");
                sb.AppendLine($"using {ProjectNameSpace}.Screens;");
                sb.AppendLine();
                sb.AppendLine($"namespace {ProjectNameSpace}.Functions");
                sb.AppendLine("{");
                sb.AppendLine($"    public static class {className}");
                sb.AppendLine("    {");
                sb.AppendLine("        public static void Execute()");
                sb.AppendLine("        {");

                // now calls the zero-parameter overload
                sb.Append(func.GenerateCSharpBody(indentSpaces: 8));

                sb.AppendLine("        }");
                sb.AppendLine("    }");
                sb.AppendLine("}");

                File.WriteAllText(filePath, sb.ToString());
            }
        }
        private void GenerateProgramCs(string outputDir, string projectNameSpace)
        {
            string mainCall;
            if (!string.IsNullOrEmpty(Program?.Mainfun.Name) &&
                Funcs.Functions.Exists(f => f.Name == Program.Mainfun.Name))
            {
                mainCall = $"{Program.Mainfun.Name}.Execute();";
            }
            else if (Maps.Maps.Count > 0)
            {
                mainCall = $"{Maps.Maps[0].MapName}.Render();";
            }
            else
            {
                mainCall = "// nothing to run";
            }

            var sb = new StringBuilder();
            sb.AppendLine("using System;");
            sb.AppendLine($"using {projectNameSpace}.Screens;");
            sb.AppendLine($"using {projectNameSpace}.Functions;");
            sb.AppendLine();
            sb.AppendLine($"namespace {projectNameSpace}");
            sb.AppendLine("{");
            sb.AppendLine("  class Program");
            sb.AppendLine("  {");
            sb.AppendLine("    static void Main(string[] args)");
            sb.AppendLine("    {");
            sb.AppendLine($"      Console.Title = \"{Program?.Name}\";");
            sb.AppendLine("      Console.Clear();");
            sb.AppendLine("      " + mainCall);
            sb.AppendLine("      Console.ReadLine();");
            sb.AppendLine("    }");
            sb.AppendLine("  }");
            sb.AppendLine("}");

            File.WriteAllText(Path.Combine(outputDir, "Program.cs"), sb.ToString());

            // DataAccess.cs...
        }
    }

}


/*

        private void GenerateMapClasses(string outputDir, string ProjectNameSpace)
        {
            var screensDir = Path.Combine(outputDir, "Screens");
            Directory.CreateDirectory(screensDir);
            // CfieldTag.cs
            File.WriteAllText(
                Path.Combine(screensDir, "CfieldTag.cs"),
                $@"using System;

namespace {ProjectNameSpace}.Screens
{{
    /// <summary>
    /// A constant (read‐only) field on the screen.
    /// </summary>
    public class CfieldTag
    {{
        public int Row    {{ get; set; }}
        public int Column {{ get; set; }}
        public string Type  {{ get; set; }} = """";
        public int Bytes   {{ get; set; }}
        public string Value {{ get; set; }} = """";

        public override string ToString() =>
            $""CFIELD [{{Row}},{{Column}}] {{Type}}/{{Bytes}}B: '{{Value}}'"";
    }}
}}");

            // VfieldTag.cs
            File.WriteAllText(
                Path.Combine(screensDir, "VfieldTag.cs"),
                $@"using System;

namespace {ProjectNameSpace}.Screens
{{
    /// <summary>
    /// A variable (read‐write) field on the screen, with runtime MDT & intensity.
    /// </summary>
    public class VfieldTag
    {{

        /// <summary>Raised when someone calls SetCursor()</summary>
        public event Action<VfieldTag>? OnCursor;

        public int    Row     {{ get; set; }}
        public int    Column  {{ get; set; }}
        public string Name    {{ get; set; }} = """";
        public string Type    {{ get; set; }} = """";
        public int    Bytes   {{ get; set; }}
        public string Value   {{ get; set; }} = """";

        public bool IsModified {{ get; private set; }}
        public string Intensity {{ get; private set; }} = ""NORMAL"";

        public void SetModified()   => IsModified = true;
        public void ClearModified() => IsModified = false;
        public void SetDark()       => Intensity = ""DARK"";
        public void SetBright()     => Intensity = ""BRIGHT"";
        public void SetNormal()     => Intensity = ""NORMAL"";

        /// <summary>Move the cursor to this field’s position on screen.</summary>
        public void SetCursor()
            => OnCursor?.Invoke(this);

        public override string ToString() =>
            $""VFIELD [{{Row}},{{Column}}] {{Name}}/{{Type}}/{{Bytes}}B: '{{Value}}' (MDT={{IsModified}}, Intensity={{Intensity}})"";
    }}
}}");

            foreach (var map in Maps.Maps)
            {
                var className = map.MapName;
                var filePath = Path.Combine(screensDir, className + ".cs");
                var sb = new StringBuilder();

                // usings
                sb.AppendLine("using System;");
                sb.AppendLine("using System.Collections.Generic;");
                sb.AppendLine("using System.Reflection;");  // for CopyFrom
                sb.AppendLine("using System.Linq;    // for GroupBy/ToDictionary");
                sb.AppendLine();
                sb.AppendLine($"namespace {ProjectNameSpace}.Screens");
                sb.AppendLine("{");
                sb.AppendLine($"    public static class {className}");
                sb.AppendLine("    {");

                // Constant fields (Cfields) …
                // … same as before …

                // Variable fields (Vfields):
                sb.AppendLine("        /// <summary>All variable fields on this map</summary>");
                sb.AppendLine("        public static readonly IReadOnlyList<VfieldTag> Vfields = new List<VfieldTag>");
                sb.AppendLine("        {");
                foreach (var vf in map.Vfields)
                {
                    var val = vf.InitialValue?.Replace("\\", "\\\\").Replace("\"", "\\\"") ?? "";
                    sb.AppendLine(
                        $"            new VfieldTag {{ Row = {vf.Row}, Column = {vf.Column}, Name = \"{vf.Name}\", Type = \"{vf.Type}\", Bytes = {vf.Bytes}, Value = \"{val}\" }},");
                }
                sb.AppendLine("        };");
                sb.AppendLine();

                // group by Name
                sb.AppendLine("        // group variable fields by Name");
                sb.AppendLine("        private static readonly Dictionary<string, IReadOnlyList<VfieldTag>> _vfieldsByName =");
                sb.AppendLine("            Vfields");
                sb.AppendLine("             .GroupBy(v => v.Name, StringComparer.OrdinalIgnoreCase)");
                sb.AppendLine("             .ToDictionary(g => g.Key, g => (IReadOnlyList<VfieldTag>)g.ToList());");
                sb.AppendLine();

                // count occurrences
                var groups = map.Vfields
                                .GroupBy(v => v.Name, StringComparer.OrdinalIgnoreCase)
                                .ToDictionary(g => g.Key, g => (g.ToList(), g.Count()), StringComparer.OrdinalIgnoreCase);

                // emit accessors
                foreach (var kv in groups)
                {
                    var name = kv.Key;
                    var list = kv.Value.Item1;
                    var count = kv.Value.Item2;
                    var safe = name.Replace('-', '_');

                    // determine C# type from the ESF Type of the first occurrence
                    var esfType = list[0].Type.ToString().ToUpperInvariant();
                    var csType = esfType switch
                    {
                        "BIN" => "int",
                        "NUM" => "string",
                        "PACK" or "PACF" => "int",
                        "CHA" or "DBCS" or "MIX" => "string",
                        _ => "string"
                    };

                    sb.AppendLine($"        /// <summary>Variable field “{name}” {(count > 1 ? "(multiple)" : "")}</summary>");

                    if (count == 1)
                    {
                        // single occurrence → both a Tag and a typed property

                        sb.AppendLine($"        public static VfieldTag {safe}Tag => _vfieldsByName[\"{name}\"][0];");
                        sb.AppendLine();

                        sb.AppendLine($"        public static {csType} {safe}");
                        sb.AppendLine("        {");
                        if (csType == "string")
                        {
                            sb.AppendLine($"            get => {safe}Tag.Value;");
                            sb.AppendLine($"            set => {safe}Tag.Value = value;");
                        }
                        else // numeric
                        {
                            sb.AppendLine($"            get => int.TryParse({safe}Tag.Value, out var v) ? v : default;");
                            sb.AppendLine($"            set => {safe}Tag.Value = value.ToString();");
                        }
                        sb.AppendLine("        }");
                        sb.AppendLine();
                    }
                    else
                    {
                        // multiple occurrences → array of typed values

                        sb.AppendLine($"        public static {csType}[] {safe}");
                        sb.AppendLine("        {");
                        // getter
                        sb.AppendLine("            get");
                        sb.AppendLine("            {");
                        if (csType == "string")
                        {
                            sb.AppendLine($"                return _vfieldsByName[\"{name}\"].Select(t => t.Value).ToArray();");
                        }
                        else
                        {
                            sb.AppendLine($"                return _vfieldsByName[\"{name}\"].Select(t => int.TryParse(t.Value, out var v) ? v : default).ToArray();");
                        }
                        sb.AppendLine("            }");
                        // setter
                        sb.AppendLine("            set");
                        sb.AppendLine("            {");
                        sb.AppendLine($"                var list = _vfieldsByName[\"{name}\"];");
                        sb.AppendLine("                for (int i = 0; i < list.Count && i < value.Length; i++)");
                        if (csType == "string")
                        {
                            sb.AppendLine("                    list[i].Value = value[i];");
                        }
                        else
                        {
                            sb.AppendLine("                    list[i].Value = value[i].ToString();");
                        }
                        sb.AppendLine("            }");
                        sb.AppendLine("        }");
                        sb.AppendLine();
                    }
                }
                // 0a) Track cursor events
                sb.AppendLine($"        static {className}()");
                sb.AppendLine("        {");
                sb.AppendLine("            foreach (var tag in Vfields)");
                sb.AppendLine("                tag.OnCursor += t =>");
                sb.AppendLine("                {");
                sb.AppendLine("                    CursorRow = t.Row;");
                sb.AppendLine("                    CursorColumn = t.Column;");
                sb.AppendLine("                    Console.SetCursorPosition(CursorColumn - 1, CursorRow - 1);");
                sb.AppendLine("                };");
                sb.AppendLine("        }");
                sb.AppendLine();
                                // 0b) Expose cursor position
                sb.AppendLine("        /// <summary>Last SetCursor() row (1-based)</summary>");
                sb.AppendLine("        public static int CursorRow { get; private set; }");
                sb.AppendLine("        /// <summary>Last SetCursor() column (1-based)</summary>");
                sb.AppendLine("        public static int CursorColumn { get; private set; }");
                sb.AppendLine();
                // 6) Render()
                var renderer = map.GenerateConsoleRenderer()
                                  .Replace($"void {map.MapName}()", "void Render()");
                foreach (var line in renderer.Split(Environment.NewLine))
                    sb.AppendLine("        " + line);
                sb.AppendLine();

                // 7) SetClear()
                sb.AppendLine("        /// <summary>");
                sb.AppendLine("        /// Clears all variable fields (value, MDT, intensity) then re-renders.");
                sb.AppendLine("        /// </summary>");
                sb.AppendLine("        public static void SetClear()");
                sb.AppendLine("        {");
                sb.AppendLine("            foreach (var tag in Vfields)");
                sb.AppendLine("            {");
                sb.AppendLine("                tag.Value = string.Empty;");
                sb.AppendLine("                tag.ClearModified();");
                sb.AppendLine("                tag.SetNormal();");
                sb.AppendLine("            }");
                sb.AppendLine("            Render();");
                sb.AppendLine("        }");

                                // 8) CopyFrom(object)
                sb.AppendLine("        /// <summary>");
                sb.AppendLine("        /// Copies matching record‐property values into the Vfields, then re‐renders.");
                sb.AppendLine("        /// </summary>");
                sb.AppendLine("        public static void CopyFrom(object record)");
                sb.AppendLine("        {");
                sb.AppendLine("            var props = record");
                sb.AppendLine("                .GetType()");
                sb.AppendLine("                .GetProperties(BindingFlags.Public | BindingFlags.Instance);");
                sb.AppendLine("            foreach (var tag in Vfields)");
                sb.AppendLine("            {");
                sb.AppendLine("                var p = props.FirstOrDefault(p =>");
                sb.AppendLine("                    string.Equals(p.Name, tag.Name, StringComparison.OrdinalIgnoreCase));");
                sb.AppendLine("                if (p is null) continue;");
                sb.AppendLine("                var val = p.GetValue(record);");
                sb.AppendLine("                tag.Value = val?.ToString() ?? string.Empty;");
                sb.AppendLine("            }");
                sb.AppendLine("            Render();");
                sb.AppendLine("        }");


                sb.AppendLine("    }");
                sb.AppendLine("}");

                File.WriteAllText(filePath, sb.ToString());
            }
        }
        
*/
