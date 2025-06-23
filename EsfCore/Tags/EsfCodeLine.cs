namespace EsfCore.Tags
{
    public class EsfCodeLine
    {
        public string Original { get; set; }
        public string Translated { get; set; }

        public override string ToString() => $"{Original} => {Translated}";
    }
}