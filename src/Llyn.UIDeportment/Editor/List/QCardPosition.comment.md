# QCardPosition.cs
Hash: `4d1fe50beea3d0c3`

## `internal sealed class QCardPosition`

The editor's driver for a card's position badge, which reorders a card by its typed number.
It holds both card lists, since a list of one offers no badge to open.
The move itself goes to the card gate, and the engine owns the numbering.

## `internal QCardPosition(ObservableCollection<PCard> meanings, ObservableCollection<PCard> collocations)`

Holds the meaning and collocation lists the card driver draws.

## `internal void QCardPositionIntroduce(CCardList list)`

Holds the Conduct card list facet whose move gate a typed number reaches.

## `internal void QCardPositionApply(FrameworkElement container)`

Subscribes one card's badge and its number box to the open, escape and commit handlers.
Each handler is removed before it is added, so a refill never subscribes twice.

## Inline notes

### `private void QCardPositionRefine(object sender, MouseButtonEventArgs e)`

Opens the badge for writing on a double click, and stops that click from starting a drag.
A list of one shows no editable badge, as it shows no drag ghost.
That only decides whether the editor appears, so it stays here.
The box is focused after the layout runs, because it is not hit tested until then.

### `private void QCardPositionRefine(object sender, KeyEventArgs e)`

Escape drops the typed number and shuts the badge.

### `private void QCardPositionObserve(object sender, RoutedEventArgs e)`

Enter or leaving the badge hands the box's own typed text to the move gate, then shuts the badge.
The typed text stays in the box as pure medium, so the card's number has one writer.
One handler hears both, since both commit the same text.
The open badge is checked, so a badge shut by Enter or Escape is not read again.

### `private ObservableCollection<PCard>? QCardPositionFind(PCard card)`

Which of the two lists a card belongs to, since both are drawn from the same template.
