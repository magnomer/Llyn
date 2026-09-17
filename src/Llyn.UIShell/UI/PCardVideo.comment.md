# PCardVideo.cs

## `internal sealed partial class PCard`

The Video rows a card shows, kept the way the Image rows are.
A clip is an addition the user asks for.
So a card starts with no row and offers none it is not given.
The engine holds the rows, blank ones included, and the card renders them by id.

## `internal void PCardVideoShow(IReadOnlyList<LVideoDraft> rows)`

Makes the rows show the engine's Videos, matched by id.
A field already reading what the engine holds is left alone.

## Inline notes

### `private static PVideo PCardVideoCreate(LVideoDraft draft)`

Builds one row from the engine's row.
The row holds the engine's values and nothing typed, so nothing on it is listened to.
What is typed into its fields leaves through the editor's own handler.
