# Card

These tests isolate entry card gates and draft wording from window behavior.
Real workspace tests inspect held drafts to distinguish completed edits from pending input and suggestions.
Typed chip cases go through a `CCaret` built over the desk, the card and a `CLedgerNoticed`.
Card and caret calls pass `false` for the collocation flag, so these cases stay on the meaning cards.
Caret cases check that settles and picks land before the anchor and deletes take the chip beside the caret.

## `TCard`

Direct draft construction checks wording without requiring a workspace.
Shared desk helpers keep field gate tests on immediate persistence.
The shown name of a picture or video is the file name of its resolved address.
Else it is the file name of its written location.
When neither holds a file name, the address or the written location itself is the name.
The written location is split at either slash or the drive colon, so Windows and Unix paths name alike.
A store value that could not be read shows as uncertain and keeps its own text.
Normalizing that value clears the uncertainty.
The test normalizes through the Interface relay, since a Conduct test never calls a Core record itself.

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
A failing source find shows `Input.FindFailed` once for two finds and offers no rows.
Two sentences distinguish suppressing an already cited byline from excluding that Source globally.
The stored title is read back for a cited sentence and is empty for an uncited one.
Finding the stored title answers null and leaves a lit row of another dropdown lit.

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

Every case opens an English draft to exercise preset suggestions.
Reopening and undoing the draft check that pending text cannot preserve a stale speech list.

## `TCardTag`

Completed tags, pending text and stored picks are tested separately.
The tag offer is read as a `CProffer`, the shape every chip field's dropdown has.
A tag add whose tenure observer throws shows `Input.UpdateFailed`.
A part that lands before the failure stays in the draft, and the untried part stays in the kept text.
A refused part stays in the kept text as typed.
Repeating the last answer's text hands that answer again and keeps its lit row.
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

Commit: `9377e657b395`
- `TCard.cs` `cfcb44c8b6fc70ee`
- `TCardList.cs` `f6fbdaec88c415d7`
- `TCardReference.cs` `f00a4d6d1315659e`
- `TCardRegister.cs` `5cddb184e2e14620`
- `TCardSentence.cs` `ab4b603ea6546af2`
- `TCardSituation.cs` `32d8033f8ebedc3f`
- `TCardSpeech.cs` `26854dc20c0f1a67`
- `TCardTag.cs` `43668d10865ba917`
- `TCardTranslation.cs` `de20dd6ed25364df`
- `TCaretTrail.cs` `46a47c7f4587b3b0`
