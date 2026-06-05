using EsfConsoleConverter;
using System.Text;

// ─────────────────────────────────────────────────────────────────────────────
// EsfConsoleConverter — verb dispatcher.
//
//   convert <input.esf> [--out <folder>] [--namespace <ns>]   (default when no verb)
//   prepare-latest-esf --input <folder> --output <folder> [...]
//
// Backward compatible: a bare ESF path with no verb still runs the converter.
// ─────────────────────────────────────────────────────────────────────────────

Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
Console.OutputEncoding = Encoding.UTF8;

if (args.Length > 0 && args[0] == "prepare-latest-esf")
    return PrepareLatestCommand.Run(args[1..]);

if (args.Length > 0 && args[0] == "coverage")
    return CoverageCommand.Run(args[1..]);

if (args.Length > 0 && args[0] == "convert")
    return ConvertCommand.Run(args[1..]);

// No verb (or a bare filename) → convert, preserving the original CLI.
return ConvertCommand.Run(args);
