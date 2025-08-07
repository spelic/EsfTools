
using EsfParser;
using EsfParser.Analytics;
using EsfParser.Builder;
using EsfParser.Parser;
using EsfParser.Parser.Logic;
using EsfParser.Parser.Logic.Statements;
using System;
using System.IO;
using System.Text;


// EsfProgramFunctions.LoadFunctionsFromJson("esf_functions.json");

//foreach (var func in EsfProgramFunctions.Functions)
//{

//    var preprocessedLines = EsfLogicPreprocessor.Preprocess(func.Lines);

//    VisualAgeLogicParser vageLogicParser = new VisualAgeLogicParser(preprocessedLines);
//    var tree = vageLogicParser.Parse();
//    var allStatements = EsfProgramAnalytics.GetAllStatementsRecursive(tree);
//    var unknowns = allStatements.Where(s => s.Type == StatementType.Move).ToList();

//    if (unknowns.Count > 0)
//    {
//        foreach (var unknown in unknowns)
//        {
//            Console.WriteLine($"{unknown.OriginalCode.PadRight(60)} -> {unknown.ToCSharp()}");
            
//        }   
//    }
//}

//return;
Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
string path = args.Length > 0 ? args[0] : "D133A-V68.esf";

path = "D133A-V68.esf";

 if (!File.Exists(path))
    {
        Console.WriteLine($"File not found: {path}");
        return;
    }

    var lines = File.ReadAllLines(path, Encoding.GetEncoding(1250));
    Console.OutputEncoding = Encoding.UTF8;

    var nodes = MyEsfParser.Parse(lines);
    var program = EsfProgramBuilder.GenerateEsfProgram(nodes);

    string name = path.ToLower().Replace(".esf", "").Replace("-", "_").ToUpper();
    program.ExportToSingleProgramFile(@"..\..\..\..\" + name + ".Console.cs", name + "_ConsoleApp");

    Console.WriteLine("Done.");