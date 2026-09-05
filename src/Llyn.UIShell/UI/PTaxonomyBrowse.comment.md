# PTaxonomyBrowse.cs

## `public partial class PTaxonomy`

Browsing behavior of the taxonomy panel.
The search field and the sorting dropdown refill the tag catalog.
A chosen tag refills the entries beside it, and a chosen entry is loaded back from the workspace.
It is rendered read-only in the reader, which the mode toggle swaps for the editor.
This is the same read half of the entry round trip the library panel offers, reached through a tag instead of a headword.

## Inline notes

### `private string? _pDirectoryChoice;`

The tag the membership list stands on, or null when none is chosen.
Null is not an absence to be corrected.
It is the whole workspace, which is what the panel shows before anything is picked.

### `private string? _pDisplayEntry;`

The entry the right-hand side stands on, or null when none is selected.
The reader may show it and the editor may be correcting it.

### `private string _pFunnelChoice = "Name";`

Which ordering the tag catalog is listed in.
It is the tag the chosen dropdown row carries, not the words that row showed.
The ordering is the same one whatever language names it.

### `private void PTaxonomyHandle(object sender, DependencyPropertyChangedEventArgs e)`

The panel opens with whatever the database already holds.
So a tag written in the input panel is there the moment the tab is switched to.
Rebinding is idempotent, which keeps the binding out of the window constructor and beside the code that owns it.

### `private IEnumerable<LTag> PFunnelSort(IReadOnlyList<LTag> tags)`

The sorting acts on the tag catalog alone.
The entries under a tag stay in headword order, which the database already gives them.
A tag is read as a word, so the comparison is the reader's culture and not an ordinal one.

### `private void PDirectoryFind(string query)`

The search filters the catalog by a contains over the tag text.
The chosen tag is re-marked as the catalog is rebuilt, so the selection survives a re-sort.
A chosen tag that the workspace no longer holds is dropped, and the panel falls back to every entry.

### `PMembershipFind();`

The entry list is never refilled on its own.
It is refilled whenever the catalog is, because the chosen tag may have just changed or vanished.

### `_pDirectoryChoice = item.PDirectoryItemChosen ? null : item.PDirectoryItemText;`

Clicking the chosen tag lets go of it.
That is how the panel is put back on the whole workspace without a separate control saying so.

### `private void PMembershipEntryUpdate(string id)`

A store is answered by re-reading the catalog rather than the entry list alone.
Storing may have written a tag no other card carries, or taken away the last card that carried one.
