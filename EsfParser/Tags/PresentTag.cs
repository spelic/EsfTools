using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using EsfParser.Esf;

namespace EsfParser.Tags
{
    public class PresentTag : IEsfTagModel
    {
        [JsonIgnore] public string TagName => "PRESENT";

        /// <summary>
        /// DEFFOLD=Y|N
        /// </summary>
        [JsonPropertyName("defaultFold")]
        public bool DefaultFold { get; set; }

        /// <summary>
        /// VARFOLD=Y|N
        /// </summary>
        [JsonPropertyName("varFold")]
        public bool VarFold { get; set; }

        /// <summary>
        /// TABPOS numbers (from child :EMAP)
        /// </summary>
        [JsonPropertyName("tabPositions")]
        public List<int> TabPositions { get; set; } = new();

        public override string ToString()
        {
            var sb = new System.Text.StringBuilder();
            sb.Append($"PRESENT: DEFFOLD={(DefaultFold ? "Y" : "N")}, VARFOLD={(VarFold ? "Y" : "N")}");
            if (TabPositions.Count > 0)
                sb.Append($", TABPOS=[{string.Join(",", TabPositions)}]");
            return sb.ToString();
        }

        public static PresentTag Parse(TagNode node)
        {
            var tag = new PresentTag
            {
                DefaultFold = node.Attributes
                                  .TryGetValue("DEFFOLD", out var df)
                              && df.FirstOrDefault()?.Equals("Y", StringComparison.OrdinalIgnoreCase) == true,
                VarFold = node.Attributes
                                  .TryGetValue("VARFOLD", out var vf)
                              && vf.FirstOrDefault()?.Equals("Y", StringComparison.OrdinalIgnoreCase) == true
            };

            // Look for a child EMAP tag that carries TABPOS
            foreach (var child in node.Children)
            {
                if (!child.TagName.Equals("EMAP", StringComparison.OrdinalIgnoreCase))
                    continue;

                if (child.Attributes.TryGetValue("TABPOS", out var positions))
                {
                    foreach (var block in positions)
                    {
                        // split on whitespace
                        foreach (var tok in block
                            .Split(' ', StringSplitOptions.RemoveEmptyEntries))
                        {
                            if (int.TryParse(tok, out var p))
                                tag.TabPositions.Add(p);
                        }
                    }
                }
            }

            return tag;
        }
    }
}
