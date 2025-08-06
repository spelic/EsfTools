using EsfParser.Parser.Logic.Statements;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace EsfParser.Parser.Logic.Parsers
{
    public class IfStatementParser : IStatementParser
    {
        public bool CanParse(string line) => line.TrimStart().StartsWith("IF", StringComparison.OrdinalIgnoreCase);
        public IStatement Parse(List<PreprocessedLine> lines, ref int index, int currentLevel)
        {
            var overallStartIndex = index;
            var conditionBuilder = new StringBuilder();
            var conditionStartIndex = index;

            // Accumulate multiline IF condition without extra spaces
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

            var ifStmt = new IfStatement
            {
                Condition = Regex.Replace(conditionBuilder.ToString().Trim()[2..].Trim().TrimEnd(';'), @"\s+", " "),  // Normalize spaces
                LineNumber = lines[conditionStartIndex].StartLineNumber
            };

            index++;  // Move past condition

            bool isTrue = true;

            while (index < lines.Count)
            {
                var current = lines[index];
                var text = current.CleanLine.Trim();
                if (text.StartsWith("ELSE", StringComparison.OrdinalIgnoreCase))
                {
                    // Add comment to ElseStatements if present
                    if (!string.IsNullOrWhiteSpace(current.InlineComment))
                        ifStmt.ElseStatements.Add(new CommentStatement { OriginalCode = current.InlineComment, Text = current.InlineComment.TrimStart('/', '*').Trim(), LineNumber = current.StartLineNumber });
                    
                    ifStmt.ElseStatements.Add(new ElseStatement { OriginalCode = current.OriginalBlock, LineNumber = current.StartLineNumber, NestingLevel = currentLevel, InlineComent = "ELSE"  });

                    isTrue = false;
                    index++;
                    continue;
                }
                else if (text.StartsWith("END", StringComparison.OrdinalIgnoreCase))
                {
                    // Add comment to current branch
                    if (!string.IsNullOrWhiteSpace(current.InlineComment))
                    {
                        var comment = new CommentStatement { OriginalCode = current.InlineComment, Text = current.InlineComment.TrimStart('/', '*').Trim(), LineNumber = current.StartLineNumber, NestingLevel = currentLevel };
                        if (isTrue)
                        {
                            ifStmt.TrueStatements.Add(comment);
                        }
                        else
                        {
                            ifStmt.ElseStatements.Add(comment);
                        }
                    }

                    if (isTrue)
                    {
                        ifStmt.TrueStatements.Add(new EndStatement { OriginalCode = current.OriginalBlock, LineNumber = current.StartLineNumber, NestingLevel = currentLevel, InlineComent = "ENDIF" });
                    }
                    else
                    {
                        ifStmt.ElseStatements.Add(new EndStatement { OriginalCode = current.OriginalBlock, LineNumber = current.StartLineNumber, NestingLevel = currentLevel, InlineComent = "ENDELSE" });
                    }


                    //index++;  // Advance past END;
                    break;
                }

                // Parse sub-statement (advances index internally)
                var parsed = VisualAgeLogicParser.TryParse(lines, ref index, currentLevel + 1);

                if (isTrue)
                    ifStmt.TrueStatements.AddRange(parsed);
                else
                    ifStmt.ElseStatements.AddRange(parsed);

                // No extra index++ here
            }

            // Set full OriginalCode including body, ELSE, END
            ifStmt.OriginalCode = lines[overallStartIndex].OriginalBlock;//string.Join("\n", lines.GetRange(overallStartIndex, index - overallStartIndex).Select(l => l.OriginalBlock));

            return ifStmt;
        }

    }
}
