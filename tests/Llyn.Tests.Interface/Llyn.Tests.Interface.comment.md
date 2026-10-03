# Llyn.Tests.Interface.csproj
Hash: `5a64ed92eefef899`

Builds the relay layer, the one place the behaviour tests reach production code.

## `<TargetFramework>net10.0</TargetFramework>`

The relays drive only portable projects, so they target the portable framework.

## `<IsTestProject>true</IsTestProject>`

The project carries `TInterfaceBoundary`, which guards the relay rule for every behaviour project.

## `<WarningsAsErrors>CA1416</WarningsAsErrors>`

A Windows API reached from here fails the build, as it does in every portable project.

## `<ItemGroup>`

The engine seeds the controlled vocabularies from the language packs when it binds to a workspace.
The packs copy beside these binaries, and each referencing test project receives them with the reference.

## `<InternalsVisibleTo Include="Llyn.Tests.Engine" />`

The relays are internal, so each behaviour project needs a grant to reach them.

## `<InternalsVisibleTo Include="Llyn.Tests.Conduct" />`

The Conduct tests reach the same relays through this grant.

## `<InternalsVisibleTo Include="Llyn.Tests.Windows" />`

The Windows tests reach the relays through this grant instead of keeping a second relay layer.

## `<ProjectReference Include="../../src/Llyn.Core/Llyn.Core.csproj" />`

Fakes and fixtures build Core records directly.

## `<ProjectReference Include="../../src/Llyn.Infrastructure/Llyn.Infrastructure.csproj" />`

`TWorkspace` opens a real workspace over the Infrastructure stores.

## `<ProjectReference Include="../../src/Llyn.ShellEngine/Llyn.ShellEngine.csproj" />`

The engine relays call ShellEngine.

## `<ProjectReference Include="../../src/Llyn.Conduct/Llyn.Conduct.csproj" />`

Conduct is the deepest portable layer the tests drive, through its gates.
