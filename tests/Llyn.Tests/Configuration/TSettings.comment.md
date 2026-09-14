# TSettings.cs

## `public sealed class TSettings`

Covers the settings file as two writers reach it: the interface language and the window geometry.
Each is pushed as a field of its own, so whichever is written second must leave the first standing.
The same must hold once the workspace is opened again, since the file is what carries both.
The respelling switch round-trips through the loader as a JSON boolean.
A missing key or a non-boolean value loads as off, so an older or hand-edited file never turns it on.
The engine's save reaches both the held settings and the file under the `respelling` key.
The frequency switch round-trips under the `frequency` key and is written even when off.
A missing key or a non-false value loads as on, so only an explicit false turns the fill off.
The morphology switch round-trips under the `morphology` key with the same rule.
A file that is not JSON loads as defaults and is copied aside as `settings.broken.json` first.
A save leaves no pending file behind and the saved file reports as existing.
A window geometry saved twice unchanged writes the file once, because an equal record is not written again.
