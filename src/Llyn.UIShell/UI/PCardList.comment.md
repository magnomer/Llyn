# PCardList.cs

## `public partial class PEditor`

Which cards the editor holds.
That is the meaning list and the collocation list.
It is also the buttons that add and remove a card.
The order the cards are in is the order they are saved in.
The numbering that keeps each list reading 1, 2, 3 is the engine's now.
The form asks for it and takes back what it is given.
Each card carries the id the engine minted for it.
That is how an answer names one card rather than a place in a list.

## Inline notes

### `PEditorChangeSave();`

Adding a card is a whole action, so it is written at once.
Nothing further is coming to end a wait.

### `private PCard PCardCreate(string prefix, int position)`

Builds one card the form owns and asks the engine to name it.
A card added by hand is attached exactly as a loaded one is.
Otherwise its Translation field would take typing and resolve none of it.
The id is asked for here rather than at the first write.
So no answer can arrive naming a card the form cannot find.

### `internal void PCardHandle(object sender, RoutedEventArgs e)`

Removing the last card would leave the tab with nothing to type into, so a list of one keeps it.
The shorter list is written before the renumber, so the engine closes the gap in the cards it now holds.

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
Anything unreadable as a number changes no order at all.
The move is then handed to the engine exactly as a drag is.

### `private void PCardOrderApply(ObservableCollection<PCard> list, int from, int target)`

Hands one reorder to the engine and shows the order it answers with.
The form has already written what it holds, so the engine reorders the same cards it shows.

### `private void PCardOrderApply(ObservableCollection<PCard> list)`

Asks for the numbering alone, which is what a removal leaves behind.
A move of nothing said this by accident before, and a reorder that never happened is not what was meant.

### `private void PCardOrderShow(ObservableCollection<PCard> list, IReadOnlyList<LCardDraft> ordered)`

Puts the shown cards into the order the engine answered, matching each answer to a card by id.
A card is placed by the name it carries, not by where it sits in either list.
An answer naming a card the form does not show means the two disagree about what is held.
So does an answer of a different length.
Neither can be shown card by card, and going on would number the wrong cards in silence.

### `private void PCardOrderRestore(Exception? exception)`

Reports the failed reorder and fills the form back from the draft as stored.
A silent return left the shown order disagreeing with the stored one, with nothing to say so.
A refusal carries its reason, and a disagreement has none to carry.

### `private static PCard? PCardFind(ObservableCollection<PCard> list, List<PCard> shown, string id)`

The card one answer names, passing over the cards already placed.
Two cards cannot answer to one name, so a card is claimed once.

### `private ObservableCollection<PCard>? PCardListFind(PCard card)`

Which of the two lists a card belongs to, since both are drawn from the same template.
