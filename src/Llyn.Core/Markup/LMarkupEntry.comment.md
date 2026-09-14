# LMarkupEntry.cs

## `public sealed record LMarkupEntry(`

One entry as a markup file carries it, with no id anywhere in it.
Every link is by natural key, a headword and a language, and every row stands in file order.
Metadata such as grasp, frequency, favorite and timestamps is not content and never travels.
Two entries are equal when every row is equal in order, so a round trip can be checked.

**Parameters**

- `LMarkupEntryHeadword` — The headword, required for the entry to enter a workspace.
- `LMarkupEntryLanguage` — The language code, required beside the headword.
- `LMarkupEntrySpeech` — Parts of speech by name, a value name or a custom one.
- `LMarkupEntryForm` — Forms with no entry id, positioned by their order.
- `LMarkupEntryInflection` — Inflections with their speech and morphologies by name.
- `LMarkupEntryPronunciation` — Pronunciation drafts with id zero.
- `LMarkupEntryTranscription` — Transcription drafts with id zero.
- `LMarkupEntryMeaning` — Meaning cards, each of which may nest.
- `LMarkupEntryCollocation` — Collocation cards, which never nest.
- `LMarkupEntryNote` — The note markdown, empty when there is none.
- `LMarkupEntryLine` — The line the entry opened on in its file, zero for an entry built in memory.

## `public bool Equals(LMarkupEntry? other)`

Row-by-row equality in order, so two parses of the same text compare equal.
The line is left out, since it says where the entry was read and not what it holds.

## `private static bool LMarkupPronunciationMatch(LPronunciationDraft first, LPronunciationDraft second)`

Compares two pronunciation drafts by content, since the draft itself compares its syllable list by reference.

## `public override int GetHashCode()`

A hash over the headword, language, note and card counts.
