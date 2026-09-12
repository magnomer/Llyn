# LOutline.cs

## `public static class LOutline`

Renders a portrait as Markdown.
Markdown carries no theme, so only the reading order and the emphasis survive.

## `public static string LOutlineFormat(LPortrait portrait)`

The headings match the panel's sections, so the document reads in the same order.
The crest facts share one line, because Markdown has no chips.
The note is Markdown already, so `LOutlineNote` carries it over unescaped.

## `public static string LOutlineNormalize(string? text)`

Entry text may hold any character, and a stray hash or asterisk would silently restructure the document.
Every control character is escaped rather than stripped, so nothing the reader wrote is lost.

## `private static string LOutlineReadingFormat(LPortraitReading reading, string open, string close)`

One pronunciation or transcription as a crest run: the label in italics, then the text between the given marks.
A pronunciation is bracketed and a transcription is not, so a reader tells IPA from a scheme at a glance.

## `private static void LOutlineBandAppend(StringBuilder page, string heading, IReadOnlyList<LPortraitCard> cards)`

An empty section is skipped, as it is on screen.

## `private static void LOutlineCardAppend(StringBuilder page, LPortraitCard card)`

The position and title form the card heading, so a reader can cite a card by number.
Chips become separated runs, since Markdown offers nothing closer.
