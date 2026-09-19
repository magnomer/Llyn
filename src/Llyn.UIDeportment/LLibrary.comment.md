# LLibrary.cs

## `public sealed class LLibrary`

The deportment of the library panel: the shared panel state, the entry rows it browses, and the markup import.
The panel state is held rather than inherited, because a shell type deriving from logic is a custody hit.
It keeps its own handle on the vista.
A vista read off the panel would be an engine answer to branch on.

## `public event Action<string, Exception>? LLibraryFailed;`

An import that failed, named by the key the window localizes and carrying the exception.
The engine imports every entry or none, so nothing needs undoing before it is shown.

## `private int _lLibraryCount;`

How many rows the last read returned, so the empty notice is a verdict rather than a veneer count.

## `public IReadOnlyList<LVistaRow> LLibraryRowsRead()`

The rows the engine returns for the vista, already filtered, sorted, numbered and marked.

## `public long LLibraryVoyageRead()`

The entry the panel shows, as the station the window records before a jump away.
Zero says no entry is shown, so there is no place to come back to.

## `public void LLibraryOrderSet(string? choice)`

Hands a chosen ordering to the vista, which saves and announces it.
A sender without a tag names no ordering and is ignored.

## `public Task LLibraryPortraitPrint(LPortraitLabel label, LPressTicket ticket)`

Prints the chosen entry as the engine portrays it, with the labels the window localized.

## `public async Task LLibraryMarkupStart(`

The whole import, with each question the reader answers asked through a seam.
The path seam picks the file and answers null for a cancelled pick, which leaves the workspace as it was.
The file is read once, and the cargo it yields goes straight into the import as an argument.
Held as a parameter, the cargo is never a value this class keeps between two requests.
A failure in either hop is announced rather than shown, because the deportment holds no window.

## `private async Task LLibraryMarkupImport(`

The customs seam shows what the cargo declares before anything is written.
A seam answering null is a declined declaration, and the workspace is left untouched.
The engine then imports under the declared intakes, and the rows are re-read from what the workspace holds.
What the import could not place goes to the omission seam after the rows already hold the entries.
