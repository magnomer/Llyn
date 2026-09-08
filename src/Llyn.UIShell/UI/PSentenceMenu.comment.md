# PSentenceMenu.cs

## `public partial class PEditor`

The editor's side of an Example row: opening and dropping rows, and the Source list a row cites from.
The rows themselves hold no engine.
So every request a row makes arrives here.
The card that owns the row is found and the engine is called.
One list of Sources serves every row on the form, Example and Situation alike.
The frame the rows read a sentence under is settled here too.
That is the order its two fields take and what each has been saved holding.
Both follow the language the entry is written in.
Switching language redraws every row rather than leaving one language's order over another's.

## `internal void PSentenceLoad()`

Reads every Source the workspace holds into the list the form offers.
It then has every row read the name of the Source it cites again.
A workspace that cannot be read leaves the list empty rather than failing the form.

## `internal void PSentenceFrameLoad(string language)`

Settles the Example frame for `language`: which of its two fields is written first, and what each offers.
The order comes from the language pack and is handed to every card on the form.
A row never keeps the order of the language it was drawn in.
The two lists come from what the workspace has already saved for that language.
Nothing ships a marker or a role.
A workspace that cannot be read leaves a list empty rather than failing the form.
An unnamed language is the one the speaker field currently shows.

## `internal void PSentenceAddHandle(object sender, RoutedEventArgs e)`

Opens a further Example row directly under the row the user asked from.

## `internal void PSentenceRemoveHandle(object sender, RoutedEventArgs e)`

Drops the Example row the user asked from, or empties it when it is the only one the card has.

## `internal void PSentenceCitationClear(object sender, RoutedEventArgs e)`

Stops the row citing any Source.

## Inline notes

### `private static void PSentenceFrameShow(ObservableCollection<string> catalog, IReadOnlyList<string> values)`

The list is refilled in place rather than replaced, because every row already holds it.
