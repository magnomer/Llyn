# QCollocation.cs
Hash: `3f18052e6475fa98`

## `internal sealed class QCollocation`

The editor's driver for the collocation list and the button that adds a collocation card.
The cards themselves answer to `QCard`, which this list hands its fill to.

## `internal QCollocation(FrameworkElement surface, ObservableCollection<PCard> cards, QCard card)`

Hands the collocation list its cards and the card fill, and hears the add button.

## `internal void QCollocationIntroduce(CCardList list)`

Holds the Conduct card list facet whose add gate the button reaches.

## Inline notes

### `private void QCollocationAddObserve(object sender, RoutedEventArgs e)`

Asks the gate for a new collocation card.
The card arrives through the bulletin, already named and placed, so the form never guesses an id or a place.
