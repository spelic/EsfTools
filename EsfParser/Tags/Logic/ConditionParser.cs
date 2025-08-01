using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace EsfParser.Tags.Logic
{
    #region Tokens & Lexer

    public enum TokenType
    {
        Identifier,
        Number,
        StringLiteral,
        EQ,    // = or IS or EQ
        NE,    // =/= or NE
        GT,    // GT or >
        LT,    // LT or <
        GE,    // GE or >=
        LE,    // LE or <=
        AND,
        OR,
        NOT,
        LParen,
        RParen,
        EndOfFile
    }

    public class Token
    {
        public TokenType Type { get; }
        public string Text { get; }
        public Token(TokenType t, string txt) { Type = t; Text = txt; }
    }

    public static class Lexer
    {
        // matches:
        //  - multi‐char ops (=>, >=, <=, =/=)
        //  - word‐ops (EQ, NE, GT, LT, GE, LE, IS)
        //  - logical (AND, OR, NOT)
        //  - parens, literals, numbers
        //  - identifiers with optional dot‐path and subscripts:  Foo.Bar[123]
        private static readonly Regex _tokenRx = new Regex(@"
            (?ix)                          # ignore case, verbose
            \s*                            # leading whitespace
            (
                =>|>=|<=|=/=               # multi-char ops
              | EQ\b|NE\b|GT\b|LT\b|GE\b|LE\b|IS\b    # word ops
              | AND\b|OR\b|NOT\b           # logical words
              | \(|\)                      # parens
              | '[^']*'                    # single-quoted literals
              | ""[^""]*""                 # double-quoted literals
              | \d+(\.\d+)?                # numbers
              | \w+(?:\.\w+)*(?:\[[^\]]+\])*  # identifiers w/ . and [sub]
              | [=<>]                      # single-char ops
            )",
            RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace
        );

        public static List<Token> Tokenize(string src)
        {
            var tokens = new List<Token>();
            foreach (Match m in _tokenRx.Matches(src))
            {
                var txt = m.Groups[1].Value;
                tokens.Add(txt switch
                {
                    "(" => new Token(TokenType.LParen, txt),
                    ")" => new Token(TokenType.RParen, txt),
                    "AND" => new Token(TokenType.AND, txt),
                    "OR" => new Token(TokenType.OR, txt),
                    "NOT" => new Token(TokenType.NOT, txt),
                    // comparisons:
                    "=/=" or "NE" => new Token(TokenType.NE, txt),
                    "GT" or ">" => new Token(TokenType.GT, txt),
                    "LT" or "<" => new Token(TokenType.LT, txt),
                    "GE" or ">=" or "=>" => new Token(TokenType.GE, txt),
                    "LE" or "<=" or "=<" => new Token(TokenType.LE, txt),
                    "IS" or "=" or "EQ" => new Token(TokenType.EQ, txt),
                    var s when Regex.IsMatch(s, @"^\d+(\.\d+)?$")
                                        => new Token(TokenType.Number, s),
                    var s when (s.StartsWith("'") && s.EndsWith("'")) ||
                                (s.StartsWith("\"") && s.EndsWith("\""))
                                        => new Token(TokenType.StringLiteral, s),
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

    public class BinaryExpr : Expr
    {
        public Expr Left;
        public TokenType Op;
        public Expr Right;
        public BinaryExpr(Expr l, TokenType op, Expr r)
        { Left = l; Op = op; Right = r; }
    }

    public class UnaryExpr : Expr
    {
        public TokenType Op;
        public Expr Inner;
        public UnaryExpr(TokenType op, Expr inner)
        { Op = op; Inner = inner; }
    }

    public class LiteralExpr : Expr
    {
        public string Value;
        public LiteralExpr(string v) { Value = v; }
    }

    public class IdentifierExpr : Expr
    {
        public string Name;
        public IdentifierExpr(string n) { Name = n; }
    }

    // Add a new AST node for IS ERR
    public class IsErrExpr : Expr
    {
        public Expr Target;
        public IsErrExpr(Expr target) { Target = target; }
    }

    #endregion

    #region Parser

    public class Parser
    {
        private readonly List<Token> _tokens;
        private int _pos;

        private Token Current => _tokens[_pos];

        public Parser(List<Token> tokens) => _tokens = tokens;

        private Token Eat(TokenType want)
        {
            if (Current.Type != want)
                throw new InvalidOperationException($"Expected {want}, got {Current.Type}");
            return _tokens[_pos++];
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

            // Handle "IS ERR" (or "IS NOT ERR")
            if (Current.Type == TokenType.EQ && PeekIsErr())
            {
                Eat(TokenType.EQ); // Eat IS/EQ/ =
                bool isNot = false;
                if (Current.Type == TokenType.NOT)
                {
                    Eat(TokenType.NOT);
                    isNot = true;
                }
                if (Current.Type == TokenType.Identifier && Current.Text.Equals("ERR", StringComparison.OrdinalIgnoreCase))
                {
                    Eat(TokenType.Identifier);
                    var isErrExpr = new IsErrExpr(left);
                    return isNot ? new UnaryExpr(TokenType.NOT, isErrExpr) : isErrExpr;
                }
                // fallback: treat as normal comparison if not followed by ERR
                _pos--; // rewind to before IS/EQ
            }

            if (new[] {
                    TokenType.EQ, TokenType.NE,
                    TokenType.GT, TokenType.LT,
                    TokenType.GE, TokenType.LE
                 }.Contains(Current.Type))
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

        // Helper to peek for "ERR" after IS/EQ/=
        private bool PeekIsErr()
        {
            int peek = _pos + 1;
            if (peek < _tokens.Count && _tokens[peek].Type == TokenType.NOT)
                peek++;
            return peek < _tokens.Count &&
                   _tokens[peek].Type == TokenType.Identifier &&
                   _tokens[peek].Text.Equals("ERR", StringComparison.OrdinalIgnoreCase);
        }
    }

    #endregion

    #region Code Generator

    // Must implement IConditionConverter
    public class ConditionCodeGenerator : IConditionConverter
    {
        private readonly IExpressionConverter _exprConv;
        private readonly ITypeResolver _typeResolver;

        public ConditionCodeGenerator(IExpressionConverter exprConv,
                                      ITypeResolver typeResolver)
        {
            _exprConv = exprConv;
            _typeResolver = typeResolver;
        }

        public string Convert(string cond)
        {
            // 1) Tokenize
            var tokens = Lexer.Tokenize(cond);
            // 2) Parse to AST
            var ast = new Parser(tokens).ParseExpression();
            // 3) Generate C#
            return GenerateInternal(ast);
        }

        private string GenerateInternal(Expr node)
        {
            switch (node)
            {
                case LiteralExpr lit:
                    // normalize quotes
                    return _exprConv.Convert(lit.Value);

                case IdentifierExpr id:
                    return _exprConv.Convert(id.Name);

                case UnaryExpr u when u.Op == TokenType.NOT:
                    return "!" + GenerateInternal(u.Inner);

                case BinaryExpr b:
                    var left = GenerateInternal(b.Left);
                    var right = GenerateInternal(b.Right);
                    // if it's a >0 on a string field → translate to != ""
                    if (b.Op == TokenType.GT &&
                        _typeResolver.GetTypeOf(((IdentifierExpr)b.Left).Name) == "string" &&
                        double.TryParse(((LiteralExpr)b.Right).Value.Trim('\''), out _))
                    {
                        // e.g.  > 0 on string → != ""
                        return $"({left} != \"\")";
                    }

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

                // ... existing cases ...
                case IsErrExpr iserr:
                    return $"{GenerateInternal(iserr.Target)}.IsErr";

                default:
                    throw new InvalidOperationException("Unknown AST node");
            }
        }
    }

    #endregion
}
