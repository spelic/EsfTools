

namespace EsfParser.Tags
{
    public class RecordItemTag
    {
        public string Name { get; set; } = string.Empty;
        public string Level { get; set; } = string.Empty;
        public string Occurs { get; set; } = string.Empty;
        public string Usage { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string Bytes { get; set; } = string.Empty;
        public int Decimals { get; set; }
        public string EvenSql { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public override string ToString()
        {
            return $"{Name} - {Description} ({Type}, {Bytes} bytes, level {Level}, occurs {Occurs})";
        }

        public static RecordItemTag Parse(TagNode node)
        {
            
            return new RecordItemTag
            {
                Name = node.GetString("NAME"),
                Level = node.GetString("LEVEL"),
                Occurs = node.GetString("OCCURS"),
                Usage = node.GetString("USAGE"),
                Type = node.GetString("TYPE"),
                Bytes = node.GetString("BYTES"),
                Decimals = (node.GetString("DECIMALS").Trim() == "") ? 0 : int.Parse(node.GetString("DECIMALS").Trim()),
                EvenSql = node.GetString("EVENSQL"),
                Description = node.GetString("DESC")
            }; 
        }
    }
}