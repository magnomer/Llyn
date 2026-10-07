# LScriptArchive.cs
Hash: `db0edbbcbeffce1d`

## `public sealed class LScriptArchive : LScriptVault`

Stores and reads the glyph pictures of a character, per language.
A character's pictures are written as one set and read back in the order they were written.
That order is storage order, so `LScriptImage.LScriptImageSort` decides what a view shows.

## `public void LScriptSave(string language, string character, IReadOnlyList<LScriptImage> images)`

Replaces whatever the character held with the given pictures, in one transaction.
The styles arrive in pack order and the positions in source order, and the row ids keep that order.

## `public IReadOnlyList<LScriptImage> LScriptRead(string language, string character)`

Every stored picture of the character in insertion order, with its bytes, age code and row id.
The row id is carried only as the last tie-break of `LScriptImage.LScriptImageSort`.
An empty list means the character was never fetched, or nothing was found and nothing stored.
