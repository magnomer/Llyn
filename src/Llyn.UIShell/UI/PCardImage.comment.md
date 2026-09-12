# PCardImage.cs

## `internal sealed partial class PCard`

The Image rows a card shows.
Unlike Example and Situation, a card starts with no Image row at all.
A picture is an addition the user asks for through the card's Extra row.
So an empty card shows no picture field, and one whose last picture is dropped goes back to none.
The engine holds the rows, blank ones included, and the card renders them by id.

## `internal Action<PImage>? PCardImageNotice { get; set; }`

Where a changed location goes.
The editor sets it when it builds the card and turns the change into a request.

## `internal void PCardImageShow(IReadOnlyList<LImageDraft> rows, Func<PImage, bool> pending)`

Makes the rows show the engine's Images, matched by id.
A location with a request still waiting is left as typed, which `pending` answers per row.

## Inline notes

### `private PImage PCardImageCreate(LImageDraft draft)`

Builds one row from the engine's row and starts listening to its location.

### `private void PCardImageChange(object? sender, PropertyChangedEventArgs arguments)`

Only the location is the engine's.
The preview is drawn from it and never reported.
