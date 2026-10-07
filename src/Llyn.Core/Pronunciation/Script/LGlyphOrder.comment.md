# LGlyphOrder.cs
Hash: `9adea187dbce619b`

## `public static class LGlyphOrder`

The one declared rule ordering lists of characters for every view.
Characters list by Unicode code point, so storage and fetch order never decide.

## `public static Comparer<string> LGlyphOrderComparer { get; }`

Compares two texts rune by rune on the code point value.
UTF-16 ordinal order would put ideographs beyond the basic plane before the compatibility block.
Walking runes keeps a surrogate pair whole and sorts it by its true value.
A text that is a prefix of the other sorts first.
A caller holding rows sorts by this comparer first and by the stored id last.

## `public static IReadOnlyList<string> LGlyphOrderSort(IReadOnlyList<string> characters)`

The characters in code point order.
The sort is stable, so equal texts keep the order they arrived in.
