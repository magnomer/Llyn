# LUnitKey.cs
Hash: `82b5114c87f826cb`

## `public static class LUnitKey`

The text key of each lexical unit, shared by markup and language packs, and its localization key.
One map keeps a unit written by an export readable by the import and by a pack.

## `public static LUnit LUnitKeyParse(string? key)`

Reads a key regardless of case and surrounding space.
An unknown or missing key reads as `LUnitEmpty`, so a foreign file never fails on it.
The retired keys `content` and `function` read as unknown, so an older file leaves the unit unchosen.

## `public static string LUnitKeyRead(LUnit unit)`

The localization key that names a unit, empty for `LUnitEmpty`.
The editor, the reading view and the exports all name a unit through it, so they never disagree.

## `public static string LUnitKeyFormat(LUnit unit)`

Writes the key of a unit, empty for `LUnitEmpty`.
