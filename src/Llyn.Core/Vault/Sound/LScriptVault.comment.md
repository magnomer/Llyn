# LScriptVault.cs

## `public interface LScriptVault`

The persistence port for the Script rows the engine reads and writes.
It lists exactly what the engine asks of script storage, and nothing about how rows are kept.
`LScriptArchive` in Infrastructure is its adapter over the workspace database.

## `void LScriptSave(string language, string character, IReadOnlyList<LScriptImage> images);`

Replaces whatever the character held with the given pictures, in one transaction.
The styles arrive in pack order and the positions in source order, and the row ids keep that order.

## `IReadOnlyList<LScriptImage> LScriptRead(string language, string character);`

Every stored picture of the character, in insertion order, with its bytes.
An empty list means the character was never fetched, or nothing was found and nothing stored.
