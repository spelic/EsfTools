using EsfParser.Esf;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EsfParser.Tags
{
    public class MapTagParser : EsfParser.Builder.IEsfTagParser
    {
        public string TagName => "MAP";

        public IEsfTagModel Parse(List<TagNode> nodes)
        {
            var maps = nodes
                .Where(n => string.Equals(n.TagName, TagName, StringComparison.OrdinalIgnoreCase))
                .Select(MapTag.Parse)
                .ToList();

            return new MapTagCollection { Maps = maps };
        }
    }

    
}