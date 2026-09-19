# TVaultSettings.cs

## `public sealed class TVaultSettings`

Covers `LSettingsLoader` keeping the promise of `LSettingsVault` on a real workspace folder.
The test sees only the port, so it proves what the engine may rely on.
Settings saved read back field for field, and the workspace then reports that it holds its own.
A workspace holding nothing answers the defaults and reports no settings of its own.
