# PSettingsEpithet.cs

## `public partial class PSettings`

The epithet switch the user ticks in the settings panel: whether lists print a reading after the headword.
It is kept as a setting so the next run opens with it.
Nothing on screen is renamed when it changes.
Every list asks the engine for the epithet as it fills, so the next fill reflects the switch.

## Inline notes

### `if (_pSettingsReady)`

Skip persistence while the stored choice is being applied.
Only user changes save.
