# CAccentTyped.cs

## `public sealed record CAccentTyped(string CAccentTypedText)`

The answer to one typed accent row, as `CTimbre.CTimbreAccentSet` gives it.
A driver writes its row from this answer alone, never from what the user typed.

**Parameters**

- `CAccentTypedText`: the text the row now holds.
  It is the typed text when the gate took it, else the text the draft holds.
