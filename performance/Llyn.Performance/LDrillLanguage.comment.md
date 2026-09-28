# LDrillLanguage.cs

## `internal sealed class LDrillLanguage : LDrill`

Loads every language pack beside the binary, as the engine does when it binds a workspace.

## `private IReadOnlyList<string> _lDrillLanguageNames = [];`

The pack names, scanned once in preparation.
So a cycle measures loading the packs, not finding them.
