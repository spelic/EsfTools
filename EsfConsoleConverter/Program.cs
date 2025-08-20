using EsfParser;
using EsfParser.Analytics;
using EsfParser.Builder;
using EsfParser.CodeGen;
using EsfParser.Parser;
using EsfParser.Parser.Logic;
using EsfParser.Parser.Logic.Statements;
using EsfParser.Tags;
using System;
using System.Collections;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;


Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
string path = args.Length > 0 ? args[0] : "D133A-V68.esf";

path = "D133A-V68.esf";
path = "IS00A-V26.esf";

if (!File.Exists(path))
    {
        Console.WriteLine($"File not found: {path}");
        return;
    }

    var lines = File.ReadAllLines(path, Encoding.GetEncoding(1250));
    Console.OutputEncoding = Encoding.UTF8;

    
    var nodes = MyEsfParser.Parse(lines);
//your problematic ESF statements (exact lines you want to debug)
var problemLines = $@"
  MOVE ""{{""""username"""":"""""" TO IS00W06.JSON;
";


foreach (var item in nodes)
{
    if (item.TagName == "FUNC" && item.Children[0].TagName == "BEFORE")
    {
        item.Attributes["NAME"][0] = "__DEBUG_ONLY__";
        item.Children[0].Content = problemLines;
        break;
    }
}

var program = EsfProgramBuilder.GenerateEsfProgram(nodes);
CSharpUtils.Program = program;
string name = path.ToLower().Replace(".esf", "").Replace("-", "_").ToUpper();

RoslynExporter.WriteProjectFiles(program, @"C:\Users\denis.spelic\source\repos\Test\"+name, name + "_ConsoleApp");

Console.WriteLine("Done.");

