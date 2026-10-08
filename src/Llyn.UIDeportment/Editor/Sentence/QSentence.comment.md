# QSentence.cs
Hash: `fe52c754a27a97b9`

## `internal sealed class QSentence`

The sentence row's driver.
It hears the row's adding, dropping and typing, its mention commands and its frame.
The row's template is filled by [QExample](QExample.comment.md), which hooks these handlers to the row's parts.
It walks the editor's two card lists and holds the Conduct card and sentence facets once introduced.
The citation field is [QCitation](QCitation.comment.md), the Gloss rows are [QGloss](../Gloss/QGloss.comment.md) and the mention dropdown is `QProspect`.

## `internal QSentence(ObservableCollection<PCard> meaning, ObservableCollection<PCard> collocation)`

Holds the two card lists the editor keeps.

## `internal void QSentenceIntroduce(CCard card, CSentence sentence, CMention mention, QMentionMenu mentionMenu, QProspect prospect)`

Holds the card and sentence facets, the span-checking mention area, the meaning menu and the mention dropdown.
The dropdown is handed in because it was built with this driver.

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

## `internal void QSentenceGlossObserve(PSentence row, PGloss gloss, string language)`

Finds the row's card and hands a Gloss's language pick to the sentence gate.

## `internal void QSentenceTextObserve(object sender, TextChangedEventArgs e)`

Finds the row's card and hands the typed sentence to the sentence text gate.
Each field is hooked to its own observer, so no field name decides the gate.
Only a field with the keyboard in it reports, since a write from the draft echoes through the same event.

## `internal void QSentenceParticleObserve(object sender, TextChangedEventArgs e)`

Hands the particle choice's typed text to the particle gate.
The choice hears the text change of its inner box, which is the raw text handed on.
`QSentenceDependenceObserve` does the same for the dependence choice.

## `private static void QSentenceListRefine(ObservableCollection<string> catalog, IReadOnlyList<string> values)`

Refills one offered list.

## `private static void QSentenceChipRefine(IReadOnlyList<PCard> cards, IReadOnlyDictionary<long, IReadOnlyList<CMentionLabel>> lines)`

Gives each row on the cards its chip line.
Conduct answers one entry per sentence, with an empty list for none.
A row with no entry is drawn with no chips, so a dropped line never leaves stale chips.

## `internal void QSentenceAddObserve(object sender, RoutedEventArgs e)`

Finds the row's card and asks the gate to add a row after it.
It hands the row's index in the card's sentences, the place Conduct defines for `below`.

## `internal void QSentenceRemoveObserve(object sender, RoutedEventArgs e)`

Finds the row's card and asks the gate to drop the row.

## `internal void QSentenceLinkRefine(object sender, ExecutedRoutedEventArgs e)`

Places the mention dropdown under the selection and fills it from the card gate's read.

## `internal void QSentenceMeaningRefine(object sender, ExecutedRoutedEventArgs e)`

Asks the gate for the selection's senses and opens the meaning menu under it.
It subscribes the `QMentionAsk` the menu answers, so it hears only its own pick.

## `private void QSentenceSenseObserve(FrameworkElement anchor, long sense)`

Hands the picked sense and the selection of the asking box to the gate.
Only this driver's own ask reaches it, so the box is always one of its sentence rows.

## `internal void QSentenceSilenceObserve(object sender, ExecutedRoutedEventArgs e)`

Hands the selection to the silence gate as a mention with no target.
The gate is its own, so no magic id is sent.

## `internal void QSentenceUnlinkObserve(object sender, ExecutedRoutedEventArgs e)`

Removes the mention a chip names, or the one under the selection.

## `internal void QSentenceSpanRefine(object sender, CanExecuteRoutedEventArgs e)`

Enables a command while the selection holds a span the mention gate accepts.

## `internal void QSentenceSenseRefine(object sender, CanExecuteRoutedEventArgs e)`

Enables the meaning command while the gate finds senses for the selection.

## `internal void QSentenceUnlinkRefine(object sender, CanExecuteRoutedEventArgs e)`

Enables the unlink command for a chip, or while the selection holds a mention.
