# CLibrary.cs

## `public sealed class CLibrary`

The library panel: every entry, the vista it holds and the panel over it.
It finds the rows, and takes the query, order and language filter.
Its panel loads, edits and deletes the chosen entry, and its own editor edits it.
It prints and exports the chosen entry, and imports a markup file into the workspace.

## `private CLibrary(CAtelier atelier, Func<bool> shownSeam, CEnvoy envoy)`

Takes the atelier's ports and builds the panel's own entry editor.
The panel asks the editor's desk before it leaves an entry, and finishes through the editor.
A cleared panel empties the editor, and an edited row opens in it.

## `public static CLibrary CLibraryCreate(CAtelier atelier, Func<bool> shownSeam, CEnvoy envoy)`

Builds the panel over the atelier, so the driver hands it no port.
Building it is no user action, so it is no gate on the atelier.

## `public CEditor CLibraryEditor { get; }`

The entry editor the panel opens its chosen entry in.
The driver wraps it for the editor view and the lectern.

## `public bool CLibraryFiltered`

Whether the language filter hides any language, as the vista answers it.

## `public void CLibraryVistaRestore()`

Starts the library vista through the atelier, headword order before any order is saved.
The panel and the editor both take it, on a workspace start or switch.

## `public void CLibraryOrderSet(CCatalogOrder? order)`

Hands a chosen ordering to the vista, which keeps its own when the sender is no order row.

## `public IReadOnlyList<CVistaRow> CLibraryRowsRead()`

The rows the engine returns for the vista, none before a vista arrives.
They come already filtered, sorted, numbered and marked, so the list decides nothing about them.

## `internal string LLibraryFileRead()`

The file name an export of the chosen entry is offered under.

## `public Task CLibraryPortraitPrint()`

Prints the chosen entry.
The reader is asked for the printer through the panel's envoy, and a decline prints nothing.
`CPortrait` words the page through the engine and shows `Print.Failed` through the panel's envoy.

## `public Task CLibraryPortraitExport()`

Exports the chosen entry to `path` in the chosen format.
The reader is asked for the file and format through the panel's envoy, and a decline exports nothing.
`CPortrait` words the page through the engine and shows `Export.Failed` through the panel's envoy.

## `public async Task CLibraryMarkupImport(string? path)`

Imports the markup file the user picked, and a null path is a cancelled pick that changes nothing.
The engine reads the file, asks for the intakes through this panel, and stores the cargo in one call.
A declined declaration stores nothing, so the rows are not refreshed and nothing is reported.
After a store the rows refresh, and what the import could not place is shown through the envoy.
Any failure is shown under `List.ImportFailed`, and the engine stores every entry or none.

## `private IReadOnlyList<LMarkupIntake>? LLibraryIntakeRead(IReadOnlyList<LMarkupEntry> entries)`

Asks the customs question over the parsed entries, and maps each declared row to its engine intake.
A row's place in the answer is its entry's place in the file, so the index is the position.
The mode maps by name, and the engine's factory drops the target of a new entry.

## `private static LMarkupMode LLibraryModeRead(CSCustomsMode mode)`

Maps each declared mode to the engine's mode by name, holding no rule.
A mode it does not know throws, so a new member cannot slip through as another.

## `private static CMarkupEntry LLibraryEntryRead(LMarkupEntry entry)`

Copies what the customs question shows of one parsed entry, holding no rule.

## `private static CMarkupOmission LLibraryOmissionRead(LMarkupOmission omission)`

Copies one line the import left behind, holding no rule.
