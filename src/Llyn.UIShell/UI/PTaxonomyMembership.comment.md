# PTaxonomyMembership.cs

## `public partial class PTaxonomy`

The entry list of the taxonomy panel: the entries carrying the chosen tag, and the one the reader stands on.
Choosing a row loads it back from the workspace into the reader, or the editor when that side is open.

## `private void PMembershipHandle(object sender, RoutedEventArgs e)`

A clicked row is shown once the leave check has settled any unsaved draft.

## `private void PMembershipEntryShow(long id)`

Loads the chosen entry and hands it to the reader, and to the editor when that side is open.
An entry the workspace no longer holds clears the panel and rebuilds the catalog without it.

## Inline notes

### `private void PMembershipSelect(long? id)`

Marks the entry row the reader stands on and clears the mark from every other row.
A null id leaves no row marked, which is what a cleared panel shows.
It is called wherever the shown entry changes, so the mark and the reader never disagree.

### `private void PMembershipFind()`

The entry list is never refilled on its own.
It is refilled whenever the catalog is, because the chosen tag may have just changed or vanished.
Rows sharing a headword are numbered afterwards, so the reader can tell them apart.

### `private void PMembershipEntryUpdate(long id)`

A store is answered by re-reading the catalog rather than the entry list alone.
Storing may have written a tag no other card carries, or taken away the last card that carried one.

### `private void PMembershipEntryCreate()`

Starts a fresh entry in the editor, carrying the chosen tag on its first sense.
The clear leaves the tag choice standing, so the list still shows that tag's entries beside the fresh one.
An entry started with no tag chosen comes up blank, since every entry is listed then.
