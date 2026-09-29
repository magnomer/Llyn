# QFellowItem.cs

## `internal sealed class QFellowItem`

Presentation item for one co-author row of the vita: the Author's id, its name, and how many Sources credit both.
The count arrives worded by Core, so the row binds to text.

## `internal QFellowItem(CFellow fellow)`

Copies one fellow row: its id, its name and its worded shared source count.

## `internal static IReadOnlyList<QFellowItem> QFellowItemBuild(IReadOnlyList<CFellow> fellows)`

A plain copy loop over the fellows of one Author.

## `internal static void QFellowItemApply(FrameworkElement container, object item, string? _)`

Fills one co-author row with the guild icon, the name and the shared count.
