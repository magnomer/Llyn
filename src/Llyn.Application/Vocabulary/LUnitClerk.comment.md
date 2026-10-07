# LUnitClerk.cs
Hash: `19dc027c56585773`

## `public static class LUnitClerk`

The rules of an entry's lexical unit against its language.
The language's spacing flag alone decides which units are offered.

## `public static IReadOnlyList<LUnit> LUnitScan(bool spaced)`

The units a language offers, in the order the dropdown shows them.
A spaced language offers Content, Function and Morpheme.
Any other offers Word and Morpheme.

## `public static string LUnitFormat(LUnit unit)`

The localization key that names a unit, read from `LUnitKey` for the engine.
The engine names no Core rule itself, so it reaches the key through here.

## `public static LUnit LUnitSettle(LUnit unit, bool spaced)`

The unit kept when an entry moves to a language with the given flag.
A content or function word becomes a word, since a word is what an unspaced language still offers.
A word in a spaced language cannot tell content from function, so it falls back to unchosen.
A morpheme stays a morpheme everywhere.
