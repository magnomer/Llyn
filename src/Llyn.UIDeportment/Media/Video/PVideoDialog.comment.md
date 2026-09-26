# PVideoDialog.cs

## `public partial class PEditor`

What the Video rows of a card ask the editor for.
That is a row opened, a row dropped, and a file chosen from this machine.

## `internal void PVideoAddHandle(object sender, RoutedEventArgs e)`

Opens a video row on the card the Extra row belongs to.

## `public void PVideoRemoveHandle(object sender, RoutedEventArgs e)`

Drops the row from the card that holds it.
Public because it answers `PVideoHost`, the seam the row template reaches the editor through.

## `public void PVideoOpenHandle(object sender, RoutedEventArgs e)`

Chooses a video file through the row's own chooser and sends its path as the row's location at once.
A typed location or span goes the same way through the editor's field handler, only deferred.
As with a picture, the field takes a web address just as well.
Nothing is copied into the workspace, because a Video is never kept.

## `private readonly PVideoTemplate _pVideoTemplate`

The video row dictionary this editor merged, whose forwarders its row fill subscribes.

## `internal void PVideoApply(FrameworkElement container, object item, string? name)`

The editor's video row fill, reached from the card templates' list forwarders.
It draws the row through the shared fill and subscribes the browse and remove forwarders.
