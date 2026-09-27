# PLedger.cs

## `private void PLedgerShow(IReadOnlyList<CLedgerPage> pages)`

Builds the catalog rows from `pages` the first time, and afterwards writes each row's title and summary in place.
The order is Conduct's page order, which the Dial table follows for its cards.
The empty text is hidden here and only shown once a search leaves nothing.
Rows are written in place rather than rebuilt, so a narrowed catalog keeps its rows.

## `private void PLedgerMetaApply()`

Writes the Layout row's summary, as `CLedgerMetaRead` words it for the linked flag the posture holds.
The linked switch calls it too, since that flag is GUI-only state no ledger state carries.

## `private void PLedgerFind(string text)`

Narrows the catalog to the rows of the pages `CLedgerFind` names for the search text.
Conduct matches the labels and ignores case, so a console searches the same way.
Hidden rows leave the items source rather than collapsing, so the frame keeps its sibling spacing.
The chosen row may leave the catalog while its card stays, since the card is the Dial's to swap.

## `private void PLedgerHandle(object sender, RoutedEventArgs e)`

A click on a row shows the card of that row's group.
Marking the row chosen is left to the card showing, so the first card marks its row the same way.

## `private void PLedgerApply(FrameworkElement container, object item, string? _)`

Fills one ledger row from its item, the work its bindings did before.
The row carries the `Chosen` cue on the chosen item and none otherwise, which the look sheet paints.
The click is subscribed once per row, removed first so a refill never doubles it.
It runs again on every change the item raises, so a chosen row moves without a refill.
A new title or summary on the item refills the row the same way.

## `private void PWinnowHandle(object sender, TextChangedEventArgs e)`

The search field's text changed, so the catalog is narrowed to it.
A null text is read as empty, which shows every row.
