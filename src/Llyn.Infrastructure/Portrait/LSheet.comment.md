# LSheet.cs

## `public static class LSheet`

Renders a page likeness as one standalone HTML page.
An entry, an example, a source and a situation all print in this one dress.
It is the fidelity anchor of the whole export.
The PDF is this page printed, so anything fixed here is fixed in both.

## `public static string LSheetFormat(LPortraitPage page, LTheme theme)`

The style sheet is inlined, and pictures are embedded, so the file stands alone once moved.
The title and crest come first, then the sections in the order the likeness carries them.
Nothing is decided here: an absent section was already left out by whoever built the likeness.

## `public static string LSheetNormalize(string? text)`

Every value written into the page passes through here.
Entry text is written by the reader and may hold anything, including markup.

## `private static void LSheetCrestAppend(StringBuilder sheet, LPortraitPage page)`

The title, language chip and star form the masthead, as one block above the sections.
Each reading line follows as a sound row, its label muted before its text.
The line's open and close marks are drawn around the text in their own muted style.
The further chips share one row beneath, and no row is written when there are none.

## `private static void LSheetMarkAppend(StringBuilder sheet, string mark)`

One bracket mark in its muted style, or nothing when the line carries none.
