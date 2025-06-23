using System;
using System.Collections.Generic;

namespace EsfCodeGen
{
    public class EsfToCSharpConverter
    {
        public string ConvertFunctionBody(string esfCode)
        {
            var lines = esfCode.Split('\n');
            var output = new List<string>();
            foreach (var line in lines)
            {
                var translated = ConvertSingleLine(line.Trim());
                if (!string.IsNullOrWhiteSpace(translated))
                    output.Add(translated);
            }
            return string.Join("\n", output);
        }

        public string ConvertSingleLine(string line)
        {
            line = line.Trim();

            if (line.StartsWith("IF ")) return $"if ({line[3..].TrimEnd(';')}) {{";
            if (line.StartsWith("WHILE ")) return $"while ({line[6..].TrimEnd(';')}) {{";
            if (line.StartsWith("ELSE")) return "} else {";
            if (line == "END;") return "}";

            if (line.StartsWith("MOVE "))
            {
                var parts = line[5..].Split(" TO ");
                if (parts.Length == 2)
                    return $"{parts[1].Trim()} = {parts[0].Trim()};";
            }

            if (line.StartsWith("CALL "))
                return $"{line[5..].TrimEnd(';')}();";

            if (line.StartsWith("DXFR "))
                return $"// Transfer: {line[5..]}";

            return $"// {line}";
        }
    }
}