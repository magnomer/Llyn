# PCardSentence.cs

## `internal sealed partial class PCard`

The Example rows a card shows.
The engine holds the rows, and the card renders them by id and reports what the user does to them.
A row belongs to the card it was written under and moves with it.
This file holds that one responsibility and nothing else the card does.

## `internal Action<PSentence, string>? PCardSentenceNotice { get; set; }`

Where a change in any row goes.
The editor sets it when it builds the card and turns the change into a request.
The card cannot send, because it holds no draft id.

## `internal void PCardSentenceApply(LSentenceOrder order)`

Hands the language pack's field order to every row the card holds, and to every row opened after.
A card written in one language never draws its rows in another language's order.

## `internal void PCardSentenceShow(IReadOnlyList<LSentenceDraft> drafts, Func<PSentence, string, bool> pending)`

Makes the rows show the engine's rows, matched by sentence id.
A field with a request still waiting is left as typed, which `pending` answers per row and field.
The engine keeps a blank row, so a card always offers somewhere to write without the card adding one.

## `internal int PCardSentenceFind(PSentence row)`

Where the row stands, which is what an addition beneath it is asked at.

## Inline notes

### `private PSentence PCardSentenceCreate(LSentenceDraft draft)`

Builds one row from the engine's row and starts listening to it.
The order is applied at birth, so a row never draws in the default order first.

### `private void PCardSentenceChange(object? sender, PropertyChangedEventArgs arguments)`

Forwards every property change with its name, and the editor decides which ones are requests.
