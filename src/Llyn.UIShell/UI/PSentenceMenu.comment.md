# PSentenceMenu.cs

## `public partial class PEditor`

The editor's side of an Example row: opening and dropping rows, and the Source list a row cites from.
The rows themselves hold no engine.
So every change in a row arrives here and leaves as a request naming the card and the row.
The card that owns the row is found and the engine is asked.
One list of Sources serves every row on the form, Example and Situation alike.
The field a row cites through is answered in [PSentenceCitation.cs](PSentenceCitation.comment.md).
The linking gesture on a row's sentence is answered here too, four commands sent as Mention requests.
The frame the rows read a sentence under is settled here too.
That is the order its two fields take and what each has been saved holding.
Both follow the language the entry is written in.
Switching language redraws every row rather than leaving one language's order over another's.

## `internal void PSentenceLoad()`

Reads every Source the workspace holds, with its byline, into the list the form offers.
It then has every row read the byline of the Source it cites again.
It runs again whenever the engine reports a Source changed, so a byline edited elsewhere is never stale here.
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

## `internal void PSentenceLinkHandle(object sender, ExecutedRoutedEventArgs e)`

Opens the Entry picker over the selected word.
On pick it asks for a Mention of that Entry on the span.
The gesture lives in the editor and not the display.
A link is an edit, and every edit goes through a draft.
The span is read in code points before anything reaches a request.
The card and row ids are taken before the picker opens, since the pick answers later.

## `internal void PSentenceSenseHandle(object sender, ExecutedRoutedEventArgs e)`

Opens the window's Meaning menu on the Entry the Mention under the selection stands for.
The chosen sense goes out as a request naming the Mention, and the redraw shows it on the chip.

## `internal void PSentenceSilenceHandle(object sender, ExecutedRoutedEventArgs e)`

Marks the selection as standing for nothing, which is an addition with Entry 0.

## `internal void PSentenceUnlinkHandle(object sender, ExecutedRoutedEventArgs e)`

Drops the Mention under the selection, or the one whose chip was asked from.
The row is read from the element the command was bound on, because a chip button is not the field.

## `internal void PSentenceLinkCheck(object sender, CanExecuteRoutedEventArgs e)`

Link and silence apply when the selection has length.

## `internal void PSentenceSenseCheck(object sender, CanExecuteRoutedEventArgs e)`

Choose applies when the selection lies inside a Mention that has an Entry.

## `internal void PSentenceUnlinkCheck(object sender, CanExecuteRoutedEventArgs e)`

Unlink applies from a chip always, and from the field when the selection lies inside any Mention.

## `internal void PSentenceMentionShow(PCard card)`

Redraws every row's chip line after the card was redrawn, since the rows hold no engine.
A headword read that fails is reported once and the rest of the card is left as drawn.

## Inline notes

### `private void PSentenceChangeHandle(PCard card, PSentence row, string field)`

Turns one row change into its request.
The sentence, the marker and the role are deferred with the debounce, one pending request per row and field.
The Source is sent at once, because it is picked rather than typed.
The typed citation line is no request at all, and only opens the dropdown of Sources answering it.

### `private bool PSentencePendingCheck(PCard card, PSentence row, string field)`

Whether a request for this row's field is still waiting, in which case a redraw must not overwrite it.

### `private void PSentencePrepare()`

Asks for a blank row on every card that shows none, after each redraw.
A card with no row to write in offers nothing.
The engine keeps the blank row rather than the card.
The cards are collected first and checked again before each ask, because each answer redraws and prepares in turn.

### `private static void PSentenceFrameShow(ObservableCollection<string> catalog, IReadOnlyList<string> values)`

The list is refilled in place rather than replaced, because every row already holds it.
