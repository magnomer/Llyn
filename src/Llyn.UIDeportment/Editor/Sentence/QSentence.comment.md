# QSentence.cs
Hash: `121db976e3f9d796`

## `internal sealed class QSentence`

The sentence row's driver.
It handles the row's opening, adding, dropping and typing, its mention commands and its frame.
It holds the editor scope, whose card lists it walks, and the Conduct editor area once introduced.
The citation field is [QCitation](QCitation.comment.md), the Gloss rows are [QGloss](../Gloss/QGloss.comment.md) and the mention dropdown is `QProspect`.

## `internal QSentence(ObservableCollection<PCard> meaning, ObservableCollection<PCard> collocation)`

Holds the two card lists the editor keeps.

## `internal void QSentenceIntroduce(CEditor editor, QWindow host, QProspect prospect, QGloss gloss, QCitation citation)`

Holds the editor area, the window that opens the meaning menu, and the drivers a row's wiring hands events to.
They are handed in because each of them was built with this driver.

## `internal ObservableCollection<string> QSentenceParticle { get; }`

The particles the frame last offered, shared by every row's particle field.

## `internal ObservableCollection<string> QSentenceDependence { get; }`

The dependences the frame last offered, shared by every row's dependence field.

## `internal CSentenceOrder? QSentenceOrder { get; private set; }`

The word order the frame last painted, which is the GUI's own copy of what the rows show.
A row built between two frame reads takes it, so no card reads the engine for an order.

## `internal PCard? QSentenceCardFind(PSentence row)`

The card whose sentence list holds the row.
A row's data context is the sentence, not the card, so every sentence gate first asks this.
The Gloss, citation and mention drivers ask it too.

## `internal void QSentenceFrameRefine(CEntryDraft _)`

Paints the Example frame the sentence gate answers for the held draft's language.
The two lists are refilled, and every row on the form takes the word order.
The gate owns the language and the empty lists a failed read leaves.

## `internal void QSentenceMentionRefine(CEntryDraft _)`

Paints the chip lines under the rows from one mention read.
It answers the draft change after the cards, so a new row already stands.

## `internal void QSentenceApply(FrameworkElement container, object item, string? _)`

Fills a row's template.
That covers the command bindings, the buttons, the citation field, the chip lines and the Gloss lines.
It hooks each typed field to its own observer, the two choices through their inner box's text change.
Each event is hooked once, so a refill leaves one handler.

## `internal void QSentenceGlossObserve(PCard card, PSentence row, PGloss gloss, string language)`

Hands a Gloss's language pick to the sentence gate.

## `private void QSentenceTextObserve(object sender, TextChangedEventArgs e)`

Finds the row's card and hands the typed sentence to the sentence text gate.
Each field is hooked to its own observer, so no field name decides the gate.
Only a field with the keyboard in it reports, since a write from the draft echoes through the same event.

## `private void QSentenceParticleObserve(object sender, TextChangedEventArgs e)`

Hands the particle choice's typed text to the particle gate.
The choice hears the text change of its inner box, which is the raw text handed on.
`QSentenceDependenceObserve` does the same for the dependence choice.

## `private void QSentenceCitationRefine(object sender, TextChangedEventArgs e)`

Finds the row's card and lets `QCitationFieldRefine` offer the references for the typed citation.

## `internal static void QSentenceRevealAttach(ItemsControl list)`

Shows a list's row controls while the pointer or keyboard focus is over the list.

## `private static void QSentenceListRefine(ObservableCollection<string> catalog, IReadOnlyList<string> values)`

Refills one offered list.

## `private static void QSentenceChipRefine(IReadOnlyList<PCard> cards, IReadOnlyDictionary<long, IReadOnlyList<CMentionLabel>> lines)`

Gives each row on the cards its chip line.

## `private static void QSentenceRevealRefine(ItemsControl list)`

Shows or hides each row's controls.
It dims an uncited row's citation field while the list is neither hovered nor focused.
Whether a row is cited is the ready verdict `PSentenceCited`, and the driver adds only its own hover state.
The two event overloads only hand their sender to it.

## `private void QSentenceOpeningRefine(object sender, RoutedEventArgs e)`

Opens or closes the row's frame.

## `private void QSentenceAddObserve(object sender, RoutedEventArgs e)`

Finds the row's card and asks the gate to add a row after it.

## `private void QSentenceRemoveObserve(object sender, RoutedEventArgs e)`

Finds the row's card and asks the gate to drop the row.

## `private void QSentenceLinkRefine(object sender, ExecutedRoutedEventArgs e)`

Places the mention dropdown under the selection and fills it from the card gate's read.

## `private void QSentenceMeaningRefine(object sender, ExecutedRoutedEventArgs e)`

Asks the gate for the selection's senses and opens the meaning menu under it.
The menu hands the pick to `QSentenceSenseObserve` with the box as its anchor.

## `private void QSentenceSenseObserve(FrameworkElement anchor, long sense)`

Hands the picked sense and the selection to the gate.

## `private void QSentenceSilenceObserve(object sender, ExecutedRoutedEventArgs e)`

Hands the selection to the gate as a mention with no target.

## `private void QSentenceUnlinkObserve(object sender, ExecutedRoutedEventArgs e)`

Removes the mention a chip names, or the one under the selection.

## `private void QSentenceSpanRefine(object sender, CanExecuteRoutedEventArgs e)`

Enables a command while the selection holds a span the mention gate accepts.

## `private void QSentenceSenseRefine(object sender, CanExecuteRoutedEventArgs e)`

Enables the meaning command while the gate finds senses for the selection.

## `private void QSentenceUnlinkRefine(object sender, CanExecuteRoutedEventArgs e)`

Enables the unlink command for a chip, or while the selection holds a mention.
