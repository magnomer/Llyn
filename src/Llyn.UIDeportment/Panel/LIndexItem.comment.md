# LIndexItem.cs

## `public sealed class LIndexItem`

Presentation item for one entry row in the Library and Duplex entry lists.
Carries the headword and language the row shows, and the entry id the row loads through.
The id is identity and never displayed.
So the row stays selectable after a headword is edited into something another entry reads as.
The flag is resolved once for the language and handed to the row, not read from disk by the row.
A row is built while its list is being filled, and reading a file there would stall the fill.

## `public bool LIndexItemChosen { get; private set; }`

Whether this row is the one the panel stands on, which the row template paints an accent edge for.
It is the only value of the row that changes after the row is built.
The engine row carries it, and `LSplice` moves the mark in place, so the list keeps its scroll position.

## `public string LIndexItemName { get; }`

The headword as the row shows it, numbered `(1)`, `(2)` while another row carries the same headword.
The engine numbers it on the row it returns, because a repeat is only visible across rows.

## `public string LIndexItemEpithet { get; }`

The epithet the row prints after the headword, small and muted, in the reading the language pack names.
It is empty when the setting is off or the entry keeps none.

## `public static IReadOnlyList<LIndexItem> LIndexItemBuild(IReadOnlyList<LVistaRow> rows)`

One item per engine row, in the order the engine returned them.
Each flag comes from `LEnsignImage`, which has drawn it before the rows are asked.

## `public static bool LIndexItemMatch(LIndexItem held, LIndexItem fresh)`

Whether the two rows show the same values, the chosen mark left aside.
`LSplice` keeps the held rows when every pair matches, so their containers survive a refresh.

## `public static void LIndexItemSync(LIndexItem held, LIndexItem fresh)`

Copies the chosen mark of the fresh row onto the held row that `LSplice` kept.
It raises the change only when the mark moved, so untouched rows repaint nothing.

## `internal static void LIndexItemApply(FrameworkElement container, object item, string? _)`

Fills one index row from its item, the work its bindings did before.
The row's tag reads Chosen on the chosen item and is cleared otherwise, which the look sheet paints.
The library index and each duplex wing share this fill, and each subscribes its own click after it.
