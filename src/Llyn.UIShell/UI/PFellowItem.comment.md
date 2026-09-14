# PFellowItem.cs

## `internal sealed class PFellowItem`

Presentation item for one co-author row of the vita: the Author's id, its name, and how many Sources credit both.
The count is worded once here, so the row binds to text.

## `internal PFellowItem(long id, string name, int shared)`

Builds the row from the Author and the number of Sources it shares with the read one.
