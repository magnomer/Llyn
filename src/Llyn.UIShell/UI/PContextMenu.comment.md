# PContextMenu.cs

## `public partial class PEditor`

The editor's side of a Situation row: opening and dropping rows.
The rows themselves hold no engine.
So every request a row makes arrives here.
The card that owns the row is found and the engine is called.

## `internal void PContextAddHandle(object sender, RoutedEventArgs e)`

Opens a further Situation row directly under the row the user asked from.

## `internal void PContextRemoveHandle(object sender, RoutedEventArgs e)`

Drops the Situation row the user asked from, or empties it when it is the only one the card has.
