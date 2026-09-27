# LLibrary.cs

## `public sealed class LLibrary`

The deportment of the library panel: the shared panel state, the entry rows it browses, and the markup import.
The panel state is held rather than inherited, because a shell type deriving from logic is a custody hit.
It keeps its own handle on the vista.
A vista read off the panel would be an engine answer to branch on.
The panel's loads and clears go straight to the lectern its view hands in, so the veneer relays no draft.
It names no WPF type, so the entry list, the droppers and the sieve mark live in `QLibrary`.

## `internal LLibrary(`

Only the panel factory builds it over the engine's ports, so no view names a port.

## `public event Action<string, Exception>? LLibraryFailed;`

An import that failed, named by the key the window localizes and carrying the exception.
The engine imports every entry or none, so nothing needs undoing before it is shown.

## `public LEditor LLibraryEditor { get; }`

The entry editor's deportment, whose desk answers whether the panel may leave and opens the row that is edited.

## `public bool LLibraryFiltered`

Whether the vista hides any language, which the view shows as the mark on the sieve button.

## `internal void LLibraryVistaRestore(LVista vista)`

Hands the vista to the panel state and the editor.
It stays internal, so only the atelier's vista restore and the Windows tests pass an engine vista.

## `public IReadOnlyList<CVistaRow> LLibraryRowsRead()`

The rows the engine returns for the vista, already filtered, sorted, numbered and marked.
They cross as shapes, so the view names no engine row.

## `public long LLibraryVoyageRead()`

The entry the panel shows, as the station the window records before a jump away.
Zero says no entry is shown, so there is no place to come back to.

## `public void LLibraryOrderSet(CCatalogOrder? order)`

Hands a chosen ordering to the vista, which saves and announces it.
A sender that is no order row hands null, which keeps the ordering it has.

## `public void LLibrarySieveSet(CCatalogFilter filter)`

Hands the ticked languages to the vista, which saves and announces them.

## `public Task LLibraryPortraitPrint(CPortraitLabel label, CPressTicket ticket)`

Prints the chosen entry as the engine portrays it, with the labels the window localized.
The label and the ticket are mapped to the engine's own by the panel's shared maps.

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

## `internal static IReadOnlyList<CMarkupEntry> LLibraryEntryRead(IReadOnlyList<LMarkupEntry> entries)`

Maps the parsed entries to what the customs dialog shows and searches by.
The rows keep the file's order, so a declared row's place is its intake's index.

## `internal static IReadOnlyList<LMarkupIntake> LLibraryIntakeRead(IReadOnlyList<CSCustomsRow> rows)`

Maps the dialog's declared rows to the engine's intakes, one per parsed entry in file order.
A fresh row carries no target, which the engine's factory enforces.

## `internal static IReadOnlyList<CMarkupOmission> LLibraryOmissionRead(IReadOnlyList<LMarkupOmission> omissions)`

Maps what the import left behind to the rows the import report lists.
