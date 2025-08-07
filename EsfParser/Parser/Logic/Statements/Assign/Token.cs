
namespace EsfParser.Parser.Logic.Translators
{
    internal enum TokenType
    {
        Identifier,
        Number,
        Plus, Minus,
        Star, Slash, Percent,
        LParen, RParen,
        Comma,
        EndOfInput
    }

    internal readonly record struct Token(TokenType Type, string Text);
}
