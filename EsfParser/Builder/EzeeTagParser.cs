
using EsfParser.Builder;
using EsfParser.Esf;
using EsfParser.Tags;
using System.Text.RegularExpressions;

public class EzeeTagParser : IEsfTagParser
{
    public string TagName => "EZEE";

    public IEsfTagModel Parse(List<TagNode> nodes)
    {
        var first = nodes.FirstOrDefault(n => n.TagName.Equals("EZEE", StringComparison.OrdinalIgnoreCase));
        if (first == null) 
            throw new InvalidOperationException("No EZEE tag found in nodes.");

        var tag = new EzeeTag { TagName = "EZEE" };
        var rx = new Regex(@"(?<version>\d+)?\s+(?<date>\d{2}/\d{2}/\d{2})?\s+(?<time>\d{2}:\d{2}:\d{2})?");
        var m = rx.Match(first.Content);
        if (m.Success)
        {
            tag.Version = m.Groups["version"].Value;
            tag.CreationDate = m.Groups["date"].Value;
            tag.CreationTime = m.Groups["time"].Value;
        }

        return tag;
    }
}