# QCard.cs
Hash: `905ec0f1679444bc`

## `internal sealed class QCard`

Meaning and collocation lists share one renderer and one set of card handlers.
User gestures reach Conduct gates, while ready draft values drive presentation.
Card identity is independent of list position.

## `internal QCard(QCardDrag drag, QContext context, QRegister register, QLink link, QLabel label, QSentence sentence, QCitation citation, QExample example, QImage image, QVideo video, ObservableCollection<PLanguageItem> languages, ObservableCollection<PCard> meanings, ObservableCollection<PCard> collocations)`

Field drivers and sentence-menu collections are shared by both card kinds.
The badge driver receives both lists so position requests can locate their card.

## `internal void QCardIntroduce(CCardField field, CCardList list)`

Field edits and list gestures retain separate Conduct facets.
The badge driver shares the same list facet.

## `internal void QCardApply(FrameworkElement container, object item, string? changed)`

Only card items receive handlers and presentation.
Handlers are removed before reattachment, avoiding duplicate subscriptions during refills.
The changed-property name limits field text replacement.

## `internal static void QCardRowRefine(FrameworkElement container, PCard card, string? changed)`

Unrelated property changes leave field text alone, preserving another field's caret.
Badge opening, closing and renumbering restore its position text.
Hints, folded peeks and fold shape follow every refill.
The title box remains visible when a folded untitled card shows its peek, preserving editing focus.
`QCardFold` owns the shared writing and reading fold shape.

## `private void QCardFieldApply(FrameworkElement container, PCard card)`

Each inner list receives the card's retained presentation collection and its own driver.
Sentence rows share the supplied example filler and hover-reveal behavior.

## `private void QCardHingeObserve(object sender, RoutedEventArgs e)`

The hinge toggle sends card identity and checked state to `CCardFoldToggle`.
Its verdict reaches `QLook.QLookCheckedRefine`, which restores refused clicks.
Accepted folds repaint through ready draft notifications.

## `private void QCardTitleObserve(object sender, TextChangedEventArgs e)`

Only keyboard-focused title fields report raw text to the title gate.
Ordinary unfocused rendering echoes therefore do not become edits.

## `private void QCardRemoveObserve(object sender, RoutedEventArgs e)`

Removal sends identity rather than editing the presentation collection.
Conduct decides whether removal is allowed.

## `internal void QCardRefine(ObservableCollection<PCard> cards, string prefix, IReadOnlyList<CCardDraft> drafts, Func<PCard, CStateWording> peek)`

Each draft claims one existing card by id or receives a new presentation item.
Repeated ids can produce separate items without sharing one claimed card.
The supplied draft order is authoritative.
Reconciliation keeps its longest leading sequence found in the existing order, removing other items and appending the remainder.
Only that kept sequence retains controls and caret.
Re-added items retain their objects but may lose focus when their controls rebuild.
The caller chooses each new card's peek wording, which remains derived rather than copied.

## `private void QCardDraftRefine(PCard card, CCardDraft draft)`

Every retained or new card receives ready values without a per-card Conduct lookup.
Sentence rows share the sentence driver's current order.
The supplied position is independent of loop position.
Fold verdicts arrive last, allowing an in-place fold repaint after other fields are current.

## `private void QCardExpressionObserve(object sender, TextChangedEventArgs e)`

Only keyboard-focused expression fields report raw text to the expression gate.

## `private void QCardDefinitionObserve(object sender, TextChangedEventArgs e)`

Only keyboard-focused definition fields report raw text to the meaning gate.
