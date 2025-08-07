
namespace EsfParser.Parser.Logic.Translators
{
    internal abstract record Expr;

    internal record NumberExpr(string Value)              : Expr;
    internal record IdentifierExpr(string Name)           : Expr;
    internal record UnaryExpr(TokenType Op, Expr Child)   : Expr;
    internal record BinaryExpr(TokenType Op, Expr Left, Expr Right) : Expr;
    internal record CallExpr(string Name, System.Collections.Generic.List<Expr> Args) : Expr;
    internal record RoundedExpr(Expr Inner)               : Expr;
}
