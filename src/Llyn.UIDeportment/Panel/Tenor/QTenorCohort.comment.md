# QTenorCohort.cs

## `internal sealed partial class QTenor`

The entry list of the tenor panel: the Entries carrying the chosen Register, and the one the reader stands on.
Choosing a row loads it back from the workspace into the reader, or the editor when that side is open.

## `private void QCohortObserve(object sender, RoutedEventArgs e)`

A clicked row hands its entry's id raw to the row gate, or null when it carries no item.
The gate ignores null, asks the leave question and opens the row.
This panel attaches no station, so the gate records none.

## Inline notes

### `private void QCohortRefine()`

Answers the panel's rows event, which the area raises after a successful Register read.
The Entries are never refilled on their own, because the chosen Register may have just changed or vanished.
A Register carrying no id stands for the whole workspace, which is what the engine reads an empty id as.
A failed read has already been shown by the area, which then answers no rows.
Conduct picks the empty line's key from whether the entry search holds text.

## `private void QCohortItemRefine(FrameworkElement container, object item, string? _)`

Fills one entry row from its item, the work its bindings did before.
The row carries the `Chosen` cue on the chosen item and none otherwise, which the look sheet paints.
The click is subscribed once per row, removed first so a refill never doubles it.
It runs again on every change the item raises, so a chosen row moves without a refill.
The epithet leads with an en space, as its string format did.
