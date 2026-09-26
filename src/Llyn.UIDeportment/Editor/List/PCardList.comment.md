# PCardList.cs

## `public partial class PEditor`

Which cards the editor holds.
That is the meaning list and the collocation list.
It is also the buttons that add and remove a card.
Adding, removing and reordering are requests to the engine, never edits to the lists.
The engine answers with a draft bulletin, and the render brings the lists into line with it.
The numbering that keeps each list reading 1, 2, 3 is the engine's.
Each card carries the id the engine minted for it.
That is how a request names one card rather than a place in a list.

## `private readonly PMeaningTemplate _pMeaningTemplate`

The meaning card dictionary, held so its fill can subscribe the card's forwarders.

## `private void PMeaningApply(FrameworkElement container, object item, string? changed)`

Fills one meaning card and subscribes its drag, badge, removal and media forwarders.
The changed property is passed on, so the card rewrites only that property's text.

## `private void PCollocationApply(FrameworkElement container, object item, string? changed)`

Fills one collocation card as the meaning fill does, through the collocation dictionary's forwarders.
The two stay apart because each dictionary holds its own forwarders.

## `private void PCardApply(FrameworkElement container, PCard card)`

Hands every list inside a card its items and attaches each list's fill.
Setting the same items again changes nothing, so a refill does not rebuild the lists.

## `private void PCardListAttach()`

Hands both card lists their cards and fill, and subscribes the drag and add handlers the markup named.

## Inline notes

### `private void PMeaningHandle(object sender, RoutedEventArgs e)`

Asks for a new card at the end of the list.
The card arrives through the bulletin, already named, so the form never guesses an id.

### `internal void PCardHandle(object sender, RoutedEventArgs e)`

Removing the last card would leave the tab with nothing to type into, so a list of one keeps it.
The engine closes the gap in the numbering when it drops the card.

### `internal void PCardPositionHandle(object sender, MouseButtonEventArgs e)`

Opens the badge for writing on a double click, and stops that click from starting a drag.
A single card cannot be reordered, so its badge never opens.
The box is focused after the layout runs, because it is not hit tested until then.

### `internal void PCardPositionAccept(object sender, KeyEventArgs e)`

Enter takes the typed number and Escape drops it.

### `internal void PCardPositionCommit(object sender, RoutedEventArgs e)`

Leaving the badge takes the typed number, as leaving any field here takes what it holds.
The open badge is checked, so a badge closed by Escape is not read again.

### `private void PCardPositionApply(PCard card)`

Moves the card to the place the typed number names, counting from one.
A number above the count lands the card last and a number below one lands it first.
That is the answer a writer means by "9 of 3", rather than a refusal.
Anything unknown as a number changes no order at all.
The move is then handed to the engine exactly as a drag is.

### `private void PCardMove(PCard card, int target)`

One shift request, naming the card and the place it should land in its own list.
The list is not moved here.
The bulletin's render moves it, so a refused shift leaves the shown order as stored.

### `private ObservableCollection<PCard>? PCardListFind(PCard card)`

Which of the two lists a card belongs to, since both are drawn from the same template.
