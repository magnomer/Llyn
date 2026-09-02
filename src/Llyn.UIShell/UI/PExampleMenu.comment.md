# PExampleMenu.cs

## `public partial class PEditor`

The editor's side of an Example row: opening and dropping rows, and the Source list a row cites from.
The rows themselves hold no engine.
So every request a row makes arrives here.
The card that owns the row is found and the engine is called.
One list of Sources serves every row on the form, Example and Situation alike.

## `internal void PExampleLoad()`

Reads every Source the workspace holds into the list the form offers.
It then has every row read the name of the Source it cites again.
A workspace that cannot be read leaves the list empty rather than failing the form.

## `internal void PExampleAddHandle(object sender, RoutedEventArgs e)`

Opens a further Example row directly under the row the user asked from.

## `internal void PExampleRemoveHandle(object sender, RoutedEventArgs e)`

Drops the Example row the user asked from, or empties it when it is the only one the card has.

## `internal void PExampleReferenceClear(object sender, RoutedEventArgs e)`

Stops the row citing any Source.

## `internal void PExampleReferenceCreate(object sender, RoutedEventArgs e)`

Stores the Source written into the row's list at once.
An Example cites a Source by id.
An id is only worth citing once something stands behind it.
The row it was written on then cites it, and every other row on the form is offered it.
