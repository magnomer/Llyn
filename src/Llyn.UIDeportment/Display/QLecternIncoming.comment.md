# QLecternIncoming.cs
Hash: `2013dfbcd1cfc565`

## `public sealed class QLecternIncoming`

The reading view's incoming section, listing the entries that link to the shown one.
[QLectern](QLectern.comment.md) builds it once in its constructor over the view's page.
It pulls its own parts by contract ID, and the lectern subscribes its redraw to the display's open and close.

## `public QLecternIncoming(FrameworkElement surface, CDisplayCard area, CNavigation navigation)`

Pulls the incoming list and its section from `surface` and binds the list to its rows.
`area` is the display's card area, which answers the incoming read.
The navigation is the atelier's, whose gate a clicked row opens its record through.
The click bubbles from the row button to the list, so one handler on the list hears every row.

## `public void QLecternIncomingRefine()`

Lists one incoming row per usage `CDisplayIncomingRead` answers, its owner named under the key Conduct chose.
The lectern subscribes it to both open and close, since a closed display answers no usages.
The section collapses when no entry links here, because an empty relationship does not occupy the page.

## `private void QLecternIncomingObserve(object sender, RoutedEventArgs e)`

Hears a click on an incoming row and hands its usage to the navigation's gate `CNavigationUsageOpen`.
The row is read from the click's original source, since the click bubbles from its button.
