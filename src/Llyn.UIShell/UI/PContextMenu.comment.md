# PContextMenu.cs

## `public partial class PEditor`

The editor's side of a Situation row: opening and dropping rows, and the Source list a row cites from.
The rows themselves hold no engine.
So every request a row makes arrives here.
The card that owns the row is found and the engine is called.

## `internal void PContextAddHandle(object sender, RoutedEventArgs e)`

Opens a further Situation row directly under the row the user asked from.

## `internal void PContextRemoveHandle(object sender, RoutedEventArgs e)`

Drops the Situation row the user asked from, or empties it when it is the only one the card has.

## `internal void PContextReferenceClear(object sender, RoutedEventArgs e)`

Stops the row citing any Source.

## `internal void PContextReferenceCreate(object sender, RoutedEventArgs e)`

Stores the Source written into the row's list at once.
A Situation cites a Source by id.
An id is only worth citing once something stands behind it.
The row it was written on then cites it, and every other row on the form is offered it.
