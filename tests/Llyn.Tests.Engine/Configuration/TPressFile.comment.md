# TPressFile.cs
Hash: `82f2578d9e5e9c1d`

## `public sealed class TPressFile`

Covers the print setting adapter over a keep file.
No file reads as nothing, and a saved setting reads back equal under `press.json`.
Unreadable, blank or missing text reads as nothing and never throws.
