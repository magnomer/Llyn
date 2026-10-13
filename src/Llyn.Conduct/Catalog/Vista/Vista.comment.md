# Vista

This folder holds what one catalog tab's vista carries to a driver.
That is the ordering and language-filter menus and the entry row.

## `CCatalogOrder`

One ordering a catalog can list its rows in, as a driver offers it in a menu.
Its members keep the engine's order and mirror it member for member.
A driver never learns the engine's ordering, so its menus survive the move below the cut.

## `CCatalogFilter`

The languages a catalog hides, as a driver ticks them in a menu.
`CCatalogFilterMatch` compares names without case, as the engine's filter does.

## `CVistaRow`

The same entry-row shape serves catalog lists and the prospect popup.
An entry without an epithet carries an empty epithet rather than a display fallback.

## Stamp

Commit: `c2f335c331fe`
- `CCatalogFilter.cs` `e9f0bc44660f8df4`
- `CCatalogOrder.cs` `28b1ad168467f1c9`
- `CVistaRow.cs` `7137dc910c76071b`
