# PRespellingChoice.cs

## `public partial class PSettings`

The respelling switch the user ticks in the settings panel.
It is kept as a setting so the next run opens with it.
The engine raises a settings bulletin when it changes, and every open reading redraws in the form now picked.
Nothing is refetched and nothing stored changes, because both forms are already kept.

## Inline notes

### `if (_pSettingsReady)`

Skip persistence while the stored choice is being applied.
Only user changes save.
