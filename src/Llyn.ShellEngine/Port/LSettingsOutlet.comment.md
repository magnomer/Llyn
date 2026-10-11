# LSettingsOutlet.cs
Hash: `8616ce696d985f3d`

## `public sealed class LSettingsOutlet : LSettingsPort`

The engine's face for `LSettingsPort`, handed to the deportment in place of the engine itself.
Every member forwards to the same-named member of the engine facade that owns it, adding no logic.
The engine is its only state.
The notebook save forwards too, and hands back the title the facade stored.
Forwards already target the owning facades through the held engine.

## `public LSettingsOutlet(LEngine engine)`

Rejects a null engine and keeps it for every forward.
