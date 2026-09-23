# PIndexItem.cs

## `internal sealed class PIndexItem`

Presentation item for one entry row in `PIndex`.
Carries the headword and language the row shows, and the entry id the row loads through.
The id is identity and never displayed.
So the row stays selectable after a headword is edited into something another entry reads as.
The flag is resolved once for the language and handed to the row, not read from disk by the row.
A row is built while its list is being filled, and reading a file there would stall the fill.

## `public bool PIndexItemChosen`

Whether this row is the one the panel stands on, which the row template paints an accent edge for.
It is the only value of the row that changes after the row is built.
The engine row carries it, and `PSplice` moves the mark in place, so the list keeps its scroll position.

## `public string PIndexItemName`

The headword as the row shows it, numbered `(1)`, `(2)` while another row carries the same headword.
The engine numbers it on the row it returns, because a repeat is only visible across rows.
`PIndexItemHeadword` keeps the plain headword for everything that is not display.
The constructor takes it, so the row has one writer.

## `public string PIndexItemEpithet { get; }`

The epithet the row prints after the headword, small and muted, in the reading the language pack names.
The epithet is the reading the pack names, empty when the setting is off or the entry keeps none.

## `internal static IReadOnlyList<PIndexItem> PIndexItemBuild(IReadOnlyList<LVistaRow> rows)`

One item per engine row, in the order the engine returned them.
A plain copy loop, so the panel that asks for it carries no loop of its own.

## `internal static bool PIndexItemMatch(PIndexItem held, PIndexItem fresh)`

Whether the two rows show the same values, the chosen mark left aside.
`PSplice` keeps the held rows when every pair matches, so their containers survive a refresh.

## `internal static void PIndexItemSync(PIndexItem held, PIndexItem fresh)`

Copies the chosen mark of the fresh row onto the held row that `PSplice` kept.
