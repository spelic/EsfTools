// ==============================
// ESF FUNC support – phase 1
// AST + Block Parser + Planner stubs (no codegen yet)
// Namespaces follow your existing style: EsfParser.Esf / Parsing / Planning
// Drop these files into your solution. Wire the parser in your ESF loader
// so it detects ":func" blocks and returns EsfFunctionPart objects.
// ==============================

// ─────────────────────────────────────────────────────────────────────────────
// File: Esf/Esql/FuncEnums.cs
// ─────────────────────────────────────────────────────────────────────────────
namespace EsfParser.Esf
{
    public enum EsfFuncOption
    {
        EXECUTE, CONVERSE, DISPLAY,
        INQUIRY, UPDATE, ADD, REPLACE, DELETE,
        SETINQ, SETUPD, SCAN, SCANBACK, CLOSE,
        SQLEXEC
    }

    public enum SqlClauseKind
    {
        SELECT, INTO, WHERE, ORDERBY, SET,
        VALUES, INSERTCOLNAME, FORUPDATEOF, SQLEXEC
    }
}