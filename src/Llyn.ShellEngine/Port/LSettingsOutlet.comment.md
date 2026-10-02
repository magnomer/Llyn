# LSettingsOutlet.cs
Hash: `db176525bba1c308`

## `public sealed class LSettingsOutlet : LSettingsPort`

The engine's face for `LSettingsPort`, handed to the deportment in place of the engine itself.
Every member forwards to the same-named member of the engine part that owns it and holds no state or logic.
A forward can later target a facade instead of the engine without changing the port.

## `public LSettingsOutlet(LEngine engine)`

Rejects a null engine and keeps it for every forward.
