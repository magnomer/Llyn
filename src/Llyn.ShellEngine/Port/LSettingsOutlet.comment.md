# LSettingsOutlet.cs
Hash: `ac8dada68d0b7cf3`

## `public sealed class LSettingsOutlet : LSettingsPort`

The engine's face for `LSettingsPort`, handed to the deportment in place of the engine itself.
Every member forwards to the same-named member of the engine part that owns it and holds no state or logic.
A forward can later target a facade instead of the engine without changing the port.

## `public LSettingsOutlet(LEngine engine)`

Rejects a null engine and keeps it for every forward.

## `public event Action? LEngineFoldChanged`

Forwards each handler to the engine's settings facade, so the notice is the engine's, not the outlet's.
Two outlets over one engine therefore hear the same fold change.
