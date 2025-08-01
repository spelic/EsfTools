using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace EsfCore.Tags
{
    public class MapTagCollection : Esf.IEsfTagModel
    {
        [JsonIgnore]
        public string TagName => "MAPS";

        public List<MapTag> Maps { get; set; } = new();

        public override string ToString()
        {
            
            if (Maps.Count == 0)
                return $"{TagName}: No maps defined";
            var result = $"{TagName}: {Maps.Count} defined\n";
            foreach (var map in Maps)
            {
                result += $"  - {map.GrpName}/{map.MapName} ({map.Date} {map.Time})\n";
                // add detailed information for each cfield and vfield
                if (map.Cfields.Count > 0)
                {
                    result += "    Cfields:\n";
                    foreach (var cfield in map.Cfields)
                    {
                        result += $"      - {cfield}\n";
                    }
                }
                else
                {
                    result += "    Cfields: None\n";
                }
                if (map.Vfields.Count > 0)
                {
                    result += "    Vfields:\n";
                    foreach (var vfield in map.Vfields)
                    {
                        result += $"      - {vfield}\n";
                    }
                }
                else
                {
                    result += "    Vfields: None\n";
                }
                result += $"    Cfields: {map.Cfields.Count}, Vfields: {map.Vfields.Count}\n";
            }

            return result;
        }
    }
}