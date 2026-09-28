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

## `public void CFootnoteQuerySet(string query)`

Takes the text typed into the rummage field as the list's query.

## `public IReadOnlyList<CVistaRow> CFootnoteRowsRead()`

Reads the entries citing the parent's chosen Source, mapped as the quotation list maps its own.

## `public string CFootnoteFileRead()`

The file name an export of the chosen entry offers, read from the list's own vista.

## `internal void LFootnoteEntryCreate()`

Opens a fresh entry without asking, for the shelf that already asked for the whole tab.
The fresh entry starts already citing the chosen Source, in one ShellEngine call.
So the first draft shown carries the citation, as the corpus and repertoire twins do.

## `internal Task LFootnotePortraitPrint(CPortraitLabel label, CPressTicket ticket)`

Prints the chosen entry, mapping the label and ticket to the engine's records unread.

## `internal Task LFootnotePortraitExport(string path, CPortraitMedium format, CPortraitLabel label)`

Exports the chosen entry to the path, mapping the medium and label to the engine's records unread.
