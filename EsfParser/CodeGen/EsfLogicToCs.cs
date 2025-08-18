using EsfParser.Esf;
using EsfParser.Tags;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace EsfParser.CodeGen
{
    public static class EsfLogicToCs
    {
        public static EsfProgram program { get; set; }

        /// <summary>
        /// Translate ESF logic to C#, automatically qualifying SpecialFunctions,
        /// GlobalItems, and WORKSTOR.<fields>.
        /// Make sure you've called Configure(...) first.
        /// </summary>
        public static string GenerateCSharpFromLogic(FuncTag func,
            List<string> esfLines,
            int indentSpaces = 6)
        {

            if (func.BeforeLogicStatements == null || func.BeforeLogicStatements.Count == 0)
                return "// No logic found for this function.";

            var sb = new StringBuilder();

            string indent = new string(' ', indentSpaces);
            string doubleIndent = new string(' ', indentSpaces * 2);

            foreach (var stmt in func.BeforeLogicStatements)
            {
                string codeLine = stmt.ToCSharp();

                if (!string.IsNullOrWhiteSpace(codeLine))
                {
                    foreach (var line in codeLine.Split('\n'))
                    {
                        sb.AppendLine(doubleIndent + line.TrimEnd());
                    }
                }
                //sb.AppendLine($"// ORG: {stmt.OriginalCode}");
            }

            

            return sb.ToString();

        }
    }
}
