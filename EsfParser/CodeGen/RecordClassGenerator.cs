using EsfParser.Tags;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;


namespace EsfParser.CodeGen
{
    /// <summary>
    /// Generates C# record classes (static) from ESF RecordTag definitions.
    /// Uses reflection-based CopyFrom and SetEmpty methods.
    /// </summary>
    public class RecordClassGenerator
    {
        private readonly IEnumerable<RecordTag> _records;
        private readonly string _outputDirectory;
        private readonly string _targetNamespace;

        private readonly string _static = "";
        //private readonly string _static = " static ";

        public RecordClassGenerator(IEnumerable<RecordTag> records, string outputDirectory, string rootNamespace)
        {
            _records = records;
            _outputDirectory = outputDirectory;
            _targetNamespace = rootNamespace + ".Records";
        }

        public void Generate()
        {
            var dir = Path.Combine(_outputDirectory, "Records");
            Directory.CreateDirectory(dir);

            var workstorList = new List<string>();
            foreach (var rec in _records)
            {
                workstorList.Add(rec.Name);
                if (rec.Org.Equals("WORKSTOR", StringComparison.OrdinalIgnoreCase))
                    GenerateRecordClassForWorkStorage(rec, dir);
                else if (rec.Org.Equals("SQLROW", StringComparison.OrdinalIgnoreCase))
                    GenerateRecordClassForSqlRow(rec, dir);
                else
                    throw new InvalidOperationException($"Unknown record organization: {rec.Org} for record {rec.Name}.");
            }

            if (workstorList.Any())
                GenerateWorkstorClass(dir, workstorList);
        }

        private void GenerateRecordClassForWorkStorage(RecordTag rec, string dir)
        {
            var sb = new StringBuilder();
            WriteHeader(sb, rec);
            var properties = rec.Items
                    .Select(i => new FieldDescriptor(i))
                    .ToList();

            WritePropertiesWithLevelSupport(sb, properties);
            WriteCopyFrom(sb,rec);
            WriteSetEmpty(sb, properties);
            sb.AppendLine("    }");
            sb.AppendLine("}");

            var path = Path.Combine(dir, rec.Name + ".cs");
            File.WriteAllText(path, sb.ToString());
        }

        private void GenerateRecordClassForSqlRow(RecordTag rec, string dir)
        {
            var sb = new StringBuilder();
            WriteHeader(sb, rec);

            sb.AppendLine("        public " + _static + " string SqlTable { get; set; } = " + $"\"{rec.SqlTables[0].TableId}\";");
            sb.AppendLine("        public " + _static + " string SqlTableId { get; set; } = " + $"\"{rec.SqlTables[0].Label}\";");


            var properties = rec.Items
                                .Select(i => new FieldDescriptor(i))
                                .ToList();
            WriteProperties(sb, properties);
            WriteCopyFrom(sb, rec);
            WriteSetEmpty(sb, properties);
            sb.AppendLine("    }");
            sb.AppendLine("}");

            var path = Path.Combine(dir, rec.Name + ".cs");
            File.WriteAllText(path, sb.ToString());
        }
        private void WriteHeader(StringBuilder sb, RecordTag rec)
        {
            sb.AppendLine("using System;");
            sb.AppendLine("using System.Reflection;");
            sb.AppendLine("using System.Linq;");
            sb.AppendLine();
            sb.AppendLine($"namespace {_targetNamespace}");
            sb.AppendLine("{");
            sb.AppendLine("    /// <summary>");
            sb.AppendLine($"    /// Record '{rec.Name}', Org = {rec.Org}");
            sb.AppendLine("    /// </summary>");
            if (rec.Prol != null && rec.Prol.Length > 0)
            {
                sb.AppendLine("    /* <remarks>");
                sb.AppendLine($"    /// {rec.Prol}");
                sb.AppendLine("    /// </remarks>*/");
            }
            sb.AppendLine($"    public {_static} class {rec.Name}");
            sb.AppendLine("    {");
            sb.AppendLine("        public " + _static + " bool IsErr { get; set; }");
            sb.AppendLine();
        }
        private void WriteProperties(StringBuilder sb, List<FieldDescriptor> props)
        {
            foreach (var p in props)
            {
                if (p.Name == "*")
                {
                    // Skip the '*' field, which is a special case in some records
                    continue;
                }
                sb.AppendLine("        /// <summary>");
                sb.AppendLine($"        /// {p.Description}");
                sb.AppendLine("        /// </summary>");
                sb.AppendLine($"        public {_static} {p.CsType} {p.Name} {{ get; set; }}{p.ArrayInit}");
                sb.AppendLine();
            }
        }
        private void WritePropertiesWithLevelSupport(StringBuilder sb, List<FieldDescriptor> props)
        {
            FieldDescriptor? currentParent = null;
            int offset = 0;

            foreach (var p in props)
            {
                // Common XML doc comment
                sb.AppendLine($"        /// {p.Description}");

                if (p.Level == 3)
                {
                    // Parent (level 3): full read/write
                    sb.AppendLine(
                        $"        public {_static} {p.CsType} {p.Name} {{ get; set; }}{p.ArrayInit}");
                    sb.AppendLine();

                    // reset offset and mark this as the current parent
                    currentParent = p;
                    offset = 0;
                }
                else if (p.Level == 4 && currentParent != null)
                {
                    if (p.Name != "*")
                    {
                        sb.AppendLine($"        // Sub field of: {currentParent} substring start at: {offset} and length of {p.Bytes}");
                        sb.AppendLine($"        public string {p.Name} => {currentParent.Name}.Substring({offset}, {p.Bytes});");
                    }

                    offset += p.Bytes;
                }
                else
                {
                    if (p.Name != "*")
                    {
                        sb.AppendLine(
                            $"        public {_static}  {p.CsType} {p.Name} {{ get; set; }}{p.ArrayInit}");
                        sb.AppendLine();
                    } else
                    {
                        sb.AppendLine($"        // SKIPPING * : {p.ToString()}");
                    }
                }
            }
        }
        private void WriteCopyFrom(StringBuilder sb, RecordTag rec)
        {
            sb.AppendLine("        /// <summary>");
            sb.AppendLine("        /// Copies all matching fields/properties from another record.");
            sb.AppendLine("        /// </summary>");
            sb.AppendLine("        public static void CopyFrom(object from)");
            sb.AppendLine("        {");
            sb.AppendLine("            if (from == null) throw new ArgumentNullException(nameof(from));");
            sb.AppendLine("            var fromType = from.GetType();");
            sb.AppendLine("            var fromProps = fromType.GetProperties(BindingFlags.Public | BindingFlags.Instance);");
            sb.AppendLine();
            sb.AppendLine($"            var toType = typeof({rec.Name});");
            sb.AppendLine("            var toProps = toType.GetProperties(BindingFlags.Public | BindingFlags.Static);");
            sb.AppendLine();
            sb.AppendLine("            foreach (var toProp in toProps)");
            sb.AppendLine("            {");
            sb.AppendLine("                var fromProp = fromProps.FirstOrDefault(p => string.Equals(p.Name, toProp.Name, StringComparison.OrdinalIgnoreCase));");
            sb.AppendLine("                if (fromProp == null) continue;");
            sb.AppendLine("                try");
            sb.AppendLine("                {");
            sb.AppendLine("                    var value = fromProp.GetValue(from);");
            sb.AppendLine("                    if (toProp.CanWrite)");
            sb.AppendLine("                        toProp.SetValue(null, value);");
            sb.AppendLine("                }");
            sb.AppendLine("                catch { }");
            sb.AppendLine("            }");
            sb.AppendLine("        }");
            sb.AppendLine();
        }
        private void WriteSetEmpty(StringBuilder sb, List<FieldDescriptor> props)
        {
            sb.AppendLine("        /// <summary>");
            sb.AppendLine("        /// Clears all fields to their default values.");
            sb.AppendLine("        /// </summary>");
            sb.AppendLine($"        public {_static} void SetEmpty()");
            sb.AppendLine("        {");
            foreach (var p in props)
            {
                if (p.Name == "*" || p.Level == 4)
                {
                    // Skip the '*' field, which is a special case in some records
                    continue;
                }
                sb.AppendLine($"            {p.Name} = default({p.CsType});");
            }
            sb.AppendLine("        }");
            sb.AppendLine();
        }
        private void GenerateWorkstorClass(string dir, List<string> names)
        {
            var sb = new StringBuilder();
            sb.AppendLine("using System;");
            sb.AppendLine();
            sb.AppendLine($"namespace {_targetNamespace}");
            sb.AppendLine("{");
            sb.AppendLine("    /// <summary>");
            sb.AppendLine("    /// Singletons for all WORKSTOR records");
            sb.AppendLine("    /// </summary>");
            sb.AppendLine("    public static class Workstor");
            sb.AppendLine("    {");
            foreach (var n in names)
                sb.AppendLine($"        public static readonly {n} {n} = new {n}();");
            sb.AppendLine("    }");
            sb.AppendLine("}");

            File.WriteAllText(Path.Combine(dir, "Workstor.cs"), sb.ToString());
        }
        /// <summary>
        /// Describes a single record field for generation.
        /// </summary>
        private class FieldDescriptor
        {
            public string Name { get; }
            public string CsType { get; }
            public string ArrayInit { get; }
            public string Description { get; }
            public int Level { get; }

            public int Bytes { get; set; }

            public FieldDescriptor(RecordItemTag fld)
            {
                Name = fld.Name.Replace('-', '_');
                Level = int.TryParse(fld.Level, out var lvl) ? lvl : 0;
                Bytes = int.TryParse(fld.Bytes, out var b) ? b : 0;
                var baseType = fld.Type.ToUpperInvariant();
                CsType = baseType switch
                {
                    "BIN" => "int",
                    "NUM" => fld.Decimals > 0 ? "decimal" : "int",
                    "PACK" or "PACF" => "string",
                    "CHA" or "DBCS" or "MIX" => "string",
                    _ => "string"
                };
                if (int.TryParse(fld.Occurs, out var occ) && occ > 1)
                {
                    ArrayInit = $" = new {CsType}[{occ}];";
                    CsType += "[]";
                }
                Description = fld.ToString();
            }
        }
    }
}
