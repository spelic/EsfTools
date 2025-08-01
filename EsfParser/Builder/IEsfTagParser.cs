using System.Collections.Generic;
using EsfParser.Esf;
using EsfParser.Tags;

namespace EsfParser.Builder
{
    public interface IEsfTagParser
    {
        IEsfTagModel Parse(List<TagNode> nodes);
    }
}