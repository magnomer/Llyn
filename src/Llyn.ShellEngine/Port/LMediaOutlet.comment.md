# LMediaOutlet.cs
Hash: `cc595890330f42a2`

## `public sealed class LMediaOutlet : LMediaPort`

The engine's face for `LMediaPort`, handed to the deportment in place of the engine itself.
Every member forwards to the same-named member of the engine part that owns it and holds no state or logic.
A forward can later target a facade instead of the engine without changing the port.

## `public LMediaOutlet(LEngine engine)`

Rejects a null engine and keeps it for every forward.
