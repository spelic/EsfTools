using EsfParser.Esf;
using EsfParser.Parser.Logic.Statements;
using EsfParser.CodeGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;

namespace EsfParser.Tags
{
    /// <summary>
    /// Represents a collection of function definitions within an ESF program.  This class
    /// exposes a method to generate C# code for all functions by delegating to each
    /// <see cref="FuncTag"/> contained in the collection.
    /// </summary>
    public class FuncTagCollection : IEsfTagModel
    {
        [JsonIgnore]
        public string TagName => "FUNCTIONS";

        /// <summary>
        /// The list of functions defined in this collection.
        /// </summary>
        public List<FuncTag> Functions { get; set; } = new();

        /// <summary>
        /// Flattens all before and after logic statements across all functions.
        /// </summary>
        [JsonIgnore]
        public List<IStatement> AllStatements => Functions
            .SelectMany(f => f.BeforeLogicStatements.Concat(f.AfterLogicStatements))
            .ToList();

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"FUNCTIONS: {Functions.Count} defined");
            int idx = 1;
            foreach (var func in Functions)
            {
                sb.AppendLine($"  {idx++}. FUNC {func.Name}");
                var detailLines = func.ToString()
                                      .Split(new[] { Environment.NewLine }, StringSplitOptions.None);
                foreach (var line in detailLines)
                {
                    sb.AppendLine("    " + line);
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// Generates a C# code snippet that defines a static class containing methods for each function.
        /// Each function is emitted as a static method whose body comes from <see cref="FuncTag.ToCSharp"/>.
        /// Function names are sanitized for use as method identifiers via <see cref="CSharpUtils.CleanUnderscore"/>.
        /// If a function includes a description, it is emitted as an XML summary comment above the method.
        /// </summary>
        /// <returns>The generated C# class definition for all functions.</returns>
        public string ToCSharp()
        {
            if (Functions == null || Functions.Count == 0)
            {
                return "public static class GlobalFunctions {}";
            }

            var sb = new StringBuilder();
            sb.AppendLine(CSharpUtils.Indent(1) + "public static class GlobalFunctions");
            sb.AppendLine(CSharpUtils.Indent(1) + "{");
            foreach (var func in Functions)
            {
                var methodName = CSharpUtils.CleanName(func.Name);
                if (!string.IsNullOrWhiteSpace(func.Desc))
                {
                    sb.AppendLine(CSharpUtils.Indent(2) + "/// <summary>");
                    sb.AppendLine(CSharpUtils.Indent(2) + $"/// {func.Desc}");
                    sb.AppendLine(CSharpUtils.Indent(2) + "/// </summary>");
                }
                sb.AppendLine(CSharpUtils.Indent(2) + $"public static void {methodName}()");
                sb.AppendLine(CSharpUtils.Indent(2) + "{");
                var body = func.ToCSharp() ?? string.Empty;
                var lines = body.Replace("\r\n", "\n").Split('\n');
                foreach (var line in lines)
                {
                    if (line.Length > 0)
                        sb.AppendLine(CSharpUtils.Indent(3) + line);
                    else
                        sb.AppendLine();
                }
                sb.AppendLine(CSharpUtils.Indent(2) + "}");
                sb.AppendLine();
            }
            sb.AppendLine(CSharpUtils.Indent(1) + "}");
            sb.AppendLine();
            return sb.ToString();
        }
    }
}