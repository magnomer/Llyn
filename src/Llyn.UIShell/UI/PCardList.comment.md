# PCardList.cs

## `public partial class PEditor`

Which cards the editor holds.
That is the sense list and the collocation list.
It is also the buttons that add and remove a card.
It is also the numbering that keeps each list reading 1, 2, 3 after every change.
The order the cards are in is the order they are saved in.
So it is kept here rather than derived.

## Inline notes

### `PLinkAttach(card);`

A card added by hand is attached exactly as a loaded one is.
Otherwise its Translation field would take typing and resolve none of it.

### `internal void PCardHandle(object sender, RoutedEventArgs e)`

Removing the last card would leave the tab with nothing to type into, so a list of one keeps it.

### `private ObservableCollection<PCard>? PCardListFind(PCard card)`

Which of the two lists a card belongs to, since both are drawn from the same template.
