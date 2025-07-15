using System;
using System.IO;
using EsfCore.Esf;
using EsfParser.Parser;
using EsfParser.Builder;
using EsfCore.Tags;
using System.Text;
using EsfCore;

Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
string path = args.Length > 0 ? args[0] : "M000A-V22.esf";
//path =  "D123A-V54.esf";
path = "IS00A-V26.esf";
path = "debug.esf";


bool analitics = false;

if (analitics)
{

    var files = EsfUtils.GetEsfFilesInFolder(@"C:\Users\denis.spelic\Desktop\esf");
    var programVersions = EsfUtils.GetLatestEsfVersions(files);


    var sb = new StringBuilder();
    int count = 0;
    foreach (var p in programVersions)
    {
        var lines = File.ReadAllLines(p.FileName, Encoding.GetEncoding(1250));
        Console.OutputEncoding = Encoding.UTF8;
        var nodes = MyEsfParser.Parse(lines);
        var program = EsfProgramBuilder.GenerateEsfProgram(nodes);

        foreach (var func in program.Funcs.Functions)
        {
            sb.AppendLine($"/*Program: {p} -> Func {func.Name} : Desc: {func.Desc}");
            if (func.BeforeLogic.Count > 0)
            {
                sb.AppendLine($"  Before Logic: {string.Join(Environment.NewLine, func.BeforeLogic)}");
            }
            if (func.AfterLogic.Count > 0)
            {
                sb.AppendLine($"  After Logic: {string.Join(Environment.NewLine, func.AfterLogic)}");
            }
            if (func.SqlClauses.Count > 0)
            {
                sb.AppendLine($"  SQL Clauses: {string.Join(Environment.NewLine, func.SqlClauses)}");
            }

        }
        count++;
        Console.WriteLine($"[{count}/{programVersions.Count}] - {p.Name}");

    }

    File.WriteAllText("esf_analysis.txt", sb.ToString());


    int a = 10;


}
else
{

    //var a = Console.ReadKey(true);

    //if (a.Key != ConsoleKey.Enter)
    //{
    //    Console.WriteLine("Press Enter to continue...");
    //    Console.ReadLine();
    //}


    if (!File.Exists(path))
    {
        Console.WriteLine($"File not found: {path}");
        return;
    }

    var lines = File.ReadAllLines(path, Encoding.GetEncoding(1250));
    Console.OutputEncoding = Encoding.UTF8;
    var nodes = MyEsfParser.Parse(lines);

    //foreach (var node in nodes) Console.WriteLine(node.ToString());

    var program = EsfProgramBuilder.GenerateEsfProgram(nodes);
    string name = path.ToLower().Replace(".esf", "").Replace("-", "_").ToUpper();
    program.ExportToConsoleProject(name + ".Console", name + "_ConsoleApp");


    //Console.WriteLine(program);

    // build FuncTagCollection
    var funcs = nodes
        .Where(n => n.TagName == "FUNC")
        .Select(FuncTag.Parse)
        .ToList();

    Console.WriteLine($"Found {funcs.Count} FUNC tags\n");

    // print first three
    for (int i = 0; i < Math.Min(3, funcs.Count); i++)
    {
        var f = funcs[i];
        Console.WriteLine($"{i + 1}. {f.Name}");
        Console.WriteLine($"   Before-lines: {f.BeforeLogic.Count}, After-lines: {f.AfterLogic.Count}, SQL-clauses: {f.SqlClauses.Count}");
    }

    Console.WriteLine("Done.");
}