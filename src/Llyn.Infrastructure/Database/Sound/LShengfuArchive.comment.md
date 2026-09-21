# LShengfuArchive.cs

## `public sealed class LShengfuArchive : LShengfuVault`

The workspace-database adapter of the phonetic-series store, one row per language and character.

## `public LShengfuArchive(LDatabase database)`

Holds the database the sessions are opened on.

## `public void LShengfuSave(string language, LShengfu row)`

Writes the series of the character, replacing the text and source already kept under its key.

## `public LShengfu? LShengfuRead(string language, string character)`

The stored series of the character, or null while nothing was stored for it.

## `public IReadOnlyList<LShengfu> LShengfuScan(string language, IReadOnlyList<string> characters)`

Reads the characters in the order given and drops the ones with nothing stored.
