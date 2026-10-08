# CFootnote.cs
Hash: `b801457c942195a6`

## `public sealed class CFootnote`

The sources panel's entry list: the entries citing the chosen Source, or every entry while none is chosen.
It holds the vista and portrait ports, the source vista as its parent and its own vista.
The engine narrows the rows by the parent's choice, so the list decides nothing about matching.
Its panel has no delete scope, because an entry is never deleted from this list.

## `internal CFootnote(LVistaPort vistas, LPortraitPort portraits, LSettingsPort settings, CEditor editor, CEnvoy envoy, Func<bool, bool> finishSeam, Func<bool> shownSeam)`

Builds the list's panel under the `List.LoadFailed` key over the shelf's entry editor.
The panel asks the editor's desk before it leaves an entry, and finishes through the shelf's seam.
A cleared panel drops the editor's draft, and an edited row opens in it.
A fresh entry is started by `LFootnoteEntryCreate` alone, so no blank draft paints before the cited one.
It keeps `envoy` too, so a failed flag fill in `CFootnoteRowsLoad` can be shown.

## `public CPanel CFootnotePanel { get; }`

The panel that holds the chosen entry and the edit mode of the entry side.

## `public string CFootnoteEmptyKey`

The wording key of the empty list, chosen by whether the vista holds a query.
No query means nothing cites the Source, and a query means nothing matched.

## `internal void LFootnoteVistaRestore(LVista parent, LVista vista)`

Takes the source vista as the parent the rows follow, and its own vista for the panel.
The former vista's query is carried into the fresh one before the panel takes it.

## `internal void LFootnoteObserverAttach(Action<Action> marshal, Action roll, Action chosen)`

Attaches the entry list's subjects, each answered through `marshal`.
An Entry notice selects a stored entry, refills the entry rows, and hands `roll` the Source rows to refill.
The chosen entry's notice is handed to `chosen`, and a Vista notice refills the entry rows.

## `public void CFootnoteQuerySet(string query)`

Takes the text typed into the rummage field as the list's query.

## `public IReadOnlyList<CVistaRow> CFootnoteRowsRead()`

Reads the entries citing the parent's chosen Source, mapped as the quotation list maps its own.
A failed read shows `List.LoadFailed` through the envoy and answers no rows.
The load task runs this read after the flag fill, so a throw here would fault the driver.

## `public Task<CEnsignSheet<IReadOnlyList<CVistaRow>>> CFootnoteRowsLoad(Func<IReadOnlyList<CEnsignRow>, Action<string, Exception>, Action> store)`

Runs the flag fill into the driver's `store`, then answers `CFootnoteRowsRead` beside the loaded languages.
The shared rule `CCatalog.LCatalogEnsignLoad` orders the two, so the driver makes one request.
A failed flag fill shows `List.LoadFailed` and still answers the rows with no languages.
The driver awaits it from an event handler, where a fault would end the app.

## `internal string LFootnoteFileRead()`

The file name an export of the chosen entry offers, read from the list's own vista.
`CPortrait` reads it only once the export starts, and a failed read shows `Export.NameFailed`.

## `internal void LFootnoteEntryCreate()`

Opens a fresh entry without asking, for the shelf that already asked for the whole tab.
The fresh entry starts already citing the chosen Source, in one ShellEngine call.
So the first draft shown carries the citation, as the corpus and repertoire twins do.

## `internal Task LFootnotePortraitPrint(CEnvoy envoy, LSettingsPort settings)`

Prints the chosen entry through `CPortrait`, worded through `settings`, with failures shown through `envoy`.

## `internal Task LFootnotePortraitExport(CEnvoy envoy, LSettingsPort settings)`

Exports the chosen entry to a file through `CPortrait`, worded through `settings`.
