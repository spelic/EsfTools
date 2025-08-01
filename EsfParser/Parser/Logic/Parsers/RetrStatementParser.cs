using System.Text.RegularExpressions;
using EsfParser.Parser.Logic.Statements;

namespace EsfParser.Parser.Logic.Parsers
{
    public class RetrStatementParser : IStatementParser
    {
        public bool CanParse(string line)
        {
            return line.TrimStart().StartsWith("RETR ", StringComparison.OrdinalIgnoreCase);
        }

        public IStatement Parse(List<PreprocessedLine> lines, ref int index)
        {
            var current = lines[index];
            var rawLine = current.OriginalBlock;
            var body = current.CleanLine.TrimEnd(';').Substring(5).Trim(); // remove "RETR "

            // Split by whitespace
            var tokens = Regex.Split(body, @"\s+");

            // Expecting at least 3 tokens
            if (tokens.Length < 3)
            {
                return new UnknownStatement
                {
                    OriginalCode = rawLine,
                    LineNumber = current.StartLineNumber,
                };
            }

            string sourceItem = tokens[0];
            string tableAndColumn = tokens[1];
            string targetItem = tokens[2];
            string tableName = tableAndColumn.Split('.')[0];
            string searchColumn = tableAndColumn;

            // Optional 4th token for explicit return column
            string returnColumn = tokens.Length >= 4 ? tokens[3] : "(DEFAULT)";

            return new RetrStatement
            {
                OriginalCode = rawLine,
                SourceItem = sourceItem,
                TableName = tableName,
                SearchColumn = searchColumn,
                TargetItem = targetItem,
                ReturnColumn = returnColumn,
                LineNumber = current.StartLineNumber,
            };
        }
    }
}
