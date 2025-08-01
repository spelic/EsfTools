using EsfParser.Builder;
using EsfParser.Esf;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EsfParser.Tags
{
    public class RecordTagParser : IEsfTagParser
    {
        public string TagName => "RECORD";

        public IEsfTagModel Parse(List<TagNode> nodes)
        {
            var records = nodes
                .Where(n => string.Equals(n.TagName, TagName, StringComparison.OrdinalIgnoreCase))
                .Select(RecordTag.Parse)
                .ToList();

            return new RecordTagCollection { Records = records };
        }
    }

  
}
