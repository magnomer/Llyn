# LGlyphItem.cs

## `public sealed record LGlyphItem(string LGlyphItemText, string LGlyphItemLanguage)`

One character chip of the reading view's glyph row.
The language is the one the character's entry lives in.
It is blank for a rune that is not Han.
A blank language makes the chip inert, so punctuation is drawn like the editor field draws it but opens nothing.

## `internal static void LGlyphItemApply(FrameworkElement container, object item, string? _)`

Fills one chip of `Theme.Glyph.Display` with its character and its item as the command parameter.
A chip with no language is made inert: no hit test, no hand cursor, no tooltip.
