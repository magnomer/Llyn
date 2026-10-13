# Llyn.ShellEngine

The shell-facing engine that connects application workflows to the interface.
It references Application alone and carries Core records through it.

## `Llyn.ShellEngine.csproj`

It grants internal access to Llyn.Tests.Interface, whose relays reach the engine for the tests.
It grants Llyn.Tests.Engine too, since some pronunciation tests name the internal `LTrove`.
A Windows-only call turns the build red through `CA1416` as an error, so the platform stays out of ShellEngine.

## Stamp

Commit: `c2f335c331fe`
- `Llyn.ShellEngine.csproj` `4770c005e4deab10`
