﻿// New File: EsfParser/Builder/ProgramTagParser.cs
using System;
using System.Collections.Generic;
using System.Linq;
using EsfCore.Esf;
using EsfCore.Tags;

namespace EsfParser.Builder
{
    public class ProgramTagParser : IEsfTagParser
    {
        public IEsfTagModel Parse(List<TagNode> nodes)
        {
            var node = nodes.FirstOrDefault(n => n.TagName.Equals("PROGRAM", StringComparison.OrdinalIgnoreCase));
            return node != null ? ProgramTag.Parse(node) : null;
        }
    }
}