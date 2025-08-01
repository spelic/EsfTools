// UiPropTag.cs
using EsfParser.Esf;
using System.Text.Json.Serialization;

namespace EsfParser.Tags
{
    /// <summary>
    /// Marker for the UIPROP…EUIPROP tag block (no attributes of its own).
    /// Child tags (GENEDITS, UIMSGS, NUMEDITS, FLDHELP, LABEL) are parsed separately.
    /// </summary>
    public class UiPropTag : IEsfTagModel
    {
        [JsonIgnore] public string TagName => "UIPROP";

        public static UiPropTag Parse(TagNode node) => new UiPropTag();

        public override string ToString() => "UIPROP";
    }
}
