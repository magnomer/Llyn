# PCardSentence.cs

## `internal sealed partial class PCard`

The Example rows a card carries.
A card owns its rows rather than the editor owning them.
A row belongs to the card it was written under and moves with it.
This file holds that one responsibility and nothing else the card does.

## `internal void PCardSentenceApply(LSentenceOrder order)`

Hands the language pack's field order to every row the card holds, and to every row opened after.
A card written in one language never draws its rows in another language's order.

## `internal void PCardSentenceShow(IReadOnlyList<LExampleDraft> drafts)`

Replaces the rows with the stored Examples of the card being loaded.
It leaves one empty row when the card references none.
So a card always offers somewhere to write.

## `internal IReadOnlyList<LExampleDraft> PCardSentenceRead()`

What the card says its Examples are.
A row nothing was written in is left out rather than read as an empty Example.
A row stating a frame and no sentence is kept.
The frame is the card's own and is lost nowhere else.

## `internal void PCardSentenceInsert(PSentence row)`

Opens a new row directly beneath the one the user asked from.
So a sentence is added where they were reading.

## `internal void PCardSentenceRemove(PSentence row)`

Drops the row, except when it is the last one.
The last row is emptied instead.
A card with no row to write in offers nothing.
