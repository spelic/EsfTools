using System;
using System.Collections.Generic;
using System.IO;
using System.Text;


namespace EsfCodeGen
{
    public class EsfFunctionExporter
    {
        public void Export(EsfCore.Tags.FuncTag func, string outputDir)
        {
            Directory.CreateDirectory(outputDir);

            string className = $"FUNC_{func.Name}";
            string csPath = Path.Combine(outputDir, className + ".cs");
            string mdPath = Path.Combine(outputDir, className + ".md");

            // Export .cs file
            File.WriteAllText(csPath, GenerateCSharp(func));

            // Export .md file
            File.WriteAllText(mdPath, GenerateMarkdown(func));
        }

        private string GenerateCSharp(EsfCore.Tags.FuncTag func)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"// Auto-generated from FUNC: {func.Name}");
            sb.AppendLine("public class " + func.Name);
            sb.AppendLine("{");
            foreach (var line in func.Lines)
                sb.AppendLine("    " + line.Translated);
            sb.AppendLine("}");
            return sb.ToString();
        }

        private string GenerateMarkdown(EsfCore.Tags.FuncTag func)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"# FUNC: {func.Name}\n");
            sb.AppendLine("| ESF | Translated C# |");
            sb.AppendLine("|-----|----------------|");
            foreach (var line in func.Lines)
            {
                string esf = line.Original.Replace("|", "¸\\|");
                string csharp = line.Translated.Replace("|", "\\|");
                sb.AppendLine($"| `{esf}` | `{csharp}` |");
            }
            return sb.ToString();
        }
    }
}