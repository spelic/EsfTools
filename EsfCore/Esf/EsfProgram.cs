using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using EsfCore.Tags;

namespace EsfCore.Esf
{
    public class EsfProgram
    {
        [JsonIgnore]
        public List<TagNode> Nodes { get; set; } = new();

        public List<IEsfTagModel> Tags { get; set; } = new();

        public T GetTag<T>() where T : class, IEsfTagModel => Tags.Find(t => t is T) as T;

        public override string ToString() => System.Text.Json.JsonSerializer.Serialize(this, new JsonSerializerOptions
        {
            WriteIndented = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        });
    }
}