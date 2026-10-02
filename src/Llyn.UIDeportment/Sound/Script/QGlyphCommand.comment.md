# QGlyphCommand.cs

## `public static class QGlyphCommand`

The routed commands the glyph row and its character chips raise, bound by the editor and the display.
Each carries the row or the character as its parameter, because the templates have no code of their own.

## `public static RoutedCommand QGlyphCommandNotation { get; }`

Opens the lookup menu for the glyph row, searching in the glyph scheme.

## `public static RoutedCommand QGlyphCommandEntry { get; }`

Opens the entry one character chip of the reading view stands for, making it first when none exists.
