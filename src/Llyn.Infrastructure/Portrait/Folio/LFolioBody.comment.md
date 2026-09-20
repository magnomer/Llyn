# LFolioBody.cs

## `public static class LFolioBody`

Builds the document body of a Word export.

## `public static string LFolioBodyFormat(LPortraitPage page, LTheme theme, List<LPortraitAsset> plates)`

The plate list is filled while the body is written, and the caller packs what it collects.
The drawing namespaces are declared once on the root, because every picture run uses them.
The title heads the document, with the star after it when the page is a favourite.
The language, the reading lines and the chips share one sound line, each reading led by its label.
A reading's open and close marks are written plainly around its text.
The sections follow in the order the likeness carries them, through `LFolioSection`.
The page is A4 with even margins, matching the printed PDF.
