# PCardList.cs

## `public partial class PEditor`

Which cards the editor holds.
That is the sense list and the collocation list.
It is also the buttons that add and remove a card.
The order the cards are in is the order they are saved in.
The numbering that keeps each list reading 1, 2, 3 is the engine's now.
The form asks for it and takes back what it is given.

## Inline notes

### `PLinkAttach(card);`

A card added by hand is attached exactly as a loaded one is.
Otherwise its Translation field would take typing and resolve none of it.

### `PEditorChangeSave();`

Adding a card is a whole action, so it is written at once.
Nothing further is coming to end a wait.

### `private void PCardOrderApply(ObservableCollection<PCard> list, int from, int target)`

Hands one reorder to the engine and takes back the numbering it computed.
The form has already written what it holds, so the engine reorders the same cards it shows.
Removing a card asks for the same call with no move in it.
The gap it left still has to close.
A refused call leaves the list as the user sees it and the draft as it was.

### `internal void PCardHandle(object sender, RoutedEventArgs e)`

Removing the last card would leave the tab with nothing to type into, so a list of one keeps it.

### `private ObservableCollection<PCard>? PCardListFind(PCard card)`

Which of the two lists a card belongs to, since both are drawn from the same template.
