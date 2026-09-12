# PTenorBrowse.cs

## `public partial class PTenor`

Browsing behavior of the tenor panel.
The search field and the sorting dropdown refill the register catalog.
A chosen Register refills the Entries beside it, and a chosen Entry is loaded back from the workspace.
It is rendered read-only in the reader, which the mode toggle swaps for the editor.
This is the same read half of the entry round trip the library panel offers, reached through a Register.

## Inline notes

### `private string? _pGamutChoice;`

The Register the entry list stands on, held by its id rather than its name.
A name may be rewritten while the panel stands on it; an id never is.
Null is not an absence to be corrected: it is the whole workspace, which is what the panel shows first.

### `private LCatalogOrder _pDegreeChoice;`

Which ordering the register catalog is listed in, held as one of the orderings the engine supports.
It starts as whatever the workspace stored, which the window applies before the panel is first shown.

### `private async void PTenorBulletinHandle(LBulletin bulletin)`

The panel answers the engine rather than its own visibility.
So a Register written on a card in the input panel is in the catalog at once, with no tab switch.
A workspace that moved is the one announcement that empties the panel first.

### `private void PGamutFind(string query)`

The engine returns the Registers answering the query, already counted and in the chosen ordering.
The rows a language pack ships and the rows the user wrote arrive as one shelf.
The chosen Register is re-marked as the catalog is rebuilt, so the selection survives a re-sort.
A chosen Register the workspace no longer holds is dropped, and the panel falls back to every Entry.

### `internal void PGamutRegisterShow(long id)`

Browses by one Register for a caller outside the panel.
That is how a register chip read on a card reaches this panel.
The query is emptied first, or a Register left out by the standing query would be chosen and dropped at once.

### `private void PCohortFind()`

The Entries are never refilled on their own.
They are refilled whenever the catalog is, because the chosen Register may have just changed or vanished.
A Register carrying no id stands for the whole workspace, which is what the engine reads an empty id as.
Rows sharing a headword are numbered afterwards, so the reader can tell them apart.

### `_pGamutChoice = item.PGamutItemChosen ? null : item.PGamutItemId;`

Clicking the chosen Register lets go of it.
That is how the panel is put back on the whole workspace without a separate control saying so.

### `private void PCohortEntryUpdate(long id)`

A store is answered by re-reading the catalog rather than the entry list alone.
Storing may have marked a card with a Register nothing carried, or taken away the last card that carried one.

### `internal void PDegreeRestore(LCatalogOrder order)`

Puts the panel back on the ordering the workspace stored, and moves the dropdown mark onto it.
The window calls it once on attach, so the panel never reads the stored state for itself.

### `internal void PTenorScribeRestore(bool editing)`

Puts the panel back on the side it was left standing on.
A session that ended on the editor with nothing selected comes back on the reading side instead.
Otherwise the launch would open a blank draft nobody asked for.
