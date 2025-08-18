using EsfParser.CodeGen;
using EsfParser.Parser.Logic.Statements;
using EsfParser.Tags;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace EsfParser.Esf
{
    public class EsfProgram
    {
        [JsonIgnore]
        public List<TagNode> Nodes { get; set; } = new();

        public EzeeTag? Ezee { get; set; }
        public ProgramTag? Program { get; set; }
        public FuncTagCollection Functions { get; set; } = new();
        public List<IStatement> GetAllFunctionStatements() => Functions.AllStatements;
        public MapTagCollection Maps { get; set; } = new();
        public RecordTagCollection Records { get; set; } = new();
        public ItemTagCollection Items { get; set; } = new();
        public TableTagCollection Tables { get; set; } = new();

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

        ////public List<IEsfTagModel> Tags => new IEsfTagModel[] {
        //    Ezee, Program, Functions, Maps, Records, Items, Tables
        //}.Where(t => t != null).ToList();
        //public T? GetTag<T>() where T : class, IEsfTagModel
        //    => Tags.OfType<T>().FirstOrDefault();
        public void AddTag(IEsfTagModel tag)
        {
            if (tag is EzeeTag e) Ezee = e;
            else if (tag is ProgramTag p) Program = p;
            else if (tag is FuncTagCollection f) Functions.Functions.AddRange(f.Functions);
            else if (tag is MapTagCollection m) Maps.Maps.AddRange(m.Maps);
            else if (tag is RecordTagCollection r) Records.Records.AddRange(r.Records);
            else if (tag is ItemTagCollection i) Items.Items.AddRange(i.Items);
            else if (tag is TableTagCollection t) Tables.Tables.AddRange(t.Tables);
        }
        /// <summary>
        /// Generate a single C# file containing all code needed to run the ESF
        /// program as a console application.  This helper builds a combined source
        /// using the <c>ToCSharp()</c> methods of each collection rather than scaffolding
        /// a full project.  It also inserts a placeholder <c>Main</c> method for the
        /// console application entry point.
        /// </summary>
        /// <param name="outputFile">Full path to the Program.cs file to generate.</param>
        /// <param name="projectNameSpace">Namespace for generated types (default "EsfConsoleApp").</param>
        public void ExportToSingleProgramFile(string outputFile, string projectNameSpace = "EsfConsoleApp")
        {
            if (string.IsNullOrWhiteSpace(outputFile))
                throw new ArgumentException("Output file path must be provided", nameof(outputFile));

            var sb = new StringBuilder();

            // Required usings for the generated program
            sb.AppendLine("using System;");
            sb.AppendLine("using System.Collections.Generic;");
            sb.AppendLine("using System.Reflection;");
            sb.AppendLine("using System.Linq;");
            sb.AppendLine();

            // Begin namespace
            sb.AppendLine($"namespace {projectNameSpace}");
            sb.AppendLine("{");

            // Append code for each collection if present
            if (Items != null && Items.Items.Count > 0)
            {
                var itemsCode = Items.ToCSharp();
                if (!string.IsNullOrWhiteSpace(itemsCode)) sb.AppendLine(itemsCode);
            }
            if (Records != null && Records.Records.Count > 0)
            {
                var recordsCode = Records.ToCSharp();
                if (!string.IsNullOrWhiteSpace(recordsCode)) sb.AppendLine(recordsCode);
            }
            if (Maps != null && Maps.Maps.Count > 0)
            {
                var mapsCode = Maps.ToCSharp();
                if (!string.IsNullOrWhiteSpace(mapsCode)) sb.AppendLine(mapsCode);
            }
            if (Tables != null && Tables.Tables.Count > 0)
            {
                var tablesCode = Tables.ToCSharp();
                if (!string.IsNullOrWhiteSpace(tablesCode)) sb.AppendLine(tablesCode);
            }
            if (Functions != null && Functions.Functions.Count > 0)
            {
                var funcsCode = Functions.ToCSharp();
                if (!string.IsNullOrWhiteSpace(funcsCode)) sb.AppendLine(funcsCode);
            }
            // Emit a basic Program class with Main method.  This can be extended to
            // instantiate and use the generated static classes as needed.
            sb.AppendLine("    public static class Program");
            sb.AppendLine("    {");
            sb.AppendLine("        public static void Main()");
            sb.AppendLine("        {");
            sb.AppendLine("            // TODO: Write application logic here using the generated Global* classes.");
            sb.AppendLine("            Console.WriteLine(\"ESF program initialized.\");");
            sb.AppendLine("        }");
            sb.AppendLine("    }");

            // Close namespace
            sb.AppendLine("}");

            // Ensure output directory exists
            var outDir = Path.GetDirectoryName(outputFile);
            if (!string.IsNullOrEmpty(outDir) && !Directory.Exists(outDir))
                Directory.CreateDirectory(outDir);

            // Write combined source to the output file
            File.WriteAllText(outputFile, sb.ToString());
        }
        public void RoslynExportToSingleProgramFile(string outputFile, string projectNameSpace = "EsfConsoleApp")
        {
            if (string.IsNullOrWhiteSpace(outputFile))
                throw new ArgumentException("Output file path must be provided", nameof(outputFile));

            RoslynExporter.WriteSourceFile(this, outputFile);
        }
    }

}