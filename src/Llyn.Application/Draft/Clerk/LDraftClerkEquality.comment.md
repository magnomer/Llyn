# LDraftClerkEquality.cs
Hash: `e66facd1e63d8aa9`

## `public static class LDraftClerkEquality`

Whether two forms of the same record say the same thing.
Every call here is static and answers by content, because records compare lists by reference.
A reference comparison would call every reloaded draft a change and never settle.
This is what tells held work apart from the record it was started from.
The engine asks it for the dirty check and the chronicle.
The save asks it to know whether anything changed.

## `public static bool LDraftMatch(LDraft one, LDraft other)`

Whether two held drafts say the same thing, whatever kind of record they hold.
An example, situation or reference draft is compared through that record's own clerk.
A reference draft also compares its author draft through the author clerk.
An author draft is compared by held name, trimmed, because a name is all it carries.
An entry draft is compared through its content, so blank scaffolding is ignored as it is everywhere.

## `public static bool LDraftMatch(LEntryDraft one, LEntryDraft other)`

Field by field, whether two forms of an entry say the same thing.
Language sits beside the headword, because changing only the tongue is still an edit.
The headword is compared trimmed and the note canonical, since the commit stores both that way.
A form that differs only there would otherwise report itself changed forever.
The lists inside are compared by their contents, since records compare them by reference.
The Meaning and Collocation cards are compared through `LCardEquality`, which owns the card rule.

## `public static IReadOnlyList<LTranscriptionDraft> LTranscriptionScan(IReadOnlyList<LTranscriptionDraft> drafts)`

The transcription rows that count are every row the user added, and a seeded row once it holds text.
A seeded row still blank is left out, because the form offered it and nobody asked for it.
The transcription sync writes the same rows, so the match and the save agree on what a row is.

## `public static IReadOnlyList<LReflexDraft> LReflexScan(IReadOnlyList<LReflexDraft> drafts)`

The reflex rows that carry something, in their original order.
The reflex sync and the reflex fetch read the same rows.

## `private static bool LPronunciationMatch(IReadOnlyList<LPronunciationDraft> one, IReadOnlyList<LPronunciationDraft> other)`

Whether two drafts record the same pronunciations in the same order.
A seeded row still blank is left out first, because the form offered it and nobody asked for it.
A row the user added counts even blank, so adding one is a change.

## `private static bool LPronunciationMatch(LPronunciationDraft one, LPronunciationDraft other)`

Whether two rows record the same pronunciation.
The lists inside a row are compared by content, because two equal lists are rarely the same object.

## `private static bool LTranscriptionMatch(IReadOnlyList<LTranscriptionDraft> one, IReadOnlyList<LTranscriptionDraft> other)`

Whether two drafts record the same transcriptions in the same order.
A seeded row still blank is left out, as it is for pronunciations.

## `private static bool LReflexMatch(IReadOnlyList<LReflexDraft> one, IReadOnlyList<LReflexDraft> other)`

Whether two drafts record the same reflexes in the same order, blank rows left out.
The anchors are compared by content, and the rest of the row as a record.

## `private static bool LSpeechMatch(IReadOnlyList<LSpeechDraft> one, IReadOnlyList<LSpeechDraft> other)`

Whether two drafts name the same parts of speech in the same order.
A language-pack value and a custom name of the same wording are two different parts.
