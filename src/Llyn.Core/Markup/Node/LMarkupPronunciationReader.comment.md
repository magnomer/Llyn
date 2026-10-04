# LMarkupPronunciationReader.cs
Hash: `f10ade4a00de35ba`

## `internal static class LMarkupPronunciationReader`

Turns the sound elements of one entry into drafts.
It reads `pronunciation`, `transcription` and `reflex` elements and the syllables under a pronunciation.
Each parser walks its children in file order like `LMarkupReader`, and an unknown name becomes an omission.

## `internal static LPronunciationDraft LMarkupPronunciationParse(LMarkupNode element, List<LMarkupOmission> omissions)`

Reads one `pronunciation` element, audio and source kept verbatim.
A `respelling` child fills the draft's respelling, and a file without one leaves it blank.

## `private static LSyllable LMarkupSyllableParse(LMarkupNode element, int position, List<LMarkupOmission> omissions)`

Reads one `syllable` element at the given position under a pronunciation with no id.

## `internal static LTranscriptionDraft LMarkupTranscriptionParse(LMarkupNode element, List<LMarkupOmission> omissions)`

Reads one `transcription` element as a scheme and its text.

## `internal static LReflexDraft LMarkupReflexParse(LMarkupNode element, List<LMarkupOmission> omissions)`

Reads one `reflex` element as a language, kind, text, respelling, romanization, meaning, note and region.
An empty `owned` element marks a user-entered meaning and an empty `main` element marks the common reading.
