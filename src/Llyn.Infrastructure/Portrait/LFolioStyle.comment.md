# LFolioStyle.cs

## `public static class LFolioStyle`

Builds the style part of a Word export from the display's theme.

## `public static string LFolioStyleRead(LTheme theme)`

Each display role becomes one named style, so the body carries no inline formatting.
Sizes are the panel's own, converted from device-independent pixels to half-points.

## `public static string LFolioStyleNormalize(string color)`

The format takes six hex digits with no leading hash and no alpha.
An unreadable colour falls back to black rather than breaking the part.

## `private static void LFolioStyleAppend(StringBuilder styles, string id, string name, int size, bool bold, string color, string? family, int after)`

Spacing after a paragraph carries the panel's vertical rhythm, which the format has no margins for.
