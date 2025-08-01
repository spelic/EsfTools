using EsfParser.Parser.Logic.Statements;
using System.Text;

namespace EsfParser.Parser.Logic.Parsers
{
    public class WhileStatementParser : IStatementParser
    {
        public bool CanParse(string line) =>
            line.TrimStart().StartsWith("WHILE", StringComparison.OrdinalIgnoreCase);

        public IStatement Parse(List<PreprocessedLine> lines, ref int index)
        {
            var startIndex = index;
            var conditionBuilder = new StringBuilder();

            // === 1. Accumulate multiline WHILE condition
            while (index < lines.Count)
            {
                var current = lines[index];

                // Strip inline comment for condition parsing
                var lineText = current.CleanLine;
                if (lineText.Contains("/*"))
                    lineText = lineText.Substring(0, lineText.IndexOf("/*")).Trim();

                conditionBuilder.Append(lineText + " ");

                if (lineText.TrimEnd().EndsWith(";"))
                    break;

                index++;
            }

            var stmt = new WhileStatement
            {
                OriginalCode = string.Join("\n", lines.GetRange(startIndex, index - startIndex + 1)
                                                      .Select(l => l.OriginalBlock)),
                LineNumber = lines[startIndex].StartLineNumber,
            };

            string fullCondition = conditionBuilder.ToString().Trim();
            stmt.Condition = fullCondition[5..].Trim().TrimEnd(';'); // remove "WHILE"

            index++; // move past last line of condition

            bool foundEnd = false;

            // === 2. Parse body
            while (index < lines.Count)
            {
                var current = lines[index];
                string line = current.CleanLine.Trim();

                if (line.StartsWith("END;", StringComparison.OrdinalIgnoreCase))
                {
                    if (!string.IsNullOrWhiteSpace(current.InlineComment))
                    {
                        stmt.BodyStatements.Add(new CommentStatement
                        {
                            OriginalCode = current.InlineComment,
                            Text = current.InlineComment.TrimStart('/', '*').Trim(),
                            LineNumber = current.StartLineNumber,
                        });
                    }

                    stmt.BodyStatements.Add(new EndStatement
                    {
                        OriginalCode = current.OriginalBlock,
                        LineNumber = current.StartLineNumber,
                    });

                    index++;
                    foundEnd = true;
                    break;
                }

                var parsed = VisualAgeLogicParser.TryParse(lines, ref index);
                stmt.BodyStatements.AddRange(parsed);
                index++;
            }

            // === 3. Auto-insert END if missing
            if (!foundEnd)
            {
                stmt.BodyStatements.Add(new EndStatement
                {
                    OriginalCode = "// [auto-inserted] END;",
                    LineNumber = lines.Last().EndLineNumber + 1,
                });
            }

            return stmt;
        }
    }
}
