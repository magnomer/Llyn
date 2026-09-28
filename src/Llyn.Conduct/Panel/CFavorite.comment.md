# CFavorite.cs

## `public sealed class CFavorite`

The favorites panel: the marked entries, the vista it holds and the panel over it.
It finds the rows, and takes the query, order and language filter.
Its panel loads, edits and deletes the chosen entry, and its own editor edits it.
It also prints and exports the chosen entry.

## `private CFavorite(CAtelier atelier, Func<bool> shownSeam, CEnvoy envoy)`

Takes the atelier's ports and builds the panel's own entry editor.
The panel asks the editor's desk before it leaves an entry, and finishes through the editor.
A cleared panel empties the editor, and an edited row opens in it.

## `public static CFavorite CFavoriteCreate(CAtelier atelier, Func<bool> shownSeam, CEnvoy envoy)`

Builds the panel over the atelier, so the driver hands it no port.
Building it is no user action, so it is no gate on the atelier.

## `public CEditor CFavoriteEditor { get; }`

The entry editor the panel opens its chosen entry in.
The driver wraps it for the editor view and the lectern.

## `public bool CFavoriteFiltered`

Whether the language filter hides any language, as the vista answers it.

## `public void CFavoriteVistaRestore()`

Starts the favorite vista through the atelier, headword order before any order is saved.
The panel and the editor both take it, on a workspace start or switch.

## `public void CFavoriteGraspResonate()`

Answers a grasp notice by refreshing the rows, but only while they are ordered by grasp.
A grasp change in any other order moves no row, so the list is left alone.

## `public void CFavoriteOrderSet(CCatalogOrder? order)`

Hands a chosen ordering to the vista, which keeps its own when the sender is no order row.

## `public IReadOnlyList<CVistaRow> CFavoriteRowsRead()`

The marked rows the engine returns for the vista, none before a vista arrives.
They come already filtered, sorted and marked, so the list decides nothing about them.

## `public IReadOnlyList<string> CFavoriteLanguageRead()`

The languages the filter offers, as the settings list them.

## `internal string LFavoriteFileRead()`

The file name an export of the chosen entry is offered under.

## `public Task CFavoritePortraitPrint()`

Prints the chosen entry.
The reader is asked for the printer through the panel's envoy, and a decline prints nothing.
`CPortrait` words the page through the engine and shows `Print.Failed` through the panel's envoy.

## `public Task CFavoritePortraitExport()`

Exports the chosen entry to `path` in the chosen format.
The reader is asked for the file and format through the panel's envoy, and a decline exports nothing.
`CPortrait` words the page through the engine and shows `Export.Failed` through the panel's envoy.
