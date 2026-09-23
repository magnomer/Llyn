# LPortraitOutlet.cs

## `public sealed class LPortraitOutlet : LPortraitPort`

The engine's face for `LPortraitPort`, handed to the deportment in place of the engine itself.
Every member forwards to the `LEngine` member of the same name and holds no state or logic.
A forward can later target a facade instead of the engine without changing the port.

## `public LPortraitOutlet(LEngine engine)`

Rejects a null engine and keeps it for every forward.
