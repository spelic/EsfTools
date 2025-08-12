using System;
using System.Text.RegularExpressions;

namespace EsfParser.Parser.Logic
{
    /// <summary>
    /// Translates your CONDITION strings into C# boolean expressions.
    /// Handles:
    ///  - EZEAID IS PF3/ENTER/PA2
    ///  - <record> IS ERR/NRF/DUP
    ///  - NE/EQ, =</=>, <, >, >=, <=
    ///  - AND/OR (-> &&/||)
    ///  - '...' -> "..."
    ///  - Decimal commas: 0,01 -> 0.01
    ///  - IS NULL, IS MODIFIED, IS CURSOR
    ///  - Hyphenated identifiers: DIM-C -> DIM_C
    ///  - Indexing: DM[STEVEC5]
    ///  - Parentheses
    /// Customize output names via ConditionOptions.
    /// </summary>
    public static class ConditionDsl
    {
        public sealed class ConditionOptions
        {
            /// <summary>
            /// The C# symbol that holds the current AID key in your runtime (e.g., "AID" or "ctx.AID").
            /// </summary>
            public string AidSymbol { get; init; } = "AID";

            /// <summary>
            /// The C# enum that contains AID keys (PF3, Enter, PA2…). E.g. "AID".
            /// </summary>
            public string AidEnum { get; init; } = "AID";

            /// <summary>
            /// Function to emit record state checks: IsStatus(CT01R01, RowStatus.Nrf)
            /// </summary>
            public string IsStatusFunc { get; init; } = "IsStatus";

            /// <summary>
            /// The RowStatus enum name used with IsStatus(..., RowStatus.X).
            /// </summary>
            public string RowStatusEnum { get; init; } = "RowStatus";

            /// <summary>
            /// Function used for “X IS MODIFIED” → IsModified(X).
            /// </summary>
            public string IsModifiedFunc { get; init; } = "IsModified";

            /// <summary>
            /// Function used for “X IS CURSOR” → IsCursor(X).
            /// </summary>
            public string IsCursorFunc { get; init; } = "IsCursor";

            /// <summary>
            /// Optional callback to rewrite identifiers (e.g., prepend "ctx." or map to indexer access).
            /// Input is already normalized (hyphens → underscores), including dotted paths.
            /// Return the final C# identifier text to emit.
            /// </summary>
            public Func<string, string>? ResolveIdentifier { get; init; }
        }

        // Precompiled regexes for the more complex rewrites.
        static readonly Regex RxDecimalComma = new(@"(?<=\d),(?=\d)", RegexOptions.Compiled);
        static readonly Regex RxSingleQuoted = new(@"'([^']*)'", RegexOptions.Compiled);
        static readonly Regex RxSpaces = new(@"\s+", RegexOptions.Compiled);
        static readonly Regex RxAid = new(@"\bEZEAID\s+IS\s+(ENTER|PA2|PF\d{1,2})\b", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        static readonly Regex RxRecState = new(@"\b(?<rec>[A-Za-z0-9_.]+)\s+IS\s+(ERR|NRF|DUP)\b", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        static readonly Regex RxIsModified = new(@"\b(?<lhs>[A-Za-z0-9_.\[\]-]+)\s+IS\s+MODIFIED\b", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        static readonly Regex RxIsCursor = new(@"\b(?<lhs>[A-Za-z0-9_.\[\]-]+)\s+IS\s+CURSOR\b", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        static readonly Regex RxIsNull = new(@"\s+IS\s+NULL\b", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        // Replace = with == only when it's a comparator (surrounded by operands), not after we already made <=, >=, !=, etc.
        static readonly Regex RxBareEquals = new(@"(?<=[\w\]\)""])\s=\s(?=[\w""\('])", RegexOptions.Compiled);

        // Hyphenated identifiers → underscores (keeps dots and indexers).
        static readonly Regex RxHyphenId = new(@"(?<![""'])\b([A-Za-z_][\w]*-[A-Za-z_0-9][\w-]*)\b", RegexOptions.Compiled);

        public static string ToCSharp(string condition, ConditionOptions? options = null)
        {
            if (string.IsNullOrWhiteSpace(condition))
                return "true";

            options ??= new ConditionOptions();

            // 1) Canonicalize whitespace
            var s = RxSpaces.Replace(condition.Trim(), " ");

            // 2) Decimal commas -> dot
            s = RxDecimalComma.Replace(s, ".");

            // 3) Strings: '...' -> "..."
            s = RxSingleQuoted.Replace(s, m => "\"" + m.Groups[1].Value.Replace("\"", "\\\"") + "\"");

            // 4) Logical connectors AND/OR
            s = Regex.Replace(s, @"\bAND\b", "&&", RegexOptions.IgnoreCase);
            s = Regex.Replace(s, @"\bOR\b", "||", RegexOptions.IgnoreCase);

            // 5) Comparison keywords and odd operators
            s = Regex.Replace(s, @"\bNE\b", "!=", RegexOptions.IgnoreCase);
            s = Regex.Replace(s, @"\bEQ\b", "==", RegexOptions.IgnoreCase);
            s = Regex.Replace(s, @"=<", "<=", RegexOptions.IgnoreCase);
            s = Regex.Replace(s, @"=>", ">=", RegexOptions.IgnoreCase);

            // 6) IS NULL
            s = RxIsNull.Replace(s, " == null");

            // 7) EZEAID IS PFx/ENTER/PA2
            s = RxAid.Replace(s, m =>
            {
                var raw = m.Groups[1].Value.ToUpperInvariant();
                var member = raw switch
                {
                    "ENTER" => "Enter",
                    "PA2" => "PA2",
                    _ => raw // PF3, PF12, ...
                };
                return $"{options.AidSymbol} == {options.AidEnum}.{member}";
            });

            // 8) <record> IS ERR/NRF/DUP
            s = RxRecState.Replace(s, m =>
            {
                var rec = NormalizeIdentifier(m.Groups["rec"].Value, options);
                var state = m.Groups[2].Value.ToUpperInvariant() switch
                {
                    "ERR" => "Err",
                    "NRF" => "Nrf",
                    "DUP" => "Dup",
                    _ => m.Groups[2].Value
                };
                return $"{options.IsStatusFunc}({rec}, {options.RowStatusEnum}.{state})";
            });

            // 9) X IS MODIFIED
            s = RxIsModified.Replace(s, m =>
            {
                var lhs = NormalizeIdentifier(m.Groups["lhs"].Value, options);
                return $"{options.IsModifiedFunc}({lhs})";
            });

            // 10) X IS CURSOR
            s = RxIsCursor.Replace(s, m =>
            {
                var lhs = NormalizeIdentifier(m.Groups["lhs"].Value, options);
                return $"{options.IsCursorFunc}({lhs})";
            });

            // 11) Hyphenated identifiers -> underscores
            s = RxHyphenId.Replace(s, m => m.Value.Replace("-", "_"));

            // 12) Bare "=" that are still comparisons → "=="
            s = RxBareEquals.Replace(s, " == ");

            // 13) Finally, allow caller to rewrite identifiers (e.g., prepend "ctx.")
            if (options.ResolveIdentifier != null)
            {
                s = RewriteIdentifiers(s, options.ResolveIdentifier);
            }

            return s;
        }

        /// <summary>
        /// Convert a DSL identifier into a C# identifier string, applying hyphen→underscore
        /// and letting the caller further map it with ResolveIdentifier if provided.
        /// (Keeps dots and indexers; does NOT touch quoted strings.)
        /// </summary>
        private static string NormalizeIdentifier(string raw, ConditionOptions options)
        {
            if (string.IsNullOrWhiteSpace(raw)) return raw;
            // hyphens inside id segments -> underscores
            var normalized = raw.Replace("-", "_");
            return options.ResolveIdentifier != null ? options.ResolveIdentifier(normalized) : normalized;
        }

        /// <summary>
        /// Very light identifier pass: try to apply ResolveIdentifier to bare identifiers and
        /// dotted paths without changing operators/numbers/strings.
        /// Safe default: do nothing if not clearly an identifier token.
        /// </summary>
        private static string RewriteIdentifiers(string input, Func<string, string> resolve)
        {
            // Token-ish pass: replace only tokens that look like identifiers (and not already namespaced like RowStatus.X or AID.X)
            return Regex.Replace(
                input,
                @"(?<![""'])\b([A-Za-z_][A-Za-z0-9_]*)(\.[A-Za-z_][A-Za-z0-9_]*|\[[^\]]+\])*",
                m =>
                {
                    var token = m.Value;

                    // Skip known left sides we just injected (RowStatus., AID., IsStatus(, IsModified(, IsCursor()
                    if (token.StartsWith("RowStatus.") || token.StartsWith("AID.") ||
                        token.StartsWith("IsStatus") || token.StartsWith("IsModified") || token.StartsWith("IsCursor"))
                        return token;

                    // Skip logicals/keywords
                    if (token is "null" or "true" or "false") return token;

                    return resolve(token);
                });
        }
    }

    // Optional: enums you can use on the output side.
    public enum RowStatus { Err, Nrf, Dup }
    public enum AID { Enter, PF2, PF3, PF4, PF5, PF6, PF7, PF8, PF9, PF10, PF11, PF12, PA2 }
}
