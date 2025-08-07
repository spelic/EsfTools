using System;
using System.Linq;
using EsfParser.CodeGen;
using EsfParser.Esf;
using System.Text.RegularExpressions;

namespace EsfParser.Parser.Logic.Statements
{
    public class MoveStatement : IStatement
    {
        public StatementType Type => StatementType.Move;
        public string OriginalCode { get; set; } = string.Empty;
        public string Source { get; set; } = string.Empty;
        public string Destination { get; set; } = string.Empty;

        public int LineNumber { get; set; }
        public int NestingLevel { get; set; } = 0;

        public override string ToString()
               => $"MoveStatement: {Source} -> {Destination} (Line: {LineNumber}, Nesting: {NestingLevel})";

        // add method to convert this statement to C# code


        /// <summary>
        /// Emit C# code for a MOVE statement.
        ///
        /// The ESF MOVE statement copies the value of <see cref="Source"/> to
        /// <see cref="Destination"/>.  When both source and destination are
        /// records or maps, ESF moves the corresponding items by name; in the
        /// generated code this is treated as a simple assignment, relying on
        /// generated classes or user code to implement field-wise copying.
        /// </summary>
        /// <returns>A C# assignment statement.</returns>
        public string ToCSharp()
        {
            // Translate ESF MOVE into a C# assignment or record/map copy.  The
            // semantics of MOVE are:
            //   * MOVE <src> TO <dst>;            -- copy value
            //   * MOVE <record> TO <record>;      -- copy all matching fields
            //   * MOVE <record> TO <map>;         -- populate Vfields from record
            //
            // To correctly emit C# we first qualify each operand.  Items live
            // in the GlobalItems class, records live under Workstor and maps
            // live directly in the Screens namespace.  Subscripted indices are
            // converted from 1‑based (ESF) to 0‑based (C#).

            string srcExpr = ConvertOperand(Source);
            string dstExpr = ConvertOperand(Destination);

            // Determine whether source and/or destination refer to entire
            // records or maps (no dot).  If both refer to complex objects
            // (record or map) then we use the CopyFrom helper.  Otherwise we
            // emit a simple assignment.
            bool srcHasDot = Source.Contains('.');
            bool dstHasDot = Destination.Contains('.');

            if (!srcHasDot && !dstHasDot)
            {
                var prog = EsfParser.CodeGen.EsfLogicToCs.program;
                if (prog != null)
                {
                    // Clean names for comparison
                    string cleanSrc = CleanName(Source);
                    string cleanDst = CleanName(Destination);
                    bool srcIsRecord = prog.Records.Records.Any(r => string.Equals(CleanName(r.Name), cleanSrc, StringComparison.OrdinalIgnoreCase));
                    bool dstIsRecord = prog.Records.Records.Any(r => string.Equals(CleanName(r.Name), cleanDst, StringComparison.OrdinalIgnoreCase));
                    bool srcIsMap = prog.Maps.Maps.Any(m => string.Equals(CleanName(m.MapName), cleanSrc, StringComparison.OrdinalIgnoreCase));
                    bool dstIsMap = prog.Maps.Maps.Any(m => string.Equals(CleanName(m.MapName), cleanDst, StringComparison.OrdinalIgnoreCase));
                    if ((srcIsRecord || srcIsMap) && (dstIsRecord || dstIsMap))
                    {
                        return $"{dstExpr}.CopyFrom({srcExpr});";
                    }
                }
            }

            // Default: simple assignment for value/field copies
            return $"{dstExpr} = {srcExpr};";
        }

        /// <summary>
        /// Convert an ESF operand into a qualified C# expression.
        /// </summary>
        private static string ConvertOperand(string operand)
        {
            operand = operand.Trim();
            if (string.IsNullOrEmpty(operand)) return operand;

            // Literal strings: single or double quoted.  Emit double‑quoted
            // literal with escaped backslashes and quotes.
            if ((operand.StartsWith("\"") && operand.EndsWith("\"")) ||
                (operand.StartsWith("'") && operand.EndsWith("'")))
            {
                string content = operand.Substring(1, operand.Length - 2);
                content = content.Replace("\\", "\\\\").Replace("\"", "\\\"");
                return "\"" + content + "\"";
            }

            // Numeric literal (integer or decimal) → emit verbatim
            if (System.Text.RegularExpressions.Regex.IsMatch(operand, "^-?\\d+(\\.\\d+)?$"))
            {
                return operand;
            }

            // Convert array subscripts from 1‑based to 0‑based and clean names
            string expr = System.Text.RegularExpressions.Regex.Replace(operand, @"(\w+)\[(\d+)\]", m =>
            {
                string name = CleanName(m.Groups[1].Value);
                int idx = int.Parse(m.Groups[2].Value);
                return $"{name}[{idx - 1}]";
            });

            // If operand is of the form "A.B" then A may be a record or map
            // and B is the field.  Qualify accordingly.
            int dot = expr.IndexOf('.');
            if (dot > 0)
            {
                string left = expr.Substring(0, dot);
                string right = expr.Substring(dot + 1);
                left = CleanName(left);
                right = CleanName(right);

                var prog = EsfParser.CodeGen.EsfLogicToCs.program;
                bool isRecord = false;
                bool isMap = false;
                if (prog != null)
                {
                    isRecord = prog.Records.Records.Any(r => string.Equals(CleanName(r.Name), left, StringComparison.OrdinalIgnoreCase));
                    isMap = prog.Maps.Maps.Any(m => string.Equals(CleanName(m.MapName), left, StringComparison.OrdinalIgnoreCase));
                }

                if (isRecord)
                {
                    return $"Workstor.{left}.{right}";
                }
                else
                {
                    return $"{left}.{right}";
                }
            }

            // Operand without dot: could be an item, record, map or simple identifier
            {
                string name = CleanName(expr);
                var prog = EsfParser.CodeGen.EsfLogicToCs.program;
                bool isItem = false;
                bool isRecord = false;
                bool isMap = false;
                if (prog != null)
                {
                    isItem = prog.Items.Items.Any(i => string.Equals(CleanName(i.Name), name, StringComparison.OrdinalIgnoreCase));
                    isRecord = prog.Records.Records.Any(r => string.Equals(CleanName(r.Name), name, StringComparison.OrdinalIgnoreCase));
                    isMap = prog.Maps.Maps.Any(m => string.Equals(CleanName(m.MapName), name, StringComparison.OrdinalIgnoreCase));
                }
                if (isItem)
                {
                    return $"GlobalItems.{name}";
                }
                if (isRecord)
                {
                    return $"Workstor.{name}";
                }
                if (isMap)
                {
                    return name;
                }
                return name;
            }
        }

        /// <summary>
        /// Normalize an identifier for C#: replace hyphens with underscores and
        /// prefix an underscore if the first character is not a letter or '_'.
        /// </summary>
        private static string CleanName(string name)
        {
            if (string.IsNullOrEmpty(name)) return name;
            string result = name.Replace('-', '_');
            if (!char.IsLetter(result[0]) && result[0] != '_')
            {
                result = "_" + result;
            }
            return result;
        }
    }

}



