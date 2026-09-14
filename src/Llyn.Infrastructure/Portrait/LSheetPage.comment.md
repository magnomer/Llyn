# LSheetPage.cs

## `public static class LSheetPage`

Renders a page likeness as one standalone HTML page.
It shares the entry sheet's style, so an example, a source or a situation prints in the same dress.

## `public static string LSheetPageFormat(LPortraitPage page, LTheme theme)`

The title, the chip row, then the sections in the order the likeness carries them.
Nothing is decided here: an absent section was already left out by whoever built the likeness.

## `private static void LSheetCrestAppend(StringBuilder sheet, LPortraitPage page)`

The title as the head of the page, and the language and further chips on one row beneath it.
No chip row is written when the page has neither a language nor a chip.

## `private static void LSheetSectionAppend(StringBuilder sheet, LPortraitSection section)`

One headed band: its lines, then its note as Markdown blocks, then its plates.
A line's label is a small tag before the text, which is how a gloss shows its language.
