# LRequestEntry.cs

The entry-level requests, one per field of the form that is not a card.
Each carries the draft id and the new value, and nothing else.
A text field sends the whole text, not a delta, so the last request to arrive wins outright.

## `public sealed record LRequestHeadword(long LRequestDraftId, string LRequestText)`

Replaces the headword.

## `public sealed record LRequestLanguage(long LRequestDraftId, string LRequestText)`

Replaces the entry's language.
The property is named for its text rather than its meaning, because a member may not share its type's name.

## `public sealed record LRequestNote(long LRequestDraftId, string LRequestText)`

Replaces the note with the Markdown text the editor holds.
The engine normalizes it at commit, so the draft keeps what was typed.

## `public sealed record LRequestIpa(long LRequestDraftId, string LRequestText)`

Replaces the typed reading of the primary pronunciation, the first of the list, leaving its recording and its stored detail alone.
When the list is empty the request creates that first row.
A pronunciation with neither reading nor recording is dropped by the engine, not by the request.
A form that edits every row sends [LRequestPronunciationIpa](LRequestPronunciation.comment.md) instead.

## `public sealed record LRequestAudio(long LRequestDraftId, string LRequestFile, string? LRequestSource)`

Replaces the recording of the primary pronunciation and the source it came from.
An empty file clears it.

## `public sealed record LRequestSpeech(long LRequestDraftId, IReadOnlyList<LSpeechDraft> LRequestSpeeches)`

Replaces the parts of speech as a whole list.
A chip is added or removed whole, and the list is short, so a delta would save nothing.
