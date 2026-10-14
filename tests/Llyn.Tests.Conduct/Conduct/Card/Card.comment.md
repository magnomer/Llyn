# Card

These tests isolate entry card gates and draft wording from window behavior.
Real workspace tests inspect held drafts to distinguish completed edits from pending input and suggestions.
Typed chip cases go through a `CCaret` built over the desk and the card, as the editor builds it.
Caret cases check that settles and picks land before the anchor and deletes take the chip beside the caret.

## `TCard`

Direct draft construction checks wording without requiring a workspace.
Shared desk helpers keep field gate tests on immediate persistence.

## `TCardPrepare`

Disabling delay lets field tests inspect edits immediately on a held entry desk.

## `TCardSheetAdd`

Tag, translation, register, situation and caret trail tests share this meaning card setup.

## `TCardTranslationRead`

Translation tests inspect linked ids independently of their suggestions.

## `TCardSentenceAdd`

Reference tests share this sentence setup, and a second call puts the next sentence on a separate card.

## `TCardList`

Typed ordinals and dragged positions have separate tests because their accepted ranges differ.
Removal checks protect the lone meaning while allowing one of two meanings to leave.

## `TCardReference`

Citation failures are isolated from suggestion matching.
Two sentences distinguish suppressing an already cited byline from excluding that Source globally.

## `TCardRegister`

Completed names, pending text and stored picks are tested separately.
Two cards check that suggestion exclusion belongs to the card holding the register.

## `TCardSentence`

Sentence edits are checked through the resulting draft rather than a window.
An unopened desk tests addition without a held draft.

## `TCardSituation`

Completed titles, pending text and stored picks are tested separately.
Two cards check that suggestion exclusion belongs to the card holding the situation.

## `TCardSpeech`

Most cases open an English draft to exercise preset suggestions.
An unopened editor supplies the empty-catalog case.
Reopening and undoing the draft check that pending text cannot preserve a stale speech list.

## `TCardTag`

Completed tags, pending text and stored picks are tested separately.
Two cards check that suggestion exclusion belongs to the card holding the tag.

## `TCaretTrail`

Each case checks one rule of the caret, against the register row of a held draft.
A step past either end refuses and keeps the caret, and a delete past an end removes nothing.
A removed anchor reads the next chip, and a chip added before the anchor leaves it standing.
One case reads every trail of a card at once.

## `TCardTranslation`

Suggestions, resolution and insertion are tested separately to distinguish choosing from linking.
Fresh translation removal checks court cleanup, while stored translation removal checks that the target remains findable.
Duplicate headwords check display numbering without changing headword text.
The mention picker reads live here too, since they map the same translation offer.
A caret moved back inserts a picked row before its anchor and leaves the anchor standing.

## Stamp

Commit: `c6e60141a172`
- `TCard.cs` `a1c183924f8ad673`
- `TCardList.cs` `4700acc755d2cbf7`
- `TCardReference.cs` `14ed3d1cc2cc9837`
- `TCardRegister.cs` `3d15e4b41aed92e9`
- `TCardSentence.cs` `04006230f6c0a70c`
- `TCardSituation.cs` `fa4fc1da8387b068`
- `TCardSpeech.cs` `8c3b0ecae8677f2f`
- `TCardTag.cs` `276f014b44fa7962`
- `TCardTranslation.cs` `b990397e938bff90`
- `TCaretTrail.cs` `8cbc3ec4f9a7069d`
