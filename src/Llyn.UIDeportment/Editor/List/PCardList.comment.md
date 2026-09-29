# PCardList.cs

## `public partial class PEditor`

Which cards the editor holds.
That is the meaning list and the collocation list.
It is also the buttons that add and remove a card, and the badge that moves one.
Adding, removing and reordering go to the card gates, never edits to the lists.
The engine answers with a draft bulletin, and the render brings the lists into line with it.
The numbering that keeps each list reading 1, 2, 3 is the engine's.
Each card carries the id the engine minted for it.
That is how a request names one card rather than a place in a list.

## `private void PMeaningApply(FrameworkElement container, object item, string? changed)`

Fills one meaning card and subscribes its drag, badge, removal and media handlers.
The changed property is passed on, so the card rewrites only that property's text.

## `private void PCollocationApply(FrameworkElement container, object item, string? changed)`

Fills one collocation card as the meaning fill does.
The media choosers still go through the collocation dictionary's forwarders, so the two fills stay apart.

## `private void PCardApply(FrameworkElement container, PCard card)`

Hands every list inside a card its items and attaches each list's fill.
Setting the same items again changes nothing, so a refill does not rebuild the lists.

## `private void PCardListAttach()`

Hands both card lists their cards and fill, and subscribes the drag and add handlers the markup named.

## Inline notes

### `private void PMeaningAddObserve(object sender, RoutedEventArgs e)`

Asks the gate for a new meaning card.
The card arrives through the bulletin, already named and placed, so the form never guesses an id or a place.

### `private void PCardRemoveObserve(object sender, RoutedEventArgs e)`

Hands the pressed card's id to the gate.
A list of one keeps its card, which the clerk decides, so the eraser sends every press.

### `private void PCardPositionRefine(object sender, MouseButtonEventArgs e)`

Opens the badge for writing on a double click, and stops that click from starting a drag.
A list of one shows no editable badge, as it shows no drag ghost.
That only decides whether the editor appears, so it stays here.
The box is focused after the layout runs, because it is not hit tested until then.

### `private void PCardPositionRefine(object sender, KeyEventArgs e)`

Escape drops the typed number and shuts the badge.

### `private void PCardPositionObserve(object sender, RoutedEventArgs e)`

Enter or leaving the badge hands the raw typed text to the move gate, then shuts the badge.
One handler hears both, since both commit the same text.
The open badge is checked, so a badge shut by Enter or Escape is not read again.

### `private ObservableCollection<PCard>? PCardListFind(PCard card)`

Which of the two lists a card belongs to, since both are drawn from the same template.
