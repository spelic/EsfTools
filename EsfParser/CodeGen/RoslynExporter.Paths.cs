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

    // Conversation runtime helpers (AidKey, ConsoleMapRenderer, ConverseConsole)
    private static readonly string AidKeyPath =
        Path.GetFullPath(Path.Combine(AppContext.BaseDirectory,
            @"..\..\..\..\EsfParser\Runtime\AidKey.cs"));

    private static readonly string ConsoleMapRendererPath =
        Path.GetFullPath(Path.Combine(AppContext.BaseDirectory,
            @"..\..\..\..\EsfParser\Runtime\ConsoleMapRenderer.cs"));

    private static readonly string ConverseConsolePath =
        Path.GetFullPath(Path.Combine(AppContext.BaseDirectory,
            @"..\..\..\..\EsfParser\Runtime\ConverseConsole.cs"));

    // startup.json lives at EsfParser root (your setup)
    private static readonly string StartupJsonPath =
        Path.GetFullPath(Path.Combine(AppContext.BaseDirectory,
            @"..\..\..\..\EsfParser\Startup.json"));
}
