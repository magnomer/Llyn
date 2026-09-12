# LSheet.cs

## `public static class LSheet`

Renders a portrait as one standalone HTML page.
It is the fidelity anchor of the whole export.
The PDF is this page printed, so anything fixed here is fixed in both.

## `public static string LSheetFormat(LPortrait portrait, LTheme theme)`

The style sheet is inlined, and pictures are embedded, so the file stands alone once moved.
The section order is the display panel's own order.

## `public static string LSheetNormalize(string? text)`

Every value written into the page passes through here.
Entry text is written by the reader and may hold anything, including markup.

## `private static void LSheetCrestAppend(StringBuilder page, LPortrait portrait)`

The headword, language, star, pronunciations, transcriptions and speech chips form the panel's masthead.
They are written together because the display draws them as one block above the sections.
Each pronunciation and transcription takes a sound line of its own, in stored order.
An empty pronunciation list, transcription list or speech list draws nothing, as on screen.

## `private static void LSheetReadingAppend(StringBuilder page, LPortraitReading reading)`

The label of a reading, the variety or the scheme, written muted before its text.
A reading with no label writes nothing here.

## `private static void LSheetBandAppend(StringBuilder page, string heading, IReadOnlyList<LPortraitCard> cards)`

An empty section is not drawn at all, matching the panel's collapsed sections.

## `private static void LSheetIncomingAppend(StringBuilder page, LPortrait portrait)`

The rows carry no link, because an exported page cannot open another entry.

## `private static void LSheetNoteAppend(StringBuilder page, LPortrait portrait)`

The note is Markdown, so `LSheetNote` renders its blocks inside the note box.
