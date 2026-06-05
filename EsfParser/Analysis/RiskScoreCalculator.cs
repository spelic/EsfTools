using System;

namespace EsfParser.Analysis;

/// <summary>Pure inputs for risk scoring — no I/O, no dependency on the program model.</summary>
public readonly record struct RiskInputs(
    bool ParseFailed,
    bool ModelBuildFailed,
    bool GenerationFailed,
    bool BuildFailed,
    double UnknownPercent,
    bool HasUnsupportedEze,
    bool HasUnsupportedSql,
    bool UsesConverse,
    bool UsesDxfrOrXfer,
    bool UsesMoveA,
    bool UsesPackedOrZoned,
    bool UsesOccurs,
    bool UsesRedefinedRecords,
    bool UsesSqlCursorFlow);

/// <summary>
/// Pure risk scoring. Deterministic, side-effect free, fully unit-testable.
/// Score is capped at 100; levels: 0–20 Low, 21–50 Medium, 51–79 High, 80–100 Blocked.
/// </summary>
public static class RiskScoreCalculator
{
    public static int Score(RiskInputs i)
    {
        int score = 0;

        if (i.ParseFailed) score += 100;
        if (i.ModelBuildFailed) score += 90;
        if (i.GenerationFailed) score += 80;
        if (i.BuildFailed) score += 60;

        if (i.UnknownPercent > 20) score += 40;
        else if (i.UnknownPercent > 5) score += 25;
        else if (i.UnknownPercent > 1) score += 10;

        if (i.HasUnsupportedEze) score += 20;
        if (i.HasUnsupportedSql) score += 20;

        if (i.UsesConverse) score += 15;
        if (i.UsesDxfrOrXfer) score += 15;
        if (i.UsesMoveA) score += 10;
        if (i.UsesPackedOrZoned) score += 15;
        if (i.UsesOccurs) score += 10;
        if (i.UsesRedefinedRecords) score += 20;
        if (i.UsesSqlCursorFlow) score += 15;

        return Math.Min(100, score);
    }

    public static RiskLevel Level(int score) => score switch
    {
        <= 20 => RiskLevel.Low,
        <= 50 => RiskLevel.Medium,
        <= 79 => RiskLevel.High,
        _ => RiskLevel.Blocked,
    };

    /// <summary>
    /// First matching recommendation, in priority order. <paramref name="build"/> lets
    /// "build passes" rules fire only when a build actually succeeded.
    /// </summary>
    public static string RecommendAction(RiskInputs i, BuildStatus build, int score, bool hasUnknownStatements)
    {
        if (i.ParseFailed) return "Fix parser";
        if (i.ModelBuildFailed) return "Fix model builder";
        if (i.GenerationFailed) return "Fix code generator";
        if (i.BuildFailed) return "Fix generated C# / generator";
        if (hasUnknownStatements) return "Extend statement parser";
        if (i.HasUnsupportedEze) return "Extend EZE runtime";
        if (i.HasUnsupportedSql) return "Extend SQL generator/runtime";

        bool buildPasses = build == BuildStatus.Succeeded;
        bool semanticRisk = i.UsesMoveA || i.UsesPackedOrZoned || i.UsesOccurs
                            || i.UsesRedefinedRecords || i.UsesSqlCursorFlow || i.UsesDxfrOrXfer;

        if (buildPasses && score <= 20) return "Pilot candidate";
        if (buildPasses && i.UsesConverse) return "UI/map runtime needed";
        if (buildPasses && semanticRisk) return "Add characterization tests";

        // Build not run (default): give the same guidance tentatively, by risk/feature.
        if (score <= 20) return "Pilot candidate (verify with build)";
        if (i.UsesConverse) return "UI/map runtime needed";
        if (semanticRisk) return "Add characterization tests";
        return "Review";
    }
}
