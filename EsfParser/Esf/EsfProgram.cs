using EsfParser.CodeGen;
using EsfParser.Parser.Logic.Statements;
using EsfParser.Tags;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace EsfParser.Esf
{
    public class EsfProgram
    {
        [JsonIgnore]
        public List<TagNode> Nodes { get; set; } = new();

        public EzeeTag? Ezee { get; set; }
        public ProgramTag? Program { get; set; }
        public FuncTagCollection Functions { get; set; } = new();
        public List<IStatement> GetAllFunctionStatements() => Functions.AllStatements;
        public MapTagCollection Maps { get; set; } = new();
        public RecordTagCollection Records { get; set; } = new();
        public ItemTagCollection Items { get; set; } = new();
        public TableTagCollection Tables { get; set; } = new();

        /// <summary>
        /// The actual RecordTag corresponding to PROGRAM.WORKSTOR
        /// (e.g. if WORKSTOR="IS00W01", this will return the IS00W01 record).
        /// </summary>
        [JsonIgnore]
        public RecordTag? WorkstorRecord =>
                                            Program == null
                                                         ? null
                                                         : Records.Records
                                                             .FirstOrDefault(r =>
                                                                string.Equals(r.Name, Program.Workstor, StringComparison.OrdinalIgnoreCase)
                                                                                 );

        /// <summary>
        /// All MapTag instances in the program’s MAPGROUP.
        /// </summary>
        [JsonIgnore]
        public IEnumerable<MapTag> ProgramMaps =>
                                                     Program == null
                                                     ? Enumerable.Empty<MapTag>()
                                                     : Maps.Maps
                                                                .Where(m =>
                                                                string.Equals(m.GrpName, Program.MapGroup, StringComparison.OrdinalIgnoreCase)
                                                         );

        ////public List<IEsfTagModel> Tags => new IEsfTagModel[] {
        //    Ezee, Program, Functions, Maps, Records, Items, Tables
        //}.Where(t => t != null).ToList();
        //public T? GetTag<T>() where T : class, IEsfTagModel
        //    => Tags.OfType<T>().FirstOrDefault();
        public void AddTag(IEsfTagModel tag)
        {
            if (tag is EzeeTag e) Ezee = e;
            else if (tag is ProgramTag p) Program = p;
            else if (tag is FuncTagCollection f) Functions.Functions.AddRange(f.Functions);
            else if (tag is MapTagCollection m) Maps.Maps.AddRange(m.Maps);
            else if (tag is RecordTagCollection r) Records.Records.AddRange(r.Records);
            else if (tag is ItemTagCollection i) Items.Items.AddRange(i.Items);
            else if (tag is TableTagCollection t) Tables.Tables.AddRange(t.Tables);
        }
    }

}