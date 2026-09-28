# QGlyphItem.cs

## `public sealed record QGlyphItem(string QGlyphItemText, string QGlyphItemLanguage, bool QGlyphItemLinked)`

One character chip of the reading view's glyph row.
The language is the one the character's entry lives in.
The linked verdict is the engine's, and it is false for a rune that is not Han.
An unlinked chip is inert, so punctuation is drawn like the editor field draws it but opens nothing.

## `internal static void QGlyphItemRefine(FrameworkElement container, object item, string? _)`

Fills one chip of `Theme.Glyph.Display` with its character and its item as the command parameter.
An unlinked chip is made inert: no hit test, no hand cursor, no tooltip.
