namespace EsfCore.Tags.Logic
{
    /// <summary>
    /// Knows the C# type of a given ESF token (variable, field, global‐item, etc).
    /// </summary>
    public interface ITypeResolver
    {
        /// <summary>
        /// Given the *raw* ESF name (e.g. “IS00R10.ZAPOREDJE” or “GlobalItems.FLOK1”),
        /// return its C# type (e.g. “int”, “string”, “decimal”, etc).
        /// </summary>
        ///   /// <summary>Given an ESF name like "FLDELA" or "IS00M02.ZS[0]", return its C# type (e.g. "string", "int", "decimal", etc.)</summary>
        string GetTypeOf(string rawEsfName);
    }
}
