# PMorphologyChoice.cs

## `public partial class PSettings`

The morphology switch the user ticks in the settings panel.
It is kept as a setting so the next run opens with it.
Turning it off cancels any fetch still in flight, and the engine does that under its own gate.
Forms already stored stay, because the switch governs asking, not keeping.
The next display of an unfilled entry reads the switch, because the engine checks it per fetch.

## Inline notes

### `if (_pSettingsReady)`

Skip persistence while the stored choice is being applied.
Only user changes save.
