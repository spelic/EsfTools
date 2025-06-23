using EsfCore.Esf;

namespace EsfCore.Tags
{
    public class VfieldTag
    {
        public string Row { get; set; }
        public string Column { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Bytes { get; set; }

        public string? Value { get; set; }

        public override string ToString() =>
            $"VFIELD [{Row},{Column}] {Name} {Type} {Bytes}B Value='{Value}'";

        public static VfieldTag Parse(TagNode node)
        {
            return new VfieldTag
            {
                Row = node.GetString("ROW"),
                Column = node.GetString("COLUMN"),
                Name = node.GetString("NAME"),
                Type = node.GetString("TYPE"),
                Bytes = node.GetString("BYTES"),
                Value = node.Content?.TrimStart('.').Trim()
            };
        }
    }
}