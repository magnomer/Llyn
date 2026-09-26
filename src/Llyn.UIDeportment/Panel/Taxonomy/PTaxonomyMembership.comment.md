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

## `private void PMembershipApply(FrameworkElement container, object item, string? _)`

Fills one entry row from its item, the work its bindings did before.
The row's tag reads Chosen on the chosen item and is cleared otherwise, which the look sheet paints.
The click is subscribed once per row, removed first so a refill never doubles it.
It runs again on every change the item raises, so a chosen row moves without a refill.
The entry list is a catalog like the library's, so it is marked the same way.
The epithet leads with an en space, as its string format did.
