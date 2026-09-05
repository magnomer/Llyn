# PSentence.cs

## `internal sealed class PSentence`

One Example row on a card.
The row carries the id of the Example it edits.
So a sentence the user rewrites stays the same Example.
A row that has never been written carries an empty id.
It is given one the moment a sentence is typed into it.
That is the id it is then stored under, and the id the row shows while writing.

A row standing empty is not one thing.
The sentence may never have been written.
Or it may have been written and be unreadable now.
The second is marked rather than shown.
The mark stands until the user writes over it or clears the row.
So nothing unreadable is quietly turned into nothing at all.
Writing in the row is the user saying what the sentence is, which is why any edit ends the mark.

## `internal PSentence(ObservableCollection<PCitationItem> catalog)`

An empty row nothing has been written in.

## `internal PSentence(ObservableCollection<PCitationItem> catalog, LStateValue text, string id, LStateValue reference)`

The row for a stored Example.
It holds the sentence and the Source it cites as the store knows them.
Both stand under the id that names it.

## `public ObservableCollection<PCitationItem> PSentenceCitationCatalog { get; }`

The Sources the whole form offers, shared by every row.
So a Source written on one row is on offer to the next without reloading anything.

## `internal LStateValue PSentenceTextRead()`

What the row says its sentence is: nothing written, unreadable, or the text it shows.

## `internal LStateValue PSentenceCitationRead()`

What the row says about the Source it cites: none, unreadable, or the one it names.

## `internal void PSentenceIdentityApply()`

Gives the row the id its Example will be stored under, once the row holds a sentence.
An id, once given, stays with the row.

## `internal void PSentenceClear()`

Empties the row without dropping it, which is what removing the last row on a card leaves.
An emptied row says nothing was written, because the user said so.

## `internal void PSentenceCitationShow()`

Reads the name of the cited Source again, for when the list of Sources changed underneath the row.
