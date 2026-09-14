# PLocalizationChoice.cs

## `public partial class PSettings`

The interface language the user picks in the settings panel.
The chosen catalog is applied to the live resource dictionary at once.
It is kept as a setting so the next run opens in it.
The language alone is handed downstream, never the whole settings record, so a geometry saved elsewhere is not overwritten.

## `private void PLocalizationApply(string language)`

Applies one language to the live resources and rewrites the text this panel holds as plain strings.
The ledger titles, the summaries and the footer are read from the catalog, not bound, so they are written here.
A workspace change calls it as well, since the workspace moved onto may prefer another language.

## Inline notes

### `if (!_pSettingsReady || PLocalization.SelectedValue is not string language)`

Skip while the stored choice is being applied to the box.
The program already opened in that language, so applying it again would only repeat the work.
Only user changes apply and save.
