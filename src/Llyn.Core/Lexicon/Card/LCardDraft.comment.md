# LCardDraft.cs
Hash: `4575bde59cbc571c`

## `public sealed record LCardDraft(LStateValue LCardDraftTitle, LStateValue LCardDraftExpression, LStateValue LCardDraftMeaning, IReadOnlyList<LSentenceDraft> LCardDraftSentence, IReadOnlyList<LSituationDraft> LCardDraftSituation, IReadOnlyList<LRegisterDraft> LCardDraftRegister, IReadOnlyList<long> LCardDraftTranslation, IReadOnlyList<LTagDraft> LCardDraftTag, IReadOnlyList<LImageDraft> LCardDraftImage, IReadOnlyList<LVideoDraft> LCardDraftVideo, int LCardDraftPosition, long LCardDraftId = 0, IReadOnlyList<LCardDraft>? LCardDraftChild = null)`

Meanings and collocations share one immutable card shape, so their common fields have one definition.
The record carries lexical content and row identity, but no fold state.
Collection order belongs to the supplied draft, rather than being reconstructed by this value.
Generated equality compares collection references, so equal row contents alone do not guarantee equal cards.

**Parameters**

- `LCardDraftTitle`: the title and its knowledge state.
- `LCardDraftExpression`: the expression field shared by the card shape.
- `LCardDraftMeaning`: the meaning field, labelled Definition for meanings and Meaning for collocations.
- `LCardDraftSentence`: ordered sentence holds, which can carry a frame without an example.
- `LCardDraftSituation`: ordered situation references and their ready values.
- `LCardDraftRegister`: ordered register references and their ready names.
- `LCardDraftTranslation`: ordered target entry ids, rather than copies of target headwords.
- `LCardDraftTag`: ordered tag references and their text.
- `LCardDraftImage`: ordered image rows with location state and row identity.
- `LCardDraftVideo`: ordered video rows with location state and row identity.
- `LCardDraftPosition`: the supplied display number, independent of this record's row id.
- `LCardDraftId`: the stored row identity, zero by default.
  Negative editor ids and zero both remain unsaved according to `LCardDraftStored`.
- `LCardDraftChild`: nested cards, empty when the constructor receives null.

## `public IReadOnlyList<LCardDraft> LCardDraftChild { get; init; }`

Constructor null becomes an empty child list, so ordinary readers need no constructor-null check.

## `public LStateValue LCardDraftTitle { get; init; }`

Constructor null becomes unspecified, preserving a value rather than a missing title object.

## `public LStateValue LCardDraftExpression { get; init; }`

Constructor null becomes unspecified, matching the title's initialization rule.

## `public LStateValue LCardDraftMeaning { get; init; }`

Constructor null becomes unspecified, matching the other scalar value fields.

## `public LCardDraft LCardDraftNormalize()`

Normalizes scalar values, supporting rows and descendants without changing card identity or position.
Translation ids and tags remain untouched, since this normalization walks state-bearing values.

## `public bool LCardDraftExemplified`

Any sentence hold counts, even when that hold is empty.

## `public bool LCardDraftStored`

Only a positive id represents a stored card.
This shared verdict lets views decide fold eligibility without deriving it from ids themselves.

## `public int LCardDraftTally`

Counts this card and all descendants, so nested cards contribute at every depth.

## `public bool LCardDraftEmpty`

Emptiness covers the scalar fields, supporting rows and translations, but not identity or position.
Any child prevents emptiness, even when that child's own fields are empty.
Situation and register rows contribute their title and name respectively.
Only zero translation ids are empty, and blank tag text contributes nothing.
