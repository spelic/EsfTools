# EsfTools — Solution Design & Migration Strategy

**Status:** Draft for external review
**Audience:** A reviewer who knows software architecture but may not know IBM VisualAge Generator / ESF
**Purpose:** Explain *what* this solution does, *how* it works, and the *design decisions* behind it — so a reviewer can judge whether the approach is sound and whether the chosen target (a .NET console application) is the right migration target, or whether another technology stack would serve better.

> **If you only read one section,** read [§9 Target-stack analysis](#9-target-stack-analysis) and [§11 Questions for the reviewer](#11-questions-for-the-reviewer). Everything before that is the context needed to answer them.

---

## Table of Contents

1. [What problem this solves](#1-what-problem-this-solves)
2. [Background: ESF, VisualAge Generator, CSP](#2-background-esf-visualage-generator-csp)
3. [Migration strategy taxonomy — where this sits](#3-migration-strategy-taxonomy--where-this-sits)
4. [Current architecture](#4-current-architecture)
5. [The generated application's runtime model](#5-the-generated-applications-runtime-model)
6. [Key design decisions & trade-offs](#6-key-design-decisions--trade-offs)
7. [Semantic fidelity — the hard parts](#7-semantic-fidelity--the-hard-parts)
8. [Strengths & risks of the current approach](#8-strengths--risks-of-the-current-approach)
9. [Target-stack analysis](#9-target-stack-analysis)
10. [Verification strategy — proving equivalence](#10-verification-strategy--proving-equivalence)
11. [Questions for the reviewer](#11-questions-for-the-reviewer)

---

## 1. What problem this solves

We have a body of legacy business programs written for **IBM VisualAge Generator (VAGen) / Cross System Product (CSP)**, exported as **ESF** (External Source Format) text files. These programs implement production business logic (in our case, raw-material intake / warehouse processing — e.g. `NR11`, `IS00`, `D133`), run against **DB2**, and present **3270-style green-screen maps** to users.

The goal is to **get off the legacy 4GL runtime** and onto a maintainable, modern stack. `EsfTools` is our in-house attempt to do that automatically: parse the ESF, build a model of the program, and **generate an equivalent C# application**.

The open question this document exists to answer: **is "generate a .NET 8 console application" the right shape of the target — and is automatic source transpilation the right method?**

---

## 2. Background: ESF, VisualAge Generator, CSP

A reviewer unfamiliar with the platform needs this much:

- **VAGen/CSP** is a 1990s–2000s **4GL** for IBM hosts (mainframe / AS400 / OS/2). You define programs declaratively: data structures, screen maps, and "functions" containing procedural logic.
- A **program** has: a header (`:PROGRAM`), a **main function** (`:MAINFUN`), and many **functions** (`:FUNC`). Data is described as **records** (fixed-format, with levels and arrays), **items** (elementary fields with type/length/decimals), **tables**, and **maps** (screen layouts of constant + variable fields).
- **Functions come in two flavours:**
  - **LOGIC functions** (`EXECUTE`, `CONVERSE`) — procedural code written in VAGen statements: `IF/WHILE`, `MOVE`, `MOVEA`, `CALL`, `ASSIGN`, `TEST`, `DXFR`, `SET`, plus `EZE*` system functions/variables.
  - **SQL functions** — declarative I/O against DB2 with options like `SETINQ`, `SETUPD`, `SCAN`, `SCANBACK`, `ADD`, `UPDATE`, `REPLACE`, `DELETE`, `SQLEXEC`.
- **CONVERSE** is the interaction primitive: it displays a map, waits for the user, and returns an **AID key** (Enter, PF3, …). This is the green-screen UX.
- **ESF** is the flat, colon-tagged text serialization of all the above. It is what we parse. A sample header:

  ```
  :program   name = NR11A   workstor = NR11W01   mapgroup = NR11G   ...
  :mainfun   name = NR11P00.
  IF EZEAID IS PF3;
    NR11W01.STEVAPPL = NR11W01.STEVAPPL - 1;
    MOVE NR11W01.APPL[STEVAPPL] TO EZEAPP;
    DXFR EZEAPP NR11W01;
  ELSE;
  END;
  ```

**Why this matters for the review:** these are *interactive transaction programs*, not batch number-crunchers. Their essential nature is "screen → logic → DB2 → next screen." Any target stack has to answer "what happens to the screens?"

---

## 3. Migration strategy taxonomy — where this sits

Industry-standard options for legacy modernization, roughly in increasing cost/value:

| Strategy | Meaning | Applied here |
|---|---|---|
| **Rehost** | Move the binary as-is to new infra (emulator) | Not pursued — keeps the 4GL runtime dependency |
| **Re-platform** | Same code, minor changes, new runtime | N/A (no portable runtime) |
| **Refactor / Transpile** | Convert source to a new language, preserve structure | **← this is what EsfTools does** |
| **Rewrite** | Re-implement from requirements | Not pursued — too slow/risky across many programs |
| **Vendor tool** | IBM's own VAGen→**EGL**→Java/COBOL path, or a commercial transpiler | Not pursued (cost / lock-in / output quality concerns) — *worth a reviewer's challenge* |

EsfTools is a **transpiler**: ESF → C# **source code** that humans can then own and run. The strategic alternatives within "transpile/refactor" are discussed in [§9](#9-target-stack-analysis).

---

## 4. Current architecture

The solution is two .NET 8 projects plus a new test project:

- **`EsfParser`** — the engine (parsing, modeling, code generation, runtime helpers).
- **`EsfConsoleConverter`** — a CLI host that drives the engine.
- **`EsfParser.Tests`** — xUnit tests (parse all samples, golden skeleton, namespace-rewrite regression).

### 4.1 The pipeline

```
   ESF file (code page 1250)
        │
        ▼
┌───────────────────┐   colon-tag tokenizer
│ 1. MyEsfParser    │ ───────────────────────►  TagNode tree
└───────────────────┘                           (:PROGRAM, :FUNC, :RECORD, …)
        │
        ▼
┌───────────────────┐   per-tag parsers (Program/Func/Record/Table/Map/Item)
│ 2. EsfProgram-    │ ───────────────────────►  EsfProgram  (typed model)
│    Builder        │                           Functions / Records / Maps / Items / Tables
└───────────────────┘
        │
        ▼
┌───────────────────┐   preprocess + statement parsers (IF/MOVE/CALL/…)
│ 3. VisualAge-     │ ───────────────────────►  IStatement AST per function
│    LogicParser    │                           (BeforeLogic / AfterLogic)
└───────────────────┘
        │
        ▼
┌───────────────────┐   ESF → C# translation, Roslyn assembly
│ 4. RoslynExporter │ ───────────────────────►  structured C# console project on disk
└───────────────────┘
```

### 4.2 Stage 1 — Tag tokenizer ([MyEsfParser.cs](../EsfParser/Parser/MyEsfParser.cs))

Walks the lines and builds a tree of `TagNode` (tag name, attributes, free content, children). ESF tags open with `:NAME` and close with `:ENAME`; attributes are `key = value`; some tags carry multi-line content blocks with continuation rules. The parser is **heuristic** (string scanning, not a formal grammar) and now records non-fatal **`Diagnostics`** rather than aborting on an unexpected line.

### 4.3 Stage 2 — Model builder ([EsfProgramBuilder.cs](../EsfParser/Builder/EsfProgramBuilder.cs))

Runs a list of `IEsfTagParser` implementations over the tree and produces a strongly-typed [`EsfProgram`](../EsfParser/Esf/EsfProgram.cs): the program header, `Functions`, `Records`, `Items`, `Tables`, `Maps`, and `Ezee`. It also resolves derived facts like the **work-storage record** (`WorkstorRecord`) and the program's **map group** (`ProgramMaps`).

### 4.4 Stage 3 — Logic parser ([VisualAgeLogicParser.cs](../EsfParser/Parser/Logic/VisualAgeLogicParser.cs))

Each function's logic text is normalized by [`EsfLogicPreprocessor`](../EsfParser/Parser/Logic/EsfLogicPreprocessor.cs) (join continuation lines, split on statement-terminating `;` outside quotes, peel off comments) into `PreprocessedLine`s, then a **chain-of-responsibility** of single-statement parsers turns each line into an `IStatement` node. Nesting (`IF`/`WHILE` bodies) is handled recursively. Unrecognized lines become `UnknownStatement` and surface in analytics — the design *expects* incomplete coverage and makes it visible rather than failing.

### 4.5 Stage 4 — Code generator ([RoslynExporter.Structured.cs](../EsfParser/CodeGen/RoslynExporter.Structured.cs))

Emits a complete, self-contained C# project: a folder per concept (Items / Workstor / Records / Tables / Maps / Ezee / Functions{Logic,Sql} / EsfRuntime), `Program.cs`, a `.csproj`, and `Startup.json`. Each ESF function becomes one C# method on a single `partial class GlobalFunctions`. Translation of expressions/operands is centralized in [`CSharpUtils`](../EsfParser/CodeGen/CSharpUtils.cs); each `IStatement` knows how to render itself via `ToCSharp()`. SQL functions are rendered per option into Dapper calls.

> **Important nuance about "Roslyn":** despite the name, the generator mostly builds C# as **interpolated strings** and then re-parses them with Roslyn to normalize whitespace and split members into files. It is *templating with a Roslyn finishing pass*, not AST construction. See [§6](#6-key-design-decisions--trade-offs).

---

## 5. The generated application's runtime model

This is what an ESF program *becomes*, and it's where the "is console the right target?" question really lives.

- **One God-class of logic.** Every function (logic and SQL) is a static method on `GlobalFunctions`. `Program.Main` configures DB2 and calls the ESF main function. There are no objects/services — it mirrors the procedural 4GL structure 1:1.
- **Global state holders.** Operands are qualified to global singletons: `GlobalWorkstor.`, `GlobalSqlRow.`, `GlobalMaps.`, `GlobalItems.`, `GlobalTables.`, and `EzFunctions.` for `EZE*` system variables (see prefix logic in [CSharpUtils.cs](../EsfParser/CodeGen/CSharpUtils.cs)). This faithfully reproduces VAGen's global-data model — and equally faithfully reproduces its lack of encapsulation.
- **Data access = DB2 + Dapper.** SQL functions open a `DB2Connection` via [`DataAccess`](../EsfParser/Runtime/SqlHelpers.cs) and run Dapper queries. The target **keeps DB2** as the database. `SCAN`/`SCANBACK` are emulated with cached result sets and a cursor position; `SETUPD`/`UPDATE` follow a read-for-update pattern.
- **UI = console emulation of 3270 maps.** `CONVERSE` emits
  `Runtime.ConverseConsole.RenderAndEdit(24, 80, …)` ([FuncTag.cs:176](../EsfParser/Tags/FuncTag.cs#L176)), which draws the map to the console and edits fields in place, returning an AID key into `EzFunctions.EZEAID`. **This is the single most consequential design choice** — the green screen becomes a console screen.

```
        ESF program                         Generated C# console app
   ┌───────────────────┐               ┌─────────────────────────────────┐
   │ MAINFUN  NR11P00   │   ───────►    │ GlobalFunctions.NR11P00()        │
   │  CONVERSE map      │               │  ConverseConsole.RenderAndEdit() │ ← 3270 → console
   │  SQL SETINQ        │               │  conn.Query<Row>(sql) (Dapper)   │ ← DB2 kept
   │  MOVE / IF / CALL  │               │  GlobalWorkstor.* = …            │ ← global state
   └───────────────────┘               └─────────────────────────────────┘
```

---

## 6. Key design decisions & trade-offs

| Decision | Rationale | Trade-off / risk |
|---|---|---|
| **Transpile to C# source** (not interpret the model at runtime) | Output is human-ownable; no permanent dependency on EsfTools; debuggable | If logic coverage is incomplete, humans must finish each program by hand; re-running the generator overwrites manual edits |
| **Target = .NET 8 console app** | Simplest possible host; proves the logic/data port without UI complexity | Loses the interactive/transactional nature; not how these apps are actually used |
| **String-template codegen with a Roslyn pass** | Fast to write; easy to eyeball output | Fragile escaping; doesn't leverage Roslyn's correctness guarantees; large repetitive templates (mitigated by recent refactor) |
| **Global static state holders** | 1:1 with VAGen's global data model → faithful translation | Not idiomatic C#; not thread-safe; hard to test/compose |
| **Keep DB2 + Dapper** | Lowest-risk data story; SQL is already DB2 dialect | Carries the DB2 license/runtime forward; no DB modernization |
| **Console rendering of maps** | Lets a converted program "run" end-to-end immediately | Throwaway UX; real users won't accept a console green-screen; map semantics (protect/edit/colour/AID) only partially modeled |
| **Heuristic parser, `Unknown` fallback** | Pragmatic given messy real-world ESF | Silent gaps unless analytics are reviewed; no formal grammar to lean on |

---

## 7. Semantic fidelity — the hard parts

These are the classic traps in any 4GL→3GL migration. A reviewer should weight these heavily, because **"it compiles and runs" is not "it computes the same answers."**

1. **Numeric semantics.** VAGen uses fixed-precision packed/zoned decimals (`NUM`, `PACK`, `PACF`) with defined truncation/rounding. The generator maps these to `int`/`decimal` ([CSharpUtils.MapCsType](../EsfParser/CodeGen/CSharpUtils.cs)). Decimal scale, overflow, and rounding behavior need to match the host exactly or financial results drift.
2. **Fixed-format records & levels.** Host records are byte-laid-out with group/elementary levels, `REDEFINES`-like overlays, and `OCCURS` arrays. How faithfully the emitted record types reproduce layout (and whether anything relies on byte layout) is a key correctness question.
3. **1-based arrays.** ESF subscripts are 1-based; the generator subtracts 1 for literals. Computed indices must be handled consistently everywhere.
4. **`MOVE` vs `MOVEA` vs `ASSIGN`.** Host `MOVE` has type-coercion/padding/truncation rules by type and length; these must be reproduced, not approximated by C# assignment.
5. **EBCDIC / code page.** Source is read as CP1250; but *data* semantics (collation, sign handling, packed nibbles) belong to the host. Anything comparing or sorting strings can diverge.
6. **`EZE*` system functions/variables.** Coverage in [`EzFunctions`](../EsfParser/CodeGen/EzFunctions.cs) is partial; each unmodeled `EZE*` is a behavioral gap.
7. **Control flow & transaction model.** `DXFR` (transfer), segmented `execmode`, error routines (`ErrRtn`), and the CONVERSE/return loop encode a transaction lifecycle that a flat `Main()` + method calls only loosely represents.
8. **`EsfValueProvider` is a stub.** The symbolic host-variable bridge in [SqlHelpers.cs](../EsfParser/Runtime/SqlHelpers.cs) is `TODO` — a sign the data-binding story between SQL host vars and the global holders is not fully closed.

---

## 8. Strengths & risks of the current approach

**Strengths**
- Clean stage separation (parse → model → logic → emit); each stage independently inspectable.
- Extensible by design: chain-of-responsibility statement parsers, per-tag parsers, per-SQL-option emitters.
- Honest about gaps: unknown statements are surfaced, not hidden.
- Output is **real, compilable C#** — verified end-to-end (a non-development sample, `IS00`, generates and compiles with 0 errors).
- Now has a regression test net and behavior-locked golden output.

**Risks**
- **Coverage is unknown.** We don't yet measure "% of statements translated, not `Unknown`" across the whole portfolio. Without that number, effort estimates are guesses.
- **Correctness is unproven.** No equivalence testing against the live host (see [§10](#10-verification-strategy--proving-equivalence)).
- **The UX is a placeholder.** Console maps won't ship to users.
- **Generated code is not idiomatic** and may be hard for a .NET team to own long-term.
- **One-shot vs. iterative.** If humans hand-finish generated programs, regeneration becomes destructive — there's no merge story.

---

## 9. Target-stack analysis

The two decisions to separate:

> **(A) Is *console* the right host shape?**
> **(B) Is *automatic transpilation* even the right method, vs. alternatives?**

### 9A. If we keep transpiling, what should the target host be?

| Target | Fit for these programs | Pros | Cons |
|---|---|---|---|
| **Console (current)** | Poor as a product; fine as a proof | Trivial host; runs logic/DB2 today | Throwaway UX; hides the UI problem rather than solving it |
| **TUI (Terminal.Gui / Spectre)** | Good if users still want green-screen feel | Preserves muscle memory; maps→panels is a natural fit; AID keys map to function keys | Still terminal; limited future; another bespoke runtime |
| **Web — Blazor Server** | Strong | Maps → components; server-side state matches the global-holder model; modern UX; same .NET codebase | Requires a map→component generator (new work); session/state design |
| **Web — ASP.NET Core MVC/Razor** | Strong | Conventional; testable; each map → page | More plumbing per screen |
| **REST API + SPA (React/Angular)** | Strong if multiple front-ends/integration needed | Clean separation; reusable services; future-proof | Largest effort; forces real service boundaries onto procedural code |
| **Headless service / batch** | Right *only* for the non-CONVERSE programs | Simple; no UI | Doesn't address interactive programs |

**Observation:** because the programs are fundamentally interactive (CONVERSE-driven), the **map** is the asset that most needs a real target. The logic and SQL port relatively mechanically; the screens do not. A serious migration probably needs a **map-generation track** (ESF maps → Blazor/Razor/TUI views) as a first-class peer to the logic track, not a console stand-in.

### 9B. Transpile vs. other methods

| Method | When it wins | When it loses |
|---|---|---|
| **In-house transpiler (this)** | Many similar programs; we want to own the output; vendor tools too costly/opaque | Long tail of rare constructs; correctness burden is on us |
| **Model-driven runtime engine** (interpret the parsed `EsfProgram` directly, no code emission) | If programs change rarely and we'd rather maintain one engine than N generated apps | Performance; debugging generated-vs-interpreted; still need UI |
| **IBM EGL / Rational Business Developer** (the vendor's own VAGen→EGL→Java path) | If staying close to IBM's supported modernization is acceptable | Licensing, lock-in, Java target, EGL is itself legacy-ish |
| **Commercial transpilers** (e.g. mainframe-modernization vendors) | Large budget, want a warranty on equivalence | Cost; output-ownership; black-box fidelity |
| **Manual rewrite from requirements** | Few programs, or logic is simpler than it looks, or business rules are stale | Doesn't scale to a portfolio; loses undocumented behavior |

---

## 10. Verification strategy — proving equivalence

Whatever the target, the migration is only credible with an equivalence story. Recommended, in order:

1. **Coverage metric (cheap, do first).** Run the parser/translator across the *entire* ESF portfolio and report: % statements translated vs `Unknown`, unsupported `EZE*`, unsupported SQL options, unresolved record/field references. This converts "unknown effort" into a number and ranks programs by readiness.
2. **Golden-master on output (already started).** Lock generated output and diff on every change (we now do this for `NR11`). Prevents regressions in the *generator*.
3. **Characterization tests on behavior.** For representative programs, drive the generated app with recorded inputs and assert outputs/DB effects.
4. **Parallel run / shadowing.** The gold standard: run host and migrated program on the same inputs against a copy of the DB and diff results (screens, DB mutations, return codes). This is the only thing that proves *computational* equivalence (§7).

---

## 11. Questions for the reviewer

Please challenge any of the following — these are the decisions we most want a second opinion on:

1. **Target shape.** Is a **console app** an acceptable *first milestone*, or is it a distraction that hides the real (UI) problem? Should we invest now in a **map-generation track** (Blazor / Razor / TUI) instead of console rendering?
2. **Method.** Is an **in-house transpiler** the right bet for our portfolio size, or should we evaluate **IBM EGL/RBD** or a **commercial transpiler** before sinking more effort here?
3. **Source vs. runtime.** Is generating **owned C# source** correct, or would a **model-driven runtime** (interpret `EsfProgram` directly) be more maintainable given that we control the whole portfolio?
4. **Idiomatic vs. faithful.** The generated code is a faithful, non-idiomatic mirror of VAGen (global state, God-class). Is that the right call for *correctness-first* migration, or should we target idiomatic C# (services, DI) and accept more translation risk?
5. **Database.** Keep **DB2** (current), or fold a **DB migration** (e.g. to PostgreSQL/SQL Server) into the same effort?
6. **Correctness bar.** Is **parallel-run shadowing** ([§10](#10-verification-strategy--proving-equivalence)) feasible in our environment? If not, what equivalence evidence would be acceptable to sign off a converted program for production?
7. **Numeric/format fidelity.** Given [§7](#7-semantic-fidelity--the-hard-parts), is mapping packed/zoned decimals to `int`/`decimal` sufficient, or do we need a dedicated fixed-decimal type to guarantee host-identical arithmetic?
8. **Lifecycle.** Do we treat conversion as **one-shot** (humans then own the C#) or **repeatable** (regenerate from ESF)? This determines whether manual edits to generated code are allowed, and whether we need a merge/round-trip story.

---

### Appendix — where to look in the code

| Concern | Entry point |
|---|---|
| Tag tokenizer | [MyEsfParser.cs](../EsfParser/Parser/MyEsfParser.cs) |
| Typed model | [EsfProgram.cs](../EsfParser/Esf/EsfProgram.cs), [EsfProgramBuilder.cs](../EsfParser/Builder/EsfProgramBuilder.cs) |
| Logic AST | [VisualAgeLogicParser.cs](../EsfParser/Parser/Logic/VisualAgeLogicParser.cs), [Statements/](../EsfParser/Parser/Logic/Statements/) |
| Operand → C# | [CSharpUtils.cs](../EsfParser/CodeGen/CSharpUtils.cs) |
| Project emitter | [RoslynExporter.Structured.cs](../EsfParser/CodeGen/RoslynExporter.Structured.cs) |
| SQL emission | [RoslynExporter.Structured.cs](../EsfParser/CodeGen/RoslynExporter.Structured.cs), [SqlEmitHelpers.cs](../EsfParser/CodeGen/SqlEmitHelpers.cs) |
| Generated runtime | [SqlHelpers.cs](../EsfParser/Runtime/SqlHelpers.cs), [ConverseConsole.cs](../EsfParser/Runtime/ConverseConsole.cs), [ConsoleMapRenderer.cs](../EsfParser/Runtime/ConsoleMapRenderer.cs), [EzFunctions.cs](../EsfParser/CodeGen/EzFunctions.cs) |
| CLI host | [Program.cs](../EsfConsoleConverter/Program.cs) |
| Tests | [EsfParser.Tests/](../EsfParser.Tests/) |
