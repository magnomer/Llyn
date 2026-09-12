# PSentence.cs

## `internal sealed class PSentence`

One Example row on a card.
The row carries the id of the Example it edits, and the id of the row itself.
So a sentence the user rewrites stays the same Example.
Both ids are the engine's, read from the draft, and the row never mints one.
A row that has never been written shows an empty Example id until the engine names one.

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
Nothing is shipped, so an empty store offers nothing.
The field is a plain box until something is written in it.

## `internal PSentence(`

The row for one of the engine's rows, blank or filled.
It holds the sentence, the Source it cites, and the frame as the draft holds them.
All of it stands under the ids that name it.

## `internal long PSentenceRow`

The id of the row itself, which every request about the row names.

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

## `internal LStateValue PSentenceParticleRead()`

What the row says its marker is: nothing written, unreadable, or the text it shows.

## `internal LStateValue PSentenceDependenceRead()`

What the row says its role is: nothing written, unreadable, or the text it shows.

## `internal LStateValue PSentenceTextRead()`

What the row says its sentence is: nothing written, unreadable, or the text it shows.

## `internal LStateValue PSentenceCitationRead()`

What the row says about the Source it cites: none, unreadable, or the one it names.

## `internal void PSentenceShow(LSentenceDraft draft, Func<string, bool> pending)`

Redraws the row from the engine's row, field by field, only where the field says something else.
A field with a request still waiting is left as typed, which `pending` answers by field name.
The ids are always taken, because the engine is the only minter.
A field that already reads what the engine holds is left alone, so the caret survives its own echo.

## `internal void PSentenceCitationShow()`

Reads the name of the cited Source again, for when the list of Sources changed underneath the row.

## `public bool PSentenceFrameVisible`

Whether the row shows its frame at all.
A row that carries a marker or a role always shows one, because the reading view draws it.
A row carrying neither shows none, for the same reason, and the card is asked for one instead.

## `public bool PSentenceFrameWritten`

Whether the frame stands on what the row holds rather than on the card being asked for one.
The card's switch stands down while it does, because a frame already written cannot be opened or closed by asking.
It gives its room back rather than keeping it.
The handles beside it are not read across a gap that holds nothing.

## `public string PSentenceFrameGap`

The separator drawn between the marker and the role inside the frame.
It is one space when both are written and nothing otherwise.
The writing view spaces the frame exactly as the reading view does.
