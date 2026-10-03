# Llyn.Core.csproj
Hash: `af732680c4489b17`

Builds the domain model and application-facing contracts.

## Project ring

Has no project references, keeping the core independent of outer layers.
Grants internal access to Llyn.Tests.Interface, whose relays reach contracts and models for the tests.

## `<WarningsAsErrors>CA1416</WarningsAsErrors>`

A Windows-only call turns the build red, so the platform stays out of Core.
