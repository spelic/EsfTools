using EsfCore.Esf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace EsfTags
{
    public enum FieldColor { MONO, BLUE, RED, PINK, GREEN, TURQUOISE, YELLOW, WHITE }
    public enum CursorMode { N, Y }
    public enum AttrDataType { ALPHA, NUMERIC }
    public enum DetectMode { N, Y }
    public enum EnterMode { N, Y }
    public enum FillMode { N, Y }
    public enum HiliteMode { NOHILITE, BLINK, RVIDEO, USCORE }
    public enum IntenseMode { NORMAL, DARK, BRIGHT }
    public enum MdtMode { N, Y }
    public enum ProtectMode { UNPROTECT, PROTECT, ASKIP }

    [Flags]
    public enum OutlineMode
    {
        NOUTLINE = 0,
        BOX = 1 << 0,
        ORIGHT = 1 << 1,
        OLEFT = 1 << 2,
        OOVER = 1 << 3,
        OUNDER = 1 << 4
    }

    public class CattrTag : IEsfTagModel
    {
        [JsonIgnore] public string TagName => "CATTR";

        [JsonPropertyName("color")]
        public FieldColor Color { get; set; } = FieldColor.MONO;

        [JsonPropertyName("cursor")]
        public CursorMode Cursor { get; set; } = CursorMode.N;

        [JsonPropertyName("data")]
        public AttrDataType Data { get; set; } = AttrDataType.ALPHA;

        [JsonPropertyName("detect")]
        public DetectMode Detect { get; set; } = DetectMode.N;

        [JsonPropertyName("enter")]
        public EnterMode Enter { get; set; } = EnterMode.N;

        [JsonPropertyName("fill")]
        public FillMode Fill { get; set; } = FillMode.N;

        [JsonPropertyName("hilite")]
        public HiliteMode Hilite { get; set; } = HiliteMode.NOHILITE;

        [JsonPropertyName("intense")]
        public IntenseMode Intense { get; set; } = IntenseMode.NORMAL;

        [JsonPropertyName("mdt")]
        public MdtMode Mdt { get; set; } = MdtMode.N;

        [JsonPropertyName("outline")]
        public OutlineMode Outline { get; set; } = OutlineMode.NOUTLINE;

        [JsonPropertyName("protect")]
        public ProtectMode Protect { get; set; } = ProtectMode.ASKIP;

        public override string ToString()
        {
            return $"CATTR: Color={Color}, Cursor={Cursor}, Data={Data}, Detect={Detect}, " +
                   $"Enter={Enter}, Fill={Fill}, Hilite={Hilite}, Intense={Intense}, " +
                   $"MDT={Mdt}, Outline={Outline}, Protect={Protect}";
        }

        public static CattrTag Parse(TagNode node)
        {
            T ParseEnum<T>(string key, T @default) where T : struct, Enum
            {
                if (node.Attributes.TryGetValue(key, out var lst) &&
                    lst.FirstOrDefault() is string txt &&
                    Enum.TryParse(txt, ignoreCase: true, out T result))
                {
                    return result;
                }
                return @default;
            }

            // For Outline, which may be multiple values separated by commas or spaces
            OutlineMode ParseOutline()
            {
                if (!node.Attributes.TryGetValue("OUTLINE", out var lst) || lst.Count == 0)
                    return OutlineMode.NOUTLINE;

                var tokens = lst
                    .SelectMany(s => s.Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries))
                    .Select(tok => tok.Trim())
                    .Where(tok => !string.IsNullOrEmpty(tok));

                OutlineMode outline = 0;
                foreach (var tok in tokens)
                {
                    if (Enum.TryParse<OutlineMode>(tok, ignoreCase: true, out var val))
                        outline |= val;
                }
                return outline;
            }

            return new CattrTag
            {
                Color = ParseEnum("COLOR", FieldColor.MONO),
                Cursor = ParseEnum("CURSOR", CursorMode.N),
                Data = ParseEnum("DATA", AttrDataType.ALPHA),
                Detect = ParseEnum("DETECT", DetectMode.N),
                Enter = ParseEnum("ENTER", EnterMode.N),
                Fill = ParseEnum("FILL", FillMode.N),
                Hilite = ParseEnum("HILITE", HiliteMode.NOHILITE),
                Intense = ParseEnum("INTENSE", IntenseMode.NORMAL),
                Mdt = ParseEnum("MDT", MdtMode.N),
                Outline = ParseOutline(),
                Protect = ParseEnum("PROTECT", ProtectMode.ASKIP)
            };
        }
    }
}
