using EsfCore.Esf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;

namespace EsfCore.Tags
{
    public class FuncTagCollection : IEsfTagModel
    {
        [JsonIgnore]
        public string TagName => "FUNCTIONS";

        public List<FuncTag> Functions { get; set; } = new();

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
