# Llyn.Tests.Conduct

Builds the portable behaviour tests of Conduct.
It stands apart from its neighbours by reaching production code only through the relay project.

## `<TargetFramework>net10.0</TargetFramework>`

Conduct is portable, so its tests target the portable framework.
A test that needs Windows belongs in `Llyn.Tests.Windows`, whatever its folder.

## `<WarningsAsErrors>CA1416</WarningsAsErrors>`

A Windows API reached from here fails the build, as in every portable project.

## `<AssemblyAttribute Include="Xunit.CollectionBehaviorAttribute">`

Disposing a `TWorkspace` clears every pooled SQLite connection in the process.
The switch in `TWorkspace.cs` covers only the Interface assembly, so this project repeats it.
Two workspaces alive in parallel let one disposal close a connection the other is using.

## `<ProjectReference Include="../Llyn.Tests.Interface/Llyn.Tests.Interface.csproj" />`

The tests reach production code only through the relays, fakes and workspace of the relay project.
The reference also brings the language packs beside the test binaries.

## Stamp

Commit: `c2f335c331fe`
- `Llyn.Tests.Conduct.csproj` `2419c500b17e946c`
