# PLocalizationChoice.cs

## `public partial class PSettings`

The interface language the user picks in the settings panel.
The handler only saves, and the settings bulletin the engine raises applies the catalog.
It is kept as a setting so the next run opens in it.
The language alone is handed downstream, never the whole settings record, so a geometry saved elsewhere is not overwritten.

## `private void PLocalizationApply(string language)`

Applies one language to the live resources and rewrites the text this panel holds as plain strings.
The ledger titles and the summaries are read from the catalog, not bound, so they are written here.
A workspace change calls it as well, since the workspace moved onto may prefer another language.

## Inline notes

### `if (PLocalization.SelectedValue is not string language)`

A cleared box has nothing to save.
