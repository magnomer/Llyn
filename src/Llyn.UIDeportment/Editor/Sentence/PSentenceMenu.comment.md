# PSentenceMenu.cs

## `public partial class PEditor`

The editor's side of an Example row: opening and dropping rows, and the Source list a row cites from.
The rows themselves hold no engine.
Adding, dropping and typing in a row are torn apart by role, as `docs-work/JobPrinciple.md` section 13 asks.
Each Observe finds the card that owns the row and hands the raw value to one `CSentence` gate.
The mention commands still send their own requests, left for their own job.
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
It answers each opened workspace and every reference change the sentence area raises.
So a byline edited elsewhere is never stale here.
A workspace that cannot be read leaves the list empty rather than failing the form.

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

## `internal void PSentenceLinkHandle(object sender, ExecutedRoutedEventArgs e)`

Opens the Entry picker over the selected word, from the editor's mention read.
The gesture lives in the editor and not the display.
A link is an edit, and every edit goes through a draft.
The pick is heard by `PProspectPickObserve`, which finds the card and row from the box.
Its gate `CSentenceMentionAdd` reads the span in code points below Conduct.

## `internal void PSentenceSenseHandle(object sender, ExecutedRoutedEventArgs e)`

Opens the window's Meaning menu on the Entry the Mention under the selection stands for.
The chosen sense goes out as a request naming the Mention, and the redraw shows it on the chip.
Pending typing is persisted first, so the Mention is found against the text the field shows.

## `private void PSentenceSenseShow(TextBox box, PCard card, PSentence row, CMentionDraft? mention)`

Opens the menu for the found Mention, and does nothing when it is missing or links no Entry.
The Mention arrives as a parameter, so the handler branches on no engine answer.

## `internal void PSentenceSilenceHandle(object sender, ExecutedRoutedEventArgs e)`

Marks the selection as standing for nothing, which is an addition with Entry 0.

## `internal void PSentenceUnlinkHandle(object sender, ExecutedRoutedEventArgs e)`

Drops the Mention under the selection, or the one whose chip was asked from.
The row is read from the element the command was bound on, because a chip button is not the field.
Pending typing is persisted first, so the Mention is found against the text the field shows.

## `internal void PSentenceLinkCheck(object sender, CanExecuteRoutedEventArgs e)`

Link and silence apply when the selection has length.

## `internal void PSentenceSenseCheck(object sender, CanExecuteRoutedEventArgs e)`

Choose applies when the selection lies inside a Mention that has an Entry.

## `internal void PSentenceUnlinkCheck(object sender, CanExecuteRoutedEventArgs e)`

Unlink applies from a chip always, and from the field when the selection lies inside any Mention.

## `internal void PSentenceMentionShow(PCard card)`

Redraws every row's chip line after the card was redrawn, since the rows hold no engine.
A headword read that fails is reported once and the rest of the card is left as drawn.

## `private readonly PSentenceTemplate _pSentenceTemplate`

The sentence row dictionary, held so its fill can subscribe the row's forwarders.

## `private void PSentenceApply(FrameworkElement container, object item, string? _)`

Fills one sentence row and subscribes its forwarders, where event attributes stood.
The five command bindings are added once, since a refill finds them already there.
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
