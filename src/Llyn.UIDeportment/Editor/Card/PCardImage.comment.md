# PCardImage.cs

## `internal sealed partial class PCard`

The Image rows a card shows.
Unlike Example and Situation, a card starts with no Image row at all.
A picture is an addition the user asks for through the card's Extra row.
So an empty card shows no picture field, and one whose last picture is dropped goes back to none.
The engine holds the rows, blank ones included, and the card renders them by id.

## `internal void PCardImageShow(IReadOnlyList<LImageDraft> rows)`

Makes the rows show the engine's Images, matched by id.
A row already reading what the engine holds is left alone.

## Inline notes

### `private PImage PCardImageCreate(LImageDraft draft)`

Builds one row from the engine's row.
It is an instance member because the row takes the card's window deportment.
The row holds the engine's values and nothing typed, so nothing on it is listened to.
What is typed into its fields leaves through the editor's own handler.
