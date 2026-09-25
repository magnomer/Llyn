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
The engine row carries it, and `LSplice` moves the mark in place, so the list keeps its scroll position.

## `public string PInventoryItemName`

The headword as the row shows it, numbered `(1)`, `(2)` while another row carries the same headword.
The engine numbers it on the row it returns, because a repeat is only visible across rows.
`PInventoryItemHeadword` keeps the plain headword for everything that is not display.
The constructor takes it, so the row has one writer.

## `public string PInventoryItemEpithet { get; }`

The epithet the row prints after the headword, small and muted, in the reading the language pack names.
The epithet is the reading the pack names, empty when the setting is off or the entry keeps none.

## `internal static IReadOnlyList<PInventoryItem> PInventoryItemBuild(IReadOnlyList<LCatalogPronunciation> rows)`

One item per engine row, in the order the engine returned them.
A plain copy loop, so the panel that asks for it carries no loop of its own.

## `internal static bool PInventoryItemMatch(PInventoryItem held, PInventoryItem fresh)`

Whether the two rows show the same values, the chosen mark left aside.
`LSplice` keeps the held rows when every pair matches, so their containers survive a refresh.

## `internal static void PInventoryItemSync(PInventoryItem held, PInventoryItem fresh)`

Copies the chosen mark of the fresh row onto the held row that `LSplice` kept.
