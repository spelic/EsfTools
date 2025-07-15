using EsfCore.Tags.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace EsfCore.Tags
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
            var globalItemNames = program.Items.Items
                .Select(i => i.Name)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);
            var workstorFieldNames = program.WorkstorRecord.Items
                .Select(f => f.Name)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);
            var workstorName = program.WorkstorRecord.Name;

            Console.WriteLine("START PROCESING LOGIC: " + func.Name);

            // write out globalitem names and workstor field names seperated with commas
            Console.WriteLine("Global Items: " + string.Join(", ", globalItemNames));
            Console.WriteLine("Workstor: " + workstorName);
            Console.WriteLine("Workstor Fields: " + string.Join(", ", workstorFieldNames));

            // write out the function name and its logic with line number
            Console.WriteLine($"Function: {func.Name}");
            Console.WriteLine("Logic Lines:");
            for (int i = 0; i < esfLines.Count; i++)
            {
                Console.WriteLine($"  {i + 1}: {esfLines[i]}");
            }


            // Suppose EsfProgram.Records.Records is your List<RecordTag>,
            // and you only want those with Org == "WORKSTOR":
            var workstorDefs = program.Records.Records
                .Where(r => string.Equals(r.Org, "WORKSTOR", StringComparison.OrdinalIgnoreCase))
                .ToDictionary(
                    rec => rec.Name,                    // record class name, e.g. "IS00W01"
                    rec => rec.Items.Select(f => f.Name)// field names in that record
                );

            

            var exprConv = new ExpressionConverter(
    new SpecialFunctionQualifier(),
    new GlobalItemQualifier(globalItemNames),
    new WorkstorQualifier(workstorDefs)
);

            var condConv = new FastConditionConverter(exprConv);

            // gather all record‐ and map‐class names:
            var recordNames = program.Records.Records.Select(r => r.Name);
            var mapNames = program.Maps.Maps.Select(m => m.MapName);
            


            var processor = new LogicProcessor(
                qualifiers: new ITokenQualifier[]
                {
        new SpecialFunctionQualifier(),
        new GlobalItemQualifier(globalItemNames),
        new WorkstorQualifier(workstorDefs)    // now handles all workstor records
                },
                condConv: condConv,
                translators: new IStatementTranslator[]
                {
        new MoveTranslator(exprConv),
        new SetTranslator(exprConv),
        new CallTranslator(),
        new AssignmentTranslator(exprConv,recordNames, mapNames),
                }
            );

            string csharp = processor.Process(esfLines);
            return csharp;
        }
    }
}
