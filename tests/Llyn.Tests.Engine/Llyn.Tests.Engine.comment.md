# Llyn.Tests.Engine.csproj
Hash: `2419c500b17e946c`

Builds the portable behaviour tests of Core, Application, Infrastructure and ShellEngine.

## `<TargetFramework>net10.0</TargetFramework>`

The tests drive only portable projects, so they target the portable framework.
A test that needs Windows belongs in `Llyn.Tests.Windows`, whatever its folder.

## `<WarningsAsErrors>CA1416</WarningsAsErrors>`

A Windows API reached from here fails the build, as it does in every portable project.

## `<AssemblyAttribute Include="Xunit.CollectionBehaviorAttribute">`

Disposing a `TWorkspace` clears every pooled SQLite connection in the process.
The switch in `TWorkspace.cs` covers only the Interface assembly, so this project repeats it.
Two workspaces alive in parallel let one disposal close a connection the other is using.

## `<ProjectReference Include="../Llyn.Tests.Interface/Llyn.Tests.Interface.csproj" />`

The tests reach production code only through the relays, fakes and workspace of the relay project.
The reference also brings the language packs beside the test binaries.
