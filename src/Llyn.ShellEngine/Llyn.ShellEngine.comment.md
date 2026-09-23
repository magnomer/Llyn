# Llyn.ShellEngine.csproj

Builds the shell-facing engine that connects application workflows to the interface.

## Project ring

References Core and Application, while infrastructure adapters are supplied from outside this project.
Grants internal access to Llyn.Tests for engine-level tests.
