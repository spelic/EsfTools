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

        public static void DebugString()
        {
            var input = @" 
/* **************************************************************/
/* čitanje za ta ident vhoda v t.POTIZSUM
/* **************************************************************/
CE02R01 = CE01W03;
CE02P16();                       /* SQLEXEC-delete v t.POTIZSUM

IF EZESQCOD = 0;
CE02R02 = CE01W03;
CE02P17();                     /* SQLEXEC-delete v t.POTIZPOZ
END;


IF EZESQCOD = 0;                 /* pogoj za potrditev
/* **************************************************************/
/* POTRDITEV SPREMEMB !!!!!!!!!!!!!
/* **************************************************************/

CE01W02.MSG64 = ""Podatki potreb za stružni center po teh kriterijih so brisani"";
CE01W02.MSGN3 = 000;
EZEREPLY = 1;
EZECOMIT();                    /* potrditev sprememb
ELSE;
/* **************************************************************/
/* PREKLIC SPREMEMB !!!!!!!!!!!!!
/* **************************************************************/
CE01W02.MSG64 = ""Napaka pri brisanju potreb za stružni center"";
CE01W02.MSGN3 = 000;
CE01W02.MSGCOD = ""Koda:  -"";
CE01W02.MSGN3 = EZESQCOD;
EZEREPLY = 1;
EZEROLLB();                    /* preklic sprememb
END;    

";

            var output = EsfLogicPreprocessor.Preprocess(input.Split('\n').ToList());

            var vageLogicParser2 = new VisualAgeLogicParser(output);
            var tree2 = vageLogicParser2.Parse();
            Console.WriteLine("Visualized Statement Tree:");
            StatementTreeVisualizer.Print(tree2);
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

                    PrettyOutputWriter.PrintPreprocessedLines(preprocessedLines);

                    StatementTreeVisualizer.Print(tree);

                    Console.WriteLine("ORG:");

                    foreach (var line in func.Lines)
                    {
                        Console.WriteLine(line);
                    }


                    Console.ReadLine();
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
