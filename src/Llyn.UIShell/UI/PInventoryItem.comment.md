# PInventoryItem.cs

## `internal sealed class PInventoryItem`

Presentation item for one entry row in `PInventory`.
Carries the pronunciation and the headword the row shows, and the entry id the row loads through.
The id is identity and never displayed.
The pronunciation is kept raw as well as bracketed, because the ordering reads the raw one.
An entry with no pronunciation yet still reads as a bracket pair, so the row stays a row.
The language stands at the far end of the row, behind its flag, as it does in the other catalogs.
The flag is resolved once for the language and handed to the row, not read from disk by the row.

## `public bool PInventoryItemChosen`

Whether this row is the one the panel stands on, which the row template paints an accent edge for.
It is the only value of the row that changes after the row is built.
The panel sets it instead of refilling the list, so the catalog keeps its scroll position.

## `public string PInventoryItemName`

The headword as the row shows it, numbered `(1)`, `(2)` while another row carries the same headword.
`PTwin` writes it once the list is filled, because a repeat is only visible across rows.
`PInventoryItemHeadword` keeps the plain headword for everything that is not display.
