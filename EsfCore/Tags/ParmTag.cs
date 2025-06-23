using System.Text.Json.Serialization;
using EsfCore.Esf;
using EsfCore.Tags;

namespace EsfCore.Tags
{
    public class ParmTag
    {
        public string Name { get; set; }
        public ParamType ParmType { get; set; }
        public DataType Type { get; set; }
        public int? Bytes { get; set; }
        public int Decimals { get; set; }
        public string Desc { get; set; }

        public static ParmTag Parse(TagNode n)
        {
            var p = new ParmTag();
            p.Name = n.Attributes["NAME"].First();
            p.ParmType = Enum.Parse<ParamType>(n.Attributes.GetValueOrDefault("PARMTYPE")?.First() ?? "ITEM", true);
            p.Type = Enum.Parse<DataType>(n.Attributes.GetValueOrDefault("TYPE")?.First() ?? "CHA", true);
            if (n.Attributes.TryGetValue("BYTES", out var b)) p.Bytes = int.Parse(b.First());
            if (n.Attributes.TryGetValue("DECIMALS", out var d)) p.Decimals = int.Parse(d.First());
            p.Desc = n.Attributes.GetValueOrDefault("DESC")?.First();
            return p;
        }
    }
}
