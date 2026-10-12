# LMarkupCard.cs
Hash: `4f245dcabda5b7bc`

## `public sealed record LMarkupCard(LStateValue LMarkupCardTitle, LStateValue LMarkupCardExpression, LStateValue LMarkupCardMeaning, IReadOnlyList<LMarkupSentence>? LMarkupCardSentence = null, IReadOnlyList<LSituationDraft>? LMarkupCardSituation = null, IReadOnlyList<LRegisterDraft>? LMarkupCardRegister = null, IReadOnlyList<LMarkupTranslation>? LMarkupCardTranslation = null, IReadOnlyList<LTagDraft>? LMarkupCardTag = null, IReadOnlyList<LImageDraft>? LMarkupCardImage = null, IReadOnlyList<LVideoDraft>? LMarkupCardVideo = null, IReadOnlyList<LMarkupCard>? LMarkupCardChild = null, IReadOnlyList<string>? LMarkupCardSense = null)`

One meaning or collocation as a markup file carries it.
The shape is `LCardDraft` with every id and position gone.
Translations name their target by headword and language rather than by id.
Position is the order in the list holding the card.

**Parameters**

- `LMarkupCardTitle` — The card title and what is known about it.
- `LMarkupCardExpression` — The expression, used on a collocation, and what is known about it.
- `LMarkupCardMeaning` — The definition and what is known about it.
- `LMarkupCardSentence` — Sentence rows in file order.
- `LMarkupCardSituation` — Situation drafts with id zero.
- `LMarkupCardRegister` — Register drafts with id zero.
- `LMarkupCardTranslation` — Translation targets by natural key.
- `LMarkupCardTag` — Tag drafts with id zero.
- `LMarkupCardImage` — Image drafts, locations verbatim.
- `LMarkupCardVideo` — Video drafts, locations and spans verbatim.
- `LMarkupCardChild` — Nested sub-senses, always empty on a collocation.
- `LMarkupCardSense` — Dotted paths of the meanings a collocation links, in link order, empty on a meaning.

## `public bool Equals(LMarkupCard? other)`

Row-by-row equality in order, children and sense paths included.
Paths are compared ordinally.

## `public override int GetHashCode()`

A hash over the three stated values and the sentence and child counts.
