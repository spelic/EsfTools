# EsfTools

**EsfTools** is a .NET 8 toolkit that parses **ESF** (External Source Format) files produced by
**IBM VisualAge Generator / Cross System Product (CSP)** and converts them into modern,
ready-to-run **C# console projects**.

It reads the legacy tag- and logic-based ESF syntax, builds a strongly-typed in-memory model of
the program (functions, records, tables, maps, work storage, SQL access), translates the
VisualAge logic and SQL into C#, and emits a structured .NET project using
[Roslyn](https://github.com/dotnet/roslyn) — including runtime helpers, a console map renderer for
`CONVERSE` screens, and [Dapper](https://github.com/DapperLib/Dapper)-based DB2 data access.

---

## Table of Contents

- [Solution Layout](#solution-layout)
- [How It Works](#how-it-works)
- [What Gets Generated](#what-gets-generated)
- [Getting Started](#getting-started)
- [Configuring the Converter](#configuring-the-converter)
- [Supported ESF Constructs](#supported-esf-constructs)
- [Architecture Reference](#architecture-reference)
- [Dependencies](#dependencies)
- [Limitations & Notes](#limitations--notes)

---

## Solution Layout

```
EsfTools.sln
│
├── EsfParser/                       # Core library: parsing, modeling, code generation, runtime
│   ├── Parser/
│   │   ├── MyEsfParser.cs           # Tag tokenizer → TagNode tree
│   │   ├── FuncBlockParser.cs       # Splits FUNC blocks into BEFORE/AFTER logic + SQL
│   │   ├── SqlClausePlanner.cs      # Plans SQL clauses for SQL functions
│   │   └── Logic/                   # VisualAge logic statement parser
│   │       ├── EsfLogicPreprocessor.cs   # Normalizes raw logic lines, strips comments
│   │       ├── VisualAgeLogicParser.cs   # Statement AST builder (recursive)
│   │       ├── Parsers/             # One parser per statement kind (IF, MOVE, CALL, …)
│   │       └── Statements/          # AST node types (IfStatement, MoveStatement, …)
│   │
│   ├── Builder/                     # TagNode tree → typed EsfProgram model
│   │   ├── EsfProgramBuilder.cs     # Orchestrates all tag parsers
│   │   └── *TagParser.cs            # Program/Func/Record/Table/Map/Item/Ezee parsers
│   │
│   ├── Tags/                        # Strongly-typed ESF tag models (PROGRAM, FUNC, MAP, …)
│   │
│   ├── Esf/                         # Top-level program model (EsfProgram, collections, enums)
│   │
│   ├── CodeGen/                     # ESF → C# translation
│   │   ├── RoslynExporter*.cs       # Emits the structured C# project (Roslyn syntax)
│   │   ├── EsfLogicToCs.cs          # Logic statements → C#
│   │   ├── ConditionBuilder.cs      # IF/WHILE condition translation
│   │   ├── CSharpUtils.cs           # Operand/identifier conversion helpers
│   │   ├── SqlEmitHelpers.cs        # SQL clause composition + Dapper parameter binding
│   │   └── EzFunctions.cs           # EZE* system function/variable shims
│   │
│   ├── Runtime/                     # Helpers copied into the generated project
│   │   ├── AidKey.cs                # PF/AID key model
│   │   ├── ConsoleMapRenderer.cs    # Draws ESF maps to the console
│   │   ├── ConverseConsole.cs       # CONVERSE input loop (field editing)
│   │   ├── CursorStore.cs           # Cursor/position state
│   │   └── SqlHelpers.cs            # DB2 connection + Dapper helpers
│   │
│   ├── Analytics/
│   │   └── EsfProgramAnalytics.cs   # Statement counts, unknown-statement reporting
│   │
│   └── Startup.json                 # DB2 connection string template (copied to output)
│
├── EsfConsoleConverter/             # Console host that drives the conversion pipeline
│   ├── Program.cs                   # Entry point
│   └── *.esf                        # Sample ESF programs (NR11, IS00, D133, …)
│
└── Specifications/                  # ESF / Programmer's reference docs and statement specs
```

---

## How It Works

The conversion runs as a pipeline:

1. **Read** — The ESF file is read with code page **1250** encoding (these are legacy host
   sources). Console output is switched to UTF-8.

2. **Tokenize tags** — [`MyEsfParser.Parse`](EsfParser/Parser/MyEsfParser.cs) walks the lines and
   builds a tree of `TagNode` objects. ESF uses colon-delimited tags (`:program`, `:func`,
   `:record`, …) with matching end tags (`:eprogram`, `:efunc`, …), key=value attributes, and free
   content blocks (including continuation lines and dotted content).

3. **Build the model** — [`EsfProgramBuilder.GenerateEsfProgram`](EsfParser/Builder/EsfProgramBuilder.cs)
   runs the per-tag parsers and produces a strongly-typed
   [`EsfProgram`](EsfParser/Esf/EsfProgram.cs): the `PROGRAM` header, `Functions`, `Maps`,
   `Records`, `Items`, `Tables`, and `Ezee`. It also resolves the work-storage record
   (`WorkstorRecord`) and the program's map group (`ProgramMaps`).

4. **Parse the logic** — Each function's `BEFORE` / `AFTER` logic is normalized by
   [`EsfLogicPreprocessor`](EsfParser/Parser/Logic/EsfLogicPreprocessor.cs) (join continuation
   lines, split on statement-terminating semicolons, extract comments) and turned into a statement
   AST by [`VisualAgeLogicParser`](EsfParser/Parser/Logic/VisualAgeLogicParser.cs), which dispatches
   to a chain of single-purpose statement parsers.

5. **Generate C#** — [`RoslynExporter.WriteProjectFiles`](EsfParser/CodeGen/RoslynExporter.cs)
   translates statements and SQL functions into C# and writes a complete, structured console
   project to disk.

---

## What Gets Generated

The exporter produces a self-contained .NET 8 console project (no reference back to `EsfParser`):

```
<OutputFolder>/
├── Program.cs            # Loads Startup.json, configures DB2, calls the program's main function
├── <Name>.csproj         # net8.0 console project (Dapper + Net.IBM.Data.Db2)
├── Startup.json          # DB2 connection string / parameter style
├── EsfRuntime/           # EzFunctions, SqlHelpers, CursorStore, AidKey,
│                         #   ConsoleMapRenderer, ConverseConsole (namespaces rewritten to app)
├── Items/                # Elementary data item types
├── Workstor/             # Work-storage record(s) + GlobalWorkstor
├── Records/              # Other records
├── Tables/               # Table definitions
├── Maps/                 # Screen/map definitions
├── Ezee/                 # EZEE metadata placeholder
└── Functions/
    ├── Logic/            # EXECUTE / CONVERSE functions → C# methods on GlobalFunctions
    └── Sql/              # SQL functions → Dapper methods on GlobalFunctions
```

Logic and SQL functions are emitted as methods on a single `partial class GlobalFunctions`, so the
generated program is driven by calling the ESF main function from `Program.Main`.

---

## Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- A Windows-capable environment (the runtime helpers and DB2 client target Windows; sample inputs
  are CP1250-encoded)

### Build

```sh
dotnet build EsfTools.sln
```

### Run the converter

```sh
dotnet run --project EsfConsoleConverter
```

This parses the configured ESF file and writes the generated C# project to the output folder, then
prints the `dotnet restore` / `dotnet run` commands needed to build and run that project.

---

## Configuring the Converter

> ⚠️ The console host currently has its input and output **hardcoded** in
> [`EsfConsoleConverter/Program.cs`](EsfConsoleConverter/Program.cs) rather than reading them from
> command-line arguments. Adjust these before running:

- **Input file** — `path` is reassigned near the top of `Program.cs` (e.g. `path = "NR11av28.esf"`).
  Sample programs (`IS00A-V26.esf`, `D133A-V68.esf`, `NA70A-V25.esf`, …) ship in the
  `EsfConsoleConverter` folder and are copied to the build output.
- **Output folder** — the `RoslynExporter.WriteProjectFiles(...)` call points at an absolute path
  (e.g. `C:\Users\...\source\repos\Test\<NAME>`). Change this to your target directory.

The generated project reads its DB2 connection from `Startup.json` (or the `DB2_CONN_STR`
environment variable as a fallback).

---

## Supported ESF Constructs

**Top-level tags:** `PROGRAM`, `MAINFUN`, `FUNC`, `RECORD` / `TABREC`, `TABLE`, `MAP`, `ITEM`,
`EZEE`, `PROL`, plus map/record sub-tags (`CFIELD`, `VFIELD`, `CATTR`, `PRESENT`, edits, messages,
SQL/SSA/QUAL, etc.).

**Logic statements** (one parser each, in [`Parser/Logic/Parsers`](EsfParser/Parser/Logic/Parsers/)):
`IF` / `ELSE` / `END`, `WHILE`, `MOVE`, `MOVEA`, `RETR`, `CALL` (explicit and implicit), `ASSIGN`,
`TEST`, `DXFR`, `SET`, system functions (`EZE*`), and comments. Anything unrecognized becomes an
`UnknownStatement` and is reported by the analytics layer.

**SQL function options** (translated to Dapper methods in
[`RoslynExporter.Structured.cs`](EsfParser/CodeGen/RoslynExporter.Structured.cs)):
`ADD`, `UPDATE` (read-for-update), `REPLACE`, `DELETE`, `SETINQ`, `SETUPD`, `SCAN`, `SCANBACK`
(stateful forward/backward cursors), `SQLEXEC` (SELECT and non-SELECT), and `CLOSE`. The original
SQL is preserved as a comment alongside the executed, parameter-bound SQL.

---

## Architecture Reference

| Concern | Type / File |
| --- | --- |
| Tag tokenizer | [`MyEsfParser`](EsfParser/Parser/MyEsfParser.cs) |
| Tag tree node | [`TagNode`](EsfParser/Tags/TagNode.cs) |
| Model builder | [`EsfProgramBuilder`](EsfParser/Builder/EsfProgramBuilder.cs) |
| Program model | [`EsfProgram`](EsfParser/Esf/EsfProgram.cs) |
| Logic preprocessing | [`EsfLogicPreprocessor`](EsfParser/Parser/Logic/EsfLogicPreprocessor.cs) |
| Logic AST builder | [`VisualAgeLogicParser`](EsfParser/Parser/Logic/VisualAgeLogicParser.cs) |
| Project emitter | [`RoslynExporter`](EsfParser/CodeGen/RoslynExporter.cs) |
| Logic → C# | [`EsfLogicToCs`](EsfParser/CodeGen/EsfLogicToCs.cs) |
| SQL emission | [`SqlEmitHelpers`](EsfParser/CodeGen/SqlEmitHelpers.cs) |
| Analytics | [`EsfProgramAnalytics`](EsfParser/Analytics/EsfProgramAnalytics.cs) |
| Console converse runtime | [`ConverseConsole`](EsfParser/Runtime/ConverseConsole.cs), [`ConsoleMapRenderer`](EsfParser/Runtime/ConsoleMapRenderer.cs) |

The `RoslynExporter` is implemented as a `partial class` split across
[`RoslynExporter.cs`](EsfParser/CodeGen/RoslynExporter.cs),
[`RoslynExporter.Structured.cs`](EsfParser/CodeGen/RoslynExporter.Structured.cs),
[`RoslynExporter.Paths.cs`](EsfParser/CodeGen/RoslynExporter.Paths.cs), and
[`RoslynExporter.Utils.cs`](EsfParser/CodeGen/RoslynExporter.Utils.cs).

---

## Dependencies

Both projects target **`net8.0`** with implicit usings and nullable reference types enabled.

`EsfParser` references:

- **Microsoft.CodeAnalysis.CSharp** `4.14.0` — Roslyn, used to build/format the generated C#
- **Dapper** `2.1.66` — data access in generated SQL functions
- **Net.IBM.Data.Db2** `9.0.0.300` — IBM DB2 client (CLI driver shipped under `clidriver/`)

---

## Limitations & Notes

- The console host's input file and output directory are hardcoded; see
  [Configuring the Converter](#configuring-the-converter).
- SQL functions whose referenced record cannot be resolved currently throw during generation
  (map-object / VFIELD-backed SQL is not yet supported).
- Input ESF files are expected in code page **1250**; the runtime and DB2 client target Windows.
- The `Specifications/` folder contains the ESF and Programmer's Reference documentation along with
  per-statement specs (ASSIGN, CALL, DXFR, MOVE, MOVEA, RETR, SET, TEST, WHILE) for reference.
