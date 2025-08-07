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
                var stmts = TryParse(_lines, ref _currentIndex, 1);
                result.AddRange(stmts);
            }
            return result;
        }

        public static List<IStatement> TryParse(List<PreprocessedLine> lines, ref int index, int currentLevel)
        {
            if (index >= lines.Count) return new List<IStatement>();

            var results = new List<IStatement>();
            PreprocessedLine line = lines[index];
            string clean = line.CleanLine.Trim();
            int lineNumber = line.StartLineNumber;

            if (string.IsNullOrWhiteSpace(clean))
            {
                // Add comments if any
                if (!string.IsNullOrWhiteSpace(line.InlineComment))
                {
                    results.Add(new CommentStatement
                    {
                        OriginalCode = line.OriginalBlock,
                        Text = line.InlineComment.TrimStart('/', '*').Trim(),
                        LineNumber = line.StartLineNumber,
                        NestingLevel = currentLevel,
                    });
                }
                if (!string.IsNullOrWhiteSpace(line.FullLineComment))
                {
                    results.Add(new CommentStatement
                    {
                        OriginalCode = line.OriginalBlock,
                        Text = line.FullLineComment.TrimStart('/', '*').Trim(),
                        LineNumber = line.StartLineNumber,
                        NestingLevel = currentLevel,
                    });
                }
                index++;  // Advance for empty/comment-only
                return results;
            }

            foreach (var parser in _parsers)
            {
                if (parser.CanParse(line.CleanLine))
                {
                    var statement = parser.Parse(lines, ref index, currentLevel);
                    statement.LineNumber = line.StartLineNumber;
                    statement.NestingLevel = currentLevel;

                    results.Add(statement);  // Add statement first

                    if (!string.IsNullOrWhiteSpace(line.InlineComment) && statement.Type != StatementType.Comment)
                    {
                        results.Add(new CommentStatement  // Then comment
                        {
                            OriginalCode = line.InlineComment,
                            Text = line.InlineComment.TrimStart('/', '*').Trim(),
                            LineNumber = line.StartLineNumber,
                            NestingLevel = currentLevel,
                        });
                    }

                    index++;  // Advance if parser didn't (single-line case)
                 
                    return results;
                }
            }

            // Unknown or comment-only
            results.Add(new UnknownStatement
            {
                OriginalCode = line.OriginalBlock,
                LineNumber = lineNumber,
                NestingLevel = currentLevel,
            });

            if (!string.IsNullOrWhiteSpace(line.InlineComment))
            {
                results.Add(new CommentStatement  // Comment after unknown
                {
                    OriginalCode = line.InlineComment,
                    Text = line.InlineComment.TrimStart('/', '*').Trim(),
                    LineNumber = lineNumber,
                    NestingLevel = currentLevel,
                });
            }     
            index++;  // Advance for unknown
            return results;
        }
    }
}