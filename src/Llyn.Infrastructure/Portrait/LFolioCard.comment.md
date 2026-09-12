# LFolioCard.cs

## `public static class LFolioCard`

Renders one card as a bordered Word table.
A table is the nearest thing the format has to the panel's card, with a shaded header row.

## `public static void LFolioCardAppend(StringBuilder body, LPortraitCard card, List<LPortraitAsset> plates, LTheme theme)`

Borders and shading take the display's own line and accent colours.
Chips have no counterpart, so each run becomes a separated line in its own style.
A picture that cannot be read is written as its address instead of a broken frame.
A video becomes a marked line, because a document plays nothing.
An empty paragraph follows the table, which is how Word keeps two tables apart.

## `private static (long Across, long Down) LFolioCardClamp(int width, int height)`

Word sizes a picture in the document rather than by its own pixels.
A picture wider than the text column is scaled down with its proportions kept.
An unknown size falls back to a plain widescreen frame.
