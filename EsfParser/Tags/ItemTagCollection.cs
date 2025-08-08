using EsfParser.CodeGen;
using EsfParser.Esf;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace EsfParser.Tags
{
    public class ItemTagCollection : IEsfTagModel
    {
        [JsonIgnore]
        public string TagName => "ITEMS";

        public List<ItemTag> Items { get; set; } = new();

        public override string ToString()
        {
            // foreach itemtag detailed description output
            if (Items.Count == 0)
                return $"{TagName}: No items defined";
            var details = new List<string>();
            foreach (var item in Items)
            {
                details.Add(item.ToString());
            }
            return $"{TagName}: {Items.Count} defined\n" + string.Join("\n", details);
        }

        public string ToCSharp()
        {
            if (Items == null || Items.Count == 0)
            {
                // Return an empty list initializer when there are no items.
                return "public static class GlobalItems {}";
            }
            var sb = new System.Text.StringBuilder();
            // GlobalItems class
            sb.AppendLine(CSharpUtils.Indent(1) + "public static class GlobalItems");
            sb.AppendLine(CSharpUtils.Indent(1) + "{");
            foreach (var fld in Items)
            {
                var csType = CSharpUtils.MapCsType(fld.Type.ToString(), fld.Decimals);
                var name = CSharpUtils.CleanName(fld.Name);
                if (!string.IsNullOrWhiteSpace(fld.Description))
                {
                    sb.AppendLine(CSharpUtils.Indent(2) + "/// <summary>");
                    sb.AppendLine(CSharpUtils.Indent(2) + $"/// {fld.Description}");
                    sb.AppendLine(CSharpUtils.Indent(2) + "/// </summary>");
                }
                sb.AppendLine(CSharpUtils.Indent(2) + $"public static {csType} {name};");
                sb.AppendLine();
            }
            sb.AppendLine(CSharpUtils.Indent(1) + "}");
            var code = sb.ToString();
            return code;
        }
    }
}