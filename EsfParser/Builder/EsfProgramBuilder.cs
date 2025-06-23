using System.Collections.Generic;
using EsfCore.Esf;
using EsfCore.Tags;

namespace EsfParser.Builder
{
    public static class EsfProgramBuilder
    {

        public static EsfProgram GenerateEsfProgram(List<TagNode> nodes)
        {
            var program = new EsfProgram { Nodes = nodes };
            var parsers = new List<IEsfTagParser>
            {
                new EzeeTagParser(),
                new ProgramTagParser(),
                new ProlTagParser(),
                new FuncTagParser()
            };

            foreach (var parser in parsers)
            {
                var tag = parser.Parse(nodes);
                if (tag != null)
                    program.Tags.Add(tag);
            }
            // Add all FUNCs individually
            program.Tags.AddRange(FuncTagParser.ParseAll(nodes));
            return program;
        }

    }
}