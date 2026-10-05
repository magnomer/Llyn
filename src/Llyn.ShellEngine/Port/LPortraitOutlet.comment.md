# LPortraitOutlet.cs
Hash: `0dc69b6e886a953d`

## `public sealed class LPortraitOutlet : LPortraitPort`

The engine's face for `LPortraitPort`, handed to the deportment in place of the engine itself.
Every member forwards to the same-named member of the engine part that owns it and holds no state or logic.
A forward can later target a facade instead of the engine without changing the port.

## `public LPortraitOutlet(LEngine engine)`

Rejects a null engine and keeps it for every forward.
