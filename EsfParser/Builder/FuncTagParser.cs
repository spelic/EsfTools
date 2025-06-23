using System;
using System.Collections.Generic;
using System.Linq;
using EsfCore.Esf;
using EsfCore.Tags;

namespace EsfParser.Builder
{
    public class FuncTagParser : IEsfTagParser
    {
        public IEsfTagModel Parse(List<TagNode> nodes)
        {
            // This parser returns null; use batch parser in builder instead
            return null;
        }

        public static List<IEsfTagModel> ParseAll(List<TagNode> nodes)
        {
            return nodes
                .Where(n => n.TagName.Equals("FUNC", StringComparison.OrdinalIgnoreCase))
                .Select(n => (IEsfTagModel)FuncTag.Parse(n))
                .ToList();
        }
    }
}
