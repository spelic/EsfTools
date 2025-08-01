using System;
using System.Collections.Generic;
using System.Linq;

using EsfParser.Esf;
using EsfParser.Tags;

namespace EsfParser.Builder
{
    /// <summary>
    /// Parser for the PROL tag, which represents comment/documentation text.
    /// </summary>
    public class ProlTagParser : IEsfTagParser
    {
        public IEsfTagModel Parse(List<TagNode> nodes)
        {
            var node = nodes.FirstOrDefault(n => n.TagName.Equals("PROL", StringComparison.OrdinalIgnoreCase));
            return node != null ? ProlTag.Parse(node) : null;
        }
    }
}
