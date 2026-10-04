# QCard.cs
Hash: `0031b532e71ca530`

## `internal sealed class QCard`

The editor's driver for the meaning and collocation cards.
It renders the draft's cards into a list, and wires each card's header, badge, eraser and inner lists.
Adding, removing and reordering go to the card gates, never edits to the lists.
The engine answers with a draft bulletin, and the render brings the lists into line with it.
The numbering that keeps each list reading 1, 2, 3 is the engine's.
Each card carries the id the engine minted for it.
That is how a request names one card rather than a place in a list.
A Meaning card and a Collocation card are the same card, so one pair of paths serves both lists.
Nothing is read back from a card, because every edit reached the engine as a request.

## `internal QCard(QCardDrag drag, QContext context, QRegister register, QLink link, QLabel label, QSentence sentence, QCitation citation, QImage image, QVideo video, ObservableCollection<PLanguageItem> languages, ObservableCollection<PCard> meanings, ObservableCollection<PCard> collocations)`

Holds the drag driver for the header, and the field drivers each card's inner lists answer to.
It also holds the shared language list a new card's Gloss picker shows, and both card lists.

## `internal void QCardIntroduce(CEditor editor)`

Holds the Conduct editor whose card gates the clicks and fields reach.

## `internal void QCardApply(FrameworkElement container, object item, string? changed)`

Fills one card and subscribes its drag, badge, removal, media and typed-field handlers.
The changed property is passed on, so the card rewrites only that property's text.
Both lists use this one fill, since both draw the same card.

## `private void QCardFieldApply(FrameworkElement container, PCard card)`

Hands every list inside a card its items and attaches each list's fill.
Setting the same items again changes nothing, so a refill does not rebuild the lists.
The sentence list's fill and hover reveal come from `QExample`, which the sentence driver holds.

## `private void QCardTitleObserve(object sender, TextChangedEventArgs e)`

Hands the typed title to the title gate.
Each card field is hooked to its own observer, so no field name decides the gate.
Only a field with the keyboard in it reports, since a write from the draft echoes through the same event.
`QCardExpressionObserve` and `QCardDefinitionObserve` do the same for the expression and the meaning.

## Inline notes

### `private void QCardRemoveObserve(object sender, RoutedEventArgs e)`

Hands the pressed card's id to the gate.
A list of one keeps its card, which the clerk decides, so the eraser sends every press.

### `private void QCardPositionRefine(object sender, MouseButtonEventArgs e)`

Opens the badge for writing on a double click, and stops that click from starting a drag.
A list of one shows no editable badge, as it shows no drag ghost.
That only decides whether the editor appears, so it stays here.
The box is focused after the layout runs, because it is not hit tested until then.

### `private void QCardPositionRefine(object sender, KeyEventArgs e)`

Escape drops the typed number and shuts the badge.

### `private void QCardPositionObserve(object sender, RoutedEventArgs e)`

Enter or leaving the badge hands the box's own typed text to the move gate, then shuts the badge.
The typed text stays in the box as pure medium, so the card's number has one writer.
One handler hears both, since both commit the same text.
The open badge is checked, so a badge shut by Enter or Escape is not read again.

### `private ObservableCollection<PCard>? QCardListFind(PCard card)`

Which of the two lists a card belongs to, since both are drawn from the same template.

### `internal void QCardRefine(ObservableCollection<PCard> cards, string prefix, IReadOnlyList<CCardDraft> drafts)`

Brings one list into line with the draft's cards, so the list shows exactly those cards in their order.
Each draft card claims the first unclaimed PCard with its id, else a new card is built.
A claimed card is never claimed again, so two draft cards never share one PCard.
Conduct does not promise unique ids, so a repeated id is shown twice and never throws.
Removing repeats is behaviour, owned by Conduct or a layer below it, never by this list.
The engine alone owns the order, so the list only removes by identity and appends, never inserting or moving.
A card the form does not show yet is built, then painted like a kept one.
Its sentence notice is subscribed as it is built, so a Gloss pick reaches the editor.
Every card, new or kept, is painted, since any of its values may have changed.
The kept cards are the longest head of the draft's order already standing in that sequence.
They are compared by object identity, so a kept card is the same PCard, not an equal one.
Kept cards need not be adjacent in the old list.
Every other card is removed by identity, and the rest of the draft is appended in its order.
A card the draft no longer names is therefore dropped, and any other removed card returns at its draft place.
Only kept cards keep their controls and caret, since they never leave the list.
A re-added card keeps its object but gets a rebuilt control, which loses focus and caret.
Every card after the first mismatch is re-added, even one the change never passed.
Moving B to the front of A, B, C, D rebuilds A, C and D.
An insert in the middle rebuilds every later card, and moving a card to the end rebuilds only it.
That cost is the price of never inserting or moving.

### `private void QCardDraftRefine(PCard card, CCardDraft draft)`

Paints one card from its ready draft card, and the card redraws only a value that changed.
The sentence frame's order goes with the rows, as the last `CSentenceFrameRead` answered it.
The frame is painted before the cards show, so a new row is built in its language's order.
The links come ready on the draft card, so the paint asks Conduct nothing per card.
The chip lines under the rows are painted by `QSentenceMentionRefine`, which answers the same draft change after the cards.
A picture or film row is built with the atelier, which the card itself does not hold.
Each card takes the number the draft carries, not its place in the loop.
