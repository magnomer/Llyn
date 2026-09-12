# PSentenceMenu.cs

## `public partial class PEditor`

The editor's side of an Example row: opening and dropping rows, and the Source list a row cites from.
The rows themselves hold no engine.
So every change in a row arrives here and leaves as a request naming the card and the row.
The card that owns the row is found and the engine is asked.
One list of Sources serves every row on the form, Example and Situation alike.
The frame the rows read a sentence under is settled here too.
That is the order its two fields take and what each has been saved holding.
Both follow the language the entry is written in.
Switching language redraws every row rather than leaving one language's order over another's.

## `internal void PSentenceLoad()`

Reads every Source the workspace holds into the list the form offers.
It then has every row read the name of the Source it cites again.
A workspace that cannot be read leaves the list empty rather than failing the form.

## `internal void PSentenceFrameLoad(string language)`

Settles the Example frame for `language`: which of its two fields is written first, and what each offers.
The order comes from the language pack and is handed to every card on the form.
A row never keeps the order of the language it was drawn in.
The two lists come from what the workspace has already saved for that language.
Nothing ships a marker or a role.
A workspace that cannot be read leaves a list empty rather than failing the form.
An unnamed language is the one the speaker field currently shows.

## `internal void PSentenceAttach(PCard card)`

Points the card's row changes at this editor, so each becomes a request.

## `internal void PSentenceAddHandle(object sender, RoutedEventArgs e)`

Asks for a further Example row directly under the row the user asked from.

## `internal void PSentenceRemoveHandle(object sender, RoutedEventArgs e)`

Asks the engine to drop the Example row the user asked from.
A card left with no row is given a blank one again by the prepare that follows every redraw.

## `internal void PSentenceCitationClear(object sender, RoutedEventArgs e)`

Stops the row citing any Source, which the row reports and the change handler sends.

## Inline notes

### `private void PSentenceChangeHandle(PCard card, PSentence row, string field)`

Turns one row change into its request.
The sentence, the marker and the role are deferred with the debounce, one pending request per row and field.
The Source is sent at once, because it is picked rather than typed.

### `private bool PSentencePendingCheck(PCard card, PSentence row, string field)`

Whether a request for this row's field is still waiting, in which case a redraw must not overwrite it.

### `private void PSentencePrepare()`

Asks for a blank row on every card that shows none, after each redraw.
A card with no row to write in offers nothing.
The engine keeps the blank row rather than the card.
The cards are collected first and checked again before each ask, because each answer redraws and prepares in turn.

### `private static void PSentenceFrameShow(ObservableCollection<string> catalog, IReadOnlyList<string> values)`

The list is refilled in place rather than replaced, because every row already holds it.
