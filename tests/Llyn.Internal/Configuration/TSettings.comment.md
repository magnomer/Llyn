# TSettings.cs
Hash: `95636eecf95d5576`

## `public sealed class TSettings`

Covers the settings file the engine keeps: the interface language and the switches beside it.
The epithet switch round-trips under the `epithet` key and is written even when off.
The tally switch loads as off unless the key is an explicit true, and the engine's save writes it.
A language the catalog does not list is kept as saved but reads back as the default.
A file written before the window posture moved out still loads, its posture keys passed over.
The respelling switch round-trips through the loader as a JSON boolean.
A missing key or a non-boolean value loads as off, so an older or hand-edited file never turns it on.
The engine's save reaches both the held settings and the file under the `respelling` key.
The frequency switch round-trips under the `frequency` key and is written even when off.
A missing key or a non-false value loads as on, so only an explicit false turns the fill off.
The morphology switch round-trips under the `morphology` key with the same rule.
A file that is not JSON loads as defaults and is copied aside as `settings.broken.json` first.
A save leaves no pending file behind and the saved file reports as existing.
A switch saved with the value already held raises no settings bulletin, so an echoing control is a no-op.
A switch that changes raises the bulletin once, and saving the same value again raises nothing more.
