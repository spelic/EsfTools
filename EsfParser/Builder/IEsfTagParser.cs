using System.Collections.Generic;
using EsfCore.Esf;
using EsfCore.Tags;

namespace EsfParser.Builder
{
    public interface IEsfTagParser
    {
        IEsfTagModel Parse(List<TagNode> nodes);
    }
}