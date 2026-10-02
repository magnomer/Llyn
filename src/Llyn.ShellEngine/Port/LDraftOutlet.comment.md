# LDraftOutlet.cs
Hash: `66146c7a340ecf63`

## `public sealed class LDraftOutlet : LDraftPort`

The engine's face for `LDraftPort`, handed to the deportment in place of the engine itself.
Every member forwards to the same-named member of the engine part that owns it and holds no state or logic.
A forward can later target a facade instead of the engine without changing the port.

## `public LDraftOutlet(LEngine engine)`

Rejects a null engine and keeps it for every forward.
