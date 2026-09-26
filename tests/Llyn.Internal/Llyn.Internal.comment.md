# Llyn.Internal.csproj

Builds the portable behaviour tests, everything that runs without Windows.

## `<TargetFramework>net10.0</TargetFramework>`

The tests drive only portable projects, so they target the portable framework.
A test that needs Windows belongs in `Llyn.Windows`, whatever its folder.

## `<WarningsAsErrors>CA1416</WarningsAsErrors>`

A Windows API reached from here fails the build, as it does in every portable project.

## `<ItemGroup>`

The engine seeds the controlled vocabularies from the language packs when it binds to a workspace.
So the packs have to sit beside the test binaries as they sit beside the application's.

## `<InternalsVisibleTo Include="Llyn.Windows" />`

The relays under `Interface` are internal.
The Windows tests reach them through this grant instead of keeping a second relay layer.

## `<ProjectReference Include="../../src/Llyn.Conduct/Llyn.Conduct.csproj" />`

Conduct is the deepest portable layer the tests drive, through its gates.
