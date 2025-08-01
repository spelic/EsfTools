using EsfCore.Tags;
using System.Collections.Generic;

namespace EsfTags
{
    public static class TagNodeExtensions
    {
        public static string GetString(this TagNode node, string key)
        {
            if (node.Attributes.TryGetValue(key.ToUpperInvariant(), out var values))
            {
                return values.Count > 0 ? values[0] : string.Empty;
            }
            return string.Empty;
        }

        public static List<string> GetList(this TagNode node, string key)
        {
            return node.Attributes.TryGetValue(key.ToUpperInvariant(), out var values) ? values : new List<string>();
        }
    }
}
