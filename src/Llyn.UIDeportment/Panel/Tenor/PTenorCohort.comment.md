# PTenorCohort.cs

## `public partial class PTenor`

The entry list of the tenor panel: the Entries carrying the chosen Register, and the one the reader stands on.
Choosing a row loads it back from the workspace into the reader, or the editor when that side is open.

## `private void PCohortHandle(object sender, RoutedEventArgs e)`

A clicked row is shown once the leave check has settled any unsaved draft.

## Inline notes

### `private void PCohortFind()`

The Entries are never refilled on their own.
They are refilled whenever the catalog is, because the chosen Register may have just changed or vanished.
A Register carrying no id stands for the whole workspace, which is what the engine reads an empty id as.
Rows sharing a headword are numbered afterwards, so the reader can tell them apart.

## `private void PCohortApply(FrameworkElement container, object item, string? _)`

Fills one entry row from its item, the work its bindings did before.
The row's tag reads Chosen on the chosen item and is cleared otherwise, which the look sheet paints.
The click is subscribed once per row, removed first so a refill never doubles it.
It runs again on every change the item raises, so a chosen row moves without a refill.
The epithet leads with an en space, as its string format did.
