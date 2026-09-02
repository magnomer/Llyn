# PLangcodeItem.cs

## `internal sealed class PLangcodeItem`

Presentation item for one language row in the `PLangcode` dropdown.
Carries the language name the row shows and the resolved flag image beside it.
It is `null` when the pack declares no flag.
The row then falls back to a neutral globe.
The flag is resolved once when the list is built so the menu paints without a per-row download.
