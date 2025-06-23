using EsfCore.Esf;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace EsfCore.Tags
{
    public class EsfProgram
    {
        [JsonIgnore]
        public List<TagNode> Nodes { get; set; } = new();

        public EzeeTag? Ezee { get; set; }
        public ProgramTag? Program { get; set; }
        public FuncTagCollection Funcs { get; set; } = new();
        public MapTagCollection Maps { get; set; } = new();
        public RecordTagCollection Records { get; set; } = new();
        public ItemTagCollection Items { get; set; } = new();
        public TbleTagCollection Tables { get; set; } = new();

        public List<IEsfTagModel> Tags => new IEsfTagModel[] {
        Ezee, Program, Funcs, Maps, Records, Items, Tables
    }.Where(t => t != null).ToList();

        public T? GetTag<T>() where T : class, IEsfTagModel => Tags.OfType<T>().FirstOrDefault();

        public override string ToString()
        {
            var sb = new StringBuilder();
            foreach (var tag in Tags)
            {
                sb.AppendLine(tag?.ToString());
            }
            return sb.ToString();
        }

        public void AddTag(IEsfTagModel tag)
        {
            Console.WriteLine($"Tag runtime type: {tag.GetType().FullName}, from: {tag.GetType().Assembly.FullName}");
            if (tag is EzeeTag e)
            {
                Ezee = e;
            }
            else if (tag is ProgramTag p)
            {
                Program = p;
            }
            else if (tag is FuncTagCollection f)
            {
                Funcs.Functions.AddRange(f.Functions);
            }
            else if (tag is MapTagCollection m)
            {
                Maps.Maps.AddRange(m.Maps);
            }
            else if (tag is RecordTagCollection r)
            {
                Records.Records.AddRange(r.Records);
            }
            else if (tag is ItemTagCollection i)
            {
                Items.Items.AddRange(i.Items);
            }
            else if (tag is TbleTagCollection t)
            {
                Tables.Tables.AddRange(t.Tables);
            }
        }
    }

}