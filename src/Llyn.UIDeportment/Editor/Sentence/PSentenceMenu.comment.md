# PSentenceMenu.cs

## `public partial class PEditor`

The editor's side of an Example row: opening and dropping rows, and the mentions and gloss it carries.
The rows themselves hold no engine.
Adding, dropping and typing in a row are torn apart by role, as `docs-work/JobPrinciple.md` section 13 asks.
Each Observe finds the card that owns the row and hands the raw value to one `CSentence` gate.
The mention commands each hand the raw selection to one sentence gate or read.
The field a row cites through is answered in [PSentenceCitation.cs](PSentenceCitation.comment.md).
The linking gesture on a row's sentence is answered here too, four commands over the sentence area.
The frame the rows read a sentence under is settled here too.
That is the order its two fields take and what each has been saved holding.
Both follow the language the entry is written in.
Switching language redraws every row rather than leaving one language's order over another's.

## `internal void PSentenceFrameRefine(CEntryDraft _)`

Paints the Example frame the sentence gate answers for the held draft's language.
The two lists are refilled, and every row on the form takes the word order.
The editor keeps the order it painted, so a row opened later is built under it.
The gate owns the language and the empty lists a failed read leaves.

## `private CSentenceOrder? _pSentenceOrder;`

The word order the frame last painted, which is the GUI's own copy of what the rows show.
A row built between two frame reads takes it, so no card reads the engine for an order.

## `private void PSentenceAddObserve(object sender, RoutedEventArgs e)`

Hears the add button of a row and hands the row's place to the gate, which adds the row below.

## `private void PSentenceRemoveObserve(object sender, RoutedEventArgs e)`

Hears the erase button of a row and hands the row to the gate to drop.

## `private void PSentenceLinkRefine(object sender, ExecutedRoutedEventArgs e)`

Opens the Entry picker over the selected word, from the editor's mention read.
The gesture lives in the editor and not the display.
A link is an edit, and every edit goes through a draft.
The command runs only on a selection with length, so no span is measured here.
The pick is heard by `PProspectPickObserve`, which finds the card and row from the box.
Its gate `CSentenceMentionAdd` reads the span in code points below Conduct.

## `private void PSentenceMeaningRefine(object sender, ExecutedRoutedEventArgs e)`

Opens the window's Meaning menu on the Meanings the sentence area reads under the selection.
The read persists pending typing first, and answers none when no linked Mention lies there.
It reports its own failure, so the menu only shows what it answered.
The menu hands the pick to `PSentenceSenseObserve` with the box as its anchor.

## `private void PSentenceSenseObserve(FrameworkElement anchor, long sense)`

Hears a sense picked in the Meaning menu and hands it to the sense gate with the box's raw selection.
The row and card are found from the anchor.
So no Mention id is held between the open and the pick.

## `private void PSentenceSilenceObserve(object sender, ExecutedRoutedEventArgs e)`

Marks the selection as standing for nothing, through the mention gate with Entry 0.

## `private void PSentenceUnlinkObserve(object sender, ExecutedRoutedEventArgs e)`

Drops the Mention whose chip was asked from, or the one under the field's selection.
The row is read from the element the command was bound on, because a chip button is not the field.
A chip names its Mention by id, and a field hands its raw selection to the other remove gate.

## `private void PSentenceSpanRefine(object sender, CanExecuteRoutedEventArgs e)`

Link and silence apply when the selection spans a code point, the verdict the mention gates answer.

## `private void PSentenceSenseRefine(object sender, CanExecuteRoutedEventArgs e)`

Choose applies when the sentence area says the selection lies inside a Mention that has an Entry.

## `private void PSentenceUnlinkRefine(object sender, CanExecuteRoutedEventArgs e)`

Unlink applies from a chip always, and from the field when the sentence area finds a Mention there.

## `internal void PSentenceMentionRefine(CEntryDraft _)`

Paints every row's chip line from the sentence area's read after the cards were redrawn.
`QEditor` subscribes it to every draft change after the two card lists, so every row stands first.
A failed read answers none, so the form keeps its lines.

## `private static void PSentenceChipRefine(IReadOnlyList<PCard> cards, IReadOnlyDictionary<long, IReadOnlyList<CMentionLabel>> lines)`

Paints the chip line of each row on the given cards from its line, paired by the row's id.

## `private void PSentenceApply(FrameworkElement container, object item, string? _)`

Fills one sentence row and subscribes its handlers, where event attributes stood.
The five command bindings are added once, since a refill finds them already there.
Each binding takes the editor's own handlers, since the dictionary keeps no forwarders.
It hands the chip line and the Gloss line their items and fills.
The citation field hears the dropdown's key Refine and key Observe before its own key handler.
It then asks the list to redraw its reveal, since the citation may have emptied.

## `private void PSentenceOpeningRefine(object sender, RoutedEventArgs e)`

A Refine, since the switch only shows or hides the row's frame fields and asks no gate.
It writes the switch back into the row, where a two-way binding stood.

## Inline notes

### `private void PSentenceGlossObserve(PCard card, PSentence row, PGloss gloss, string language)`

Hands a language picked in a Gloss row to the sentence's language gate, with the row it stands in.
The row paints the language only when the draft returns it.

### `private void PSentenceFieldObserve(PCard card, PSentence row, string field, TextBox box)`

Hands the text typed into one field of a row to the gate for that field.
The sentence, the marker and the role each have their own gate, which defers the edit.
The typed citation line writes nothing.
Its gate answers the dropdown of Sources, which `PProfferCitationRefine` paints.
The field is told apart by the row property name its caller passes.
The gate takes the box's raw text, so the row holds no copy of what was typed.

### `private static void PSentenceListRefine(ObservableCollection<string> catalog, IReadOnlyList<string> values)`

The list is refilled in place rather than replaced, because every row already holds it.
