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

Each of the two frame fields offers what has already been saved for the language.
Nothing is shipped, so an empty store offers nothing and the field is a plain box until something is written in it.

## `internal PSentence(ObservableCollection<PCitationItem> catalog, ObservableCollection<string> particles, ObservableCollection<string> dependences)`

An empty row nothing has been written in.

## `internal PSentence(ObservableCollection<PCitationItem> catalog, ObservableCollection<string> particles, ObservableCollection<string> dependences, LExampleDraft draft)`

The row for a stored Example.
It holds the sentence, the Source it cites, and the frame as the store knows them.
All of it stands under the id that names it.

## `public ObservableCollection<PCitationItem> PSentenceCitationCatalog { get; }`

The Sources the whole form offers, shared by every row.
So a Source written on one row is on offer to the next without reloading anything.

## `public ObservableCollection<string> PSentenceParticleCatalog { get; }`

The markers already saved for the language, shared by every row.
Nothing ships one, so the list is empty until a user writes and saves one.

## `public ObservableCollection<string> PSentenceDependenceCatalog { get; }`

The roles already saved for the language, shared by every row.
Nothing ships one, so the list is empty until a user writes and saves one.

## `internal void PSentenceOrderApply(LSentenceOrder order)`

Puts the marker and the role in the places the language pack states.
The row states no order of its own, so it holds only what it was told.

## `internal LExampleDraft PSentenceDraftRead()`

What the row says its Example is, frame included.

## `internal LStateValue PSentenceParticleRead()`

What the row says its marker is: nothing written, unreadable, or the text it shows.

## `internal LStateValue PSentenceDependenceRead()`

What the row says its role is: nothing written, unreadable, or the text it shows.

## `internal LStateValue PSentenceTextRead()`

What the row says its sentence is: nothing written, unreadable, or the text it shows.

## `internal LStateValue PSentenceCitationRead()`

What the row says about the Source it cites: none, unreadable, or the one it names.

## `internal bool PSentenceCheck()`

Whether the row says anything at all: a sentence, a marker, or a role.
A frame written on a row with no sentence still stands, so the row is read rather than dropped.

## `internal void PSentenceIdentityApply()`

Gives the row the id its Example will be stored under, once the row holds a sentence.
An id, once given, stays with the row while the sentence does.
Emptying the sentence takes the id with it, because the row then holds a frame and no Example to name.

## `internal void PSentenceClear()`

Empties the row without dropping it, which is what removing the last row on a card leaves.
An emptied row says nothing was written, because the user said so.

## `internal void PSentenceCitationShow()`

Reads the name of the cited Source again, for when the list of Sources changed underneath the row.
