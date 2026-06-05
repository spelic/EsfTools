using System;

namespace EsfParser.CodeGen;

/// <summary>
/// Raised when ESF source cannot be translated to C# — e.g. an operand references a
/// record/field that does not exist, or a SQL function targets an unknown record.
/// Distinct from generic exceptions so callers can isolate a single failing function
/// instead of aborting the whole conversion.
/// </summary>
public sealed class EsfTranslationException : Exception
{
    public EsfTranslationException(string message) : base(message) { }
    public EsfTranslationException(string message, Exception inner) : base(message, inner) { }
}
