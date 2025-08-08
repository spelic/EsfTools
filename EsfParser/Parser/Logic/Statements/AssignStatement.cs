// AssignStatement.cs ───────────────────────────────────────────────────────
using EsfParser.CodeGen;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace EsfParser.Parser.Logic.Statements
{
    public class AssignStatement : IStatement
    {
        public StatementType Type => StatementType.Assign;

        public string OriginalCode { get; set; } = string.Empty;
        public string Left { get; set; } = string.Empty;
        public string Right { get; set; } = string.Empty;
        public int LineNumber { get; set; }
        public int NestingLevel { get; set; }


        public string ToCSharp()
        {
            // 1. Guard-clauses
            if (string.IsNullOrWhiteSpace(Left) || string.IsNullOrWhiteSpace(Right))
                return $"// ⚠ malformed assignment: {OriginalCode}";

            // 2. Detect “whole-map on both sides”
            bool IsWholeMap(string op)
            {
                // no sub-field, no sub-script → single identifier
                if (op.Contains('.') || op.Contains('[')) return false;

                string clean = CSharpUtils.CleanName(op);
                return CSharpUtils.Program is not null &&
                       CSharpUtils.Program.Maps.Maps
                           .Any(m => CSharpUtils.CleanName(m.MapName)
                                .Equals(clean, StringComparison.OrdinalIgnoreCase));
            }

            if (IsWholeMap(Left) && IsWholeMap(Right))
            {
                // source.CopyTo(destination);
                string src = CSharpUtils.ConvertOperand(Right);
                string dst = CSharpUtils.ConvertOperand(Left);
                return $"{dst}.CopyFrom({src});";
            }

            // 3. Ordinary scalar/field assignment
            string lhs = CSharpUtils.ConvertOperand(Left);
            string rhs = CSharpUtils.ConvertOperand(Right);
            return $"{lhs} = {rhs};";
        }

    }
}
