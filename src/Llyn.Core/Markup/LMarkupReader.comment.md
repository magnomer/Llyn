# LMarkupReader.cs

## `internal static class LMarkupReader`

Turns the entry-level elements of a markup document into records.
Every element parser walks its children in file order and hands each name it knows to a field.
A name it does not know becomes an omission and its subtree is never entered.
Card-level elements are read by `LMarkupCardReader`.

## `internal static void LMarkupDepthValidate(string text)`

Refuses with `LRefusal.LRefusalMarkup` when any element sits deeper than `LMarkup.LMarkupDepthCeiling`, the root counted as one.
Every parser below recurses on nested elements, and so does reading an element's text.
A file nested a hundred thousand deep would end the process, since a stack overflow cannot be caught.
The text is streamed rather than read as a tree, so the check stops at the first node too deep.
The tree reader itself survives deep nesting but takes minutes over it, which is why the stream runs first.
A legitimate file is read twice, and the stream pass is the cheaper of the two.

## `internal static void LMarkupAttributeScan(XElement root, List<LMarkupOmission> omissions)`

Walks the whole tree once and reports every attribute except `state="unknown"` as an omission.
An `id` attribute is the expected case, since the format never carries ids.
A `state` with any other value is reported too.
The reader would otherwise read the text as specified in silence.

## `internal static void LMarkupOmissionAdd(List<LMarkupOmission> omissions, XElement element)`

Records that `element` was skipped, naming it by its tag and line.

## `private static void LMarkupOmissionAdd(List<LMarkupOmission> omissions, int line, string text)`

Adds one omission while the list is under `LMarkup.LMarkupOmissionCeiling`.
The first one past it is written as `...` and the rest are dropped.
A file of a million unknown attributes would otherwise be reported one row at a time.

## `internal static int LMarkupLineRead(XElement element)`

The line the element opens on, or zero when the document carried no line info.

## `internal static int? LMarkupNumberParse(XElement element)`

Reads an integer field, nothing when the text is not a number.
Each caller picks the value that marks the field as unreadable.
A bad offset is therefore dropped and not moved to zero.

## `internal static LMarkupEntry LMarkupEntryParse(XElement element, List<LMarkupOmission> omissions)`

Reads one `entry` element with all of its repeated children in file order.
The line the element opens on travels with the record, so the import can place its own omissions.

## `private static LForm LMarkupFormParse(XElement element, int position, List<LMarkupOmission> omissions)`

Reads one `form` element as a form with no entry id and the given position.

## `private static LMarkupInflection LMarkupInflectionParse(XElement element, List<LMarkupOmission> omissions)`

Reads one `inflection` element, its speech and morphologies by name.

## `private static LPronunciationDraft LMarkupPronunciationParse(XElement element, List<LMarkupOmission> omissions)`

Reads one `pronunciation` element, audio and source kept verbatim.
A `respelling` child fills the draft's respelling, and a file without one leaves it blank.

## `private static LSyllable LMarkupSyllableParse(XElement element, int position, List<LMarkupOmission> omissions)`

Reads one `syllable` element at the given position under a pronunciation with no id.

## `private static LTranscriptionDraft LMarkupTranscriptionParse(XElement element, List<LMarkupOmission> omissions)`

Reads one `transcription` element as a scheme and its text.

## `private static LReflexDraft LMarkupReflexParse(XElement element, List<LMarkupOmission> omissions)`

Reads one `reflex` element as a language, a kind, its text, its note and whether a `main` element marks it.
