
using System.Collections.Generic;
using System;

namespace EsfParser.Parser.Logic.Translators
{
    internal sealed class Parser
    {
        private readonly Lexer _lex;
        private Token _current;

        public Parser(string src)
        {
            _lex     = new Lexer(src);
            _current = _lex.Next();
        }

        private Token Consume()
        {
            var t    = _current;
            _current = _lex.Next();
            return t;
        }

        private void Expect(TokenType type)
        {
            if (_current.Type != type)
                throw new FormatException($"Expected {type} but found '{_current.Text}'.");
        }

        private static int Prec(TokenType t) => t switch
        {
            TokenType.Plus  or TokenType.Minus              => 1,
            TokenType.Star  or TokenType.Slash or TokenType.Percent => 2,
            _ => 0
        };

        public Expr ParseExpression(int minPrec = 0)
        {
            Expr left = ParsePrimary();

            while (true)
            {
                int prec = Prec(_current.Type);
                if (prec < minPrec) break;

                TokenType op = _current.Type;
                Consume();
                Expr right = ParseExpression(prec + 1);
                left = new BinaryExpr(op, left, right);
            }

            return left;
        }

        private Expr ParsePrimary()
        {
            Token tok = _current;
            Consume();

            // Unary
            if (tok.Type == TokenType.Plus || tok.Type == TokenType.Minus)
            {
                Expr operand = ParseExpression(3);
                return new UnaryExpr(tok.Type, operand);
            }

            // Number
            if (tok.Type == TokenType.Number)
                return new NumberExpr(tok.Text);

            // Identifier or call
            if (tok.Type == TokenType.Identifier)
            {
                if (_current.Type == TokenType.LParen)
                {
                    Consume(); // (
                    var args = new List<Expr>();
                    if (_current.Type != TokenType.RParen)
                    {
                        args.Add(ParseExpression());
                        while (_current.Type == TokenType.Comma)
                        {
                            Consume();
                            args.Add(ParseExpression());
                        }
                    }
                    Expect(TokenType.RParen);
                    Consume();
                    return new CallExpr(tok.Text, args);
                }
                return new IdentifierExpr(tok.Text);
            }

            // Parenthesised
            if (tok.Type == TokenType.LParen)
            {
                Expr inside = ParseExpression();
                Expect(TokenType.RParen);
                Consume();
                return inside;
            }

            throw new FormatException($"Unexpected token '{tok.Text}'.");
        }
    }
}
