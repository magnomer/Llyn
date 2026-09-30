# PVideoDialog.cs

## `public partial class PEditor`

What the Video rows of a card ask the editor for.
That is a row opened, a row dropped, and a file chosen from this machine.
The rows sit in a template, so their events arrive here rather than at the card.
Each click hands one raw value to one video gate, and the engine finds the card a row belongs to.

## `internal void PVideoAddObserve(object sender, RoutedEventArgs e)`

Hands the id of the card the Extra row belongs to to the add gate, which appends the row.

## `private void PVideoRemoveObserve(object sender, RoutedEventArgs e)`

Hands the id of the clicked row to the remove gate.

## `private void PVideoOpenObserve(object sender, RoutedEventArgs e)`

Asks for a video file, owned by the window of the clicked button.
The chosen path goes raw to the file gate, which sends it as the row's location at once.
A cancelled dialog hands null, and the gate does nothing.
A typed location or span goes the same way through the editor's field handler, only deferred.
As with a picture, the field takes a web address just as well.
Nothing is copied into the workspace, because a Video is never kept.

## `internal void PVideoApply(FrameworkElement container, object item, string? name)`

The editor's video row fill, reached from the card templates' list forwarders.
It draws the row through the shared fill and subscribes the browse and remove observers.
