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

## `<ProjectReference Include="../../src/Llyn.UITerminal/Llyn.UITerminal.csproj" />`

The tests load the real console blueprints, so a broken blueprint file fails a test and not only the start.

## `<EmbeddedResource Include="Kit/TBlueprintFake.xaml" />`

The three `TBlueprint*` files are embedded so the loader finds them as it finds a UI's own.
`TBlueprintFake` lists `TBlueprintListed`, and `TBlueprintUnlisted` is embedded without a listing.

## `TConsoleFake`

The console port with no terminal behind it.
Each print lands in a list, so a test asserts stdout and stderr line by line.
The terminal answer and the line to read are set by the test.
The mode apply does nothing, since no code page exists to change.
The trail lists notices and restore calls in order, so a test asserts what printed after the restore.
Each frame also keeps a copy of the grid, so a test reads what a box showed after it closed.
A test may set a count of frames to refuse, and each refused frame throws after it is recorded.
Its theme gives every role its own exact colour, so each escape the canvas prints names one role.
The role grid keeps the role of each cell the frames drew, null where no known colour was set.
`TConsoleFakeRun` runs the test body on a background thread.
It fails the test if the body outlasts the seconds given, 60 by default.
A fault the body throws is rethrown on the test thread.
The window flag is null before the apply, true after it and false after the restore.
A test may also make it count ambiguous characters as wide, and the grid then follows that measure.

## `TConsoleFakeFrames`

Every frame the canvas printed, as the raw text with its cursor moves.
A test counts them to know how often the window drew.

## `TConsoleFakeGrid`

The screen rebuilt from the frames, one string per row.
It reads the cursor moves and the cell width of each cluster, so a wide cluster fills two cells.
A wide cluster also tints its second cell, so the role grid matches the text grid.
The width of an ambiguous character follows the fake's ambiguous setting.
Colour escapes are stepped over and change nothing, so a test reads the text under any theme.
The grid is empty until a first frame arrives.

## `TConsoleFakeKeys`

The keys the test queues for the key reader.
A key read blocks until a frame was printed, so the window is drawn before any key acts.
A default key is a placeholder that applies the next queued size.

## `TConsoleFakeAdjust`

Queues a console size together with a placeholder key.
The key reader applies the size after the first frame, then waits for the frame that size causes.
So the test sees every size drawn.

## `TConsoleFakeReads`

The count of key reads the key reader has begun.
A new read means the loop took the last key and now asks for the next.

## `TConsoleFakeSettle`

Sends one key, then waits until the loop has handled it.
A frame count cannot do this, since a key that changes nothing draws no frame.
Such a wait ran to its 5 second guard.
The method waits for the next key read, which shows the loop took the key.
It then defers two hops on the loop and runs the window's batch on the second.
The second hop lets work the key posted run first, and the batch paints and signals.
A stop token or the 5 second guard ends either wait early.

## Stamp

Commit: `c37ca1837ded`
- `Llyn.Tests.Console.csproj` `bceadb309d05ccaa`
- `TConsoleFake.cs` `09ac3e609fe8878c`
