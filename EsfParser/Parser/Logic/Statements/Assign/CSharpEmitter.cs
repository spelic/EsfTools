
using System.Collections.Generic;
using System;

namespace EsfParser.Parser.Logic.Translators
{
    internal static class CSharpEmitter
    {
        public static string Emit(Expr e) => e switch
        {
            NumberExpr n      => n.Value,
            IdentifierExpr id => id.Name,
            UnaryExpr u       => TokenText(u.Op) + Emit(u.Child),
            BinaryExpr b      => $"{Emit(b.Left)} {TokenText(b.Op)} {Emit(b.Right)}",
            CallExpr c        => $"{c.Name}({string.Join(", ", c.Args.ConvertAll(Emit))})",
            RoundedExpr r     => $"Math.Round({Emit(r.Inner)})",
            _                 => throw new InvalidOperationException("Unknown Expr type")
        };

        private static string TokenText(TokenType t) => t switch
        {
            TokenType.Plus  => "+",
            TokenType.Minus => "-",
            TokenType.Star  => "*",
            TokenType.Slash => "/",
            TokenType.Percent => "%",
            _ => throw new ArgumentOutOfRangeException(nameof(t))
        };
    }
}
