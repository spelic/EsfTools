using EsfConsoleConverter;
using EsfParser;
using EsfParser.Analytics;
using EsfParser.Builder;
using EsfParser.Parser;
using EsfParser.Parser.Logic;
using EsfParser.Parser.Logic.Statements;
using System;
using System.IO;
using System.Text;

Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
string path = args.Length > 0 ? args[0] : "M000A-V22.esf";
path =  "D123A-V54.esf";
path = "IS00A-V26.esf";
path = "debug.esf";
path = "D133A-V68.esf";


//EsfDebuggerHelper.DebugString();
//EsfDebuggerHelper.DebugFunction();
//EsfDebuggerHelper.ExportConsoleApplication("D133A-V68.esf");

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
    program.ExportToConsoleProject(@"..\..\..\" + name + ".Console", name + "_ConsoleApp");

    Console.WriteLine("Done.");