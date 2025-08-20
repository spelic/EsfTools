// EsfParser/CodeGen/RoslynExporter.Paths.cs
using System;
using System.IO;

namespace EsfParser.CodeGen;

public static partial class RoslynExporter
{
    // helper sources we copy/embed
    private static readonly string EzFunctionsPath =
        Path.GetFullPath(Path.Combine(AppContext.BaseDirectory,
            @"..\..\..\..\EsfParser\CodeGen\EzFunctions.cs"));

    private static readonly string SqlHelpersPath =
        Path.GetFullPath(Path.Combine(AppContext.BaseDirectory,
            @"..\..\..\..\EsfParser\Runtime\SqlHelpers.cs"));

    private static readonly string CursorStorePath =
        Path.GetFullPath(Path.Combine(AppContext.BaseDirectory,
            @"..\..\..\..\EsfParser\Runtime\CursorStore.cs"));

    // startup.json lives at EsfParser root (your setup)
    private static readonly string StartupJsonPath =
        Path.GetFullPath(Path.Combine(AppContext.BaseDirectory,
            @"..\..\..\..\EsfParser\Startup.json"));
}
