using EsfCore.Esf;
using System.Text.Json.Serialization;

public class EzeeTag : IEsfTagModel
{
    [JsonIgnore] public string TagName { get; set; } = "EZEE";
    public string Version { get; set; }
    public string CreationDate { get; set; }
    public string CreationTime { get; set; }

    public override string ToString() => $"EZEE Version={Version}, Date={CreationDate}, Time={CreationTime}";
}
