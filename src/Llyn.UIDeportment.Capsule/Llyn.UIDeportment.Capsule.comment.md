# Llyn.UIDeportment.Capsule.csproj
Hash: `c26a2a97a04873fe`

Builds Deportment's own storage, which keeps the GUI-only state that outlives a run.

## `<TargetFramework>net10.0</TargetFramework>`

The storage reads and writes files alone, so it builds for every platform.

## `<WarningsAsErrors>CA1416</WarningsAsErrors>`

A Windows-only call turns the build red, so the platform stays out of the storage.

## `<DisableTransitiveProjectReferences>true</DisableTransitiveProjectReferences>`

The storage stands above the cut and names no Llyn project, so it compiles against nothing beyond .NET.
