using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using EsfCore.Tags;       // for ItemTag, RecordTagCollection, MapTagCollection

namespace EsfCore.Tags.Logic
{
    public class EsfTypeResolver : ITypeResolver
    {
        private readonly Dictionary<string, string> _globalTypes;
        private readonly Dictionary<string, RecordDefinition> _recordDefs;
        private readonly HashSet<string> _mapNames;

        private static readonly Dictionary<string, string> _clrToAlias = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
{
    { nameof(Int16),   "short"   },
    { nameof(Int32),   "int"     },
    { nameof(Int64),   "long"    },
    { nameof(UInt16),  "ushort"  },
    { nameof(UInt32),  "uint"    },
    { nameof(UInt64),  "ulong"   },
    { nameof(Single),  "float"   },
    { nameof(Double),  "double"  },
    { nameof(Decimal), "decimal" },
    { nameof(Boolean), "bool"    },
    { nameof(Char),    "char"    },
    { nameof(String),  "string"  },
    { nameof(Object),  "object"  },
    { nameof(Byte),    "byte"    },
    { nameof(SByte),   "sbyte"   }
};

        public EsfTypeResolver(
            ItemTagCollection items,
            RecordTagCollection records,
            MapTagCollection maps)
        {
            // build a map of ITEM-name → C# type
            _globalTypes = items.Items.ToDictionary(
                it => it.Name,
                it => it.Type.ToString().ToUpperInvariant() switch
                {
                    "BIN" => "int",
                    "NUM" => (it.Decimals > 0) ? "decimal" : "int",
                    "PACK" or "PACF" => (it.Decimals > 0) ? "decimal" : "int",
                    "CHA" or "DBCS" or "MIX" => "string",
                    _ => "string"
                },
                StringComparer.OrdinalIgnoreCase
            );

            // build a lookup of RECORD-name → its fields (name → type)
            _recordDefs = records.Records.ToDictionary(
                r => r.Name,
                r => new RecordDefinition(r),
                StringComparer.OrdinalIgnoreCase
            );

            // just track which top-level names are MAP classes
            _mapNames = new HashSet<string>(
                maps.Maps.Select(m => m.MapName),
                StringComparer.OrdinalIgnoreCase
            );
        }

        public string GetTypeOf(string esfName)
        {
            // strip subscripts: e.g. "FOO[3]" → "FOO"
            var bare = Regex.Replace(esfName, @"\[[^\]]+\]", "");

            // 1) global items?
            if (bare.StartsWith("GlobalItems."))
            {
                bare = bare.Substring(12);
                if (_globalTypes.TryGetValue(bare, out var gtype2))
                    return gtype2;
                else
                    throw new Exception("GlobalItems - invalid variable.: " + esfName);
            }

            // 1) workstor items? than parse bare and check in _recordDefs for a match
            //    qualified names like "Workstor.IS00R10.ZAPOREDJE"
            if (bare.StartsWith("Workstor.") && bare.Length > 10)
            {
                bare = bare.Substring(9);
                if (_globalTypes.TryGetValue(bare, out var gtype2))
                    return gtype2;
                else
                {
                    // check if it's a record field
                    var parts2 = bare.Split('.');
                    if (parts2.Length == 2 && _recordDefs.TryGetValue(parts2[0], out var recDef2) &&
                        recDef2.FieldTypes.TryGetValue(parts2[1], out var fldType2))
                    {
                        return fldType2;
                    }
                    throw new Exception("Workstor - invalid variable.: " + esfName);
                }
            }
            

            if (_globalTypes.TryGetValue(bare, out var gtype))
                return gtype;

            // 2) is it a record-field?
            //    qualified names like "IS00R10.ZAPOREDJE"
            var parts = bare.Split('.');
            if (parts.Length == 2
             && _recordDefs.TryGetValue(parts[0], out var recDef)
             && recDef.FieldTypes.TryGetValue(parts[1], out var fldType))
            {
                return fldType;
            }

            // 2) unqualified field: search all records for a matching field name
            foreach (var recDef2 in _recordDefs.Values)
            {
                if (recDef2.FieldTypes.TryGetValue(bare, out var fldDef))
                    return fldDef;
            }
            // 3) the record itself (copyFrom)
            if (_recordDefs.ContainsKey(bare))
                return bare;

            // 4) map classes
            if (_mapNames.Contains(bare))
                return bare;

            // special function, get fields by reflection from class SpecialFunctions
            if (bare.StartsWith("SpecialFunctions."))
            {
                bare = bare.Substring(17);
                var type = typeof(SpecialFunctions);
                var prop = type.GetProperty(bare);
                if (prop != null)
                {
                    var propType = prop.PropertyType;
                    if (propType.IsArray)
                        return propType.GetElementType().Name + "[]";
                    return _clrToAlias.TryGetValue(propType.Name, out var alias) ? alias : propType.Name;
                }
                else
                {
                    return "string";
                    //throw new Exception("SpecialFunctions - invalid function: " + esfName);
                }
            }

            // 5) numeric literal?
            if (decimal.TryParse(esfName, out _))
                return esfName.Contains('.') ? "decimal" : "int";

            // 6) string literal?
            if ((esfName.StartsWith("\"") && esfName.EndsWith("\"")) ||
                (esfName.StartsWith("'") && esfName.EndsWith("'")))
                return "string";

            // fallback
            return "string";
        }

        // helper to hold record → (field → type)
        private class RecordDefinition
        {
            public Dictionary<string, string> FieldTypes { get; }
            public RecordDefinition(RecordTag r)
            {
                FieldTypes = r.Items
                              .Where(f => f.Name != "*")
                              .ToDictionary(
                                  f => f.Name,
                                  f => f.Type.ToUpperInvariant() switch
                                  {
                                      "BIN" => "int",
                                      "NUM" => (int.TryParse(f.Decimals, out var d) && d > 0)
                                                    ? "decimal" : "int",
                                      "PACK" or "PACF" => (int.TryParse(f.Decimals, out var d2) && d2 > 0)
                                                    ? "decimal" : "int",
                                      "CHA" or "DBCS" or "MIX" => "string",
                                      _ => "string"
                                  },
                                  StringComparer.OrdinalIgnoreCase
                              );
            }
        }
    }
}
