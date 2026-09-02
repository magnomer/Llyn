# PLocalizationChoice.cs

## `public partial class PSettings`

The interface language the user picks in the settings panel.
The chosen catalog is applied to the live resource dictionary at once.
It is kept as a setting so the next run opens in it.

## Inline notes

### `if (_pSettingsReady)`

Skip persistence while the constructor is applying the stored choice.
Only user changes save.
