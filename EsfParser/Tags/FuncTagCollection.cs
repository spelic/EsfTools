using EsfParser.Esf;
using EsfParser.Parser.Logic.Statements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;

namespace EsfParser.Tags
{
    public class FuncTagCollection : IEsfTagModel
    {
        [JsonIgnore]
        public string TagName => "FUNCTIONS";

        public List<FuncTag> Functions { get; set; } = new();

        [JsonIgnore]
        public List<IStatement> AllStatements => Functions
        .SelectMany(f => f.BeforeLogicStatements.Concat(f.AfterLogicStatements))
        .ToList();

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"FUNCTIONS: {Functions.Count} defined");
            int idx = 1;
            foreach (var func in Functions)
            {
                sb.AppendLine($"  {idx++}. FUNC {func.Name}");
                // assume FuncTag has its own nice ToString; indent each of its lines
                var detailLines = func.ToString()
                                      .Split(new[] { Environment.NewLine }, StringSplitOptions.None);
                foreach (var line in detailLines)
                {
                    sb.AppendLine("    " + line);
                }
            }
            return sb.ToString();
        }
    }
}
