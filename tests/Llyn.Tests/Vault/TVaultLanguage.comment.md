# TVaultLanguage.cs

## `public sealed class TVaultLanguage`

Covers `LLanguageLoader` keeping the promise of `LLanguageVault` on the packs the test build copies beside it.
The test sees only the port, so it proves what the engine may rely on.
A shipped pack reads back under its name with its sources.
The scan lists English first and the other listed packs after it.
An unknown pack reads as a blank pack of that name.
A name that would escape the folder is refused.
