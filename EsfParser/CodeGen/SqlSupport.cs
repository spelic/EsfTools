using System;
using System.Collections.Generic;

namespace EsfParser.CodeGen;

/// <summary>
/// Single source of truth for which ESF function OPTION values the code generator can emit.
/// Keep this in sync with the <c>switch (f.Option)</c> in
/// <see cref="RoslynExporter"/>.Structured (RoslynExporter.Structured.cs). The portfolio
/// coverage analyzer reads these sets so "unsupported option" detection never drifts from
/// what the generator actually handles.
/// </summary>
public static class SqlSupport
{
    /// <summary>SQL function options the generator emits (the cases in the SQL switch).</summary>
    public static readonly IReadOnlySet<string> SupportedSqlOptions =
        new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "ADD", "UPDATE", "REPLACE", "DELETE",
            "SETINQ", "SETUPD", "SCAN", "SCANBACK",
            "SQLEXEC", "CLOSE",
        };

    /// <summary>Options routed to the logic (non-SQL) branch.</summary>
    public static readonly IReadOnlySet<string> LogicOptions =
        new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "EXECUTE", "CONVERSE" };

    /// <summary>SQL cursor-flow options (a semantic-risk signal).</summary>
    public static readonly IReadOnlySet<string> CursorFlowOptions =
        new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "SETINQ", "SETUPD", "SCAN", "SCANBACK" };

    /// <summary>
    /// True when <paramref name="option"/> is neither generator-supported SQL nor a logic option,
    /// i.e. the generator would emit only a TODO stub for it.
    /// </summary>
    public static bool IsUnsupported(string? option)
    {
        if (string.IsNullOrWhiteSpace(option)) return false;
        return !SupportedSqlOptions.Contains(option) && !LogicOptions.Contains(option);
    }
}
