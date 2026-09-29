# PCardSentence.cs

## `internal sealed partial class PCard`

The Example rows a card shows.
The engine holds the rows, and the card renders them by id and reports what the user does to them.
A row belongs to the card it was written under and moves with it.
This file holds that one responsibility and nothing else the card does.

## `internal Action<PSentence, PGloss, string>? PCardSentenceNotice { get; set; }`

Where a language picked for a Gloss goes, with its sentence row and the raw language.
The editor sets it when it builds the card and hands the pick to one gate.
The card cannot send, because it holds no Conduct type.
Typed text never comes this way, since a row holds no copy of what is typed.

## `internal void PCardSentenceApply(CSentenceOrder order)`

Hands the language pack's field order to every row the card holds.
The card keeps no order of its own.
A row opened later is built under the order the editor hands it.

## `internal void PCardSentenceShow(IReadOnlyList<CSentenceDraft> drafts, CSentenceOrder? order)`

Makes the rows show the engine's rows, matched by sentence id.
A field already reading what the engine holds is left alone.
The order is the one `CSentenceFrameRead` last answered, and a new row is built under it.
The engine keeps a blank row, so a card always offers somewhere to write without the card adding one.

## `internal int PCardSentenceFind(PSentence row)`

Where the row stands, which is what an addition beneath it is asked at.

## Inline notes

### `private PSentence PCardSentenceCreate(CSentenceDraft draft, CSentenceOrder? order)`

Builds one row from the engine's row and points its Gloss notice at the card's.
The frame's order is applied at birth, so a row never draws in the default order first.
No order yet means no frame was read, and the row keeps its default columns.

