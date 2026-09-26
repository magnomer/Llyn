# Llyn.Core.csproj

Builds the domain model and application-facing contracts.

## Project ring

Has no project references, keeping the core independent of outer layers.
Grants internal access to Llyn.Internal for direct contract and model tests.

## `<WarningsAsErrors>CA1416</WarningsAsErrors>`

A Windows-only call turns the build red, so the platform stays out of Core.
