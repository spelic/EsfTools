using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Text.Json.Serialization;
using EsfCore.Esf;

namespace EsfCore.Tags
{
    public class ProgramTag : IEsfTagModel
    {
        [JsonIgnore] public string TagName => "PROGRAM";

        // Attributes
        [JsonPropertyName("name")] public string Name { get; set; }
        [JsonPropertyName("date")] public string Date { get; set; }
        [JsonPropertyName("time")] public string Time { get; set; }
        [JsonPropertyName("type")] public string Type { get; set; }
        [JsonPropertyName("workstor")] public string Workstor { get; set; }
        [JsonPropertyName("mapgroup")] public string MapGroup { get; set; }
        [JsonPropertyName("helpkey")] public string HelpKey { get; set; }
        [JsonPropertyName("helpgrp")] public string HelpGroup { get; set; }
        [JsonPropertyName("implicit")] public string Implicit { get; set; }
        [JsonPropertyName("bypkey")] public List<string> BypKey { get; set; } = new();
        [JsonPropertyName("firstmap")] public string FirstMap { get; set; }
        [JsonPropertyName("firstui")] public string FirstUI { get; set; }
        [JsonPropertyName("msgtable")] public string MsgTable { get; set; }
        [JsonPropertyName("psb")] public string PSB { get; set; }
        [JsonPropertyName("pfequate")] public string PfEquate { get; set; }
        [JsonPropertyName("execmode")] public string ExecMode { get; set; }

        // Child Tags
        [JsonPropertyName("mainfun")] public MainfunTag Mainfun { get; set; }
        [JsonPropertyName("tabrec")] public List<TabrecTag> TabRecs { get; set; } = new();
        [JsonPropertyName("callparm")] public List<CallparmTag> CallParms { get; set; } = new();
        [JsonPropertyName("prol")] public List<ProlTag> Prols { get; set; } = new();
        [JsonPropertyName("genopts")] public List<GenoptsTag> GenOpts { get; set; } = new();
        [JsonPropertyName("gentable")] public List<TagNode> GenTables { get; set; } = new();
        [JsonPropertyName("funcs")] public List<FuncTag> Funcs { get; set; } = new();

        public static ProgramTag Parse(TagNode node)
        {
            var tag = new ProgramTag();

            string GetAttr(string key) =>
                node.Attributes.TryGetValue(key, out var list) && list.Count > 0 ? list[0] : null;

            tag.Name = GetAttr("NAME");
            tag.Date = GetAttr("DATE");
            tag.Time = GetAttr("TIME");
            tag.Type = GetAttr("TYPE");
            tag.Workstor = GetAttr("WORKSTOR");
            tag.MapGroup = GetAttr("MAPGROUP");
            tag.HelpKey = GetAttr("HELPKEY");
            tag.HelpGroup = GetAttr("HELPGRP");
            tag.Implicit = GetAttr("IMPLICIT");
            tag.FirstMap = GetAttr("FIRSTMAP");
            tag.FirstUI = GetAttr("FIRSTUI");
            tag.MsgTable = GetAttr("MSGTABLE");
            tag.PSB = GetAttr("PSB");
            tag.PfEquate = GetAttr("PFEQUATE");
            tag.ExecMode = GetAttr("EXECMODE");

            if (node.Attributes.TryGetValue("BYPKEY", out var keys))
            {
                tag.BypKey = keys.SelectMany(k => k.Split(' ', System.StringSplitOptions.RemoveEmptyEntries)).ToList();
            }

            foreach (var child in node.Children)
            {
                switch (child.TagName.ToUpperInvariant())
                {
                    case "MAINFUN": tag.Mainfun = MainfunTag.Parse(child); break;
                    case "TABREC": tag.TabRecs.Add(TabrecTag.Parse(child)); break;
                    case "CALLPARM": tag.CallParms.Add(CallparmTag.Parse(child)); break;
                    case "PROL": tag.Prols.Add(ProlTag.Parse(child)); break;
                    case "GENOPTS": tag.GenOpts.Add(GenoptsTag.Parse(child)); break;
                    case "GENTABLE": tag.GenTables.Add(child); break;
                    case "FUNC": tag.Funcs.Add(FuncTag.Parse(child)); break;
                }
            }

            return tag;
        }

        public override string ToString()
        {
            var lines = new List<string>
            {
                $"ProgramTag:",
                $"  Name       : {Name}",
                $"  Date       : {Date}",
                $"  Time       : {Time}",
                $"  Type       : {Type}",
                $"  Workstor   : {Workstor}",
                $"  MapGroup   : {MapGroup}",
                $"  HelpKey    : {HelpKey}",
                $"  HelpGroup  : {HelpGroup}",
                $"  Implicit   : {Implicit}",
                $"  BypKey     : {string.Join(", ", BypKey)}",
                $"  FirstMap   : {FirstMap}",
                $"  FirstUI    : {FirstUI}",
                $"  MsgTable   : {MsgTable}",
                $"  PSB        : {PSB}",
                $"  PfEquate   : {PfEquate}",
                $"  ExecMode   : {ExecMode}",
                $"  MAINFUN    : {(Mainfun != null ? Mainfun.Name : "(none)")}",
                $"  TABRECs    : {TabRecs.Count}",
                $"  CALLPARMs  : {CallParms.Count}",
                $"  PROLs      : {Prols.Count}",
                $"  GENOPTS    : {GenOpts.Count}",
                $"  GENTABLEs  : {GenTables.Count}",
                $"  FUNCs      : {Funcs.Count}"
            };

            return string.Join('\n', lines);
        }
    }
}
