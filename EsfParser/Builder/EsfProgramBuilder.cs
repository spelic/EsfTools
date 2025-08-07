using EsfParser.Esf;
using EsfParser.Tags;
using System.Collections.Generic;


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
               
                new FuncTagParser(),
                new RecordTagParser(),
                new TableTagParser(),
                new MapTagParser(),
                new ItemTagParser()

            };

           
            foreach (var parser in parsers)
            {
                var parsed = parser.Parse(nodes);
                if (parsed is IEsfTagModel tag)
                {
                    program.AddTag(tag);
                }
            }


            return program;
        }

    }
}