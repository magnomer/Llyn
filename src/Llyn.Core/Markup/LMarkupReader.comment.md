# LMarkupReader.cs

## `internal static class LMarkupReader`

Turns the entry-level elements of a markup document into records.
Every element parser walks its children in file order and hands each name it knows to a field.
A name it does not know becomes an omission and its subtree is never entered.
Card-level elements are read by `LMarkupCardReader`.
The tree it walks is `LMarkupNode`, which `LMarkupFile` in Infrastructure reads from the XML.

## `internal static void LMarkupAttributeScan(LMarkupNode element, List<LMarkupOmission> omissions)`

Walks `element` and everything under it once and reports every attribute except `state="unknown"` as an omission.
An `id` attribute is the expected case, since the format never carries ids.
A `state` with any other value is reported too.
The reader would otherwise read the text as specified in silence.

## `internal static void LMarkupOmissionAdd(List<LMarkupOmission> omissions, LMarkupNode element)`

Records that `element` was skipped, naming it by its tag and line.

## `private static void LMarkupOmissionAdd(List<LMarkupOmission> omissions, int line, string text)`

Adds one omission while the list is under `LMarkup.LMarkupOmissionCeiling`.
The first one past it is written as `...` and the rest are dropped.
A file of a million unknown attributes would otherwise be reported one row at a time.

## `internal static int? LMarkupNumberParse(LMarkupNode element)`

Reads an integer field, nothing when the text is not a number.
Each caller picks the value that marks the field as unreadable.
A bad offset is therefore dropped and not moved to zero.

## `internal static LMarkupEntry LMarkupEntryParse(LMarkupNode element, List<LMarkupOmission> omissions)`

Reads one `entry` element with all of its repeated children in file order.
The line the element opens on travels with the record, so the import can place its own omissions.

## `private static LForm LMarkupFormParse(LMarkupNode element, int position, List<LMarkupOmission> omissions)`

Reads one `form` element as a form with no entry id and the given position.

## `private static LMarkupInflection LMarkupInflectionParse(LMarkupNode element, List<LMarkupOmission> omissions)`

Reads one `inflection` element, its speech and morphologies by name.

## `private static LPronunciationDraft LMarkupPronunciationParse(LMarkupNode element, List<LMarkupOmission> omissions)`

Reads one `pronunciation` element, audio and source kept verbatim.
A `respelling` child fills the draft's respelling, and a file without one leaves it blank.

## `private static LSyllable LMarkupSyllableParse(LMarkupNode element, int position, List<LMarkupOmission> omissions)`

Reads one `syllable` element at the given position under a pronunciation with no id.

## `private static LTranscriptionDraft LMarkupTranscriptionParse(LMarkupNode element, List<LMarkupOmission> omissions)`

Reads one `transcription` element as a scheme and its text.

## `private static LReflexDraft LMarkupReflexParse(LMarkupNode element, List<LMarkupOmission> omissions)`

Reads one `reflex` element as a language, a kind, its text, its note, its region and its remark.
An empty `main` element marks it.
