using EsfParser.Esf;
using EsfParser.Tags;
using EsfParser.Tags.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace EsfParser.CodeGen
{
    public static class EsfLogicToCs
    {
        public static EsfProgram program { get; set; }
       
        /// <summary>
        /// Translate ESF logic to C#, automatically qualifying SpecialFunctions,
        /// GlobalItems, and WORKSTOR.<fields>.
        /// Make sure you've called Configure(...) first.
        /// </summary>
        public static string GenerateCSharpFromLogic(FuncTag func,
            List<string> esfLines,
            int indentSpaces = 6)
        {

            return "";

            var globalItemNames = program.Items.Items
                .Select(i => i.Name)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);
            var workstorFieldNames = program.WorkstorRecord.Items
                .Select(f => f.Name)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);
            var workstorName = program.WorkstorRecord.Name;

            Console.WriteLine($"Function: {func.Name}");

            var workstorDefs = program.Records.Records
                //.Where(r => string.Equals(r.Org, "WORKSTOR", StringComparison.OrdinalIgnoreCase))
                .ToDictionary(
                    rec => rec.Name,                    // record class name, e.g. "IS00W01"
                    rec => rec.Items.Select(f => f.Name)// field names in that record
                );



            // 1) Build your expression converter
            var exprConv = new ExpressionConverter(
                new WorkstorQualifier(workstorDefs, globalItemNames),
                new SpecialFunctionQualifier()
            );

            // 3) Gather all record‐ and map‐class names
            var recordNames = program.Records.Records.Select(r => r.Name);
            var mapNames = program.Maps.Maps.Select(m => m.MapName);

            // 2) create the type resolver
            var typeResolver = new EsfTypeResolver(program.Items, program.Records, program.Maps);

            var condGen = new ConditionCodeGenerator(exprConv, typeResolver);

            // 5) Finally wire up your LogicProcessor
            var processor = new LogicProcessor(
                qualifiers: new ITokenQualifier[] {
                    new WorkstorQualifier(workstorDefs,globalItemNames),
                    new SpecialFunctionQualifier()       
                },
                condConv: condGen,
                translators: new IStatementTranslator[] {
        new MoveTranslator(exprConv),
        new SetTranslator(exprConv),
        new CallTranslator(),
        // Note: now passing the typeResolver into AssignmentTranslator
        new AssignmentTranslator(exprConv, typeResolver, recordNames, mapNames),
                }
            );

            string csharp = processor.Process(esfLines);
            return csharp;
        }
    }
}
