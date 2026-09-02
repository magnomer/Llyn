# PVideoMenu.cs

## `public partial class PEditor`

What the Video rows of a card ask the editor for.
That is a row opened, a row dropped, and a file chosen from this machine.

## `internal void PVideoAddHandle(object sender, RoutedEventArgs e)`

Opens a video row on the card the Extra row belongs to.

## `internal void PVideoRemoveHandle(object sender, RoutedEventArgs e)`

Drops the row from the card that holds it.

## `internal void PVideoOpenHandle(object sender, RoutedEventArgs e)`

Chooses a video file and writes its path into the row.
As with a picture, the field takes a web address just as well.
Nothing is copied into the workspace, because a Video is never kept.
