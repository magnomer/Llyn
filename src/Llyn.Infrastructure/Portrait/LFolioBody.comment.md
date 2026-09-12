# LFolioBody.cs

## `public static class LFolioBody`

Builds the document body of a Word export.

## `public static string LFolioBodyFormat(LPortrait portrait, LTheme theme, List<LPortraitAsset> plates)`

The plate list is filled while the body is written, and the caller packs what it collects.
The drawing namespaces are declared once on the root, because every picture run uses them.
The page is A4 with even margins, matching the printed PDF.
The note is Markdown, so `LFolioNote` writes its blocks after the note band.

## `private static string LFolioReadingFormat(LPortraitReading reading, string open, string close)`

One pronunciation or transcription as a sound-line mark: the label, then the text between the given marks.
A pronunciation is bracketed and a transcription is not, so a reader tells IPA from a scheme at a glance.

## `private static void LFolioBodyAppend(StringBuilder body, string heading, IReadOnlyList<LPortraitCard> cards, List<LPortraitAsset> plates, LTheme theme)`

An empty section is skipped, as it is on screen.
