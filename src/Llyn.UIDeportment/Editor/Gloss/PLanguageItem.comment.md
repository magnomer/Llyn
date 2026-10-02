# PLanguageItem.cs
Hash: `17b6a97042d3cd3b`

## `internal sealed class PLanguageItem`

Presentation item for one language row in the `PSpeaker` dropdown.
Carries the language name the row shows and the resolved flag image beside it.
It is `null` when the pack declares no flag.
The row then shows no flag.
The flag is resolved once when the list is built so the menu paints without a per-row download.

## `internal static void PLanguageItemReset(ObservableCollection<PLanguageItem> rows, IReadOnlyList<string> languages)`

Rebuilds the dropdown rows from the pack's language names after the flags have been loaded.

## `internal static void PLanguageItemApply(FrameworkElement container, object item, string? _)`

Fills one option of `Theme.Gloss.Option` with the language's flag and name.

## `internal static string PLanguageNameRead(object sender)`

The language a clicked row names, or empty when the sender carries no row.
