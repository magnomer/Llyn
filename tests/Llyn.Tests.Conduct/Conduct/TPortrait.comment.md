# TPortrait.cs
Hash: `82c5fca76ce1769b`

## `public sealed class TPortrait`

Covers what every panel's print and export gate shares through `CPortrait`.
The export format maps by name, and an unknown member throws.
Every label and legend word is the engine's wording of a key Conduct chose.
The fake wording answers `text:` plus the key.
The unit, freedom and source kind names are worded the same way.
The legend holds one kind name per reference kind.
The formats offered and the default one come from the engine.
Hostile engine rows still give exactly one chosen format and no `|` in a suffix.
No rows give the markup format, no chosen row chooses the first, and many keep the first.
An unknown engine format is a programming mismatch, so reading it throws.
A declined file question does nothing.
A print with nothing printable opens no window and asks nothing.
A failing export or print call shows `Export.Failed` or `Print.Failed` through the envoy.
A failing name read shows `Export.NameFailed` and exports nothing.
An export handed no settings port throws.
A print window that closes or throws still disposes the relay's board.
Those facts open the session through the real print route, over a fake board they hold.
A throwing window escapes to the caller, since only the open call is guarded by `Print.Failed`.

## `private static LSettingsPort TPortraitSettingsCreate()`

A settings port whose wording answers each key as `text:` plus the key, so a fact reads the key back.
