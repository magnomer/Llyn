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

The row also carries the frame the card reads the sentence under, a marker and a role.
Nothing offers a value for either, because nothing ships one.
Which of the two is written first is the language pack's to say and never the row's.
The row is told the two places and puts each field where it was told.

The revision is the sentence rewritten, and it is an Example of its own once stored.
So the row keeps the id that rewrite is stored under, beside the id of the sentence itself.

## `internal PSentence(ObservableCollection<PCitationItem> catalog)`

An empty row nothing has been written in.

## `internal PSentence(ObservableCollection<PCitationItem> catalog, LExampleDraft draft)`

The row for a stored Example.
It holds the sentence, the Source it cites, the frame, and the rewrite as the store knows them.
All of it stands under the id that names it.

## `public ObservableCollection<PCitationItem> PSentenceCitationCatalog { get; }`

The Sources the whole form offers, shared by every row.
So a Source written on one row is on offer to the next without reloading anything.

## `internal void PSentenceOrderApply(LSentenceOrder order)`

Puts the marker and the role in the places the language pack states.
The row states no order of its own, so it holds only what it was told.

## `internal LExampleDraft PSentenceDraftRead()`

What the row says its Example is, frame and rewrite included.
The rewrite is read as a row of the same shape, and is nothing at all when none was written.

## `internal LStateValue PSentenceParticleRead()`

What the row says its marker is: nothing written, unreadable, or the text it shows.

## `internal LStateValue PSentenceDependenceRead()`

What the row says its role is: nothing written, unreadable, or the text it shows.

## `internal LStateValue PSentenceRevisionRead()`

What the row says its rewrite is: nothing written, unreadable, or the text it shows.

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
