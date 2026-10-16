# Llyn.Tests.Console

Builds the portable tests of Demeanor, the console driver.
They drive the console through a fake port and assert stdout, stderr and the exit code.

## `Llyn.Tests.Console.csproj`

Demeanor is portable, so its tests target the portable framework.
A Windows API reached from here fails the build through CA1416.

## `<AssemblyAttribute Include="Xunit.CollectionBehaviorAttribute">`

Disposing a `TWorkspace` clears every pooled SQLite connection in the process.
The switch in `TWorkspace.cs` covers only the Interface assembly, so this project repeats it.

## `<ProjectReference Include="../Llyn.Tests.Interface/Llyn.Tests.Interface.csproj" />`

The tests reach the engine and Conduct through the relays, fakes and workspace of the relay project.
The reference also brings the language packs beside the test binaries.

## `<ProjectReference Include="../../src/Llyn.UIDemeanor/Llyn.UIDemeanor.csproj" />`

Demeanor grants this project its internals, so the strict types stay internal.

## `TConsoleFake`

The console port with no terminal behind it.
Each print lands in a list, so a test asserts stdout and stderr line by line.
The terminal answer and the line to read are set by the test.
The modes do nothing, since no code page exists to change.

## Stamp

Commit: `ed7d18d90760`
- `Llyn.Tests.Console.csproj` `c830d51b81636ccb`
- `TConsoleFake.cs` `67edc837c401ad25`
