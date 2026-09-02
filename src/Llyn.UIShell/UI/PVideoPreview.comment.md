# PVideoPreview.cs

## `public partial class PEditor`

The players the Video rows show, and the one clock that keeps them inside the span each row asks for.

A player is a control the list builds and takes away as the card scrolls, so the editor holds the players by the row they belong to and drops one the moment its control leaves the tree. The span is watched rather than scheduled: a player has no way to be told "stop here", so a single timer looks at every running preview a few times a second and sends one back to its start once it has passed the end its row named. One timer serves every card, and it is started only when the first preview appears.

## `internal void PVideoLoadHandle(object sender, RoutedEventArgs e)`

Takes a preview into the editor's care once it is on screen and starts it playing, unless its row was paused. A row with no location has nothing to play and is ignored.

## `private void PVideoAddressHandle(object? sender, PropertyChangedEventArgs arguments)`

Starts a preview whose row was given a location after the player was already on screen, and closes one whose location was taken away. A player is built before anything is typed into the row above it, so waiting for the control to appear is not enough — the row has to say when there is something to play.

## `internal void PVideoReadyHandle(object sender, RoutedEventArgs e)`

Moves a preview to the start of its span, once the video is open far enough to be moved.

## `internal void PVideoDropHandle(object sender, RoutedEventArgs e)`

Lets go of a preview whose control has left the tree and closes the file it held.

## `internal void PVideoPlayHandle(object sender, RoutedEventArgs e)`

Plays or pauses the preview of the row whose button was pressed, following the state the row already moved to.

## `internal void PVideoFinishHandle(object sender, RoutedEventArgs e)`

Sends a video that has played out back to the start of its span, so a preview keeps showing the part that was asked for.

## `private void PVideoClose()`

Stops the clock and closes every preview, which is what closing the editor has to do: a player left holding a file keeps holding it.
