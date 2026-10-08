# QMeaning.cs
Hash: `d6f0fefdea6d93fc`

## `internal sealed class QMeaning`

The editor's driver for the meaning list and the button that adds a meaning card.
The cards themselves answer to `QCard`, which this list hands its fill to.

## `internal QMeaning(FrameworkElement surface, ObservableCollection<PCard> cards, QCard card)`

Hands the meaning list its cards and the card fill, and hears the add button.

## `internal void QMeaningIntroduce(CCardList list)`

Holds the Conduct card list facet whose add gate the button reaches.

## Inline notes

### `private void QMeaningAddObserve(object sender, RoutedEventArgs e)`

Asks the gate for a new meaning card.
The card arrives through the bulletin, already named and placed, so the form never guesses an id or a place.
