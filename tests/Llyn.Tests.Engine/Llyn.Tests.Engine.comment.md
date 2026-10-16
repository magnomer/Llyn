# Llyn.Tests.Engine

Builds the portable behaviour tests of Core, Application, Infrastructure, ShellEngine and Guestroom.
The tests reach production code only through the relays, fakes and workspace of the relay project.

## `Llyn.Tests.Engine.csproj`

The tests drive only portable projects, so they target the portable framework.
A test that needs Windows belongs in `Llyn.Tests.Windows`, whatever its folder.
A Windows API reached from here fails the build, as it does in every portable project.
The reference to `Llyn.Tests.Interface` also brings the language packs beside the test binaries.

## Inline notes

### Llyn.Tests.Engine.csproj: Xunit.CollectionBehaviorAttribute

Disposing a `TWorkspace` clears every pooled SQLite connection in the process.
The switch in `TWorkspace.cs` covers only the Interface assembly, so this project repeats it.
Two workspaces alive in parallel let one disposal close a connection the other is using.

## Stamp

Commit: `c2f335c331fe`
- `Llyn.Tests.Engine.csproj` `2419c500b17e946c`
