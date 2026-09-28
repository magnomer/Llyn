# CFootnote.cs

## `public sealed class CFootnote`

The sources panel's entry list: the entries citing the chosen Source, or every entry while none is chosen.
It holds the entry and portrait ports, the source vista as its parent and its own vista.
The engine narrows the rows by the parent's choice, so the list decides nothing about matching.
Its panel has no delete scope, because an entry is never deleted from this list.

## `internal CFootnote(`

Builds the list's panel under the `List.LoadFailed` key over the shelf's entry editor.
The panel asks the editor's desk before it leaves an entry, and finishes through the shelf's seam.
A cleared panel empties the editor, and an edited row opens in it.

## `public CPanel CFootnotePanel { get; }`

The panel that holds the chosen entry and the edit mode of the entry side.

## `public string CFootnoteEmptyKey`

The wording key of the empty list, chosen by whether the vista holds a query.
No query means nothing cites the Source, and a query means nothing matched.

## `internal void CFootnoteVistaRestore(LVista parent, LVista vista)`

Takes the source vista as the parent the rows follow, and its own vista for the panel.

## `public void CFootnoteQuerySet(string query)`

Takes the text typed into the rummage field as the list's query.

## `public IReadOnlyList<CVistaRow> CFootnoteRowsRead()`

Reads the entries citing the parent's chosen Source, mapped as the quotation list maps its own.

## `public void CFootnoteEntryCreate()`

Opens a fresh entry without asking, for a driver that already asked for the whole tab.
The chosen Source is cited after the editor is open, so the citation lands in the new draft.

## `public Task CFootnotePortraitPrint(CPortraitLabel label, CPressTicket ticket)`

Prints the chosen entry, mapping the label and ticket to the engine's records unread.
