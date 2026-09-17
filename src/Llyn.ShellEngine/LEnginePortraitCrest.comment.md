# LEnginePortraitCrest.cs

## `public sealed partial class LEngine`

The sections of an entry likeness that describe the headword itself.
Each builder adds one section to the list, or nothing when the entry has nothing to show.
Every read is defensive, because none of these is worth refusing an export over.

## `private void LEngineGlyphAdd(List<LPortraitSection> sections, LEntryDraft draft, LPortraitLabel label)`

One chip per character of the headword, as the panel's glyph row shows.
The transcription named after the glyph pack stands in for the headword when it is written.
A language with no glyph pack adds nothing.

## `private void LEngineFrequencyAdd(List<LPortraitSection> sections, long entryId, LPortraitLabel label)`

The band as a chip and one line per source, each carrying its raw figure and unit.

## `private static void LEngineFormAdd(List<LPortraitSection> sections, LEntryDraft draft, LPortraitLabel label)`

One line per written form, labelled by its role, with the local spelling after the text.

## `private static string LEngineLocalShow(string text, string? local)`

The text alone, or the text and its local spelling parted by a space.
Forms and paradigm slots print through it alike.

## `private void LEngineParadigmAdd(List<LPortraitSection> sections, long entryId, LPortraitLabel label)`

One line per shown slot, labelled by the morphology name.
The inflection prints as a form does, its local spelling after the text.
The part of speech joins the label when the entry inflects under more than one.
An unknown slot shows the mark, and an unspecified one is skipped.

## `private static void LEngineFanqieAdd(List<LPortraitSection> sections, IReadOnlyList<LFanqieRow> rows, LPortraitLabel label)`

One line per rime book row, labelled by the book.
The character joins the label when the headword has more than one.
The text carries the spelling or its parts, the heading, division, tone, spelling and reading.

## `private static void LEnginePartAdd(List<string> parts, string text, string open = "", string close = "")`

Adds one part between its marks, skipping an empty one.

## `private void LEngineScriptAdd(List<LPortraitSection> sections, long entryId, string language, LPortraitLabel label)`

One picture plate per fetched character form, each carrying its caption.
The bytes travel as a data address, so every writer embeds them.
