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
path = "D133A-V63.esf";


//EsfDebuggerHelper.DebugString();
EsfDebuggerHelper.DebugFunction();
//EsfDebuggerHelper.ExportConsoleApplication("D133A-V68.esf");

