# LDraftClerkMint.cs

## `public sealed class LDraftClerkMint`

The draft normalisation that mints an id for every row that has none and drops the blank rows.
A held draft is normalised before every save, so the store never sees an unidentified row.

## `public LDraftClerkMint(LIdentity identity)`

Keeps the identity the ids are minted from.

## `public LEntryDraft LDraftNormalize(LEntryDraft content)`

The content with every list normalised.
The same instance comes back when nothing changed, so a caller can tell a no-op by reference.

## `public IReadOnlyList<LReflexDraft> LReflexNormalize(IReadOnlyList<LReflexDraft> drafts)`

Blank unidentified reflex rows dropped and the rest given ids.

## `public LEtymologyDraft LEtymologyNormalize(LEtymologyDraft etymology)`

Spans naming no Entry dropped and the rest given ids.
The links carry no id of their own, so nothing is minted for them.

## `private long LIdentityCreate()`

One fresh id.

## `private IReadOnlyList<LPronunciationDraft> LPronunciationNormalize(IReadOnlyList<LPronunciationDraft> drafts)`

Blank unidentified pronunciation rows dropped and the rest given ids.

## `private IReadOnlyList<LTranscriptionDraft> LTranscriptionNormalize(IReadOnlyList<LTranscriptionDraft> drafts)`

A blank row with no scheme is dropped, since a scheme alone is a row the form keeps.

## `private IReadOnlyList<LCardDraft> LCardNormalize(IReadOnlyList<LCardDraft> cards)`

Every card and its children given ids, their lists normalised in turn.

## `private IReadOnlyList<LSentenceDraft> LSentenceNormalize(IReadOnlyList<LSentenceDraft> drafts)`

A sentence whose example has no text loses the example, and both get ids.

## `private IReadOnlyList<LSituationDraft> LSituationNormalize(IReadOnlyList<LSituationDraft> drafts)`

Untitled situations dropped and the rest given ids.

## `private IReadOnlyList<LRegisterDraft> LRegisterNormalize(IReadOnlyList<LRegisterDraft> drafts)`

Unnamed registers dropped and the rest given ids.

## `private IReadOnlyList<LTagDraft> LTagNormalize(IReadOnlyList<LTagDraft> drafts)`

Blank unidentified tags dropped and the rest given ids.

## `private IReadOnlyList<LImageDraft> LImageNormalize(IReadOnlyList<LImageDraft> drafts)`

Every image given an id.

## `private IReadOnlyList<LVideoDraft> LVideoNormalize(IReadOnlyList<LVideoDraft> drafts)`

Every video given an id.
