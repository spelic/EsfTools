using EsfParser.Parser.Logic.Statements;
using System.Text.RegularExpressions;

namespace EsfParser.Parser.Logic.Parsers
{
    public class IfStatementParser : IStatementParser
    {
        public bool CanParse(string line) => line.TrimStart().StartsWith("IF", StringComparison.OrdinalIgnoreCase);

        public IStatement Parse(List<PreprocessedLine> lines, ref int index)
        {
            var ifStmt = new IfStatement();
            int startIndex = index;

            // === 1. Parse the IF condition
            string condition = lines[index].CleanLine[2..].Trim().TrimEnd(';');
            ifStmt.OriginalCode = lines[index].OriginalBlock;
            ifStmt.Condition = condition;
            ifStmt.LineNumber = lines[index].StartLineNumber;
            index++;

            bool inElseBranch = false;
            bool foundEnd = false;

            // === 2. Parse blocks
            while (index < lines.Count)
            {
                var line = lines[index];
                var text = line.CleanLine.Trim();

                if (text.StartsWith("ELSE;", StringComparison.OrdinalIgnoreCase))
                {
                    ifStmt.ElseStatements.Add(new ElseStatement
                    {
                        OriginalCode = line.OriginalBlock,
                        LineNumber = line.StartLineNumber,
                    });

                    if (!string.IsNullOrWhiteSpace(line.InlineComment))
                    {
                        ifStmt.ElseStatements.Add(new CommentStatement
                        {
                            OriginalCode = line.InlineComment,
                            Text = line.InlineComment.TrimStart('/', '*').Trim(),
                            LineNumber = line.StartLineNumber,
                        });
                    }

                    inElseBranch = true;
                    index++;
                    continue;
                }
                else if (text.StartsWith("END;", StringComparison.OrdinalIgnoreCase))
                {
                    var targetList = inElseBranch ? ifStmt.ElseStatements : ifStmt.TrueStatements;
                    targetList.Add(new EndStatement
                    {
                        OriginalCode = line.OriginalBlock,
                        LineNumber = line.StartLineNumber,
                    });

                    if (!string.IsNullOrWhiteSpace(line.InlineComment))
                    {
                        targetList.Add(new CommentStatement
                        {
                            OriginalCode = line.InlineComment,
                            Text = line.InlineComment.TrimStart('/', '*').Trim(),
                            LineNumber = line.StartLineNumber,
                        });
                    }

                    foundEnd = true;
                    index++;
                    break;
                }
                else
                {
                    var stmts = VisualAgeLogicParser.TryParse(lines, ref index);
                    var targetList = inElseBranch ? ifStmt.ElseStatements : ifStmt.TrueStatements;
                    targetList.AddRange(stmts);
                    index++;
                }
            }

            // === 3. Auto-close
            if (!foundEnd)
            {
                var list = inElseBranch ? ifStmt.ElseStatements : ifStmt.TrueStatements;
                list.Add(new EndStatement
                {
                    OriginalCode = "// [auto-inserted] END;",
                    LineNumber = lines.Last().EndLineNumber + 1,
                });
            }

            return ifStmt;
        }
    }
}
