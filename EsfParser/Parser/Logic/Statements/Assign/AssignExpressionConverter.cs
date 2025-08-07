
using System;
using System.Text.RegularExpressions;

namespace EsfParser.Parser.Logic.Translators
{
    /// <summary>
    /// Converts the RHS of a VisualAge ASSIGN statement to C#.
    /// </summary>
    internal static class AssignExpressionConverter
    {
        private static readonly Regex RoundFlag = new(@"\(\s*R\s*$", RegexOptions.IgnoreCase);

        /// <summary>
        /// Returns emitted C# RHS as well as a flag indicating it consists of a single operand (MOVE case).
        /// </summary>
        /// <exception cref="FormatException">Thrown if expression is invalid per VAGen rules.</exception>
        public static (string CSharpRhs, bool SingleOperand) Convert(string rhsVagen)
        {
            string trimmed = rhsVagen.Trim();

            // 1. trailing (R
            bool round = false;
            if (RoundFlag.IsMatch(trimmed))
            {
                round   = true;
                trimmed = RoundFlag.Replace(trimmed, "").TrimEnd();
            }

            // 2. remainder cannot mix with rounding
            if (round && trimmed.Contains("//", StringComparison.Ordinal))
                throw new FormatException("Rounding option cannot be combined with remainder operator '//'.");

            // 3. parse
            Parser p    = new(trimmed);
            Expr expr   = p.ParseExpression();

            // 4. forbid remainder with other operators
            if (ContainsOperator(expr, TokenType.Percent) && ContainsOtherBinary(expr))
                throw new FormatException("The remainder operator '//' cannot be combined with other arithmetic operators.");

            // 5. wrap rounding
            if (round) expr = new RoundedExpr(expr);

            string emitted      = CSharpEmitter.Emit(expr);
            bool   singleOpFlag = expr is IdentifierExpr or NumberExpr or CallExpr;

            return (emitted, singleOpFlag);
        }

        private static bool ContainsOperator(Expr e, TokenType t) =>
            e is BinaryExpr b
                ? b.Op == t || ContainsOperator(b.Left, t) || ContainsOperator(b.Right, t)
                : false;

        private static bool ContainsOtherBinary(Expr e) =>
            e is BinaryExpr b
                ? b.Op != TokenType.Percent || ContainsOtherBinary(b.Left) || ContainsOtherBinary(b.Right)
                : false;
    }
}
