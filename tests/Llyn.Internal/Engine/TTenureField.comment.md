# TTenureField.cs

## `public sealed class TTenureField`

The tenure's entry field edits, one fact per member, each over a fresh entry with no delay.

## `public void HeadwordSet_Typed_WritesHeadword()`

A typed headword lands and marks the draft changed.

## `public void NoteSet_TrailingBreaks_WritesTrimmedNote()`

A typed note lands without its trailing line breaks.

## `public void LanguageSet_Empty_KeepsLanguage()`

An empty language choice leaves the chosen language as it was.

## `public void SpeechSet_Typed_WritesSpeeches()`

Typed text lands trimmed as a pending part of speech once the deferred request is flushed.
The answer says the text names a part of speech.

## `public void IpaSet_Typed_WritesPrimaryIpa()`

A typed phonetic reading lands on the primary pronunciation.

## `public void RespellingSet_Typed_WritesPrimaryRespelling()`

A typed respelling lands on the primary pronunciation.

## `private static LTenure TTenureFieldStart(LEngine engine)`

Sets no delay and starts a tenure on a fresh entry.
