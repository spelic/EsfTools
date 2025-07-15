using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace EsfCore.Tags.Logic
{
    #region Tokens & Lexer

    enum TokenType
    {
        Identifier,
        Number,
        StringLiteral,
        EQ, NE, GT, LT, GE, LE,
        IS,
        AND, OR, NOT,
        LParen, RParen,
        EndOfFile
    }

    class Token
    {
        public TokenType Type { get; }
        public string Text { get; }
        public Token(TokenType t, string txt) { Type = t; Text = txt; }
    }

    static class Lexer
    {
        private static readonly Regex _tokenRx = new Regex(@"
    (?ix)                  # ignore case & allow comments/spaces
    \s*                    # skip any leading whitespace
    (                      # begin token alternatives
       => | >= | <= | =/=   # multi-char operators
     | EQ\b | NE\b | GT\b | LT\b | GE\b | LE\b | IS\b      # word-operators
     | AND\b | OR\b | NOT\b                               # logical words
     | \(|\)                                              # parens
     | '[^']*'                                            # single-quoted literal
     | ""[^""]*""                                         # double-quoted literal
     | \d+(\.\d+)?                                        # numbers
     | \w+                                                # identifiers
     | [=<>]                                              # single-char ops
    )", RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace);

        public static List<Token> Tokenize(string src)
        {
            var matches = _tokenRx.Matches(src);
            var tokens = new List<Token>();
            foreach (Match m in matches)
            {
                var txt = m.Groups[1].Value;
                tokens.Add(txt switch
                {
                    "(" => new Token(TokenType.LParen, txt),
                    ")" => new Token(TokenType.RParen, txt),
                    "=" or "EQ" or "==" or "IS" => new Token(TokenType.EQ, txt),
                    "=/=" or "NE" or "<>" => new Token(TokenType.NE, txt),
                    "GT" or ">" => new Token(TokenType.GT, txt),
                    "LT" or "<" => new Token(TokenType.LT, txt),
                    "GE" or ">=" or "=>" => new Token(TokenType.GE, txt),
                    "LE" or "<=" or "=<" => new Token(TokenType.LE, txt),
                    var s when Regex.IsMatch(s, "^\\d+(?:\\.\\d+)?$")
                        => new Token(TokenType.Number, s),
                    var s when (s.StartsWith("'") && s.EndsWith("'")) || (s.StartsWith("\"") && s.EndsWith("\""))
                        => new Token(TokenType.StringLiteral, s),
                    "AND" => new Token(TokenType.AND, txt),
                    "OR" => new Token(TokenType.OR, txt),
                    "NOT" => new Token(TokenType.NOT, txt),
                    _ => new Token(TokenType.Identifier, txt)
                });
            }
            tokens.Add(new Token(TokenType.EndOfFile, ""));
            return tokens;
        }
    }

    #endregion

    #region AST Nodes

    public abstract class Expr { }
    class BinaryExpr : Expr
    {
        public Expr Left;
        public TokenType Op;
        public Expr Right;
        public BinaryExpr(Expr l, TokenType op, Expr r) { Left = l; Op = op; Right = r; }
    }
    class UnaryExpr : Expr
    {
        public TokenType Op;
        public Expr Inner;
        public UnaryExpr(TokenType op, Expr inner) { Op = op; Inner = inner; }
    }
    class LiteralExpr : Expr
    {
        public string Value;
        public LiteralExpr(string v) { Value = v; }
    }
    class IdentifierExpr : Expr
    {
        public string Name;
        public IdentifierExpr(string n) { Name = n; }
    }

    #endregion

    #region Parser

    class Parser
    {
        private readonly List<Token> _tokens;
        private int _pos = 0;
        private Token Current => _tokens[_pos];

        public Parser(List<Token> tokens) => _tokens = tokens;

        private Token Eat(TokenType want)
        {
            if (Current.Type == want) return _tokens[_pos++];
            throw new InvalidOperationException($"Expected {want}, got {Current.Type}");
        }

        public Expr ParseExpression() => ParseOr();

        private Expr ParseOr()
        {
            var expr = ParseAnd();
            while (Current.Type == TokenType.OR)
            {
                Eat(TokenType.OR);
                var rhs = ParseAnd();
                expr = new BinaryExpr(expr, TokenType.OR, rhs);
            }
            return expr;
        }

        private Expr ParseAnd()
        {
            var expr = ParseNot();
            while (Current.Type == TokenType.AND)
            {
                Eat(TokenType.AND);
                var rhs = ParseNot();
                expr = new BinaryExpr(expr, TokenType.AND, rhs);
            }
            return expr;
        }

        private Expr ParseNot()
        {
            if (Current.Type == TokenType.NOT)
            {
                Eat(TokenType.NOT);
                var inner = ParseNot();
                return new UnaryExpr(TokenType.NOT, inner);
            }
            return ParseComparison();
        }

        private Expr ParseComparison()
        {
            var left = ParsePrimary();
            if (new[] { TokenType.EQ, TokenType.NE, TokenType.GT, TokenType.LT, TokenType.GE, TokenType.LE }
                    .Contains(Current.Type))
            {
                var op = Current.Type;
                _pos++;
                var right = ParsePrimary();
                return new BinaryExpr(left, op, right);
            }
            return left;
        }

        private Expr ParsePrimary()
        {
            switch (Current.Type)
            {
                case TokenType.LParen:
                    Eat(TokenType.LParen);
                    var e = ParseExpression();
                    Eat(TokenType.RParen);
                    return e;
                case TokenType.StringLiteral:
                    var lit = Current.Text;
                    Eat(TokenType.StringLiteral);
                    return new LiteralExpr(lit);
                case TokenType.Number:
                    var num = Current.Text;
                    Eat(TokenType.Number);
                    return new LiteralExpr(num);
                case TokenType.Identifier:
                    var id = Current.Text;
                    Eat(TokenType.Identifier);
                    return new IdentifierExpr(id);
                default:
                    throw new InvalidOperationException($"Unexpected token {Current.Type}");
            }
        }
    }

    #endregion

    #region Code Generator

    public class ConditionCodeGenerator
    {
        private readonly IExpressionConverter _exprConv;
        public ConditionCodeGenerator(IExpressionConverter exprConv)
            => _exprConv = exprConv;

        public string Generate(Expr ast)
            => GenerateInternal(ast);

        private string GenerateInternal(Expr node)
        {
            switch (node)
            {
                case LiteralExpr lit:
                    // ensure double-quoted
                    return _exprConv.Convert(lit.Value);

                case IdentifierExpr id:
                    return _exprConv.Convert(id.Name);

                case UnaryExpr u when u.Op == TokenType.NOT:
                    return "!" + GenerateInternal(u.Inner);

                case BinaryExpr b:
                    {
                        var left = GenerateInternal(b.Left);
                        var right = GenerateInternal(b.Right);
                        var op = b.Op switch
                        {
                            TokenType.EQ => "==",
                            TokenType.NE => "!=",
                            TokenType.GT => ">",
                            TokenType.LT => "<",
                            TokenType.GE => ">=",
                            TokenType.LE => "<=",
                            TokenType.AND => "&&",
                            TokenType.OR => "||",
                            _ => throw new InvalidOperationException()
                        };
                        return $"({left} {op} {right})";
                    }
                default:
                    throw new InvalidOperationException("Unknown AST node");
            }
        }
    }

    #endregion
}
