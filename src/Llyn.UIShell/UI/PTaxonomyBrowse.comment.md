# PTaxonomyBrowse.cs

## `public partial class PTaxonomy`

Browsing behavior of the taxonomy panel.
The search field and the sorting dropdown refill the tag catalog.
A chosen tag refills the entries beside it, and a chosen entry is loaded back from the workspace.
It is rendered read-only in the reader, which the mode toggle swaps for the editor.
This is the same read half of the entry round trip the library panel offers.
It is reached through a tag instead of a headword.

## Inline notes

### `private string? _pDirectoryChoice;`

The tag the membership list stands on, or null when none is chosen.
Null is not an absence to be corrected.
It is the whole workspace, which is what the panel shows before anything is picked.

### `private string? _pDisplayEntry;`

The entry the right-hand side stands on, or null when none is selected.
The reader may show it and the editor may be correcting it.

### `private LCatalogOrder _pFunnelChoice;`

Which ordering the tag catalog is listed in, held as one of the orderings the engine supports.
It starts as whatever the workspace stored, which the window applies before the panel is first shown.
The dropdown tag is read into that set, so the panel offers nothing the engine cannot do.
The ordering is the same one whatever language names it.

### `private async void PTaxonomyBulletinHandle(LBulletin bulletin)`

The panel answers the engine rather than its own visibility.
So a tag written in the input panel is in the catalog at once.
No tab switch is needed to trigger it.
A workspace that moved is the one announcement that empties the panel first.
Its flags are reloaded before any row is built.

### `private void PDirectoryFind(string query)`

The engine returns the tags answering the query, already in the chosen ordering.
The entries under a tag stay in headword order, which the database already gives them.
The chosen tag is re-marked as the catalog is rebuilt, so the selection survives a re-sort.
A chosen tag that the workspace no longer holds is dropped, and the panel falls back to every entry.

### `internal void PDirectoryTagShow(long id)`

Browses by one tag for a caller outside the panel.
That is how a tag chip read on a card reaches this panel.
The query is emptied first.
A tag left out by the standing query would be chosen and dropped in the same breath.

### `private void PMembershipSelect(string? id)`

Marks the entry row the reader stands on and clears the mark from every other row.
A null id leaves no row marked, which is what a cleared panel shows.
It is called wherever the shown entry changes, so the mark and the reader never disagree.

### `PMembershipFind();`

The entry list is never refilled on its own.
It is refilled whenever the catalog is, because the chosen tag may have just changed or vanished.
Rows sharing a headword are numbered afterwards, so the reader can tell them apart.

### `_pDirectoryChoice = item.PDirectoryItemChosen ? 0 : item.PDirectoryItemId;`

Clicking the chosen tag lets go of it.
That is how the panel is put back on the whole workspace without a separate control saying so.

### `private void PMembershipEntryUpdate(string id)`

A store is answered by re-reading the catalog rather than the entry list alone.
Storing may have written a tag no other card carries, or taken away the last card that carried one.

### `internal void PFunnelRestore(LCatalogOrder order)`

Puts the panel back on the ordering the workspace stored, and moves the dropdown mark onto it.
The window calls it once on attach, so the panel never reads the stored state for itself.

### `internal void PTaxonomyScribeRestore(bool editing)`

Puts the panel back on the side it was left standing on.
The button is enabled first when the editor is the side restored.
An empty editor is the state a new record is written in.

A session that ended on the editor with nothing selected comes back on the reading side instead.
Otherwise the launch would open a blank draft nobody asked for.
