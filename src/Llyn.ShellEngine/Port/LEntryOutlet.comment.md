# LEntryOutlet.cs
Hash: `b9f8632835a3fad0`

## `public sealed class LEntryOutlet : LEntryPort`

The engine's face for `LEntryPort`, handed to the deportment in place of the engine itself.
Every member forwards to the same-named member of the engine part that owns it and holds no state or logic.
A forward can later target a facade instead of the engine without changing the port.

## `public LEntryOutlet(LEngine engine)`

Rejects a null engine and keeps it for every forward.
