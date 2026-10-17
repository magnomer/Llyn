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
The mode apply does nothing, since no code page exists to change.
The trail lists notices and restore calls in order, so a test asserts what printed after the restore.
The window flag is null before the apply, true after it and false after the restore.

## `TConsoleFakeFrames`

Every frame the canvas printed, as the raw text with its cursor moves.
A test counts them to know how often the window drew.

## `TConsoleFakeGrid`

The screen rebuilt from the frames, one string per row.
It reads the cursor moves and the cell width of each cluster, so a wide cluster fills two cells.
The grid is empty until a first frame arrives.

## `TConsoleFakeKeys`

The keys the test queues for the key reader.
A key read blocks until a frame was printed, so the window is drawn before any key acts.
A default key is a placeholder that applies the next queued size.

## `TConsoleFakeAdjust`

Queues a console size together with a placeholder key.
The key reader applies the size after the first frame, then waits for the frame that size causes.
So the test sees every size drawn.

## Stamp

Commit: `66a4aa567097`
- `Llyn.Tests.Console.csproj` `c830d51b81636ccb`
- `TConsoleFake.cs` `9019bccd286ef59a`
