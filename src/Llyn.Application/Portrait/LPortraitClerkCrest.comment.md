# LPortraitClerkCrest.cs
Hash: `e60a2a1605f5294c`

## `public static class LPortraitClerkCrest`

Composes glyph, frequency, form, paradigm, fanqie, and script sections from already-read data.
Methods append to the supplied section list without reading storage.

## `public static void LPortraitGlyphAdd(List<LPortraitSection> sections, LEntryDraft draft, LGlyph? glyph, LPortraitLabel label)`

A null glyph suppresses the section rather than falling back to headword chips.
Otherwise, the first nonempty transcription matching the glyph scheme wins, with the headword as fallback.
Rune-based chips keep supplementary characters intact.

## `public static void LPortraitFrequencyAdd(List<LPortraitSection> sections, IReadOnlyList<LFrequency> rows, LPortraitLabel label)`

The first non-null band supplies the only chip, even when that band is empty.
Every supplied row retains its source and raw figure, with the unit prefixed when present.

## `public static void LPortraitFormAdd(List<LPortraitSection> sections, LEntryDraft draft, LPortraitLabel label)`

One line per non-empty form, its local text beside it.

## `public static void LPortraitParadigmAdd(List<LPortraitSection> sections, IReadOnlyList<LParadigmSlot> slots, LPortraitLabel label)`

Only specified inflections and localized unknown labels can produce lines, and empty rendered text is omitted.
Speech prefixes depend on distinct speech IDs across all supplied slots, including omitted ones.
Multi-value slot names retain every value rather than only the first morphology.

## `public static void LPortraitFanqieAdd(List<LPortraitSection> sections, IReadOnlyList<LFanqieRow> rows, LPortraitLabel label)`

Initial and rime replace raw text when either exists.
Only headings receive square brackets, while readings receive slashes.
Character prefixes disambiguate books when supplied rows contain multiple characters.

## `public static void LPortraitScriptAdd(List<LPortraitSection> sections, IReadOnlyList<LScriptImage> images, LPortraitLabel label)`

One plate per script image.

## `private static string LPortraitLocalShow(string text, string? local)`

The text followed by its local form when there is one.

## `private static void LPortraitPartAdd(List<string> parts, string text, string open = "", string close = "")`

A non-empty part wrapped in its brackets.
