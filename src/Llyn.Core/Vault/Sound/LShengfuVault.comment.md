# LShengfuVault.cs

## `public interface LShengfuVault`

The persistence port for the phonetic series the engine reads and writes.
`LShengfuArchive` in Infrastructure is its adapter over the workspace database.

## `void LShengfuSave(string language, LShengfu row);`

Makes the row the stored series of its character, rewriting whatever was kept before.

## `LShengfu? LShengfuRead(string language, string character);`

The stored series of the character, or null while it was never fetched.

## `IReadOnlyList<LShengfu> LShengfuScan(string language, IReadOnlyList<string> characters);`

The stored series of the given characters, skipping every character that has none.
