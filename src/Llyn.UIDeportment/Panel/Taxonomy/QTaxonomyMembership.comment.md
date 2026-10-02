# QTaxonomyMembership.cs
Hash: `0c249cfbb6d2e493`

## `internal sealed partial class QTaxonomy`

The entry list of the taxonomy panel.
It shows the entries carrying the chosen tag, and the one the reader stands on.
Choosing a row loads it back from the workspace into the reader, or the editor when that side is open.

## `private void QMembershipObserve(object sender, RoutedEventArgs e)`

A clicked row hands its entry's id raw to the row gate, or null when it carries no item.
The gate ignores null, asks the leave question and opens the row.
This panel attaches no station, so the gate records none.

## Inline notes

### `private void QMembershipRefine()`

Answers the entry list's rows event, which every successful Tag read also raises.
So the list follows the catalog, because the chosen Tag may have just changed or vanished.
The first paint arrives the same way, from the catalog's read after the flags are loaded.
Rows sharing a headword are numbered afterwards, so the reader can tell them apart.
Conduct picks the empty line's key from whether the entry search holds text.
A failed read has already been shown by the area, which then answers no rows.

## `private void QMembershipItemRefine(FrameworkElement container, object item, string? _)`

Fills one entry row from its item, the work its bindings did before.
The row carries the `Chosen` cue on the chosen item and none otherwise, which the look sheet paints.
The click is subscribed once per row, removed first so a refill never doubles it.
It runs again on every change the item raises, so a chosen row moves without a refill.
The entry list is a catalog like the library's, so it is marked the same way.
The epithet leads with an en space, as its string format did.
