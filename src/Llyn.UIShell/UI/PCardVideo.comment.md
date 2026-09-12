# PCardVideo.cs

## `internal sealed partial class PCard`

The Video rows a card shows, kept the way the Image rows are.
A clip is an addition the user asks for.
So a card starts with no row and offers none it is not given.
The engine holds the rows, blank ones included, and the card renders them by id.

## `internal Action<PVideo, string>? PCardVideoNotice { get; set; }`

Where a changed location or span goes, with the name of the one that changed.
The editor sets it when it builds the card and turns the change into a request.

## `internal void PCardVideoShow(IReadOnlyList<LVideoDraft> rows, Func<PVideo, string, bool> pending)`

Makes the rows show the engine's Videos, matched by id.
A field with a request still waiting is left as typed, which `pending` answers per row and field.

## Inline notes

### `private PVideo PCardVideoCreate(LVideoDraft draft)`

Builds one row from the engine's row and starts listening to it.

### `private void PCardVideoChange(object? sender, PropertyChangedEventArgs arguments)`

The location and the span are the engine's.
The playback state and the preview are drawn from them and never reported.
