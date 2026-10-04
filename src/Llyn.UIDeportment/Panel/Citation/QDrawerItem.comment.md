# QDrawerItem.cs
Hash: `3faf6e5db0d974be`

## `internal sealed class QDrawerItem`

One offered Source in the citation drawer, copied from the offer row the engine split.
It holds text only, so the drawer's rows are fed no Conduct shape.

## `public string QDrawerItemLead { get; }`

The text before the match, or the whole text when the typed word is not found in it.

## `public string QDrawerItemMark { get; }`

The matched part, which the row draws in weight between the lead and the tail.

## `internal static void QDrawerItemApply(FrameworkElement container, object item, MouseButtonEventHandler press)`

Fills one drawer row's named runs and its count, and subscribes the driver's press on the row.
The press is removed first, so a refill never doubles it.
