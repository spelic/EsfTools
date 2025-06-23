using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.Json.Serialization;
using EsfCore.Esf;

namespace EsfCore.Tags
{
    public class FuncTag : IEsfTagModel
    {
        [JsonIgnore] public string TagName => "FUNC";

        // --- ATTRIBUTES OF :FUNC
        public string Name { get; set; }
        public DateTime? Date { get; set; }
        public TimeSpan? Time { get; set; }
        public string Option { get; set; }
        public string ObjectName { get; set; }
        public string ErrRtn { get; set; }
        public bool ExecBld { get; set; }
        public SqlModelType Model { get; set; }
        public bool Refine { get; set; }
        public bool SingRow { get; set; }
        public string UpdFunc { get; set; }
        public bool WithHold { get; set; }
        public string Desc { get; set; }

        // --- CHILD PARTS
        public List<ParmTag> Parameters { get; } = new();
        public List<StorageTag> Storages { get; } = new();
        public ReturnTag Return { get; set; }
        public List<string> BeforeLogic { get; } = new();
        public List<string> AfterLogic { get; } = new();
        public List<SqlClause> SqlClauses { get; } = new();
        public DlicallTag DliCall { get; set; }
        public List<SsaTag> Ssas { get; } = new();
        public List<QualTag> Quals { get; } = new();

        public override string ToString() =>
            $"{TagName} {Name} [{Option} on {ObjectName}], {Parameters.Count} parms, {BeforeLogic.Count} before-lines, {AfterLogic.Count} after-lines";

        public static FuncTag Parse(TagNode node)
        {
            var t = new FuncTag();

            // simple helpers:
            string getAttr(string k) => node.Attributes.TryGetValue(k, out var vs) ? vs.First() : null;
            bool getBool(string k, bool def = false) =>
                bool.TryParse(getAttr(k), out var b) ? b : def;
            TEnum getEnum<TEnum>(string k, TEnum def = default) where TEnum : struct =>
                Enum.TryParse(getAttr(k), true, out TEnum e) ? e : def;
            DateTime? parseDate(string s) =>
                DateTime.TryParseExact(s, "MM/dd/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var d) ? d : (DateTime?)null;
            TimeSpan? parseTime(string s) =>
                TimeSpan.TryParseExact(s, "hh\\:mm\\:ss", CultureInfo.InvariantCulture, out var ts) ? ts : (TimeSpan?)null;

            // --- ATTRIBUTES
            t.Name = getAttr("NAME");
            t.Date = parseDate(getAttr("DATE"));
            t.Time = parseTime(getAttr("TIME"));
            t.Option = getAttr("OPTION");
            t.ObjectName = getAttr("OBJECT");
            t.ErrRtn = getAttr("ERRRTN");
            t.ExecBld = getBool("EXECBLD");
            t.Model = getEnum<SqlModelType>("MODEL", SqlModelType.NONE);
            t.Refine = getBool("REFINE");
            t.SingRow = getBool("SINGROW", true);
            t.UpdFunc = getAttr("UPDFUNC");
            t.WithHold = getBool("WITHHOLD", true);
            t.Desc = getAttr("DESC");

            // --- CHILDREN
            foreach (var child in node.Children)
            {
                switch (child.TagName)
                {
                    case "PARM":
                        t.Parameters.Add(ParmTag.Parse(child));
                        break;
                    case "STORAGE":
                        t.Storages.Add(StorageTag.Parse(child));
                        break;
                    case "RETURN":
                        t.Return = ReturnTag.Parse(child);
                        break;
                    case "BEFORE":
                        t.BeforeLogic.AddRange(child.Content.Split('\n').Select(l => l.Trim()));
                        break;
                    case "AFTER":
                        t.AfterLogic.AddRange(child.Content.Split('\n').Select(l => l.Trim()));
                        break;
                    case "SQL":
                        t.SqlClauses.Add(SqlClause.Parse(child));
                        break;
                    case "DLICALL":
                        t.DliCall = DlicallTag.Parse(child);
                        break;
                    case "SSA":
                        t.Ssas.Add(SsaTag.Parse(child));
                        break;
                    case "QUAL":
                        t.Quals.Add(QualTag.Parse(child));
                        break;
                }
            }

            return t;
        }
    }

    // -------------------------------------------------
    // support types (one per child tag; show just PARM for brevity)
  

    // enums for MODEL, PARAMTYPE, DATATYPE
    public enum SqlModelType { NONE, DELETE, UPDATE }
    public enum ParamType { RECORD, ITEM, MAPITEM, SQLITEM }
    public enum DataType
    {
        BIN, CHA, DBCS, HEX, MIX,
        NUM, NUMC, PACK, PACF,
        UNICODE, ANYCHAR, ANYNUMERIC
    }
}
