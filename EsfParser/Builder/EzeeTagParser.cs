using System;
using System.Collections.Generic;
using System.Linq;
using EsfCore.Esf;
using EsfCore.Tags;

namespace EsfParser.Builder
{
    public class EzeeTagParser : IEsfTagParser
    {
        public IEsfTagModel Parse(List<TagNode> nodes)
        {
            var node = nodes.FirstOrDefault(n => n.TagName.Equals("EZEE", StringComparison.OrdinalIgnoreCase));
            return node != null ? EzeeTag.Parse(node) : null;
        }
    }
}