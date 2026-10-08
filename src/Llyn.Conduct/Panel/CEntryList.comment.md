# CEntryList.cs
Hash: `df6aa00c05ef4589`

## `public sealed class CEntryList`

The entry list beside a chooser column, with its vista, its panel and its entry editor.
The xiesheng and yunjing panels share it, so the list, the reader and the press live in one place.
The owner keeps the chooser columns and hands the list one seam that finds its rows.
It restores its own vista, so no driver holds a port or a vista.

## `internal CEntryList(CAtelier atelier, Func<bool> shownSeam, CEnvoy envoy, Action<Action> marshal, string scope, string list, Func<bool> allowed, Func<LVista, IReadOnlyList<LVistaRow>> seam)`

Takes the atelier's ports and builds the list's own entry editor.
The panel asks the editor's desk before it leaves an entry, and finishes through the editor.
A cleared panel empties the editor, and an edited row opens in it.
`scope` names the tab and prefixes the load key, and `list` names the vista and the empty keys.
`allowed` answers whether the tab is shown, which only the owner's pack check knows.
`seam` finds the rows under the owner's chosen cell, so the list never reads a column.
It registers the draft check and its close with the workspace, and the owner registers the vista restore.
It starts no vista, since the owner restores the columns and the list together.

## `public event Action? CEntryListChanged;`

Raised whenever a column or an entry notice changed the list and the owner must tell its driver.
The owner forwards it as its own change.

## `public CEditor CEntryListEditor { get; }`

The entry editor the reader column opens the chosen entry in.
The driver wraps it for the editor view and the lectern.

## `public CPanel CEntryListPanel { get; }`

The panel state of the entry list, with its mode, its chosen row and its draft.
Its aperture holds the list's vista, its count and its empty keys.

## `public bool CEntryListEditing`

True while the reader shows the editor.

## `internal void LEntryListRestore()`

Starts the list's vista off the window posture, ordered by headword until the user picks another order.
The aperture carries the search text of the vista before into the fresh one, so a workspace switch keeps it.
The vista then goes to the editor.
The owner runs it inside its own restore, after its columns started.

## `internal void LEntryListAttach()`

Attaches the list's answers to its fresh vista, each carried through the marshal.
The list's own Vista notice raises its rows, and an Entry notice raises the change and goes to the panel.
A notice on the chosen entry reads its draft again.
The owner runs it after its column observers, so the attach order stays the one before the split.

## `public IReadOnlyList<CVistaRow> CEntryListRead()`

The entry list under the owner's chosen cell, copied through the shared row map and counted for the empty verdict.
A failed read shows the scope's load key through the envoy and answers no rows, so the count reads zero.
The load task runs this read after the flag fill, so a throw here would fault the driver.

## `public Task<CEnsignSheet<IReadOnlyList<CVistaRow>>> CEntryListLoad(Func<IReadOnlyList<CEnsignRow>, Action<string, Exception>, Action> store)`

Runs the flag fill into the driver's `store`, then answers `CEntryListRead` beside the loaded languages.
The shared rule `CCatalog.LCatalogEnsignLoad` orders the two, so the driver makes one request.
A failed flag fill shows the scope's load key and still answers the rows with no languages.
The driver awaits it from an event handler, where a fault would end the app.

## `internal void LEntryListResonate()`

Answers a column notice by raising the change and refreshing the panel's own rows.
The owner's column observers run it through the marshal.

## `internal string LEntryListResolve()`

The file name an export of the read entry is offered under.
`CPortrait` reads it only once the export starts, and a failed read shows `Export.NameFailed`.

## `public Task CEntryListPrint()`

Prints the entry the reader holds, doing nothing while there is none.
The reader is asked for the printer through the list's envoy, and a decline prints nothing.
`CPortrait` words the page through the engine and shows `Print.Failed` through the list's envoy.

## `public Task CEntryListExport()`

Exports the entry the reader holds as a portrait file.
The reader is asked for the file and format through the list's envoy, and a decline exports nothing.
`CPortrait` words the page through the engine and shows `Export.Failed` through the list's envoy.
