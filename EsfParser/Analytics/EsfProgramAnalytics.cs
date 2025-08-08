using EsfParser.Esf;
using EsfParser.Parser.Logic;
using EsfParser.Parser.Logic.Statements;
using EsfParser.Tags;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EsfParser.Analytics
{
    public static class EsfProgramAnalytics
    {
        public static List<IStatement> GetAllUnparsedStatements(EsfProgram program)
            => GetAllStatementsRecursive(program).Where(s => s.Type == StatementType.Unknown).ToList();

        public static List<string> GetAllOriginalTextByType(EsfProgram program, StatementType type)
            => GetAllStatementsRecursive(program).Where(s => s.Type == type).Select(s => s.OriginalCode).ToList();

        public static List<string> GetImportantExpressionsByType(EsfProgram program, StatementType type)
            => GetAllStatementsRecursive(program)
                .Where(s => s.Type == type)
                .Select(s => s switch
                {
                    IfStatement ifs => ifs.Condition,
                    WhileStatement wh => wh.Condition,
                    AssignStatement or MoveStatement => s.OriginalCode,
                    _ => s.OriginalCode
                }).ToList();

        public static Dictionary<StatementType, int> CountByType(EsfProgram program)
            => GetAllStatementsRecursive(program)
                .GroupBy(s => s.Type)
                .ToDictionary(g => g.Key, g => g.Count());

        public static List<IStatement> GetStatementsByFunctionName(EsfProgram program, string functionName)
        {
            var func = program.Functions.Functions.FirstOrDefault(f =>
                string.Equals(f.Name, functionName, StringComparison.OrdinalIgnoreCase));

            return func == null ? new() : GetAllStatementsRecursive(func);
        }

        public static List<IStatement> GetStatementsByFunctionPrefix(EsfProgram program, string prefix)
        {
            return program.Functions.Functions
                .Where(f => f.Name.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                .SelectMany(GetAllStatementsRecursive)
                .ToList();
        }

        public static void PrintSummaryToConsole(EsfProgram program)
        {
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("==== ESF Program Statement Summary ====");
            Console.ResetColor();

            var totals = CountByType(program);
            foreach (var entry in totals.OrderByDescending(e => e.Value))
            {
                Console.ForegroundColor = entry.Key switch
                {
                    StatementType.Unknown => ConsoleColor.Red,
                    StatementType.Comment => ConsoleColor.DarkGray,
                    _ => ConsoleColor.Green
                };

                Console.WriteLine($"{entry.Key,-10} : {entry.Value}");
            }

            Console.ResetColor();
            Console.WriteLine();

            int count = 1;

            foreach (var func in program.Functions.Functions)
            {
                var parsedStatements = EsfProgramAnalytics.GetAllStatementsRecursive(func);
                var unknowns = parsedStatements.Where(s => s.Type == StatementType.Unknown).ToList();

                if (unknowns.Count == 0)
                    continue;

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"==== FUNC={func.Name} has {unknowns.Count} unknown statement(s) ====");
                Console.ResetColor();
                int lineStartColumn = 100;

                foreach (var stmt in unknowns)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    string content = $"FUNC={func.Name,-12} {stmt.OriginalCode}";
                    string padded = content.PadRight(lineStartColumn);
                    Console.WriteLine($"{padded}LINE={stmt.LineNumber}");
                }

                Console.ResetColor();
                Console.WriteLine($"-- Statement Tree for FUNC={func.Name} --");
                //StatementTreeVisualizer.Print(parsedStatements);
            }
        }

        public static bool PrintUnknownToConsole(EsfProgram program)
        {
            bool hasUnknowns = false;
            foreach (var func in program.Functions.Functions)
            {
                var parsedStatements = EsfProgramAnalytics.GetAllStatementsRecursive(func);
                var unknowns = parsedStatements.Where(s => s.Type == StatementType.Unknown).ToList();

                if (unknowns.Count == 0)
                    continue;

                hasUnknowns = true;
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"==== FUNC={func.Name} has {unknowns.Count} unknown statement(s) ====");
                Console.ResetColor();
                int lineStartColumn = 100;

                foreach (var stmt in unknowns)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    string content = $"FUNC={func.Name,-12} {stmt.OriginalCode}";
                    string padded = content.PadRight(lineStartColumn);
                    Console.WriteLine($"{padded}LINE={stmt.LineNumber}");
                }

                Console.ResetColor();
            }
            return hasUnknowns;
        }

        public static void PrintAfterStatementsToConsole(EsfProgram program)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("==== AFTER LOGIC STATEMENTS ====");
            Console.ResetColor();

            foreach (var func in program.Functions.Functions)
            {
                if (func.AfterLogicStatements == null || func.AfterLogicStatements.Count == 0)
                    continue;

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"FUNC={func.Name} ({func.AfterLogicStatements.Count} statements)");
                Console.ResetColor();

                foreach (var stmt in func.AfterLogicStatements)
                {
                    string lineInfo = $"[L:{stmt.LineNumber:D4}] [{stmt.Type}]".PadRight(25);
                    string original = stmt.OriginalCode.Trim();
                    Console.ForegroundColor = stmt.Type == StatementType.Unknown ? ConsoleColor.Red :
                                              stmt.Type == StatementType.Comment ? ConsoleColor.DarkGray :
                                              ConsoleColor.Green;

                    Console.WriteLine($"{lineInfo} {original}");
                    Console.ResetColor();
                }

                Console.WriteLine(); // space after each function
            }
        }

        public static List<IStatement> GetAllStatementsRecursive(EsfProgram program)
        {
            return program.Functions.Functions
                .SelectMany(GetAllStatementsRecursive)
                .ToList();
        }

        public static List<IStatement> GetAllStatementsRecursive(FuncTag func)
        {
            var all = new List<IStatement>();
            void Visit(IStatement s)
            {
                all.Add(s);

                switch (s)
                {
                    case IfStatement ifs:
                        foreach (var stmt in ifs.TrueStatements) Visit(stmt);
                        foreach (var stmt in ifs.ElseStatements) Visit(stmt);
                        break;
                    case WhileStatement wh:
                        foreach (var stmt in wh.BodyStatements) Visit(stmt);
                        break;
                }
            }

            foreach (var stmt in func.BeforeLogicStatements) Visit(stmt);
            foreach (var stmt in func.AfterLogicStatements) Visit(stmt);

            return all;
        }

        public static List<IStatement> GetAllStatementsRecursive(List<IStatement> statements)
        {
            var all = new List<IStatement>();
            void Visit(IStatement s)
            {
                all.Add(s);

                switch (s)
                {
                    case IfStatement ifs:
                        foreach (var stmt in ifs.TrueStatements) Visit(stmt);
                        foreach (var stmt in ifs.ElseStatements) Visit(stmt);
                        break;
                    case WhileStatement wh:
                        foreach (var stmt in wh.BodyStatements) Visit(stmt);
                        break;
                }
            }
            foreach (var stmt in statements) Visit(stmt);
            return all;
        }
    }
}
