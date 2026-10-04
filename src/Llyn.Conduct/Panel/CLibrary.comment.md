# CLibrary.cs
Hash: `4b97abb3aab16e2f`

## `public sealed class CLibrary`

The library panel: every entry, the vista it holds and the panel over it.
It finds the rows, and takes the query, order and language filter.
Its panel loads, edits and deletes the chosen entry, and its own editor edits it.
It prints and exports the chosen entry, and imports a markup file into the workspace.

## `private CLibrary(CAtelier atelier, Func<bool> shownSeam, CEnvoy envoy, Action<Action> marshal)`

Takes the atelier's ports and the marshal, and builds the panel's own entry editor.
It registers its vista restore and its close with the workspace.
The panel asks the editor's desk before it leaves an entry, and finishes through the editor.
A cleared panel empties the editor, and an edited row opens in it.
It restores its vistas last, so a built area already stands on started vistas.

## `public static CLibrary CLibraryCreate(CAtelier atelier, Func<bool> shownSeam, CEnvoy envoy, Action<Action> marshal)`

Builds the panel over the atelier, so the driver hands it no port.
The marshal puts each engine notice on the driver's thread.
Building it is no user action, so it is no gate on the atelier.

## `public event Action? CLibraryWorkspaceChanged;`

Raised once the workspace notice has closed the chosen entry, so the driver reloads its flags.

## `public CEditor CLibraryEditor { get; }`

The entry editor the panel opens its chosen entry in.
The driver wraps it for the editor view and the lectern.

## `public bool CLibraryFiltered`

Whether the language filter hides any language, as the vista answers it.

## `public bool CLibraryEmpty`

Whether the last rows read found nothing, which the list shows as its empty notice.
The library always answers a request, so an empty list is always shown as empty.

## `internal void LLibraryVistaRestore()`

Starts the library vista through the atelier, headword order before any order is saved.
The held query is carried into the fresh vista, so a switch keeps the search.
The panel and the editor both take it, on a workspace start or switch.
Then it attaches the observers for the vista, workspace, entry, reflex and settings notices through the marshal.
The constructor runs it last, and a workspace change runs it again through `CWorkspace`.

## `private void LLibraryObserverAttach()`

Attaches every notice the panel answers, each handed to the marshal first.
The chosen entry's draft notice is attached the same way.

## `private void LLibraryWorkspaceResonate()`

Answers the workspace notice by closing the chosen entry and raising `CLibraryWorkspaceChanged`.

## `private void LLibraryClose()`

The window's exit gate runs it, since the workspace holds it as a closure.
It stops the editor and cancels the display's playback.

## `public void CLibraryOrderSet(CCatalogOrder? order)`

Hands a chosen ordering to the vista, which keeps its own when the sender is no order row.

## `public static IReadOnlyList<CCatalogOrder> CLibraryOrderRead()`

The orderings the library offers, in the order its dropdown lists them.
The wings share them, so no driver names an engine ordering.

## `public IReadOnlyList<CVistaRow> CLibraryRowsRead()`

The rows the engine returns for the vista, none before a vista arrives.
They come already filtered, sorted, numbered and marked, so the list decides nothing about them.
The count is kept for `CLibraryEmpty`, so the empty notice follows the rows shown.

## `public Task<CEnsignSheet<IReadOnlyList<CVistaRow>>> CLibraryRowsLoad(Func<IReadOnlyList<CEnsignRow>, Action<string, Exception>, Action> store)`

Runs the flag fill into the driver's `store`, then answers `CLibraryRowsRead` beside the loaded languages.
The shared rule `CCatalog.LCatalogEnsignLoad` orders the two, so the driver makes one request.

## `internal string LLibraryFileRead()`

The file name an export of the chosen entry is offered under.

## `public Task CLibraryPortraitPrint()`

Prints the chosen entry.
The reader is asked for the printer through the panel's envoy, and a decline prints nothing.
`CPortrait` words the page through the engine and shows `Print.Failed` through the panel's envoy.

## `public Task CLibraryPortraitExport()`

Exports the chosen entry under the file name and format the reader picks.
The reader is asked for the file and format through the panel's envoy, and a decline exports nothing.
`CPortrait` words the page through the engine and shows `Export.Failed` through the panel's envoy.

## `public async Task CLibraryMarkupImport()`

Asks the envoy for a markup file first, before the customs and omission questions.
A cancelled pick answers no file and changes nothing.
The engine reads the file, asks for the intakes through this panel, and stores the cargo in one call.
A declined declaration stores nothing, so the rows are not refreshed and nothing is reported.
After a store the rows refresh, and what the import could not place is shown through the envoy.
A clean import asks nothing further, so the report is shown only when it holds a line.
Any failure is shown under `List.ImportFailed`, and the engine stores every entry or none.

## `private IReadOnlyList<LMarkupIntake>? LLibraryIntakeRead(IReadOnlyList<LMarkupEntry> entries, IReadOnlyList<IReadOnlyList<LMarkupTarget>> targets)`

Builds the customs gate over the parsed entries and their ready targets, then asks the customs question.
Once accepted, it reads the declared rows back from its own gate and maps each to its engine intake.
A row's place in the answer is its entry's place in the file, so the index is the position.
The mode maps by name, and the engine's factory drops the target of a new entry.

## `private static LMarkupMode LLibraryModeRead(CSCustomsMode mode)`

Maps each declared mode to the engine's mode by name, holding no rule.
A mode it does not know throws, so a new member cannot slip through as another.

## `private static CMarkupEntry LLibraryEntryRead(LMarkupEntry entry, IReadOnlyList<LMarkupTarget> targets)`

Copies what the customs question shows of one parsed entry and its engine-found target ids, holding no rule.

## `private static CMarkupTarget LLibraryTargetRead(LMarkupTarget target)`

Copies one engine-found target with its card counts, holding no rule.

## `private static CMarkupOmission LLibraryOmissionRead(LMarkupOmission omission)`

Copies one line the import left behind, holding no rule.
The line number is written in the current culture, so a driver only shows it.
