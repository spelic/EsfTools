# EsfTools

EsfTools is a modular .NET solution for parsing, analyzing, and converting ESF files. It provides a core parsing library, analytics utilities, code generation tools, and a console application for conversion and debugging. The project is organized for extensibility and ease of use in ESF file workflows.

---

## Table of Contents

- [Project Structure](#project-structure)
- [Features](#features)
- [Getting Started](#getting-started)
- [Usage](#usage)
- [Architecture](#architecture)
- [Extending EsfTools](#extending-esftools)
- [Testing](#testing)
- [License](#license)

---

## Project Structure

```
EsfTools/
│
├── EsfParser/                # Core ESF parsing library
│   ├── Builder/              # Program and node builders
│   ├── CodeGen/              # ESF to C# code generation
│   ├── Analytics/            # Analysis utilities
│   ├── Parser/               # Parsing logic and tag parsers
│   └── Esf/                  # ESF program and statement models
│
├── EsfConsoleConverter/      # Console app for conversion/debugging
│   ├── EsfDebuggerHelper.cs  # Debugging and conversion helpers
│   └── Program.cs            # Main entry point
│
├── EsfParser.Tests/          # Unit tests for parser and analytics
│
├── EsfTags/                  # Tag definitions (if present)
│
├── EsfTools.sln              # Solution file
└── README.md                 # Project documentation
```

---

## Features

- **ESF Parsing:**  
  - Reads and parses ESF files into structured objects.
  - Supports tag-based parsing for extensibility.

- **Analytics:**  
  - Extracts unparsed statements, original code by type, and important expressions.
  - Provides program-level analysis utilities.

- **Code Generation:**  
  - Converts ESF logic into C# code.
  - Auto-qualifies references to special functions, global items, and workstor fields.

- **Console Conversion & Debugging:**  
  - Command-line interface for converting and debugging ESF files.
  - Supports batch processing and exporting results.

- **Testing:**  
  - Comprehensive unit tests for parser and analytics logic.

---

## Getting Started

### Prerequisites

- [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- Visual Studio Code or compatible IDE

### Build

```sh
dotnet build EsfTools.sln
```

### Run Console Converter

```sh
dotnet run --project EsfConsoleConverter
```

---

## Usage

### Parsing an ESF File

```csharp
using EsfParser.Builder;
using EsfParser.Esf;

var esfProgram = EsfProgramBuilder.ParseFromFile("path/to/file.esf");
```

### Analyzing an ESF Program

```csharp
using EsfParser.Analytics;

var analytics = new EsfProgramAnalytics(esfProgram);
var unparsed = analytics.GetUnparsedStatements();
```

### Converting ESF Logic to C#

```csharp
using EsfParser.CodeGen;

string csCode = EsfLogicToCs.Convert(esfProgram);
```

### Using the Console Converter

```sh
dotnet run --project EsfConsoleConverter -- input.esf output.cs
```

---

## Architecture

- **EsfParser:**  
  - Implements parsing logic, tag parsers, and program builders.
  - Models ESF programs and statements.

- **EsfConsoleConverter:**  
  - Provides CLI for conversion and debugging.
  - Uses EsfDebuggerHelper for file operations.

- **EsfParser.Tests:**  
  - Validates parsing and analytics with unit tests.

- **EsfTags:**  
  - Defines tags for parsing (if present).

---

## Extending EsfTools

- **Add New Tag Parsers:**  
  - Implement new tag parser classes in `EsfParser/Parser/Tags/`.
  - Register them in `EsfProgramBuilder`.

- **Enhance Analytics:**  
  - Extend `EsfProgramAnalytics` with new analysis methods.

- **Custom Code Generation:**  
  - Modify or extend `EsfLogicToCs` for new output formats.

---

## Testing

Run all unit tests:

```sh
dotnet test EsfParser.Tests
```

---

## License

This project is licensed under the MIT License.

---

## Contact

For questions or contributions, please open an issue or pull request on