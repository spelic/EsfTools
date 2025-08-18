// ─────────────────────────────────────────────────────────────────────────────
// File: Esf/Esql/EsfFunctionPart.cs
// ─────────────────────────────────────────────────────────────────────────────
using System;
using System.Collections.Generic;

namespace EsfParser.Esf
{
    public sealed class EsfSqlClause
    {
        public SqlClauseKind Kind { get; init; }
        public string Text { get; init; } = string.Empty; // Raw text per spec
    }

    /// <summary>
    /// Parsed representation of a :func … :efunc block.
    /// </summary>
    public sealed class EsfFunctionPart
    {
        public string Name { get; set; } = string.Empty;
        public EsfFuncOption Option { get; set; } = EsfFuncOption.EXECUTE;
        public string? ObjectName { get; set; }
        public string? ErrRoutine { get; set; }
        public bool ExecBld { get; set; } = false; // EXECBLD
        public string Model { get; set; } = "NONE"; // DELETE|UPDATE|NONE (SQLEXEC)
        public bool Refine { get; set; } = false;   // UI-only
        public bool SingRow { get; set; } = true;   // default Y in spec; examples show N often
        public string? UpdFunc { get; set; }        // REPLACE linking
        public bool? WithHold { get; set; }         // applies to SETINQ/SETUPD
        public string? Desc { get; set; }
        public DateTime? ModifiedAt { get; set; }   // from DATE+TIME if present

        // Hostvar indicator aggregated across :sql blocks (default '@' per spec; exports often '?')
        public char HostVarPrefix { get; set; } = '@';

        // Logic sections (raw ESF lines as captured; you can run them through your statement preprocessor)
        public List<string> BeforeLines { get; } = new();
        public List<string> AfterLines { get; } = new();

        // SQL clauses in appearance order
        public List<EsfSqlClause> SqlClauses { get; } = new();

        public override string ToString() => $":func {Name} option={Option} object={ObjectName}";
    }
}