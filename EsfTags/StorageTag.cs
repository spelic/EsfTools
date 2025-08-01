// StorageTag.cs
using System;
using System.Globalization;
using System.Linq;
using EsfCore.Tags;
using EsfCore.Esf;
using System.Text.Json.Serialization;

namespace EsfCore.Tags
{
    public class StorageTag : IEsfTagModel
    {
        [JsonIgnore] public string TagName => "STORAGE";

        public DateTime? Date { get; set; }
        public TimeSpan? Time { get; set; }

        public override string ToString() =>
            $"STORAGE (Date={Date:MM/dd/yyyy}, Time={Time:hh\\:mm\\:ss})";

        public static StorageTag Parse(TagNode node)
        {
            var t = new StorageTag();

            if (node.Attributes.TryGetValue("DATE", out var dl) &&
                DateTime.TryParseExact(dl.First(), "MM/dd/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var d))
            {
                t.Date = d;
            }

            if (node.Attributes.TryGetValue("TIME", out var tl) &&
                TimeSpan.TryParseExact(tl.First(), "hh\\:mm\\:ss", CultureInfo.InvariantCulture, out var ts))
            {
                t.Time = ts;
            }

            return t;
        }
    }
}


