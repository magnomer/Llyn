# QInventoryItem.cs
Hash: `b8b53b7c558b9fbe`

## `internal sealed class QInventoryItem`

Presentation item for one entry row in the `PInventory` list.
It is built from the Conduct `CCatalogPronunciation` only, so it names no engine type.
Carries the pronunciation and the headword the row shows, and the entry id the row loads through.
The id is identity and never displayed.
The pronunciation arrives bracketed from Conduct and is shown as it is.
Conduct words an entry with no pronunciation yet as a bracket pair, so the row stays a row.
The language stands at the far end of the row, behind its flag, as it does in the other catalogs.
The flag is resolved once for the language and handed to the row, not read from disk by the row.

## `public bool QInventoryItemChosen`

Whether this row is the one the panel stands on, which the row template paints an accent edge for.
It is the only value of the row that changes after the row is built.
The catalog row carries it, and `QSplice` moves the mark in place, so the list keeps its scroll position.

## `public string QInventoryItemName { get; }`

The headword as the row shows it, numbered `(1)`, `(2)` while another row carries the same headword.
The engine numbers it on the row it returns, because a repeat is only visible across rows.
`QInventoryItemHeadword` keeps the plain headword for everything that is not display.
The constructor takes it, so the row has one writer.

## `public string QInventoryItemEpithet { get; }`

The epithet the row prints after the headword, small and muted, in the reading the language pack names.
The epithet is the reading the pack names, empty when the setting is off or the entry keeps none.

## `internal static IReadOnlyList<QInventoryItem> QInventoryItemBuild(IReadOnlyList<CCatalogPronunciation> rows)`

One item per catalog row, in the order the controller returned them.
Each item takes the row's chosen mark as a constructor parameter.
A plain copy loop, so the panel that asks for it carries no loop of its own.

## `internal static bool QInventoryItemMatch(QInventoryItem held, QInventoryItem fresh)`

Whether the two rows show the same values, the chosen mark left aside.
The flag counts too, so a flag loaded after the rows were built refills the list.
`QSplice` keeps the held rows when every pair matches, so their containers survive a refresh.

## `internal static void QInventoryItemSync(QInventoryItem held, QInventoryItem fresh)`

Copies the chosen mark of the fresh row onto the held row that `QSplice` kept.

## `internal static void QInventoryItemRefine(FrameworkElement container, object item, string? _)`

Fills one inventory row's named parts and marks the row while it is the chosen one.
The epithet leads with an en space, which the markup's format string once added.
