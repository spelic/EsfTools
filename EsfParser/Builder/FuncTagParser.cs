using EsfParser.Builder;
using EsfParser.Esf;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EsfParser.Tags
{
    public class FuncTagParser : IEsfTagParser
    {
        public string TagName => "FUNC";

        public IEsfTagModel Parse(List<TagNode> nodes)
        {
            var funcs = nodes
                .Where(n => string.Equals(n.TagName, TagName, StringComparison.OrdinalIgnoreCase))
                .Select(FuncTag.Parse)
                .ToList();

            return new FuncTagCollection { Functions = funcs };
        }
    }
}
