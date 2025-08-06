using EsfParser.Parser.Logic.Statements;
using System.Text;
using System.Text.RegularExpressions;

namespace EsfParser.Parser.Logic.Parsers
{
    public class WhileStatementParser : IStatementParser
    {
        public bool CanParse(string line) =>
            line.TrimStart().StartsWith("WHILE", StringComparison.OrdinalIgnoreCase);

        public IStatement Parse(List<PreprocessedLine> lines, ref int index, int currentLevel = 0)
        {
            var overallStartIndex = index;
            var conditionBuilder = new StringBuilder();
            var conditionStartIndex = index;

            // Accumulate multiline WHILE condition without extra spaces
            while (index < lines.Count)
            {
                var current = lines[index];
                var lineText = current.CleanLine;
                if (lineText.Contains("/*"))
                    lineText = lineText.Substring(0, lineText.IndexOf("/*")).Trim();

                conditionBuilder.Append(lineText.Trim()).Append(' ');

                if (lineText.Trim().EndsWith(";"))
                    break;

                index++;
            }

            var stmt = new WhileStatement
            {
                Condition = Regex.Replace(conditionBuilder.ToString().Trim().Substring(5).Trim().TrimEnd(';'), @"\s+", " "),  // Normalize spaces after removing "WHILE"
                LineNumber = lines[conditionStartIndex].StartLineNumber
            };

            index++;  // Move past condition

            // Parse body
            while (index < lines.Count)
            {
                var current = lines[index];
                string line = current.CleanLine.Trim();

                if (line.StartsWith("END", StringComparison.OrdinalIgnoreCase))
                {
                    if (!string.IsNullOrWhiteSpace(current.InlineComment))
                    {
                        stmt.BodyStatements.Add(new CommentStatement
                        {
                            OriginalCode = current.InlineComment,
                            Text = current.InlineComment.TrimStart('/', '*').Trim(),
                            LineNumber = current.StartLineNumber,
                            NestingLevel = currentLevel,
                        });
                    }

                    stmt.BodyStatements.Add(new EndStatement
                    {
                        OriginalCode = current.OriginalBlock,
                        LineNumber = current.StartLineNumber,
                        InlineComent = "END_WHILE",
                        NestingLevel = currentLevel,
                    });
                    //index++;
                    break;
                }

                // Parse sub-statement (advances index internally)
                var parsed = VisualAgeLogicParser.TryParse(lines, ref index, currentLevel + 1);
                stmt.BodyStatements.AddRange(parsed);
            }

            // Set full OriginalCode including body and END;
            stmt.OriginalCode = lines[overallStartIndex].OriginalBlock;

            return stmt;
        }
    }
}