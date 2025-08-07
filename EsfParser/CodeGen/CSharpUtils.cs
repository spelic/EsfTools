using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EsfParser.CodeGen
{
    public static class CSharpUtils
    {
        // Helper to build indentation strings
        public static string Indent(int level) => new string(' ', level * 4);

        // Helper to map ESF data types to C# types
        public static string MapCsType(string type, int decimals)
        {
            type = (type ?? "").ToUpperInvariant();
            return type switch
            {
                "BIN" => "int",
                "NUM" => decimals > 0 ? "decimal" : "int",
                "PACK" => decimals > 0 ? "decimal" : "int",
                "PACF" => decimals > 0 ? "decimal" : "int",
                "CHA" => "string",
                "DBCS" => "string",
                "MIX" => "string",
                _ => "string"
            };
        }

        // Helper to clean identifier names (replace '-' with '_' and prefix underscore if needed)
        public static string CleanUnderscore(string name)
        {
            if (string.IsNullOrEmpty(name)) return name;
            var res = name.Replace('-', '_');
            if (!char.IsLetter(res[0]) && res[0] != '_') res = "_" + res;
            return res;
        }

    }
}
