# LPortraitOutlet.cs
Hash: `4b7d31ff3f0aa3c2`

## `public sealed class LPortraitOutlet : LPortraitPort`

The engine's face for `LPortraitPort`, handed to the deportment in place of the engine itself.
Every member forwards to the same-named member of the engine part that owns it and holds no state or logic.
A forward can later target a facade instead of the engine without changing the port.
The press and export calls go to `LEnginePortrait`, and `LEngineMarkupStart` goes to `LEngineMarkup`.
The courier calls go to `LEngineCourier`.
`LEngineCourierSend` forwards the wording `lookup` the Joplin bodies are written with.
`LEngineCourierCheck` forwards the token check to the courier facade, so no view reads the settings for it.

## `public LPortraitOutlet(LEngine engine)`

Rejects a null engine and keeps it for every forward.
