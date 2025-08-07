
using System;

namespace EsfParser.Parser.Logic.Translators
{
    internal sealed class Lexer
    {
        private readonly string _src;
        private int _pos;

        public Lexer(string src) => _src = src;

        public Token Next()
        {
            SkipWhite();
            if (_pos >= _src.Length) return new(TokenType.EndOfInput, "");

            // Two‑char remainder operator
            if (Peek("//"))
            {
                _pos += 2;
                return new(TokenType.Percent, "%");
            }

            char c = _src[_pos];

            // Single‑char tokens
            switch (c)
            {
                case '+': _pos++; return new(TokenType.Plus, "+");
                case '-': _pos++; return new(TokenType.Minus, "-");
                case '*': _pos++; return new(TokenType.Star, "*");
                case '/': _pos++; return new(TokenType.Slash, "/");
                case '(': _pos++; return new(TokenType.LParen, "(");
                case ')': _pos++; return new(TokenType.RParen, ")");
                case ',': _pos++; return new(TokenType.Comma, ",");
            }

            // Identifier or dotted identifier
            if (char.IsLetter(c) || c == '_' || c == '#')
            {
                int start = _pos;
                while (_pos < _src.Length &&
                      (char.IsLetterOrDigit(_src[_pos]) || _src[_pos] == '_' || _src[_pos] == '.'))
                    _pos++;
                return new(TokenType.Identifier, _src[start.._pos]);
            }

            // Number
            if (char.IsDigit(c) || (c == '.' && _pos + 1 < _src.Length && char.IsDigit(_src[_pos + 1])))
            {
                int start = _pos;
                bool seenDot = false;
                while (_pos < _src.Length)
                {
                    char d = _src[_pos];
                    if (char.IsDigit(d)) { _pos++; continue; }
                    if (d == '.' && !seenDot) { seenDot = true; _pos++; continue; }
                    break;
                }
                // Exponent
                if (_pos < _src.Length && (_src[_pos] == 'E' || _src[_pos] == 'e'))
                {
                    int save = _pos;
                    _pos++;
                    if (_pos < _src.Length && (_src[_pos] == '+' || _src[_pos] == '-')) _pos++;
                    if (_pos < _src.Length && char.IsDigit(_src[_pos]))
                        while (_pos < _src.Length && char.IsDigit(_src[_pos])) _pos++;
                    else
                        _pos = save; // Roll back
                }
                return new(TokenType.Number, _src[start.._pos]);
            }

            throw new FormatException($"Unexpected character '{c}' at position {_pos}.");
        }

        private void SkipWhite()
        {
            while (_pos < _src.Length && char.IsWhiteSpace(_src[_pos])) _pos++;
        }

        private bool Peek(string literal)
        {
            return _src.AsSpan(_pos).StartsWith(literal, StringComparison.Ordinal);
        }
    }
}
