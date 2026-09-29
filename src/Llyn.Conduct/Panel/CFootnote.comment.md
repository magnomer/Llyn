# CFootnote.cs

## `public sealed class CFootnote`

The sources panel's entry list: the entries citing the chosen Source, or every entry while none is chosen.
It holds the entry and portrait ports, the source vista as its parent and its own vista.
The engine narrows the rows by the parent's choice, so the list decides nothing about matching.
Its panel has no delete scope, because an entry is never deleted from this list.

## `internal CFootnote(`

Builds the list's panel under the `List.LoadFailed` key over the shelf's entry editor.
The panel asks the editor's desk before it leaves an entry, and finishes through the shelf's seam.
A cleared panel drops the editor's draft, and an edited row opens in it.
A fresh entry is started by `LFootnoteEntryCreate` alone, so no blank draft paints before the cited one.

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

## `internal string LFootnoteFileRead()`

The file name an export of the chosen entry offers, read from the list's own vista.

## `internal void LFootnoteEntryCreate()`

Opens a fresh entry without asking, for the shelf that already asked for the whole tab.
The fresh entry starts already citing the chosen Source, in one ShellEngine call.
So the first draft shown carries the citation, as the corpus and repertoire twins do.

## `internal Task LFootnotePortraitPrint(CEnvoy envoy, LSettingsPort settings)`

Prints the chosen entry through `CPortrait`, worded through `settings`, with failures shown through `envoy`.

## `internal Task LFootnotePortraitExport(CEnvoy envoy, LSettingsPort settings)`

Exports the chosen entry to the path through `CPortrait`, worded through `settings`.
