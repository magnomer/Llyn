# LEngineDraftIdentity.cs

## `public sealed partial class LEngine`

The one place a draft item gets its id, so the identity rule has no second author.
Every item inside a draft ends up positive or negative, never zero.
Positive names a stored row and negative names a row the engine promised.
The UI sends zero for anything it built, and this file replaces every zero on the next save.
It also records, at commit, which real row each negative id became.

## `private LEntryDraft LEngineDraftNormalize(LEntryDraft content)`

The same content with every item named and every blank item dropped.
The content that came in is returned itself when nothing needed naming or dropping.
That sameness is the answer the form reads to know nothing was corrected.

## `private IReadOnlyList<LPronunciationDraft> LEngineSoundNormalize(IReadOnlyList<LPronunciationDraft> drafts)`

Every pronunciation row named when it carries no id.
A nameless row saying nothing is dropped, because an empty one is not work.
A named empty row is kept, because the form adds a row blank and fills it afterwards.

## `private IReadOnlyList<LTranscriptionDraft> LEngineSpellingNormalize(IReadOnlyList<LTranscriptionDraft> drafts)`

Every transcription row named when it carries no id.
A nameless row with neither scheme nor text is dropped.
A row that names its scheme is kept even before its text is typed.

## `private IReadOnlyList<LCardDraft> LEngineCardNormalize(IReadOnlyList<LCardDraft> cards)`

Every card named, with each list inside it normalized on its own terms.
Empty cards are kept, because the form always offers one and its id is what the form tracks.
A card is returned itself when it was named and none of its lists changed.

## `private IReadOnlyList<LSentenceDraft> LEngineSentenceNormalize(IReadOnlyList<LSentenceDraft> drafts)`

A row saying nothing is kept, because the form adds a row blank and fills it afterwards.
The commit skips such a row, so keeping it costs nothing in the store.
An Example with no text and no id is dropped from its row, since it names nothing.
An Example with text and no id is named, and so is a row with no id.
Image and video rows are kept blank for the same reason.
Situation, register and tag chips are never added blank, so a blank one is still dropped.

## `private static IReadOnlyList<LEngineItem> LEngineListNormalize<LEngineItem>(`

One list walked once, dropping what `blank` says and renaming what `name` changes.
The list that came in is returned itself when every item came back as itself.
A copy is taken only from the first item that changed, because a settled list is the ordinary case.

## `private static void LEngineIdentityRecord(Dictionary<long, long> identity, long draftId, long rowId)`

Records which stored row a negative id became.
A positive or zero id is not a promise, so nothing is recorded for it.
