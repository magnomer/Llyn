# TVaultEntry.cs

## `public sealed class TVaultEntry`

Covers `LEntryArchive` keeping the promise of `LEntryVault` on a real workspace database.
The test sees only the port, so it proves what the engine may rely on.
An entry read after it was created comes back with the id and fields it was stored under.
The whole content loaded for an entry agrees with the row read for it and carries its forms and speeches.
A find returns only the entries whose headword matches, and every entry for an empty query.
An unknown id reads as `null`.
