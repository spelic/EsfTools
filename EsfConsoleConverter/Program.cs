
using EsfParser;
using EsfParser.Analytics;
using EsfParser.Builder;
using EsfParser.CodeGen;
using EsfParser.Parser;
using EsfParser.Parser.Logic;
using EsfParser.Parser.Logic.Statements;
using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;



void SaveStatementsToFile(IEnumerable<IStatement> statements, StatementType stype)
{
    var sb = new StringBuilder();

    var stmt = statements.Where(s => s.Type == stype).ToList();

    foreach (var statement in stmt)
    {
        sb.AppendLine($"{statement.OriginalCode.PadRight(60)} -> {statement.ToCSharp()}");
    }
    File.WriteAllText($"{stype.ToString()}_Statements.txt", sb.ToString());
}

void functionExport()
{
    EsfProgramFunctions.LoadFunctionsFromJson("esf_functions.json");
    //var allallStatements = System.Text.Json.JsonSerializer.Deserialize<List<IStatement>>(File.ReadAllText("all_statements.json"));
    var allallStatements = new List<IStatement>();

    foreach (var func in EsfProgramFunctions.Functions)
    {

        var preprocessedLines = EsfLogicPreprocessor.Preprocess(func.Lines);

        VisualAgeLogicParser vageLogicParser = new VisualAgeLogicParser(preprocessedLines);
        var tree = vageLogicParser.Parse();
        var allStatements = EsfProgramAnalytics.GetAllStatementsRecursive(tree);

        allallStatements.AddRange(allStatements);
    }

    // save all statements to a json file
   // File.WriteAllText("all_statements.json", System.Text.Json.JsonSerializer.Serialize(allallStatements, new System.Text.Json.JsonSerializerOptions { WriteIndented = true }));

    var sb = new StringBuilder();

    var stmt = allallStatements.Where(s => s.Type == StatementType.While).ToList();

    foreach (WhileStatement statement in stmt)
    {
        sb.AppendLine($"{statement.Condition}");
    }
    File.WriteAllText($"While_Statements_Conditions.txt", sb.ToString());


    //SaveStatementsToFile(allallStatements, StatementType.Call);
    //SaveStatementsToFile(allallStatements, StatementType.Dxfr);
    //SaveStatementsToFile(allallStatements, StatementType.Move);
    //SaveStatementsToFile(allallStatements, StatementType.MoveA);
    //SaveStatementsToFile(allallStatements, StatementType.Retr);
    //SaveStatementsToFile(allallStatements, StatementType.Set);
    //SaveStatementsToFile(allallStatements, StatementType.SystemFunction);
    //SaveStatementsToFile(allallStatements, StatementType.Test);
    //SaveStatementsToFile(allallStatements, StatementType.Unknown);

    return;
}

//functionExport();
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
CSharpUtils.Program = program;
string name = path.ToLower().Replace(".esf", "").Replace("-", "_").ToUpper();
//program.ExportToSingleProgramFile(@"C:\Users\denis.spelic\source\repos\Test\Test\Program.cs", name + "_ConsoleApp");
program.RoslynExportToSingleProgramFile(@"C:\Users\denis.spelic\source\repos\Test\Test\ProgramR.cs", name + "_ConsoleAppRoslyn");

Console.WriteLine("Done.");



//var nodes2 = ChatGptEsfParser.Parse(lines);
//foreach (var node in nodes2)
//{
//    Console.WriteLine(node.TagName.ToUpperInvariant());
//}

//if (TagNodeComparer.EqualTrees(nodes, nodes2, out var diff))
//    Console.WriteLine("✅ Parsers produce identical trees.");
//else
//    Console.WriteLine("❌ Difference found:\n" + diff);

//Console.ReadLine();