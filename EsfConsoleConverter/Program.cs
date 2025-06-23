using System;
using System.IO;
using EsfCore.Esf;
using EsfParser.Parser;
using EsfParser.Builder;
using EsfCodeGen;
using EsfCore.Tags;

string path = args.Length > 0 ? args[0] : "M000A-V22.esf";
path =  "D123A-V54.esf";
string outDir = args.Length > 1 ? args[1] : "GeneratedOutput";

if (!File.Exists(path))
{
    Console.WriteLine($"File not found: {path}");
    return;
}

var lines = File.ReadAllLines(path);
var nodes = EsfParser.Parser.BlockParser.Parse(lines);
//var nodes = EsfParser.Parser.EsfParser.Parse(lines);
//foreach (var node in nodes) Console.WriteLine(node.ToString());
//EsfParser.Parser.BlockParser.PrintTagTree(nodes);
var program = EsfProgramBuilder.GenerateEsfProgram(nodes);
Console.WriteLine(program);
var funcs = program.GetTag<EsfCore.Tags.ProgramTag>()?.Funcs;

var exporter = new EsfFunctionExporter();

if (funcs != null)
{
    foreach (var func in funcs)
    {
        exporter.Export(func, outDir);
        Console.WriteLine($"Exported FUNC: {func.Name}");
    }
}

Console.WriteLine("Done.");
