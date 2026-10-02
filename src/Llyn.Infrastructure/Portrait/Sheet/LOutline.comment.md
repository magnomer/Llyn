# LOutline.cs
Hash: `04c1fe57bee401a5`

## `public static class LOutline`

Renders a page likeness as Markdown.
Markdown carries no theme, so only the reading order and the emphasis survive.

## `public static string LOutlineFormat(LPortraitPage page)`

The title heads the document, with the star after it when the page is a favourite.
The language, the reading lines and the chips share one crest line, because Markdown has no chips.
Each reading line writes its label in italics and its text in bold, the marks inside the bold and unescaped.
The sections follow in the order the likeness carries them, through `LOutlineSection`.

## `public static string LOutlineNormalize(string? text)`

Entry text may hold any character, and a stray hash or asterisk would silently restructure the document.
Every markdown special character gets a backslash rather than being stripped, so nothing the reader wrote is lost.
