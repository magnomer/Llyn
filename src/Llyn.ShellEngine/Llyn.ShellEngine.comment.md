# Llyn.ShellEngine.csproj
Hash: `c3102cc7d5899e98`

Builds the shell-facing engine that connects application workflows to the interface.

## Project ring

References Application alone and carries Core records through it.
Grants internal access to Llyn.Internal for engine-level tests.

## `<WarningsAsErrors>CA1416</WarningsAsErrors>`

A Windows-only call turns the build red, so the platform stays out of ShellEngine.
