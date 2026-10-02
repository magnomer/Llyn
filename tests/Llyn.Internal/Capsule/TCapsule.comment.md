# TCapsule.cs

## `public sealed class TCapsule`

Covers Deportment's Capsule file under a real workspace folder, with no window.
A saved state reads back whole, and no pending file is left beside it.
No file, junk text or a JSON array reads as the default state, so the window always opens.
A null or blank root reads the default state and saves nothing, since it names no file.
