# PCardExample.cs

## `internal sealed partial class PCard`

The Example rows a card carries. A card owns its rows rather than the editor owning them, because a row belongs to the card it was written under and moves with it; this file holds that one responsibility and nothing else the card does.

## `internal void PCardExampleShow(IReadOnlyList<LExampleDraft> drafts)`

Replaces the rows with the stored Examples of the card being loaded, leaving one empty row when the card references none, so a card always offers somewhere to write.

## `internal IReadOnlyList<LExampleDraft> PCardExampleRead()`

What the card says its Examples are. A row nothing was written in is left out rather than read as an empty Example.

## `internal void PCardExampleInsert(PExample row)`

Opens a new row directly beneath the one the user asked from, so a sentence is added where they were reading.

## `internal void PCardExampleRemove(PExample row)`

Drops the row, except when it is the last one: the last row is emptied instead, because a card with no row to write in offers nothing.

## `internal void PCardExampleUpdate()`

Numbers the rows while there is more than one, and numbers nothing while there is only one — a single Example needs no ordinal.
