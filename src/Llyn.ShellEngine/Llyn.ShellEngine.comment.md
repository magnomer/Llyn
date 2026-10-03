# Llyn.ShellEngine.csproj
Hash: `4770c005e4deab10`

Builds the shell-facing engine that connects application workflows to the interface.

## Project ring

References Application alone and carries Core records through it.
Grants internal access to Llyn.Tests.Interface, whose relays reach the engine for the tests.
Grants Llyn.Tests.Engine too, since some pronunciation tests name the internal `LTrove`.

## `<WarningsAsErrors>CA1416</WarningsAsErrors>`

A Windows-only call turns the build red, so the platform stays out of ShellEngine.
