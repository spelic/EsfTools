using System.Collections.Generic;
using System.Linq;
using System.Text;
using EsfCore.Esf;

namespace EsfCore.Tags
{
    public class RecordTag : IEsfTagModel
    {
        public string Name { get; set; } = string.Empty;
        public string Date { get; set; } = string.Empty;
        public string Time { get; set; } = string.Empty;
        public string Org { get; set; } = string.Empty;
        public string Usage { get; set; } = string.Empty;
        public string Prol { get; set; } = string.Empty;

        public List<RecordItemTag> Items { get; set; } = new();

        public string TagName => "RECORD";

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine($":RECORD {Name} ({Org})");
            sb.AppendLine($"  Date: {Date}  Time: {Time}");
            if (!string.IsNullOrWhiteSpace(Prol))
                sb.AppendLine($"  Prol: {Prol}");
            foreach (var item in Items)
            {
                sb.AppendLine($"  - {item}");
            }
            return sb.ToString();
        }

        public static RecordTag Parse(TagNode node)
        {
            var tag = new RecordTag
            {
                Name = node.GetString("NAME"),
                Date = node.GetString("DATE"),
                Time = node.GetString("TIME"),
                Org = node.GetString("ORG"),
                Usage = node.GetString("USAGE")
            };

            foreach (var child in node.Children)
            {
                switch (child.TagName.ToUpperInvariant())
                {
                    case "PROL":
                        tag.Prol = child.Content?.Trim() ?? "";
                        break;
                    case "RECDITEM":
                        tag.Items.Add(RecordItemTag.Parse(child));
                        break;
                }
            }

            return tag;
        }
    }
}