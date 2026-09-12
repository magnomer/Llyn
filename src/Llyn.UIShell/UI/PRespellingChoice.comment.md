# PRespellingChoice.cs

## `public partial class PSettings`

The respelling switch the user ticks in the settings panel.
It is kept as a setting so the next run opens with it.
Nothing on screen is rewritten when it changes.
The next lookup reflects the switch because the engine builds its receiver chain per lookup.

## Inline notes

### `if (_pSettingsReady)`

Skip persistence while the stored choice is being applied.
Only user changes save.
