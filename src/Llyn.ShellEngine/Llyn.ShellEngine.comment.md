# Llyn.ShellEngine.csproj

Builds the shell-facing engine that connects application workflows to the interface.

## Project ring

References Application alone and carries Core records through it.
Grants internal access to Llyn.Tests for engine-level tests.

## `<WarningsAsErrors>CA1416</WarningsAsErrors>`

A Windows-only call turns the build red, so the platform stays out of ShellEngine.
