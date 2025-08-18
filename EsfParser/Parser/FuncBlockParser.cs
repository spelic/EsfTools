// ─────────────────────────────────────────────────────────────────────────────
// File: Parsing/FuncBlockParser.cs
// ─────────────────────────────────────────────────────────────────────────────
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using EsfParser.Esf;

namespace EsfParser.Parsing
{
    /// <summary>
    /// Parses a :func … :efunc block from raw ESF lines.
    /// - Robust to spacing, quoting with ' or ", trailing dots on end-tags.
    /// - Captures BEFORE/AFTER blocks as raw lines (no statement parsing here).
    /// - Captures :sql clause text exactly as present.
    /// </summary>
    public static class FuncBlockParser
    {
        // Matches ":func" start (case-insensitive), possibly with attributes on the same line
        private static readonly Regex FuncStartRx = new Regex(@"^\s*:\s*func\b(.*)$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex AttrRx = new Regex(@"(?<k>[A-Za-z_][A-Za-z0-9_\-]*)\s*=\s*(?<v>[^\s].*?)\s*(?=\s+|$)", RegexOptions.Compiled);
        private static readonly Regex SqlStartRx = new Regex(@"^\s*:\s*sql\b(.*)$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex SqlEndRx = new Regex(@"^\s*:\s*esql\s*\.?\s*$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex BeforeStartRx = new Regex(@"^\s*:\s*before\s*\.?\s*$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex BeforeEndRx = new Regex(@"^\s*:\s*ebefore\s*\.?\s*$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex AfterStartRx = new Regex(@"^\s*:\s*after\s*\.?\s*$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex AfterEndRx = new Regex(@"^\s*:\s*eafter\s*\.?\s*$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex FuncEndRx = new Regex(@"^\s*:\s*efunc\s*\.?\s*$", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        public static bool TryParse(IReadOnlyList<string> lines, ref int i, out EsfFunctionPart part, out string error)
        {
            part = null!; error = string.Empty;
            if (i < 0 || i >= lines.Count) { error = "Index out of range."; return false; }
            var m = FuncStartRx.Match(lines[i]);
            if (!m.Success) { error = "Current line is not :func."; return false; }

            part = new EsfFunctionPart();
            // Attributes may exist on same line and subsequent indented lines
            ParseFuncAttributesFromTail(m.Groups[1].Value, part);

            i++;
            for (; i < lines.Count; i++)
            {
                var line = lines[i];
                if (string.IsNullOrWhiteSpace(line)) continue;

                if (FuncEndRx.IsMatch(line))
                {
                    // finish
                    EnsureDefaults(part);
                    return true;
                }

                // Allow attribute continuation lines directly after :func block start
                if (!line.TrimStart().StartsWith(":", StringComparison.Ordinal))
                {
                    // Continuation attribute line(s) until a tag line appears
                    ParseFuncAttributesFromTail(line, part);
                    continue;
                }

                if (BeforeStartRx.IsMatch(line))
                {
                    i++;
                    for (; i < lines.Count; i++)
                    {
                        if (BeforeEndRx.IsMatch(lines[i])) break;
                        if (IsComment(lines[i])) continue;
                        part.BeforeLines.Add(lines[i]);
                    }
                    continue;
                }

                if (AfterStartRx.IsMatch(line))
                {
                    i++;
                    for (; i < lines.Count; i++)
                    {
                        if (AfterEndRx.IsMatch(lines[i])) break;
                        if (IsComment(lines[i])) continue;
                        part.AfterLines.Add(lines[i]);
                    }
                    continue;
                }

                var sm = SqlStartRx.Match(line);
                if (sm.Success)
                {
                    // Read attributes on this :sql line (clause=..., hostvar=...)
                    var clauseKind = SqlClauseKind.SELECT;
                    char? hostVarSeen = null;
                    ParseSqlAttributesFromTail(sm.Groups[1].Value, ref clauseKind, ref hostVarSeen);
                    if (hostVarSeen.HasValue) part.HostVarPrefix = hostVarSeen.Value;

                    // Accumulate text until :esql
                    var sb = new StringBuilder();
                    i++;
                    for (; i < lines.Count; i++)
                    {
                        if (SqlEndRx.IsMatch(lines[i])) break;
                        if (IsComment(lines[i])) continue;
                        if (sb.Length > 0) sb.AppendLine();
                        sb.Append(lines[i]);
                    }

                    part.SqlClauses.Add(new EsfSqlClause { Kind = clauseKind, Text = sb.ToString().Trim() });
                    continue;
                }

                // Unknown tag – skip or extend here for DLICALL/SSA/QUAL when needed
                // For now, ignore other :tags within the function.
            }

            error = ":efunc not found before end of file.";
            return false;
        }

        private static void ParseFuncAttributesFromTail(string tail, EsfFunctionPart part)
        {
            foreach (Match m in AttrRx.Matches(tail))
            {
                var key = m.Groups["k"].Value.Trim().ToLowerInvariant();
                var valRaw = m.Groups["v"].Value.Trim();
                var val = Unquote(valRaw);

                switch (key)
                {
                    case "name": part.Name = val; break;
                    case "option":
                        part.Option = ParseOption(val);
                        break;
                    case "object": part.ObjectName = val; break;
                    case "errrtn": part.ErrRoutine = val; break;
                    case "execbld": part.ExecBld = ToYN(val); break;
                    case "model": part.Model = val.ToUpperInvariant(); break;
                    case "refine": part.Refine = ToYN(val); break;
                    case "singrow": part.SingRow = ToYN(val); break;
                    case "updproc": // seen in your example naming; spec uses updfunc
                    case "updfunc": part.UpdFunc = val; break;
                    case "withhold": part.WithHold = ToYN(val); break;
                    case "desc": part.Desc = val; break;
                    case "date": part.ModifiedAt = MergeDateTime(val, part.ModifiedAt, isDate: true); break;
                    case "time": part.ModifiedAt = MergeDateTime(val, part.ModifiedAt, isDate: false); break;
                }
            }
        }

        private static void ParseSqlAttributesFromTail(string tail, ref SqlClauseKind kind, ref char? host)
        {
            foreach (Match m in AttrRx.Matches(tail))
            {
                var key = m.Groups["k"].Value.Trim().ToLowerInvariant();
                var valRaw = m.Groups["v"].Value.Trim();
                var val = Unquote(valRaw);

                switch (key)
                {
                    case "clause": kind = ParseClause(val); break;
                    case "hostvar": host = val.Length > 0 ? val[0] : (char?)null; break;
                }
            }
        }

        private static string Unquote(string s)
        {
            if (s.Length >= 2)
            {
                if ((s[0] == '\'' && s[^1] == '\'') || (s[0] == '"' && s[^1] == '"'))
                    return s.Substring(1, s.Length - 2);
            }
            return s;
        }

        private static bool ToYN(string v)
            => v.Equals("Y", StringComparison.OrdinalIgnoreCase) || v.Equals("YES", StringComparison.OrdinalIgnoreCase) || v.Equals("TRUE", StringComparison.OrdinalIgnoreCase);

        private static DateTime? MergeDateTime(string partStr, DateTime? existing, bool isDate)
        {
            try
            {
                if (isDate)
                {
                    if (DateTime.TryParseExact(partStr, new[] { "MM/dd/yyyy", "M/d/yyyy" }, CultureInfo.InvariantCulture, DateTimeStyles.None, out var d))
                        return existing.HasValue ? new DateTime(d.Year, d.Month, d.Day, existing.Value.Hour, existing.Value.Minute, existing.Value.Second) : d;
                }
                else
                {
                    if (DateTime.TryParseExact(partStr, new[] { "HH:mm:ss", "H:m:s" }, CultureInfo.InvariantCulture, DateTimeStyles.None, out var t))
                        return existing.HasValue ? new DateTime(existing.Value.Year, existing.Value.Month, existing.Value.Day, t.Hour, t.Minute, t.Second) : t;
                }
            }
            catch { }
            return existing;
        }

        private static EsfFuncOption ParseOption(string s)
        {
            s = s.Trim().ToUpperInvariant();
            return s switch
            {
                "CONVERSE" => EsfFuncOption.CONVERSE,
                "DISPLAY" => EsfFuncOption.DISPLAY,
                "INQUIRY" => EsfFuncOption.INQUIRY,
                "UPDATE" => EsfFuncOption.UPDATE,
                "ADD" => EsfFuncOption.ADD,
                "REPLACE" => EsfFuncOption.REPLACE,
                "DELETE" => EsfFuncOption.DELETE,
                "SETINQ" => EsfFuncOption.SETINQ,
                "SETUPD" => EsfFuncOption.SETUPD,
                "SCAN" => EsfFuncOption.SCAN,
                "SCANBACK" => EsfFuncOption.SCANBACK,
                "CLOSE" => EsfFuncOption.CLOSE,
                "SQLEXEC" => EsfFuncOption.SQLEXEC,
                _ => EsfFuncOption.EXECUTE,
            };
        }

        private static SqlClauseKind ParseClause(string s)
        {
            s = s.Trim().ToUpperInvariant();
            return s switch
            {
                "SELECT" => SqlClauseKind.SELECT,
                "INTO" => SqlClauseKind.INTO,
                "WHERE" => SqlClauseKind.WHERE,
                "ORDERBY" => SqlClauseKind.ORDERBY,
                "SET" => SqlClauseKind.SET,
                "VALUES" => SqlClauseKind.VALUES,
                "INSERTCOLNAME" => SqlClauseKind.INSERTCOLNAME,
                "FORUPDATEOF" => SqlClauseKind.FORUPDATEOF,
                "SQLEXEC" => SqlClauseKind.SQLEXEC,
                _ => SqlClauseKind.SELECT
            };
        }

        private static bool IsComment(string line)
        {
            var t = line.TrimStart();
            return t.StartsWith("/*") || t.StartsWith("--");
        }

        private static void EnsureDefaults(EsfFunctionPart fp)
        {
            if (fp.SingRow == default) fp.SingRow = true; // spec default Y
            if (fp.HostVarPrefix == default) fp.HostVarPrefix = '@';
        }
    }
}