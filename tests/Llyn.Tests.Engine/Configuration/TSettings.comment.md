# TSettings.cs
Hash: `0c89cc7ae0f89b20`

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
The custom analysis switch round-trips under the `analysis` key with the same rule, on by default.
The engine's save of a changed analysis switch raises one settings and one inflection bulletin for every entry.
Saving the same value again raises nothing more.
A file that is not JSON loads as defaults and is copied aside as `settings.broken.json` first.
A save leaves no pending file behind and the saved file reports as existing.
A folder without a settings file reports none.
A switch saved with the value already held raises no settings bulletin, so an echoing control is a no-op.
A switch that changes raises the bulletin once, and saving the same value again raises nothing more.
