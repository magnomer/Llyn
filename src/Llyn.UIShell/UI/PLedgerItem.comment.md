# PLedgerItem.cs

## `internal sealed class PLedgerItem`

Presentation item for one setting group row in `PLedger`.
Carries the group title the row shows and the child name the card is found by.
The child name is identity and never displayed.
The title is read from the localization once, when the ledger is built.

## `public string PLedgerItemMeta`

The one-line summary of the values the group holds, shown under the title.
It changes whenever a control of the group is changed, so it notifies.
The ledger writes it instead of refilling the list, so the catalog keeps its rows in place.

## `public bool PLedgerItemChosen`

Whether this row is the group shown on the right, which the row template paints an accent edge for.
The panel sets it on every row when a card is shown, so exactly one row carries it.
