# QContourInk.cs
Hash: `89725f62a4ae583b`

## `public static class QContourInk`

Turns Conduct's ready contour into the contour box's own items.
The Q drivers `QLecternAccent` and `QEditorSound` call it, so the box names no Conduct type.

## `public static IReadOnlyList<QContourItem> QContourInkBuild(IReadOnlyList<CContour> syllables, FrameworkElement scope)`

Copies each syllable's text, levels and toned flag into a `QContourItem`.
Each level's theme brush key, chosen in Conduct, is looked up as a brush from `scope`.
A brush missing from the theme falls back to gray, as the box's own brushes do.
