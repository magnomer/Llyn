# LEngineDraftMatch.cs

## `public sealed partial class LEngine`

Whether two forms of the same record say the same thing.
Every call here is static and answers by content, because records compare lists by reference.
A reference comparison would call every reloaded draft a change and never settle.
This is what tells held work apart from the record it was started from.

## `private static bool LEngineDraftMatch(LEntryDraft one, LEntryDraft other)`

Field by field, whether two forms of an entry say the same thing.
Language sits beside the headword, because changing only the tongue is still an edit.
The headword is compared trimmed and the note canonical, since the commit stores both that way.
A form that differs only there would otherwise report itself changed forever.
The lists inside are compared by their contents, since records compare them by reference.

## `private static bool LEngineSoundMatch(IReadOnlyList<LPronunciationDraft> one, IReadOnlyList<LPronunciationDraft> other)`

Whether two drafts record the same pronunciations in the same order.
A seeded row still blank is left out first, because the form offered it and nobody asked for it.
A row the user added counts even blank, so adding one is a change.

## `private static bool LEngineSoundMatch(LPronunciationDraft one, LPronunciationDraft other)`

Whether two rows record the same pronunciation.
The lists inside a row are compared by content, because two equal lists are rarely the same object.
A record comparison would call every reloaded draft a change and never settle.

## `private static bool LEngineSpellingMatch(IReadOnlyList<LTranscriptionDraft> one, IReadOnlyList<LTranscriptionDraft> other)`

Whether two drafts record the same transcriptions in the same order.
A seeded row still blank is left out, as it is for pronunciations.

## `private static bool LEngineSpeechMatch(IReadOnlyList<LSpeechDraft> one, IReadOnlyList<LSpeechDraft> other)`

Whether two drafts name the same parts of speech in the same order.
A language-pack value and a custom name of the same wording are two different parts.

## `private static bool LEngineCardMatch(IReadOnlyList<LCardDraft> one, IReadOnlyList<LCardDraft> other)`

Whether two card lists carry the same cards in the same order.
Order counts, because the order cards are in is the order they are stored in.
Position is compared with it, so a reorder is caught by what it changed rather than by chance.

## `private static bool LEngineVideoMatch(IReadOnlyList<LVideoDraft> one, IReadOnlyList<LVideoDraft> other)`

Blank rows are left out first, as they are for sentences.

Whether two video lists carry the same clips in the same order.

## `private static IReadOnlyList<LCardDraft> LEngineCardScan(IReadOnlyList<LCardDraft> cards)`

The cards that carry something, in their original order.

## `private static bool LEngineCardCheck(LCardDraft card)`

Whether a card holds nothing at all.
An open form always shows one such card, and offering it is not an edit.
A blank sentence, image or video row counts as nothing, since the form offers those too.

## `private static bool LEngineSentenceMatch(IReadOnlyList<LSentenceDraft> one, IReadOnlyList<LSentenceDraft> other)`

Whether two row lists say the same thing in the same order.
Blank rows are left out first, so a row the form added and never filled is not a change.
The frame counts, so writing a marker or a role and nothing else is a change and is saved.
The stored row each names counts too, because a row that changed id is a different row.

## `private static bool LEngineExampleMatch(LExampleDraft? one, LExampleDraft? other)`

Whether two rows quote the same Example on the same terms.
A row quoting none matches only another quoting none.
The citation counts, so retagging a sentence is a change.
The Mentions count too, so a word linked or unlinked marks the draft dirty.

## `private static bool LEngineSituationMatch(IReadOnlyList<LSituationDraft> one, IReadOnlyList<LSituationDraft> other)`

Whether two situation lists say the same thing in the same order.
All three stored fields count, not the title the card happens to show.

## `private static bool LEngineImageMatch(IReadOnlyList<LImageDraft> one, IReadOnlyList<LImageDraft> other)`

Whether two image lists carry the same pictures in the same order.
Blank rows are left out first, as they are for sentences.

## `private static bool LEngineTextMatch(IReadOnlyList<string> one, IReadOnlyList<string> other)`

Whether two text lists match exactly, order included.
