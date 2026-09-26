# PFellowItem.cs

## `internal sealed class PFellowItem`

Presentation item for one co-author row of the vita: the Author's id, its name, and how many Sources credit both.
The count is worded once here, so the row binds to text.

## `internal PFellowItem(LFellow fellow)`

Copies one fellow row: its id, its name and its shared source count as text.

## `internal static IReadOnlyList<PFellowItem> PFellowItemBuild(IReadOnlyList<LFellow> fellows)`

A plain copy loop over the fellows of one Author.

## `internal static void PFellowItemApply(FrameworkElement container, object item, string? _)`

Fills one co-author row with the guild icon, the name and the shared count.
