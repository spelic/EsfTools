namespace EsfParser.Analysis;

/// <summary>
/// Which high-risk / semantically-tricky features a program uses. Most are detected from the
/// parsed model; <see cref="UsesDisplay"/> and <see cref="UsesXfer"/> are raw-text scans because
/// they are not modeled as statement types.
/// </summary>
public sealed class FeatureUsageResult
{
    public bool UsesConverse { get; set; }
    public bool UsesDxfr { get; set; }
    public bool UsesXfer { get; set; }       // raw-text scan
    public bool UsesDisplay { get; set; }    // raw-text scan
    public bool UsesCall { get; set; }
    public bool UsesMoveA { get; set; }
    public bool UsesSql { get; set; }
    public bool UsesSqlCursorFlow { get; set; }   // SETINQ/SETUPD/SCAN/SCANBACK
    public bool UsesPackedOrZoned { get; set; }   // PACK/PACF/NUM/NUMC
    public bool UsesOccursArrays { get; set; }
    public bool UsesRedefinedRecords { get; set; }
    public bool UsesMapEditRoutines { get; set; }
}
