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

## Stamp

Commit: `c2f335c331fe`
- `Llyn.Conduct.csproj` `246ef06664a7ce7e`
