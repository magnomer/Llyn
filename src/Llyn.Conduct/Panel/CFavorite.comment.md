# CFavorite.cs
Hash: `66d4b9d194189bab`

## `public sealed class CFavorite`

The favorites panel: the marked entries, the vista it holds and the panel over it.
It finds the rows, and takes the query, order and language filter.
Its panel loads, edits and deletes the chosen entry, and its own editor edits it.
It also prints and exports the chosen entry.

## `private CFavorite(CAtelier atelier, Func<bool> shownSeam, CEnvoy envoy, Action<Action> marshal)`

Takes the atelier's ports and builds the panel's own entry editor.
It keeps the marshal, so every engine notice it answers runs on the driver's thread.
It registers its vista restore and its close with the workspace.
The panel asks the editor's desk before it leaves an entry, and finishes through the editor.
A cleared panel empties the editor, and an edited row opens in it.
It restores its vistas last, so a built area already stands on started vistas.

## `public static CFavorite CFavoriteCreate(CAtelier atelier, Func<bool> shownSeam, CEnvoy envoy, Action<Action> marshal)`

Builds the panel over the atelier, so the driver hands it no port.
Building it is no user action, so it is no gate on the atelier.
The driver hands the marshal that runs an answer on the UI thread.

## `public event Action? CFavoriteWorkspaceChanged;`

Raised after a workspace notice has closed the chosen entry.
The driver reloads its language flags on it.

## `public CEditor CFavoriteEditor { get; }`

The entry editor the panel opens its chosen entry in.
The driver wraps it for the editor view and the lectern.

## `public bool CFavoriteFiltered`

Whether the language filter hides any language, as the vista answers it.

## `internal void LFavoriteVistaRestore()`

Starts the favorite vista through the atelier, headword order before any order is saved.
The panel and the editor both take it, on a workspace start or switch.
The query the former vista held is carried into the fresh one, so the search box stays true.
Then it attaches the area's observers on the fresh vista only.
The constructor runs it last, and a workspace change runs it again through `CWorkspace`.

## `private void LFavoriteObserverAttach()`

Attaches each subject the list answers, every answer running through the marshal.
Vista, favorite, reflex and settings notices refill the rows.
An entry notice selects and repaints, and a chosen entry reloads its draft.
The workspace and grasp notices go to their own answers.

## `private void LFavoriteWorkspaceResonate()`

Answers the workspace notice by closing the chosen entry, then raising `CFavoriteWorkspaceChanged`.

## `private void LFavoriteClose()`

The favorites' part of the window's exit gate `CAtelier.CAtelierClose`, registered with the workspace at build.
It closes the editor and stops the playback the display started.
The driver releases only its recording player.

## `private void LFavoriteGraspResonate()`

Answers a grasp notice by refreshing the rows, but only while they are ordered by grasp.
A grasp change in any other order moves no row, so the list is left alone.

## `public void CFavoriteOrderSet(CCatalogOrder? order)`

Hands a chosen ordering to the vista, which keeps its own when the sender is no order row.

## `public static IReadOnlyList<CCatalogOrder> CFavoriteOrderRead()`

The orderings the favorites offer, in the order the menu lists them.

## `public IReadOnlyList<CVistaRow> CFavoriteRowsRead()`

The marked rows the engine returns for the vista, none before a vista arrives.
They come already filtered, sorted and marked, so the list decides nothing about them.
A failed read shows `Favorite.LoadFailed` through the envoy and answers no rows.

## `public Task<CEnsignSheet<IReadOnlyList<CVistaRow>>> CFavoriteRowsLoad(Func<IReadOnlyList<CEnsignRow>, Action<string, Exception>, Action> store)`

Runs the flag fill into the driver's `store`, then answers `CFavoriteRowsRead` beside the loaded languages.
The shared rule `CCatalog.LCatalogEnsignLoad` orders the two, so the driver makes one request.

## `internal string LFavoriteFileRead()`

The file name an export of the chosen entry is offered under.
`CPortrait` reads it only once the export starts, and a failed read shows `Export.NameFailed`.

## `public Task CFavoritePortraitPrint()`

Prints the chosen entry.
The reader is asked for the printer through the panel's envoy, and a decline prints nothing.
`CPortrait` words the page through the engine and shows `Print.Failed` through the panel's envoy.

## `public Task CFavoritePortraitExport()`

Exports the chosen entry to a file in the chosen format.
The reader is asked for the file and format through the panel's envoy, and a decline exports nothing.
`CPortrait` words the page through the engine and shows `Export.Failed` through the panel's envoy.
