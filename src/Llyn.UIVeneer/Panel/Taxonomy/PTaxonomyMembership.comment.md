# PTaxonomyMembership.cs

## `public partial class PTaxonomy`

The entry list of the taxonomy panel: the entries carrying the chosen tag, and the one the reader stands on.
Choosing a row loads it back from the workspace into the reader, or the editor when that side is open.

## `private void PMembershipHandle(object sender, RoutedEventArgs e)`

A clicked row is shown once the leave check has settled any unsaved draft.

## Inline notes

### `private void PMembershipFind()`

The entry list is never refilled on its own.
It is refilled whenever the catalog is, because the chosen tag may have just changed or vanished.
Rows sharing a headword are numbered afterwards, so the reader can tell them apart.
