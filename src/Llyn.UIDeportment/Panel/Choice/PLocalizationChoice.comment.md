# PLocalizationChoice.cs

## `public partial class PSettings`

The interface language the user picks in the settings panel.
The observer only hands the raw language to one ledger save.
The ledger state the save brings back applies the catalog.
It is kept as a setting so the next run opens in it.
The language alone is handed downstream, never the whole settings record, so a geometry saved elsewhere is not overwritten.

## Inline notes

### `if (PLocalization.SelectedValue is not string language)`

A cleared box has nothing to save.
