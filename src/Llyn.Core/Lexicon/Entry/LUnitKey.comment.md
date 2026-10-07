# LUnitKey.cs
Hash: `5d44a8e2049ffcb0`

## `public static class LUnitKey`

The text key of each lexical unit, shared by markup and language packs, and its localization key.
One map keeps a unit written by an export readable by the import and by a pack.

## `public static LUnit LUnitKeyParse(string? key)`

Reads a key regardless of case and surrounding space.
An unknown or missing key reads as `LUnitEmpty`, so a foreign file never fails on it.

## `public static string LUnitKeyRead(LUnit unit)`

The localization key that names a unit, empty for `LUnitEmpty`.
The editor, the reading view and the exports all name a unit through it, so they never disagree.

## `public static string LUnitKeyFormat(LUnit unit)`

Writes the key of a unit, empty for `LUnitEmpty`.
