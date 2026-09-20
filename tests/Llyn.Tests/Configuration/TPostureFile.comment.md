# TPostureFile.cs

## `public sealed class TPostureFile`

Covers the posture adapter over a keep file.
No file reads as nothing and a saved state reads back equal under `posture.json`.
The legacy `settings` key reads the same shape.
A file the system will not open surfaces as `LVaultFault`, the one exception the engine catches.
