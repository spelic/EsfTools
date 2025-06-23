using EsfCore.Esf;

namespace EsfCore.Tags
{
    public class CfieldTag
    {
        public string Row { get; set; }
        public string Column { get; set; }
        public string Type { get; set; }
        public string Bytes { get; set; }

        public string? Value { get; set; }

        public override string ToString() =>
            $"CFIELD [{Row},{Column}] {Type} {Bytes}B Value='{Value}'";

        public static CfieldTag Parse(TagNode node)
        {
            return new CfieldTag
            {
                Row = node.GetString("ROW"),
                Column = node.GetString("COLUMN"),
                Type = node.GetString("TYPE"),
                Bytes = node.GetString("BYTES"),
                Value = node.Content?.TrimStart('.').Trim()
            };
        }
    }
}