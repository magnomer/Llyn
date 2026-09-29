# PProfferItem.cs

## `internal sealed class PProfferItem`

One stored row offered in the dropdown: the wording it is shown by and the id it is stored under.
The id is what the pick carries.
Choosing a row links the stored row rather than a copy of its words.

## `internal PProfferItem(long id, string lead, string mark, string tail, string count)`

Copies a ready row whose split and count the engine already made.
Every find builds its rows this way, the register, situation and citation finds alike.
A register row carries an empty count, so it shows none.

## `public string PProfferItemLead`

The wording is held in three pieces: what stands before what was typed, what was typed, and what follows.
The row draws the middle piece in weight, so a reader sees why the row is offered rather than guessing.
The split is Core's `LCatalog.LCatalogMarkFind`, made before the row arrives.

## `public string PProfferItemCount`

How many cards or Examples already use the row, shown beside the wording as it came.
Core's `LCatalog.LCatalogUsageFormat` leaves it empty for a row nobody uses.
