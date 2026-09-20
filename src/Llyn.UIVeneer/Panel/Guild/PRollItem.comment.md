# PRollItem.cs

## `internal sealed class PRollItem`

Presentation item for one roll row: the Author's name, its worded source count, and its citation count.
The mark is resolved once here, because the uncredited row wears a different one from every Author.
The same row serves the union list of the edit area, because both list Authors with their source counts.

## `internal PRollItem(LCatalogAuthor row, string work, bool chosen)`

Builds the row of one Author from its catalog row and the worded source count.
The uncredited row arrives as a catalog row with the id zero, and wears the unlink mark for it.

## `internal static IReadOnlyList<PRollItem> PRollItemBuild(IReadOnlyList<LCatalogAuthor> rows)`

A plain copy loop over the roll rows, wording each source count on the way.

## `public bool PRollItemChosen`

Whether the row is the chosen one, raised so the row restyles itself.

## `internal static bool PRollItemMatch(PRollItem held, PRollItem fresh)`

Whether the two rows show the same values, the chosen mark left aside.
`PSplice` keeps the held rows when every pair matches, so their containers survive a refresh.

## `internal static void PRollItemSync(PRollItem held, PRollItem fresh)`

Copies the chosen mark of the fresh row onto the held row that `PSplice` kept.
