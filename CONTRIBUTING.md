# Contributing to MossSharp

Thank you for contributing to **MossSharp**, the C# binding for Moss Framework.

MossSharp exposes Moss through C# while maintaining a clear boundary around the native implementation. Public API design, resource lifetime, marshaling, native loading, and deterministic cleanup are therefore important considerations.

## Before You Start

For public C# API, native interop, ownership, or ABI changes, open an issue before substantial implementation.

Please search existing issues and pull requests before starting work.

## Development Requirements

- Git
- A supported .NET SDK
- C#
- C++17
- CMake where required by the native component
- A local Moss Framework checkout

## Building and Testing

```bash
git clone https://github.com/TxbiG/MossSharp.git
cd MossSharp

dotnet build
dotnet test
```

Use the repository's documented native build/configuration process when the native Moss component must be built separately.

## Repository Structure

- `src/` — C# and native binding implementation
- `docs/` — documentation
- `examples/` — examples
- `tests/` — tests
- `performance/` — benchmarks
- `MossSharp.csproj` — project configuration
- `NuGet.config` — NuGet configuration
- `.github/workflows/` — CI

## Areas for Contribution

- C# API coverage
- Native interop
- Resource ownership
- Rendering
- Physics
- Audio
- Input
- Networking
- XR
- GUI
- Examples
- Documentation
- Performance
- Tests
- Packaging

## C# API Guidelines

Prefer idiomatic C# while preserving Moss semantics.

For native resources, make lifetime explicit. Use deterministic disposal such as `IDisposable` where appropriate rather than relying on finalizers as the primary cleanup mechanism.

Consider:

- Safe wrapper types
- Clear exception behaviour
- Nullable annotations where appropriate
- Thread-affinity requirements
- Avoiding unnecessary allocations
- Predictable ownership

## Native Interop

Keep interop code isolated and document non-obvious marshaling assumptions.

Be careful with:

- Struct layout
- Calling conventions
- Native pointers
- Strings
- Arrays
- Ownership/lifetime
- Platform-specific native library loading

When managing native resources manually, provide deterministic cleanup and make repeated disposal safe where practical.

## Testing

Run:

```bash
dotnet build
dotnet test
```

Where applicable, test:

- Resource creation/destruction
- Invalid arguments
- Native error propagation
- Marshaling
- Repeated disposal
- Platform-specific library loading
- Public API behaviour

## Packaging

Changes to NuGet packaging, project metadata, native library inclusion, or release artefacts should be tested using the actual packaging workflow.

A successful local managed build does not necessarily prove that the native runtime package is complete.

## Commit Messages

Recommended prefixes:

```text
feat: expose Moss renderer API
fix: dispose native window correctly
docs: add C# audio example
test: cover native texture lifetime
perf: reduce marshaling allocations
build: improve native library discovery
```

## Pull Requests

Include:

- What changed
- C# usage example where appropriate
- Tests performed
- .NET SDK version
- Native compiler/toolchain
- Platform information
- ABI/API considerations
- Packaging impact where relevant

## Licence

MossSharp is distributed under the **MIT License**. Contributions should be compatible with the repository's licence and applicable .NET/Moss/third-party licence requirements.

Thank you for contributing.
