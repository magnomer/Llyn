# LPortraitClerkCrest.cs

## `public static class LPortraitClerkCrest`

The head sections of the entry page: glyph chips, frequency, forms, paradigm, fanqie and script plates.
Each takes the rows already read, so the composition stays pure.

## `public static void LPortraitGlyphAdd(List<LPortraitSection> sections, LEntryDraft draft, LGlyph? glyph, LPortraitLabel label)`

One chip per rune of the glyph transcription, or of the headword when no transcription is written.

## `public static void LPortraitFrequencyAdd(List<LPortraitSection> sections, IReadOnlyList<LFrequency> rows, LPortraitLabel label)`

One line per source with its figure and unit, the first band as a chip.

## `public static void LPortraitFormAdd(List<LPortraitSection> sections, LEntryDraft draft, LPortraitLabel label)`

One line per non-empty form, its local text beside it.

## `public static void LPortraitParadigmAdd(List<LPortraitSection> sections, IReadOnlyList<LParadigmSlot> slots, LPortraitLabel label)`

One line per specified or unknown slot, tagged by speech and morphology when more than one speech shows.

## `public static void LPortraitFanqieAdd(List<LPortraitSection> sections, IReadOnlyList<LFanqieRow> rows, LPortraitLabel label)`

One line per row with its parts bracketed, tagged by character and book when more than one character shows.

## `public static void LPortraitScriptAdd(List<LPortraitSection> sections, IReadOnlyList<LScriptImage> images, LPortraitLabel label)`

One plate per script image.

## `private static string LPortraitLocalShow(string text, string? local)`

The text followed by its local form when there is one.

## `private static void LPortraitPartAdd(List<string> parts, string text, string open = "", string close = "")`

A non-empty part wrapped in its brackets.
