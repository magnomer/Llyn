# PLanguageItem.cs

## `internal sealed class PLanguageItem`

Presentation item for one language row in the `PSpeaker` dropdown.
Carries the language name the row shows and the resolved flag image beside it.
It is `null` when the pack declares no flag.
The row then falls back to a neutral globe.
The flag is resolved once when the list is built so the menu paints without a per-row download.

## `internal bool PLanguageItemMatch(string language)`

Whether this row names `language`, so a list can leave the chosen one out without comparing names itself.

## `internal static void PLanguageItemReset(ObservableCollection<PLanguageItem> rows, IReadOnlyList<string> languages)`

Rebuilds the dropdown rows from the pack's language names after the flags have been loaded.

## `internal static string PLanguageNameRead(object sender)`

The language a clicked row names, or empty when the sender carries no row.
