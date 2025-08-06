using EsfParser;
using EsfParser.Analytics;
using EsfParser.Builder;
using EsfParser.Parser;
using EsfParser.Parser.Logic;
using EsfParser.Parser.Logic.Statements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EsfConsoleConverter
{
    public static class EsfDebuggerHelper
    {

        public static void DebugEsfParser()
        {

            var files = EsfUtils.GetEsfFilesInFolder(@"C:\Users\denis.spelic\Desktop\esf\");
            var programVersions = EsfUtils.GetLatestEsfVersions(files);


            var sb = new StringBuilder();
            int count = 0;
            EsfProgramFunctions.Functions.Clear();
            foreach (var p in programVersions)
            {
                var lines = File.ReadAllLines(p.FileName, Encoding.GetEncoding(1250));
                Console.OutputEncoding = Encoding.UTF8;
                var nodes = MyEsfParser.Parse(lines);
                var program = EsfProgramBuilder.GenerateEsfProgram(nodes);

                foreach (var func in program.Funcs.Functions)
                {
                    if (func.BeforeLogic.Count > 0)
                        EsfProgramFunctions.Functions.Add(new EsfProgramFunction(p.Name, func.Name, func.BeforeLogic));

                    if (func.AfterLogic.Count > 0)
                    {
                        Console.WriteLine($"Program:{p.Name} Function {func.Name} has {func.AfterLogic.Count} after logic statements.");
                    }
                }



                var hasUnknown = EsfProgramAnalytics.PrintUnknownToConsole(program);
                // is there any unknown statements in the program copy that esf file to new folder errors
                hasUnknown = false;
                if (hasUnknown)
                {
                    string errorFolder = @"C:\Users\denis.spelic\Desktop\esf\errors\";
                    if (!Directory.Exists(errorFolder))
                        Directory.CreateDirectory(errorFolder);
                    string errorFileName = Path.Combine(errorFolder, p.Name + ".esf");
                    File.Copy(p.FileName, errorFileName, true);
                    Console.WriteLine($"Unknown statements found in {p.Name}. Copied to {errorFileName}");
                }

                // EsfProgramAnalytics.PrintAfterStatementsToConsole(program);
                count++;
                Console.WriteLine($"[{count}/{programVersions.Count}] - {p.Name}");

            }
        }

        public static IEnumerable<IStatement> FlattenStatements(IEnumerable<IStatement> root)
        {
            foreach (var stmt in root)
            {
                yield return stmt;

                switch (stmt)
                {
                    case IfStatement ifs:
                        foreach (var child in FlattenStatements(ifs.TrueStatements)) yield return child;
                        foreach (var child in FlattenStatements(ifs.ElseStatements)) yield return child;
                        break;
                    case WhileStatement wh:
                        foreach (var child in FlattenStatements(wh.BodyStatements)) yield return child;
                        break;
                   
                }
            }
        }
        private static string TruncPad(string text, int length)
        {
            return text.Length > length ? text.Substring(0, length - 3) + "..." : text.PadRight(length);
        }

        private static string StatementToDisplayString(IStatement stmt)
        {
            return stmt switch
            {
                IfStatement ifs => $"IF {ifs.Condition}",
                WhileStatement wh => $"WHILE {wh.Condition}",
                MoveStatement m => $"MOVE {m.Source} TO {m.Destination}",
                AssignStatement a => $"ASSIGN {a.Left} = {a.Right}",
                CallStatement c => $"CALL {c.ProgramName}({string.Join(", ", c.Parameters.Select(p => p.Raw))})",
                RetrStatement r => $"RETR {r.SourceItem} {r.TableName}.{r.SearchColumn} {r.TargetItem} {r.ReturnColumn}",
                SetStatement s => $"SET {s.Target} {string.Join(", ", s.Attributes)}",
                CommentStatement cm => $"COMMENT {cm.Text}",
                EndStatement => "END",
                ElseStatement => "ELSE",
                _ => $"[{stmt.Type}] {stmt.OriginalCode}"
            };
        }

        public static void PrintPreprocessedAndParsedStatements(
    List<PreprocessedLine> preprocessedLines,
    List<IStatement> parsedStatements)
        {
            var allParsed = FlattenStatements(parsedStatements).ToList();

            Console.WriteLine($"{"#",-3} {"Original",-30} {"CleanLine",-30} {"Parsed Statement",-50}");
            Console.WriteLine(new string('-', 110));

            for (int i = 0; i < preprocessedLines.Count; i++)
            {
                var line = preprocessedLines[i];
                string original = line.OriginalBlock.Replace("\n", " ").Replace("\r", "").Trim();
                string clean = line.CleanLine.Trim();
                string parsed = "";

                // Try to find the parsed statement for this line (match by line number)
                var match = allParsed.FirstOrDefault(s => s.LineNumber == line.StartLineNumber);
                if (match != null)
                {
                    parsed = StatementToDisplayString(match);
                }

                Console.WriteLine($"{line.StartLineNumber,3} {TruncPad(original, 30)} {TruncPad(clean, 30)} {parsed}");
            }
        }

        public static void DebugString()
        {
            var input = new List<string>
        {
            "WHILE DELA1 = 0;",
                "IF EZEAID IS ENTER;",
                    "MOVE BO01M01.IZBORM TO BO01R01.ZFUNKC;",
                    "IF BO01R01 IS ERR;",
                        "IF BO01R01 IS NRF;",
                            "MOVE \"Šifra nima avtorizacije\" TO BO01M01.EZEMSG;",
                        "ELSE;",
                            "MOVE \"Napaka I/O\" TO BO01M01.EZEMSG;",
                        "END;",
                        "SET BO01M01.IZBORM CURSOR,MODIFIED;",
                    "ELSE;",
                        "STEVAPPL = STEVAPPL + 1;",
                        "IF BO01M01.IZBORM = 1;",
                            "MOVE 'BO02A' TO APPL[STEVAPPL]; /* prijava",
                        "END;",
                        "IF BO01M01.IZBORM = 2;",
                            "MOVE 'BO03A' TO APPL[STEVAPPL]; /* pregled za kupca",
                        "END;",
                        "IF BO01M01.IZBORM = 3;",
                            "MOVE 'BO04A' TO APPL[STEVAPPL]; /* indikator",
                        "END;",
                        "IF BO01M01.IZBORM = 4;",
                            "MOVE 'BO05A' TO APPL[STEVAPPL]; /* pregled",
                        "END;",
                        "IF BO01M01.IZBORM = 5;",
                            "MOVE 'BO07A' TO APPL[STEVAPPL]; /* pregled",
                        "END;",
                        "MOVE APPL[STEVAPPL] TO EZEAPP;",
                        "DXFR EZEAPP BO01W01;",
                    "END;", // za err
                "END;", // za sifro
            "END;"  // za while
        };

            var inputs = @"

/*PZ22W01.STEVAPPL = 1;
/*PZ22W01.ZAPRIIM = 'P.';
/*PZ22W01.APPL[PZ22W01.STEVAPPL] = 'PZ22A';
MOVE 1 TO EZEFEC;
MOVE 1 TO EZESQISL;
MOVE 1 TO EZECNVCM;   
PZ22W02.VRSTICA = 1;
PZ22W02.PRVI = 1;
MOVE 'PZ22' TO EZESEGTR;
/*PZ22P03();
WHILE 1 = 1;
  /*PZ22P08();
/*  PZ22M01.BESEDILO1 = ABCDE.BESEDA;
  PZ22P02();
  IF EZEAID IS PF3;
/*    PZ22W01.STEVAPPL = PZ22W01.STEVAPPL - 1;
/*    EZEAPP = PZ22W01.APPL[PZ22W01.STEVAPPL];
/*    DXFR EZEAPP PZ22W01;
       EZERTN;
  ELSE;
    IF EZEAID IS PF7;
      PZ22P06();
      PZ22P08();
    ELSE;
      IF EZEAID IS PF8;
        PZ22P07();
        PZ22P08();
      ELSE;
        IF EZEAID IS PF2;
          PZ22P10();
          PZ22S01();
        ELSE;
            IF EZEAID IS PF6;
              SET PZ22M01 CLEAR;
            END;
          END;
        END;
      END;
    END;
  END;


";

           // var output = EsfLogicPreprocessor.Preprocess(inputs);
            var output = EsfLogicPreprocessor.Preprocess(inputs.Split('\n').ToList());

            var vageLogicParser2 = new VisualAgeLogicParser(output);
            var tree2 = vageLogicParser2.Parse();

            var allParsed = FlattenStatements(tree2).ToList();

            var unknowns = allParsed.Where(s => s.Type == StatementType.Unknown).ToList();

            foreach (var stmt in allParsed)
            {
                string meta = $"[L:{stmt.LineNumber:D4}][{stmt.NestingLevel:D2}]".PadRight(10);
                string oput = StatementToDisplayString(stmt);
                string indentPad = new string(' ', stmt.NestingLevel * 3);
                Console.WriteLine($"{meta}{indentPad}{oput}");
            }



            Console.WriteLine("Visualized Statement Tree:");
            StatementTreeVisualizer.Print(tree2);
            Console.ReadLine();
        }

        public static void ExportConsoleApplication(string name)
        {
            if (!File.Exists(name))
            {
                Console.WriteLine($"File not found: {name}");
                return;
            }
            var lines = File.ReadAllLines(name, Encoding.GetEncoding(1250));
            Console.OutputEncoding = Encoding.UTF8;
            var nodes = MyEsfParser.Parse(lines);
            var program = EsfProgramBuilder.GenerateEsfProgram(nodes);
            string consoleAppName = name.ToLower().Replace(".esf", "").Replace("-", "_").ToUpper();
            program.ExportToConsoleProject(consoleAppName + ".Console", consoleAppName + "_ConsoleApp");
            Console.WriteLine("Done.");
        }


        public static void DebugPreprocesingOfLines()
        {
            EsfProgramFunctions.LoadFunctionsFromJson("esf_functions.json");
            int count = EsfProgramFunctions.Functions.Count;

            foreach (var func in EsfProgramFunctions.Functions)
            {

                //foreach (var line in func.Lines)
                //{
                //    string originalLine = line;
                //    // check if there is a comment /* in line remove it drom line
                //    if (originalLine.Contains("/*"))
                //    {
                //        originalLine = originalLine.Substring(0, originalLine.IndexOf("/*")).Trim();
                //    }

                //    // count number of semicolons in the line
                //    int semicolonCount = originalLine.Count(c => c == ';');
                //    if (semicolonCount > 1)
                //    {
                //        Console.ForegroundColor = ConsoleColor.Red;
                //        Console.WriteLine($"Function: {func.FunctionName} has {semicolonCount} semicolons in line: {originalLine}");
                //        Console.ResetColor();
                //    }
                //}

                Console.WriteLine($"Program: {func.ProgramName}, Function: {func.FunctionName}, Lines: {func.Lines.Count}");

                var preprocessedLines = EsfLogicPreprocessor.Preprocess(func.Lines);
                PrettyOutputWriter.PrintPreprocessedLines(preprocessedLines);
                Console.ReadLine();
            }
        }

        public static void DebugFunction()
        {
            EsfProgramFunctions.LoadFunctionsFromJson("esf_functions.json");
            int currentFunctionIndex = 0;
            int unknownCount = 0;
            int statementCount = 0;
            int count = EsfProgramFunctions.Functions.Count;

            foreach (var func in EsfProgramFunctions.Functions)
            {
                Console.WriteLine($"Program: {func.ProgramName}, Function: {func.FunctionName}, Lines: {func.Lines.Count}");

                var preprocessedLines = EsfLogicPreprocessor.Preprocess(func.Lines);

                VisualAgeLogicParser vageLogicParser = new VisualAgeLogicParser(preprocessedLines);
                var tree = vageLogicParser.Parse();

                var allStatements = EsfProgramAnalytics.GetAllStatementsRecursive(tree);

                statementCount += allStatements.Count;

                var unknowns = allStatements.Where(s => s.Type == StatementType.Unknown).ToList();

                if (unknowns.Count > 0)
                {
                    Console.Clear();

                    unknownCount+= unknowns.Count;
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"[{currentFunctionIndex}/{count}]****Program: {func.ProgramName} ==== FUNC={func.FunctionName} has {unknowns.Count} unknown statement(s) ====");
                    Console.ResetColor();
                    int lineStartColumn = 100;

                    foreach (var stmt in unknowns)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        string content = $"FUNC={func.FunctionName,-12} {stmt.OriginalCode}";
                        string padded = content.PadRight(lineStartColumn);
                        Console.WriteLine($"{padded}LINE={stmt.LineNumber}");
                    }

                    Console.ResetColor();

                    //PrettyOutputWriter.PrintPreprocessedLines(preprocessedLines);

                    //StatementTreeVisualizer.Print(tree);

                    //Console.WriteLine("ORG:");

                    //foreach (var line in func.Lines)
                    //{
                    //    Console.WriteLine(line);
                    //}


                    //Console.ReadLine();
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"[{currentFunctionIndex}/{count}]Program: {func.ProgramName}, Function: {func.FunctionName}, Lines: {func.Lines.Count}  - PARSED OK");
                    Console.ResetColor();
                }
                currentFunctionIndex++;
            }
            // write statistics to console
            Console.WriteLine($"Total functions: {count}");
            Console.WriteLine($"Total unknown statements: {unknownCount}");
            Console.WriteLine($"Total statements: {statementCount}");
            Console.WriteLine(
                $"Average statements per function: {(statementCount / (double)count).ToString("F2")}");
            PrintStatistics();
        }


        public static void DebugUknFunction()
        {
            // CE03A ==== FUNC=CE03P02
            EsfProgramFunctions.LoadFunctionsFromJson("esf_functions.json");
            int currentFunctionIndex = 0;
            int unknownCount = 0;
            int statementCount = 0;
            int count = EsfProgramFunctions.Functions.Count;

            foreach (var func in EsfProgramFunctions.Functions)
            {
                if (func.FunctionName != "CE03P02" || func.ProgramName != "CE03A")
                {
                    int debug = 1;
                }
                    
                var preprocessedLines = EsfLogicPreprocessor.Preprocess(func.Lines);

                VisualAgeLogicParser vageLogicParser = new VisualAgeLogicParser(preprocessedLines);
                var tree = vageLogicParser.Parse();

                //PrintPreprocessedAndParsedStatements(preprocessedLines, tree);

                var allStatements = EsfProgramAnalytics.GetAllStatementsRecursive(tree);

                statementCount += allStatements.Count;

                var unknowns = allStatements.Where(s => s.Type == StatementType.Unknown).ToList();

                if (unknowns.Count > 0)
                {
                    unknownCount += unknowns.Count;
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"[{currentFunctionIndex}/{count}]****Program: {func.ProgramName} ==== FUNC={func.FunctionName} has {unknowns.Count} unknown statement(s) ====");
                    Console.ResetColor();
                    int lineStartColumn = 100;

                    foreach (var stmt in unknowns)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        string content = $"FUNC={func.FunctionName,-12} {stmt.OriginalCode}";
                        string padded = content.PadRight(lineStartColumn);
                        Console.WriteLine($"{padded}LINE={stmt.LineNumber}");
                    }

                    

                    //foreach (var stmt in allStatements)
                    //{
                    //    string meta = $"[L:{stmt.LineNumber:D4}][{stmt.NestingLevel:D2}]".PadRight(10);
                    //    string oput = StatementToDisplayString(stmt);
                    //    string indentPad = new string(' ', stmt.NestingLevel * 3);
                    //    Console.WriteLine($"{meta}{indentPad}{oput}");
                    //}

                    //Console.ReadLine();

                    Console.ResetColor();

                    //PrettyOutputWriter.PrintPreprocessedLines(preprocessedLines);

                    //StatementTreeVisualizer.Print(tree);

                    //Console.WriteLine("ORG:");

                    //foreach (var line in func.Lines)
                    //{
                    //    Console.WriteLine(line);
                    //}


                    //Console.ReadLine();
                }
                currentFunctionIndex++;
            }
            // write statistics to console
            Console.WriteLine($"Total functions: {count}");
            Console.WriteLine($"Total unknown statements: {unknownCount}");
            Console.WriteLine($"Total statements: {statementCount}");
            Console.WriteLine(
                $"Average statements per function: {(statementCount / (double)count).ToString("F2")}");
            PrintStatistics();
        }

        public static void PrintStatistics()
        {
            Console.WriteLine("EsfProgramFunctions count: " + EsfProgramFunctions.Functions.Count);
            int totalLines = EsfProgramFunctions.Functions.Sum(f => f.Lines.Count);
            Console.WriteLine("Total lines in all functions: " + totalLines);
            int unknownCount = EsfProgramFunctions.Functions.Sum(f => f.Lines.Count(l => l.Contains("UNKNOWN")));
            Console.WriteLine("Total unknown statements: " + unknownCount);
            Console.WriteLine("Average lines per function: " + (totalLines / (double)EsfProgramFunctions.Functions.Count).ToString("F2"));
        }


    }
}
