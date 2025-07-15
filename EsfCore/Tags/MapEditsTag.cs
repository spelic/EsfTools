using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using EsfCore.Esf;

namespace EsfCore.Tags
{
    public class MapEditsTag : IEsfTagModel
    {
        [JsonIgnore] public string TagName => "MAPEDITS";

        [JsonPropertyName("currencySymbolSupported")]
        public bool? CurrSymb { get; set; }

        [JsonPropertyName("dateForm")]
        public string? DateForm { get; set; }

        [JsonPropertyName("editRoutine")]
        public string? EditRtn { get; set; }

        [JsonPropertyName("fillChar")]
        public string? FillChar { get; set; }

        [JsonPropertyName("fieldFold")]
        public string? FldFold { get; set; }

        [JsonPropertyName("hexEdit")]
        public bool? HexEdit { get; set; }

        [JsonPropertyName("inputRequired")]
        public bool? InputReq { get; set; }

        [JsonPropertyName("justify")]
        public string? Justify { get; set; }

        [JsonPropertyName("minInput")]
        public int? MinInput { get; set; }

        [JsonPropertyName("numSep")]
        public bool? NumSep { get; set; }

        [JsonPropertyName("rangeLow")]
        public string? RangeLow { get; set; }

        [JsonPropertyName("rangeHigh")]
        public string? RangeHigh { get; set; }

        [JsonPropertyName("sign")]
        public string? Sign { get; set; }

        [JsonPropertyName("soSi")]
        public bool? Sosi { get; set; }

        [JsonPropertyName("zeroEdit")]
        public bool? ZeroEdit { get; set; }

        public override string ToString()
        {
            var parts = new List<string>();
            if (CurrSymb.HasValue) parts.Add($"CURRSYMB={(CurrSymb.Value ? "Y" : "N")}");
            if (DateForm != null) parts.Add($"DATEFORM={DateForm}");
            if (EditRtn != null) parts.Add($"EDITRTN={EditRtn}");
            if (FillChar != null) parts.Add($"FILLCHAR={FillChar}");
            if (FldFold != null) parts.Add($"FLDFOLD={FldFold}");
            if (HexEdit.HasValue) parts.Add($"HEXEDIT={(HexEdit.Value ? "Y" : "N")}");
            if (InputReq.HasValue) parts.Add($"INPUTREQ={(InputReq.Value ? "Y" : "N")}");
            if (Justify != null) parts.Add($"JUSTIFY={Justify}");
            if (MinInput.HasValue) parts.Add($"MININPUT={MinInput}");
            if (NumSep.HasValue) parts.Add($"NUMSEP={(NumSep.Value ? "Y" : "N")}");
            if (RangeLow != null || RangeHigh != null)
                parts.Add($"RANGE={RangeLow}–{RangeHigh}");
            if (Sign != null) parts.Add($"SIGN={Sign}");
            if (Sosi.HasValue) parts.Add($"SOSI={(Sosi.Value ? "Y" : "N")}");
            if (ZeroEdit.HasValue) parts.Add($"ZEROEDIT={(ZeroEdit.Value ? "Y" : "N")}");

            return "MAPEDITS: " + string.Join(", ", parts);
        }

        public static MapEditsTag Parse(TagNode node)
        {
            var tag = new MapEditsTag();

            foreach (var kv in node.Attributes)
            {
                var val = kv.Value.FirstOrDefault();
                if (val == null) continue;

                switch (kv.Key.ToUpperInvariant())
                {
                    case "CURRSYMB":
                        tag.CurrSymb = val.Equals("Y", StringComparison.OrdinalIgnoreCase);
                        break;
                    case "DATEFORM":
                        tag.DateForm = val;
                        break;
                    case "EDITRTN":
                        tag.EditRtn = val;
                        break;
                    case "FILLCHAR":
                        tag.FillChar = val;
                        break;
                    case "FLDFOLD":
                        tag.FldFold = val;
                        break;
                    case "HEXEDIT":
                        tag.HexEdit = val.Equals("Y", StringComparison.OrdinalIgnoreCase);
                        break;
                    case "INPUTREQ":
                        tag.InputReq = val.Equals("Y", StringComparison.OrdinalIgnoreCase);
                        break;
                    case "JUSTIFY":
                        tag.Justify = val;
                        break;
                    case "MININPUT":
                        if (int.TryParse(val, out var mi)) tag.MinInput = mi;
                        break;
                    case "NUMSEP":
                        tag.NumSep = val.Equals("Y", StringComparison.OrdinalIgnoreCase);
                        break;
                    case "RANGE":
                        // val might be "low high"
                        var parts = val.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
                        tag.RangeLow = parts.Length > 0 ? parts[0] : null;
                        tag.RangeHigh = parts.Length > 1 ? parts[1] : null;
                        break;
                    case "SIGN":
                        tag.Sign = val;
                        break;
                    case "SOSI":
                        tag.Sosi = val.Equals("Y", StringComparison.OrdinalIgnoreCase);
                        break;
                    case "ZEROEDIT":
                        tag.ZeroEdit = val.Equals("Y", StringComparison.OrdinalIgnoreCase);
                        break;
                }
            }

            return tag;
        }
    }
}
