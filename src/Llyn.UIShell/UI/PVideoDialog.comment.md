# PVideoDialog.cs

## `public partial class PEditor`

What the Video rows of a card ask the editor for.
That is a row opened, a row dropped, and a file chosen from this machine.

## `internal void PVideoAttach(PCard card)`

Points the card's location and span changes at this editor, so each becomes a deferred request for its row.

## `internal void PVideoAddHandle(object sender, RoutedEventArgs e)`

Opens a video row on the card the Extra row belongs to.

## `public void PVideoRemoveHandle(object sender, RoutedEventArgs e)`

Drops the row from the card that holds it.
Public because it answers `PVideoHost`, the seam the row template reaches the editor through.

## `public void PVideoOpenHandle(object sender, RoutedEventArgs e)`

Chooses a video file through the row's own chooser and writes its path into the row.
As with a picture, the field takes a web address just as well.
Nothing is copied into the workspace, because a Video is never kept.

## `private bool PVideoPendingCheck(PCard card, PVideo row, string field)`

Whether a request for the row's field is still waiting, in which case a redraw must not overwrite it.
