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
The panel sets it instead of refilling the list, so the catalog keeps its scroll position.
