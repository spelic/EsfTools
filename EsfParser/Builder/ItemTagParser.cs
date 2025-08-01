using EsfParser.Esf;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EsfParser.Tags
{
    public class ItemTagParser : EsfParser.Builder.IEsfTagParser
    {
        public string TagName => "ITEM";

        public IEsfTagModel Parse(List<TagNode> nodes)
        {
            var items = nodes
                .Where(n => string.Equals(n.TagName, TagName, StringComparison.OrdinalIgnoreCase))
                .Select(ItemTag.Parse)
                .ToList();

            return new ItemTagCollection { Items = items };
        }
    }

    
}