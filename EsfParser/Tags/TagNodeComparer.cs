using EsfParser.Tags;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public static class TagNodeComparer
{
    /// <summary>
    /// Deep-compare two lists of TagNodes (the “forest” you get from Parse).
    /// </summary>
    public static bool EqualTrees(
        IReadOnlyList<TagNode> expected,
        IReadOnlyList<TagNode> actual,
        out string diff)
    {
        var sb = new StringBuilder();
        bool ok = CompareNodeLists(expected, actual, sb, "/");
        diff = sb.ToString();
        return ok;
    }

    // ───────────────── helpers ─────────────────────────────────────────────

    private static bool CompareNodeLists(
        IReadOnlyList<TagNode> a, IReadOnlyList<TagNode> b,
        StringBuilder sb, string path)
    {
        if (a.Count != b.Count)
        {
            sb.AppendLine($"{path}  ⚠ Count mismatch: {a.Count} vs {b.Count}");
            return false;
        }

        for (int i = 0; i < a.Count; i++)
        {
            if (!CompareNode(a[i], b[i], sb, $"{path}{a[i].TagName}[{i}]/"))
                return false;                       // stop at first diff
        }
        return true;
    }

    private static bool CompareNode(
        TagNode x, TagNode y, StringBuilder sb, string path)
    {
        // tag name
        if (!x.TagName.Equals(y.TagName, StringComparison.OrdinalIgnoreCase))
        {
            sb.AppendLine($"{path}  ⚠ TagName mismatch: '{x.TagName}' vs '{y.TagName}'");
            return false;
        }

        // attributes (order-insensitive: key + value set)
        if (!AttrEq(x.Attributes, y.Attributes, out var attrDiff))
        {
            sb.AppendLine($"{path}  ⚠ Attribute mismatch: {attrDiff}");
            return false;
        }

        // content
        if (!string.Equals(x.Content?.TrimEnd(), y.Content?.TrimEnd(),
                           StringComparison.Ordinal))
        {
            sb.AppendLine($"{path}  ⚠ Content mismatch");
            return false;
        }

        if (x.StartLine != y.StartLine || x.EndLine != y.EndLine)
        {
            sb.AppendLine($"{path}  ⚠ Line range mismatch: {x.StartLine}-{x.EndLine} vs {y.StartLine}-{y.EndLine}");
            return false;
        }
        // children
        return CompareNodeLists(x.Children, y.Children, sb, path);
    }

    private static bool AttrEq(
        IDictionary<string, List<string>> a,
        IDictionary<string, List<string>> b,
        out string diff)
    {
        diff = "";
        if (a.Count != b.Count)
        {
            diff = $"different attribute count ({a.Count} vs {b.Count})";
            return false;
        }

        foreach (var (key, valsA) in a)
        {
            if (!b.TryGetValue(key, out var valsB))
            {
                diff = $"missing key '{key}'";
                return false;
            }
            if (valsA.Count != valsB.Count ||
                !valsA.SequenceEqual(valsB, StringComparer.Ordinal))
            {
                diff = $"key '{key}' values differ";
                return false;
            }
        }
        return true;
    }
}
