# LUnitClerk.cs
Hash: `338cd4c7dbfba260`

## `public static class LUnitClerk`

The units an entry is offered and the key that names each.
Every language offers the same units, because the user alone decides which one fits.

## `private static readonly LUnit[] LUnitClerkOffered`

Morpheme, Word and Expression, in the order the dropdown shows them.

## `public static IReadOnlyList<LUnit> LUnitScan()`

The units offered to every entry, in the order the dropdown shows them.

## `public static string LUnitFormat(LUnit unit)`

The localization key that names a unit, read from `LUnitKey` for the engine.
The engine names no Core rule itself, so it reaches the key through here.
