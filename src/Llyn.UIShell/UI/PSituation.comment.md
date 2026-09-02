# PSituation.cs

## `internal sealed class PSituation`

One Situation row on a card. The row carries the id of the Situation it edits, so a wording the user rewrites stays the same Situation; a row that has never been written carries an empty id and is given one the moment a wording is typed into it, which is the id it is then stored under and the id the row shows while they are still writing.

A row standing empty is not one thing. The wording may never have been written, or it may have been written and be unreadable now: the second is marked rather than shown, and the mark stands until the user writes over it or clears the row, so nothing unreadable is quietly turned into nothing at all. Writing in the row is the user saying what the wording is, which is why any edit ends the mark.

## `internal PSituation(ObservableCollection<PReference> catalog)`

An empty row nothing has been written in.

## `internal PSituation(ObservableCollection<PReference> catalog, LStateValue text, string id, LStateValue reference)`

The row for a stored Situation: the wording and the Source it cites as the store knows them, under the id that names it.

## `public ObservableCollection<PReference> PSituationReferenceCatalog { get; }`

The Sources the whole form offers, shared by every row so a Source written on one row is on offer to the next without reloading anything.

## `internal LStateValue PSituationTextRead()`

What the row says its wording is: nothing written, unreadable, or the text it shows.

## `internal LStateValue PSituationReferenceRead()`

What the row says about the Source it cites: none, unreadable, or the one it names.

## `internal void PSituationIdentityApply()`

Gives the row the id its Situation will be stored under, once the row holds a wording. An id, once given, stays with the row.

## `internal void PSituationClear()`

Empties the row without dropping it, which is what removing the last row on a card leaves. An emptied row says nothing was written, because the user said so.

## `internal void PSituationReferenceShow()`

Reads the name of the cited Source again, for when the list of Sources changed underneath the row.
