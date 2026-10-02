# PCardVideo.cs
Hash: `b31e3804219b1936`

## `internal sealed partial class PCard`

The Video rows a card shows, kept the way the Image rows are.
A clip is an addition the user asks for.
So a card starts with no row and offers none it is not given.
The engine holds the rows, blank ones included, and the card renders them by id.

## `internal void PCardVideoShow(IReadOnlyList<CVideoDraft> rows)`

Makes the rows show the engine's Videos, matched by id.
A field already reading what the engine holds is left alone.
A new row is built from its draft alone, which already carries the screen it plays.
The row holds the engine's values and nothing typed, so nothing on it is listened to.
What is typed into its fields leaves through the editor's own handler.
