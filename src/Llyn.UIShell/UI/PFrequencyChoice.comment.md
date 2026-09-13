# PFrequencyChoice.cs

## `public partial class PSettings`

The frequency switch the user ticks in the settings panel.
It is kept as a setting so the next run opens with it.
Nothing already stored is fetched or dropped when it changes.
The next save or first display of an unfilled entry reads the switch, because the engine checks it per fill.

## Inline notes

### `if (_pSettingsReady)`

Skip persistence while the stored choice is being applied.
Only user changes save.
