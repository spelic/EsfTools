using EsfParser.Esf;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EsfParser.Tags
{
    public class TbleTagParser : EsfParser.Builder.IEsfTagParser
    {
        public string TagName => "TBLE";

        public IEsfTagModel Parse(List<TagNode> nodes)
        {
            var tables = nodes
                .Where(n => string.Equals(n.TagName, TagName, StringComparison.OrdinalIgnoreCase))
                .Select(TbleTag.Parse)
                .ToList();

            return new TbleTagCollection { Tables = tables };
        }
    }

   
}