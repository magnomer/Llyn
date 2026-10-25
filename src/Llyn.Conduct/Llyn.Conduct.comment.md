# Llyn.Conduct

Conduct builds against ShellEngine and carries no platform-specific target framework.
Core and Application arrive through ShellEngine, never by a reference of their own.

## `Llyn.Conduct.csproj`

The target framework has no Windows qualifier.
That keeps this project's target independent of the Windows UI.

A Windows-only call turns the build red, because `CA1416` is an error.
So the platform stays out of Conduct.

The sole direct project reference is ShellEngine.
It supplies engine ports and tenure quills.

Four assemblies see Conduct internals.
- `Llyn.Tests.Interface` is the relay layer, so behaviour tests need not reach internals.
- `Llyn.Tests.Conduct` still reaches some internal members directly.
- `Llyn.Tests.Windows` shows an entry through `LDisplayRule`, the internal rules of a display.
- `Llyn`, the host, builds `CAtelier` over the ports it makes.
  The constructor naming those ports stays internal, so no driver can build one.

`LlynVersionWrite` writes `CVersion.g.cs` into the intermediate folder before each compile.
It holds `CVersionText`, the `LlynVersion` that `Directory.Build.props` reads from `version.json`.
Conduct is a pure ring, so the build hands it the version and no reflection reads it.
The file is written only when the text differs, so an unchanged version recompiles nothing.

## Stamp

Commit: `8c403d1df768`
- `Llyn.Conduct.csproj` `aa767fc9c5772586`
