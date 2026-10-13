# Card

These tests isolate entry card gates and draft wording from window behavior.
Real workspace tests inspect held drafts to distinguish completed edits from pending input and suggestions.

## `TCard`

Direct draft construction checks wording without requiring a workspace.
Shared desk helpers keep field gate tests on immediate persistence.

## `TCardPrepare`

Disabling delay lets field tests inspect edits immediately on a held entry desk.

## `TCardSheetAdd`

Tag, translation, register and situation tests share this meaning card setup.

## `TCardTranslationRead`

Translation tests inspect linked ids independently of their suggestions.

## `TCardSentenceAdd`

Reference tests share this sentence setup to exercise citations on separate cards.

## `TCardEtymology`

Span checks distinguish selections inside a linked mention from selections outside it.
An unopened desk checks that reading chips requires no held draft.

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

Each case checks one rule of the caret, against a held draft's chip rows.
A step past either end refuses and keeps the caret, and a delete past an end removes nothing.
A removed anchor reads the next chip, and a chip added before the anchor leaves it standing.
One case reads every trail of a card at once.

## `TCardTranslation`

Suggestions, resolution and insertion are tested separately to distinguish choosing from linking.
Fresh translation removal checks court cleanup, while stored translation removal checks that the target remains findable.
Duplicate headwords check display numbering without changing headword text.
The caret is built over the desk and the card, as the editor builds it.
A caret moved back inserts a picked row before its anchor and leaves the anchor standing.

## Stamp

Commit: `c2f335c331fe`
- `TCard.cs` `a1c183924f8ad673`
- `TCardEtymology.cs` `8fb076bb0cec6e22`
- `TCardList.cs` `4700acc755d2cbf7`
- `TCardReference.cs` `14ed3d1cc2cc9837`
- `TCardRegister.cs` `255eac3a81088dfc`
- `TCardSentence.cs` `04006230f6c0a70c`
- `TCardSituation.cs` `97020c28ffde233f`
- `TCardSpeech.cs` `8c3b0ecae8677f2f`
- `TCardTag.cs` `b39dd4915b874323`
- `TCardTranslation.cs` `265de7207ac29b5b`
