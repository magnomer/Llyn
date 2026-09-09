# LSheetPlate.cs

## `public static class LSheetPlate`

Renders the picture and video plates of one card.

## `public static void LSheetPlateAppend(StringBuilder page, LPortraitCard card)`

A local picture is embedded, so the page keeps its images after it is moved or mailed.
A picture that cannot be read keeps its address, which at least says what was meant.
A video cannot play in a document, so it becomes a poster frame over a link.
A hosted video takes the host's own thumbnail, and any other takes a plain plate.
