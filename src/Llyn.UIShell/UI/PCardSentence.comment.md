# PCardSentence.cs

## `internal sealed partial class PCard`

The Example rows a card shows.
The engine holds the rows, and the card renders them by id and reports what the user does to them.
A row belongs to the card it was written under and moves with it.
This file holds that one responsibility and nothing else the card does.

## `internal Action<PSentence, string>? PCardSentenceNotice { get; set; }`

Where a pick in any row goes, such as the language chosen for a Gloss.
The editor sets it when it builds the card and turns the pick into a request.
The card cannot send, because it holds no draft id.
Typed text never comes this way, since a row holds no copy of what is typed.

## `internal void PCardSentenceApply(LSentenceOrder order)`

Hands the language pack's field order to every row the card holds.
The card keeps no order of its own, so a row opened later reads the engine's when it is built.

## `internal void PCardSentenceShow(IReadOnlyList<LSentenceDraft> drafts, string language)`

Makes the rows show the engine's rows, matched by sentence id.
A field already reading what the engine holds is left alone.
The language names the field order a new row is built under.
The engine keeps a blank row, so a card always offers somewhere to write without the card adding one.

## `internal int PCardSentenceFind(PSentence row)`

Where the row stands, which is what an addition beneath it is asked at.

## Inline notes

### `private PSentence PCardSentenceCreate(LSentenceDraft draft, string language)`

Builds one row from the engine's row and starts listening to it.
The order is read from the engine at birth, so a row never draws in the default order first.

### `private void PCardSentenceChange(object? sender, PropertyChangedEventArgs arguments)`

Forwards every property change with its name, and the editor decides which ones are requests.
