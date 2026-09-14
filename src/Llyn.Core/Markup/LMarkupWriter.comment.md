# LMarkupWriter.cs

## `internal static class LMarkupWriter`

Turns entry-level records into markup elements.
Children are written in the order the format table lists them, repeated rows in list order.
An empty text field writes nothing, so a minimal entry stays short.
Card-level records are written by `LMarkupCardWriter`.

## `internal static XElement LMarkupEntryFormat(LMarkupEntry entry)`

Writes one `entry` element with every row the record holds.

## `private static XElement LMarkupFormFormat(LForm form)`

Writes one `form` element with its text, local spelling and role.

## `private static void LMarkupLocalFormat(XElement parent, string? local)`

Writes a `local` element whenever the spelling is stated, even as empty text.
A stored empty spelling would otherwise come back as none, and the next import would count that as a change.

## `private static XElement LMarkupInflectionFormat(LMarkupInflection inflection)`

Writes one `inflection` element with its speech and morphologies by name.

## `private static XElement LMarkupPronunciationFormat(LPronunciationDraft pronunciation)`

Writes one `pronunciation` element, audio and source verbatim and never copied.

## `private static XElement LMarkupSyllableFormat(LSyllable syllable)`

Writes one `syllable` element, the tone number in invariant digits.

## `private static XElement LMarkupTranscriptionFormat(LTranscriptionDraft transcription)`

Writes one `transcription` element with its scheme and text.
