using EsfParser.Parser.Logic.Parsers;
using EsfParser.Parser.Logic.Statements;
using System.Text;

namespace EsfParser.Parser.Logic
{
    public class VisualAgeLogicParser
    {
        private readonly List<PreprocessedLine> _lines;
        private int _currentIndex;
        private readonly string _origin;

        private static readonly List<IStatementParser> _parsers = new()
        {
            new IfStatementParser(),
            new WhileStatementParser(),
            new MoveStatementParser(),
            new MoveAStatementParser(),
            new RetrStatementParser(),
            new CallStatementParser(),
            new ImplicitCallStatementParser(),
            new AssignStatementParser(),
            new TestStatementParser(),
            new DxfrStatementParser(),
            new CommentStatementParser(),
            new SetStatementParser(),
            new SystemFunctionStatementParser()
        };

        public VisualAgeLogicParser(List<PreprocessedLine> preprocessedLines)
        {
            _lines = preprocessedLines;
            _currentIndex = 0;
        }

        public List<IStatement> Parse()
        {
            var result = new List<IStatement>();
            while (_currentIndex < _lines.Count)
            {
                var stmts = TryParse(_lines, ref _currentIndex);
                _currentIndex++;
                result.AddRange(stmts);
            }
            return result;
        }

        public static List<IStatement> TryParse(List<PreprocessedLine> lines, ref int index)
        {
            var results = new List<IStatement>();

            PreprocessedLine line = lines[index];

            string clean = line.CleanLine.Trim();
            int lineNumber = line.StartLineNumber;

            if (string.IsNullOrWhiteSpace(clean))
            {
                // check if comment in line
                if (!string.IsNullOrWhiteSpace(line.InlineComment))
                {
                    results.Add(new CommentStatement
                    {
                        OriginalCode = line.OriginalBlock,
                        Text = line.InlineComment,
                        LineNumber = line.StartLineNumber,
                    });
                }
                if (!string.IsNullOrWhiteSpace(line.FullLineComment))
                {
                    results.Add(new CommentStatement
                    {
                        OriginalCode = line.OriginalBlock,
                        Text = line.FullLineComment,
                        LineNumber = line.StartLineNumber,
                    });
                }
                return results;
            }

            foreach (var parser in _parsers)
            {
                if (parser.CanParse(line.CleanLine))
                {
                    var statement = parser.Parse(lines, ref index);
                    statement.LineNumber = line.StartLineNumber;

                    if (!string.IsNullOrWhiteSpace(line.InlineComment) && statement.Type != StatementType.Comment)
                    {
                        results.Add(new CommentStatement
                        {
                            OriginalCode = line.InlineComment,
                            Text = line.InlineComment.TrimStart('/', '*').Trim(),
                            LineNumber = line.StartLineNumber,
                        });
                    }

                    results.Add(statement);
                    return results;
                }
            }

            if (!string.IsNullOrWhiteSpace(line.InlineComment))
            {
                results.Add(new CommentStatement
                {
                    OriginalCode = line.InlineComment,
                    Text = line.InlineComment.TrimStart('/', '*').Trim(),
                    LineNumber = lineNumber,
                });
            }

            results.Add(new UnknownStatement
            {
                OriginalCode = line.OriginalBlock,
                LineNumber = lineNumber,
            });
            return results;
        }
    }
}