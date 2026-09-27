# QRollItem.cs

## `internal sealed class QRollItem`

Presentation item for one roll row: the Author's name, its worded source count, and its citation count.
The mark is resolved once here, because the uncredited row wears a different one from every Author.
The same row serves the union list of the edit area, because both list Authors with their source counts.

## `internal QRollItem(CCatalogAuthor row, bool chosen)`

Builds the row of one Author from its catalog row, whose source count the controller worded.
The chosen mark comes as a parameter, so the mark has one kind of writer.
The uncredited row arrives as a row that is not stored, and wears the unlink mark for it.

## `internal static IReadOnlyList<QRollItem> QRollItemBuild(IReadOnlyList<CCatalogAuthor> rows)`

A plain copy loop over the roll rows.

## `public bool QRollItemChosen`

Whether the row is the chosen one, raised so the row restyles itself.

## `internal static bool QRollItemMatch(QRollItem held, QRollItem fresh)`

Whether the two rows show the same values, the chosen mark left aside.
`LSplice` keeps the held rows when every pair matches, so their containers survive a refresh.

## `internal static void QRollItemSync(QRollItem held, QRollItem fresh)`

Copies the chosen mark of the fresh row onto the held row that `LSplice` kept.

## `internal static void QRollItemApply(FrameworkElement container, object item, string? _)`

Fills one roll row's named parts and marks the row while it is the chosen one.
The union row shares the part names, so the edit area attaches the same fill.
