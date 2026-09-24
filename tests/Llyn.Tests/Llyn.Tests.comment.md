# Llyn.Tests.csproj

## `<TargetFramework>net10.0-windows</TargetFramework>`

The tests reference Deportment, which builds for Windows.

## `<ItemGroup>`

The engine seeds the controlled vocabularies from the language packs when it binds to a workspace.
So the packs have to sit beside the test binaries as they sit beside the application's.
