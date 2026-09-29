# PLedger.cs

## `private readonly List<PLedgerItem> _pLedgerList = [];`

One row item per settings page, kept across states so a row keeps its chosen mark.

## `private void PLedgerRefine(CLedgerState state)`

Paints the catalog from one ledger state: every page's row, the narrowing the state carries, and the Layout summary.
The narrowing rides on the state, so a settings change keeps a narrowed catalog narrowed.

## `private void PLedgerPageRefine(CLedgerPage page)`

Writes one page's title and summary onto the row paired with it by the page's name.
A page with no row yet gets one, appended in the ledger's page order.

## `private void PLedgerMetaRefine()`

Writes the Layout row as `CLedgerMetaRead` words it for the linked flag the posture holds.
The linked switch calls it too, since that flag is GUI-only state no ledger state carries.

## `private void PLedgerFindRefine(CLedgerShown shown)`

Lists the rows of the pages `shown` names, and shows the empty text when it says none is shown.
Hidden rows leave the items source rather than collapsing, so the frame keeps its sibling spacing.
The chosen row may leave the catalog while its card stays, since the card is the Dial's to swap.

## `private void PLedgerRowRefine(object sender, RoutedEventArgs e)`

A click on a row shows the card of that row's group.
Marking the row chosen is left to the card showing, so the first card marks its row the same way.

## `private void PLedgerItemRefine(FrameworkElement container, object item, string? _)`

Fills one ledger row from its item, the work its bindings did before.
The row carries the `Chosen` cue on the chosen item and none otherwise, which the look sheet paints.
The click is subscribed once per row, removed first so a refill never doubles it.
It runs again on every change the item raises, so a chosen row moves without a refill.
A new title or summary on the item refills the row the same way.

## `private void PWinnowObserve(object sender, TextChangedEventArgs e)`

The search field's text changed, so `CLedgerFind` hears the raw text and answers the pages it shows.
Conduct keeps the text and matches it, so a console searches the same way.
