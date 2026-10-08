# TPortrait.cs
Hash: `a4a535be4630cb1c`

## `public sealed class TPortrait`

Covers what every panel's print and export gate shares through `CPortrait`.
The export format, the print side and the ink map by name, and an unknown member throws.
A print ticket keeps what the dialog answered, with the sheet in inches.
A missing or empty sheet size takes the local sheet.
Every label and legend word is the engine's wording of a key Conduct chose.
The fake wording answers `text:` plus the key.
The unit names are worded the same way, one per unit.
The formats offered and the default one come from the engine.
Hostile engine rows still give exactly one chosen format and no `|` in a suffix.
No rows give the markup format, no chosen row chooses the first, and many keep the first.
An unknown engine format is a programming mismatch, so reading it throws.
A declined file or printer question does nothing.
A failing dialog or engine call shows its key through the envoy.
A failing name read shows `Export.NameFailed` and exports nothing.
An export handed no settings port throws.

## `private static LSettingsPort TPortraitSettingsCreate()`

A settings port whose wording answers each key as `text:` plus the key, so a fact reads the key back.
