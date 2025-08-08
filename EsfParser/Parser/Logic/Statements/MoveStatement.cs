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

            string srcExpr = CSharpUtils.ConvertOperand(Source);
            string dstExpr = CSharpUtils.ConvertOperand(Destination);

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
                    string cleanSrc = CSharpUtils.CleanName(Source);
                    string cleanDst = CSharpUtils.CleanName(Destination);
                    bool srcIsRecord = prog.Records.Records.Any(r => string.Equals(CSharpUtils.CleanName(r.Name), cleanSrc, StringComparison.OrdinalIgnoreCase));
                    bool dstIsRecord = prog.Records.Records.Any(r => string.Equals(CSharpUtils.CleanName(r.Name), cleanDst, StringComparison.OrdinalIgnoreCase));
                    bool srcIsMap = prog.Maps.Maps.Any(m => string.Equals(CSharpUtils.CleanName(m.MapName), cleanSrc, StringComparison.OrdinalIgnoreCase));
                    bool dstIsMap = prog.Maps.Maps.Any(m => string.Equals(CSharpUtils.CleanName(m.MapName), cleanDst, StringComparison.OrdinalIgnoreCase));
                    if ((srcIsRecord || srcIsMap) && (dstIsRecord || dstIsMap))
                    {
                        return $"{dstExpr}.CopyFrom({srcExpr});";
                    }
                }
            }

            // Default: simple assignment for value/field copies
            return $"{dstExpr} = {srcExpr};  // Org: {OriginalCode}";
        }

       

        
    }

}



